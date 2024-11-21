using MRLserver;
using MRLserver.Data;
using MRLserver.Models;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;


public class SslServer {
    private static X509Certificate2 serverCertificate;
    private SharedMRLdata _sharedData;
    private IPAddress inEPip;
    private readonly MRLservContext _sql_context;
    private Dictionary<string, string> GetAllDataRoundParts = new Dictionary<string, string>();
    private string GetAllData_full = "";

    public SslServer(SharedMRLdata sharedData, MRLservContext sql_context) {
        _sql_context = sql_context;  // _context is now available for use in this class
        _sharedData = sharedData;
        // Load the SSL certificate (assuming it’s in the current directory).
        serverCertificate = new X509Certificate2("Certificate/cacertificate.pfx", "LifonOrcaMRL5687"); // Update with your certificate path and password

        // Start the SSL server
        TcpListener listener = new TcpListener(IPAddress.Any, 4242);
        listener.Start();
        Console.WriteLine("Server started on port 4242.");

        _ = AcceptClientsAsync(listener);
        /*
                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    Console.WriteLine("Client connected.");
                    Task.Run(() => HandleClient(client));
                }
        */
    }

    private async Task AcceptClientsAsync(TcpListener listener) {
        while (true) {
            TcpClient client = await listener.AcceptTcpClientAsync();
            Console.WriteLine("Client connected.");

            // Assuming 'tcpClient' is your connected TcpClient
            var clientEndpoint = client.Client.RemoteEndPoint as System.Net.IPEndPoint;

            if (clientEndpoint != null) {
                inEPip = clientEndpoint.Address;

                string clientIp = clientEndpoint.Address.ToString();

                if (clientEndpoint.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6) {
                    Console.WriteLine("Client IP (IPv6): " + clientIp);
                } else {
                    Console.WriteLine("Client IP (IPv4): " + clientIp);
                }
            } else {
                Console.WriteLine("Unable to retrieve client IP address.");
            }

            _ = Task.Run(() => HandleClientAsync(client));
            Thread.Sleep(10);
        }
    }

    private async Task HandleClientAsync(TcpClient client) {
        string liftIdWithoutPrefix = null;
        Task? _sendMessagesTask = null;
        CancellationTokenSource? _cancellationTokenSource;
        _cancellationTokenSource = new CancellationTokenSource();

        try {
            using (SslStream sslStream = new SslStream(client.GetStream(), false)) {
                // Authenticate server
                await sslStream.AuthenticateAsServerAsync(serverCertificate, clientCertificateRequired: false, checkCertificateRevocation: true);
                Console.WriteLine("SSL authentication successful.");

                bool start_SendMessagesPeriodically = false;

                DateTime lastSendTime = DateTime.UtcNow;
                // Olvasás/írás a klienssel (példaként)
                byte[] buffer = new byte[4096];
                while (true) {
                    // Check if the client is still connected
                    if (!client.Connected) {
                        Console.WriteLine("Client disconnected.");
                        break;
                    }

                    int bytesRead = await sslStream.ReadAsync(buffer, 0, buffer.Length);
                    // Ha a bytesRead 0, akkor a kliens lecsatlakozott
                    if (bytesRead == 0) {
                        Console.WriteLine("Client disconnected.");
                        break;
                    } else {
                        string fullText = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                        //Console.WriteLine("fullText received: " + fullText);

                        if (fullText.Contains("HTTP")) {
                            break;
                        } else if (fullText.Contains("MessageEnd")) {
                            // Regular expression to match the value after "_Lift ID: "
                            string pattern = @"_Lift ID:\s(0x[0-9A-Fa-f]+)";

                            Match match = Regex.Match(fullText, pattern);

                            if (match.Success) {
                                string liftIdHex = match.Groups[1].Value; // Hex string (including 0x prefix)
                                liftIdWithoutPrefix = liftIdHex.Substring(2); // Remove "0x" prefix

                                _sharedData.SetData(liftIdWithoutPrefix, "myID", liftIdWithoutPrefix);  // Különböző típusú adatokat is beállíthatunk

                                DateTime TimeStamp;
                                int DoorStateA;
                                int DoorStateB;
                                int ElevatorState;
                                int Travel1;
                                int Travel2;
                                int[] VVVFErrors = new int[5];
                                string Errors;

                                // Regex-ek az egyes értékek kinyeréséhez
                                TimeStamp = DateTime.Parse(Regex.Match(fullText, @"_TimeStamp:\s*(\d{4}\.\d{2}\.\d{2} \d{2}:\d{2}:\d{2})").Groups[1].Value);
                                DoorStateA = int.Parse(Regex.Match(fullText, @"_DoorStateA:\s*(\d+)").Groups[1].Value);
                                DoorStateB = int.Parse(Regex.Match(fullText, @"_DoorStateB:\s*(\d+)").Groups[1].Value);
                                ElevatorState = int.Parse(Regex.Match(fullText, @"_ElevatorState:(\d+)").Groups[1].Value);

                                // Travells kinyerése és felosztása
                                var travelMatch = Regex.Match(fullText, @"_Travells:\s*(\d+)/(\d+)");
                                Travel1 = int.Parse(travelMatch.Groups[1].Value);
                                Travel2 = int.Parse(travelMatch.Groups[2].Value);

                                // VVVF errors kinyerése és tömbbe rendezése
                                var vvErrors = Regex.Match(fullText, @"_VVVF errors:\s*([0-9,]+)").Groups[1].Value;
                                VVVFErrors = Array.ConvertAll(vvErrors.Split(','), int.Parse);

                                // Errors kinyerése
                                Errors = Regex.Match(fullText, @"_Errors:\s*(\w+)").Groups[1].Value;

                                _sharedData.SetData(liftIdWithoutPrefix, "TimeStamp", TimeStamp);
                                _sharedData.SetData(liftIdWithoutPrefix, "DoorStateA", DoorStateA);
                                _sharedData.SetData(liftIdWithoutPrefix, "DoorStateB", DoorStateB);
                                _sharedData.SetData(liftIdWithoutPrefix, "ElevatorState", ElevatorState);
                                _sharedData.SetData(liftIdWithoutPrefix, "Travel1", Travel1);
                                _sharedData.SetData(liftIdWithoutPrefix, "Travel2", Travel2);
                                _sharedData.SetData(liftIdWithoutPrefix, "VVVFErrors", VVVFErrors);
                                _sharedData.SetData(liftIdWithoutPrefix, "Errors", Errors);
                                /*
                                var newMRL = new MRLclass();
                                newMRL.UID = "5555555";
                                newMRL.telepitesHelye = "Z Bajor u. 7.";
                                _sql_context.MRLclass.Add(newMRL);
                                */

                                // Find the last row with the matching UID
                                var existingMRL = _sql_context.MRLmodel
                                                   .Where(m => m.UID == "313437303139510B00330027")
                                                   .OrderByDescending(m => m.ID)  // Assuming 'Id' is an auto-incremented primary key or timestamp
                                                   .FirstOrDefault();

                                if (existingMRL != null) {
                                    existingMRL.utolsoKapcsolataLifttel = TimeStamp;
                                    _sql_context.SaveChanges();
                                }

                                _sql_context.MRLtelemetryModel.Add(
                                    new MRLtelemetryModel {
                                        UID = liftIdWithoutPrefix,
                                        utolsoKapcsolataLifttel = TimeStamp,
                                        DoorStateA = DoorStateA,
                                        DoorStateB = DoorStateB,
                                        ElevatorState = ElevatorState,
                                        Travel1 = Travel1,
                                        Travel2 = Travel2,
                                        VVVFErrors = string.Join(",", VVVFErrors),
                                        Errors = Errors
                                    }
                                );
                                _sql_context.SaveChanges();

                                //await _sql_context.SaveChangesAsync();
                            } else {
                                Console.WriteLine("Lift ID not found.");
                            }
                        } else if (fullText.Contains("ResponseEnd")) {
                            // Regular expression to match the value after "_Lift ID: "
                            string pattern = @"_Lift ID:\s(0x[0-9A-Fa-f]+)";

                            Match match = Regex.Match(fullText, pattern);

                            if (match.Success) {
                                string liftIdHex = match.Groups[1].Value; // Hex string (including 0x prefix)
                                liftIdWithoutPrefix = liftIdHex.Substring(2); // Remove "0x" prefix

                                //_sharedData.SetData(liftIdWithoutPrefix, "myID", liftIdWithoutPrefix);  // Különböző típusú adatokat is beállíthatunk
                                int resp = int.Parse(Regex.Match(fullText, @"_Response:(\d+)").Groups[1].Value);
                                Console.WriteLine("Response received: " + liftIdWithoutPrefix + ", ", +resp);
                            } else {
                                Console.WriteLine("Lift ID not found.");
                            }
                        } else if (fullText.Contains("Ezt most kihagyjuk --- AllData ---")) {       // TODO kiütve
                            // Regular expression to match the value after "_Lift ID: "
                            string pattern = @"_Lift ID:\s(0x[0-9A-Fa-f]+)";

                            Match match = Regex.Match(fullText, pattern);

                            if (match.Success) {
                                string liftIdHex = match.Groups[1].Value; // Hex string (including 0x prefix)
                                liftIdWithoutPrefix = liftIdHex.Substring(2); // Remove "0x" prefix

                                //_sharedData.SetData(liftIdWithoutPrefix, "myID", liftIdWithoutPrefix);  // Különböző típusú adatokat is beállíthatunk
                                //int resp = int.Parse(Regex.Match(fullText, @"_Response:(\d+)").Groups[1].Value);
                                Console.WriteLine("AllData received: " + fullText);

                                // Az üzenet több csomagban érkezik. Meg kell várni a végét, addig összefűzni - az elejét és végét levágni - , majd a shared datában megosztani.

                                string startDelimiter = "\r\n\r\n";
                                string endDelimiter = "\r\nRound END\r\n --- AllDataEnd ---";
                                string base_text = "";

                                int startIndex = fullText.IndexOf(startDelimiter);
                                if (startIndex >= 0) {
                                    base_text = fullText.Substring(startIndex + startDelimiter.Length);
                                }

                                int endIndex = base_text.IndexOf(endDelimiter);
                                if (endIndex >= 0) {
                                    base_text = base_text.Substring(0, endIndex);
                                }

                                if (fullText.Contains("Round: 1\r\n"))  // Most kezdődik
                                {
                                    GetAllDataRoundParts[liftIdWithoutPrefix] = base_text;
                                } else if (fullText.Contains("Round: 19\r\n"))   // Ez a vége
                                {
                                    GetAllDataRoundParts[liftIdWithoutPrefix] = GetAllDataRoundParts[liftIdWithoutPrefix] + base_text;

                                    // TODO adtbázisba vagy a megosztott adatokba

                                } else {
                                    GetAllDataRoundParts[liftIdWithoutPrefix] = GetAllDataRoundParts[liftIdWithoutPrefix] + base_text;
                                }



                            } else {
                                Console.WriteLine("Lift ID not found.");
                            }
                        }

                        if (liftIdWithoutPrefix != null && _sharedData.GetData(liftIdWithoutPrefix, "GetAllDataInProgress") != null) {
                            if ((bool)_sharedData.GetData(liftIdWithoutPrefix, "GetAllDataInProgress") == true) // Erre az adatlekérésre várunk
                            {
                                // Megy az adatok beérkezésére a várakozás. Addig minden bejövő üzenetet egy string-be gyűjtünk, és a végén kiértékeljük
                                if (GetAllData_full == null) {
                                    GetAllData_full = "";
                                }
                                GetAllData_full = GetAllData_full + fullText;


                                if (fullText.Contains("--- AllDataEnd ---")) {

                                    //Console.WriteLine("A teljes adatcsomag unprocessed: " + GetAllData_full);

                                    _sharedData.SetData(liftIdWithoutPrefix, "GetAllDataInProgress", false);


                                    string processd_text = "";

                                    string pattern = @"_Lift ID:\s(0x[0-9A-Fa-f]+)";    // Mégegy ellenőrzés, hogy a megfelelő lifttől van a válasz
                                    Match match = Regex.Match(fullText, pattern);
                                    if (match.Success && match.Groups[1].Value.Substring(2) == liftIdWithoutPrefix) {

                                        string tmp_Text = Regex.Replace(GetAllData_full, @"\r\n+", "");

                                        string startDelimiter = "--- AllData ---";  // Ha más szöveg is benne lenne, azt levágjuk az elejéről
                                        int startIndex = fullText.IndexOf(startDelimiter);
                                        if (startIndex >= 0) {
                                            tmp_Text = tmp_Text.Substring(startIndex + startDelimiter.Length);
                                        }

                                        startDelimiter = "RoundSTART";
                                        startIndex = fullText.IndexOf(startDelimiter);
                                        if (startIndex >= 0) {
                                            tmp_Text = tmp_Text.Substring(startIndex + startDelimiter.Length);
                                        }

                                        // Regular expression to match the sections to remove
                                        string pattern2 = @"RoundEND--- AllData ---\s+Round: \d+_Lift ID: 0x[0-9a-fA-F]+RoundSTART";                                        
                                        processd_text = Regex.Replace(tmp_Text, pattern2, "", RegexOptions.Singleline); // Replace matched sections with an empty string

                                        pattern2 = @"RoundEND --- AllDataEnd ---$";
                                        processd_text = Regex.Replace(processd_text, pattern2, "");


                                        Console.WriteLine("A teljes adatcsomag processed: " + processd_text);

                                        GetAllData_full = "";
                                    }
                                }
                            } else {
                                GetAllData_full = "";
                            }
                        } else {
                            GetAllData_full = "";
                        }


                        if (start_SendMessagesPeriodically == false && liftIdWithoutPrefix != null) {
                            start_SendMessagesPeriodically = true;
                            _sendMessagesTask = Task.Run(async () => await SendMessagesPeriodically(sslStream, liftIdWithoutPrefix, _cancellationTokenSource.Token));  // Modified line: Periodic sending

                        }
                    }

                    //Console.WriteLine("Received: " + Encoding.UTF8.GetString(buffer, 0, bytesRead));

                    Thread.Sleep(20);
                }

            }
        }
        catch (Exception ex) {
            Console.WriteLine("Hiba: " + ex.Message);

            Exception inner = ex.InnerException;
            while (inner != null) {
                Console.WriteLine("Inner Exception: " + inner.Message);
                inner = inner.InnerException;
            }
        }
        finally {
            _cancellationTokenSource?.Cancel();
            try {
                if (_sendMessagesTask != null) {
                    await _sendMessagesTask; // Optional: Await if you want to ensure it finishes cleanly
                }
            }
            catch (TaskCanceledException) {
                Console.WriteLine("TaskCanceledException");
            }
            finally {
                Console.WriteLine("TaskEnded");
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }
            client.Close();
            Console.WriteLine("Client closed");
        }
    }

    private async Task SendMessagesPeriodically(SslStream sslStream, string lift_id, CancellationToken cancellationToken) {
        int cnt = 100;

        try {
            while (!cancellationToken.IsCancellationRequested) {
                await Task.Delay(1000);  // Check the message buffer priodically and send if exist
                byte[] messageToSend;

                Console.WriteLine("SendMessagesPeriodically: " + cnt);

                if ((_sharedData.GetData(lift_id, "GetAllDataInProgress") == null || (bool)_sharedData.GetData(lift_id, "GetAllDataInProgress") == false)) {     // TODO mindent felsorolni, új lekérés nem mehet ki, ameddig valamelyik fut
                    var data = _sharedData.GetData("313437303139510B00330027", "sendToFloor");
                    if (data is not null and > (object)0) {
                        messageToSend = Encoding.UTF8.GetBytes("_Com:SendToFloor," + (int)data + "&");
                        _sharedData.DeleteSubKey("313437303139510B00330027", "sendToFloor");
                        // Send the message asynchronously
                        await sslStream.WriteAsync(messageToSend, 0, messageToSend.Length);
                        Console.WriteLine("Sent periodic message: " + System.Text.Encoding.UTF8.GetString(messageToSend));
                    } else {
                        messageToSend = Encoding.UTF8.GetBytes("_Com:AuthTravel," + cnt + "&");
                        cnt++;
                        //await sslStream.WriteAsync(messageToSend, 0, messageToSend.Length);
                        //Console.WriteLine("Sent periodic message: " + System.Text.Encoding.UTF8.GetString(messageToSend));
                    }

                    data = _sharedData.GetData("313437303139510B00330027", "GetAllData");
                    if (data is int intData && intData == 1) {
                        _ = Task.Run(async () => await WaitForMessages(lift_id, "GetAllDataInProgress", cancellationToken));

                        messageToSend = Encoding.UTF8.GetBytes("_Com:GetAllData," + (int)data + "&");
                        _sharedData.DeleteSubKey("313437303139510B00330027", "GetAllData");
                        // Send the message asynchronously
                        await sslStream.WriteAsync(messageToSend, 0, messageToSend.Length);
                        Console.WriteLine("Sent periodic message: " + System.Text.Encoding.UTF8.GetString(messageToSend));
                    }

                }
            }
        } catch (TaskCanceledException) {
            Console.WriteLine("SendMessagesPeriodically Canceled");
        } catch (Exception ex) {
            // Handle other exceptions
            Console.WriteLine($"SendMessagesPeriodically Killed forcefully. Error: {ex.Message}");
        }    
    }

    // Elindítottunk egy adat igénylést. Felügyelni kell, hogy a megadott idő alatt megkaptuk-e
    private async Task WaitForMessages(string lift_id, string strg_key, CancellationToken cancellationToken) {
        _sharedData.SetData(lift_id, strg_key, true);
        bool excape_flag = false;
        int TIM_wait_cnt = 0;
        while (!excape_flag) {

            if (_sharedData.GetData(lift_id, "GetAllDataInProgress") != null) {
                if ((bool)_sharedData.GetData(lift_id, "GetAllDataInProgress") == true){ // Erre az adatlekérésre várunk
                
                    await Task.Delay(10);
                    TIM_wait_cnt++;
                    if (TIM_wait_cnt >= 500){ // Ennyi idő alatt nem lett meg az adat                    
                        _sharedData.SetData(lift_id, "GetAllDataInProgress", false);
                        excape_flag = true;
                    } else if ((bool)_sharedData.GetData(lift_id, "GetAllDataInProgress") == false){ // Befejeződött, a feldolgozó rész megkapta
                        excape_flag = true;
                    }
                }
            }


        }
    }


    //------------ Nem használt
    private static void HandleClient(TcpClient client) {
        using (var sslStream = new SslStream(client.GetStream(), false)) {
            try {
                // Authenticate the server
                sslStream.AuthenticateAsServer(serverCertificate, clientCertificateRequired: false, enabledSslProtocols: SslProtocols.Tls12, checkCertificateRevocation: true);

                Console.WriteLine("SSL authentication successful.");

                // Read data from client
                byte[] buffer = new byte[4096];
                int bytesRead;
                while ((bytesRead = sslStream.Read(buffer, 0, buffer.Length)) > 0) {
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine("Received: " + message);
                }
            }
            catch (Exception ex) {
                Console.WriteLine("SSL Authentication failed: " + ex.Message);
                // Végigmegyünk az összes inner exception-ön
                Exception inner = ex.InnerException;
                while (inner != null) {
                    Console.WriteLine("Inner Exception: " + inner.Message);
                    inner = inner.InnerException;
                }
            }
        }

        client.Close();
    }
}




