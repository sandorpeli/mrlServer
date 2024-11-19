using MRLserver;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using MRLserver.Data;
using MRLserver.Models;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;


public class SslServer
{
    private static X509Certificate2 serverCertificate;
    private SharedMRLdata _sharedData;
    private IPAddress inEPip;
    private readonly MRLservContext _sql_context;

    public SslServer(SharedMRLdata sharedData, MRLservContext sql_context)
    {
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

    private async Task AcceptClientsAsync(TcpListener listener)
    {
        while (true)
        {
            TcpClient client = await listener.AcceptTcpClientAsync();
            Console.WriteLine("Client connected.");

            // Assuming 'tcpClient' is your connected TcpClient
            var clientEndpoint = client.Client.RemoteEndPoint as System.Net.IPEndPoint;

            if (clientEndpoint != null)
            {
                inEPip = clientEndpoint.Address;

                string clientIp = clientEndpoint.Address.ToString();

                if (clientEndpoint.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    Console.WriteLine("Client IP (IPv6): " + clientIp);
                }
                else
                {
                    Console.WriteLine("Client IP (IPv4): " + clientIp);
                }
            }
            else
            {
                Console.WriteLine("Unable to retrieve client IP address.");
            }

            _ = Task.Run(() => HandleClientAsync(client));
            Thread.Sleep(10);
        }
    }

        private async Task HandleClientAsync(TcpClient client)
    {
        try
        {
            using (SslStream sslStream = new SslStream(client.GetStream(), false))
            {
                // Authenticate server
                await sslStream.AuthenticateAsServerAsync(serverCertificate, clientCertificateRequired: false, checkCertificateRevocation: true);
                Console.WriteLine("SSL authentication successful.");

                _ = Task.Run(async () => await SendMessagesPeriodically(sslStream));  // Modified line: Periodic sending

                DateTime lastSendTime = DateTime.UtcNow;
                // Olvasás/írás a klienssel (példaként)
                byte[] buffer = new byte[4096];
                while (true)
                {
                    // Check if the client is still connected
                    if (!client.Connected)
                    {
                        Console.WriteLine("Client disconnected.");
                        break;
                    }

                    int bytesRead = await sslStream.ReadAsync(buffer, 0, buffer.Length);
                    // Ha a bytesRead 0, akkor a kliens lecsatlakozott
                    if (bytesRead == 0)
                    {
                        Console.WriteLine("Client disconnected.");
                        break;
                    }
                    else
                    {
                        string fullText = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                        Console.WriteLine("fullText received: " + fullText);

                        if (fullText.Contains("HTTP"))
                        {
                            break;
                        }
                        else if (fullText.Contains("MessageEnd"))
                        {
                            // Regular expression to match the value after "_Lift ID: "
                            string pattern = @"_Lift ID:\s(0x[0-9A-Fa-f]+)";

                            Match match = Regex.Match(fullText, pattern);

                            if (match.Success)
                            {
                                string liftIdHex = match.Groups[1].Value; // Hex string (including 0x prefix)
                                string liftIdWithoutPrefix = liftIdHex.Substring(2); // Remove "0x" prefix

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

                                if (existingMRL != null)
                                {
                                    existingMRL.utolsoKapcsolataLifttel = TimeStamp;
                                    _sql_context.SaveChanges();
                                }

                                _sql_context.MRLtelemetryModel.Add(
                                    new MRLtelemetryModel
                                    {
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
                            }
                            else
                            {
                                Console.WriteLine("Lift ID not found.");
                            }
                        }
                        else if (fullText.Contains("ResponseEnd"))
                        {
                            // Regular expression to match the value after "_Lift ID: "
                            string pattern = @"_Lift ID:\s(0x[0-9A-Fa-f]+)";

                            Match match = Regex.Match(fullText, pattern);

                            if (match.Success)
                            {
                                string liftIdHex = match.Groups[1].Value; // Hex string (including 0x prefix)
                                string liftIdWithoutPrefix = liftIdHex.Substring(2); // Remove "0x" prefix

                                //_sharedData.SetData(liftIdWithoutPrefix, "myID", liftIdWithoutPrefix);  // Különböző típusú adatokat is beállíthatunk
                                int resp = int.Parse(Regex.Match(fullText, @"_Response:(\d+)").Groups[1].Value);
                                Console.WriteLine("Response received: " + liftIdWithoutPrefix + ", ", +resp);
                            }
                            else
                            {
                                Console.WriteLine("Lift ID not found.");
                            }
                        }
                        else if (fullText.Contains("--- AllData ---"))
                        {
                            // Regular expression to match the value after "_Lift ID: "
                            string pattern = @"_Lift ID:\s(0x[0-9A-Fa-f]+)";

                            Match match = Regex.Match(fullText, pattern);

                            if (match.Success)
                            {
                                string liftIdHex = match.Groups[1].Value; // Hex string (including 0x prefix)
                                string liftIdWithoutPrefix = liftIdHex.Substring(2); // Remove "0x" prefix

                                //_sharedData.SetData(liftIdWithoutPrefix, "myID", liftIdWithoutPrefix);  // Különböző típusú adatokat is beállíthatunk
                                //int resp = int.Parse(Regex.Match(fullText, @"_Response:(\d+)").Groups[1].Value);
                                Console.WriteLine("Response received: " + fullText);


                                /*
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
                                */

                                /*
                                var newMRL = new MRLclass();
                                newMRL.UID = "5555555";
                                newMRL.telepitesHelye = "Z Bajor u. 7.";
                                _sql_context.MRLclass.Add(newMRL);
                                */

                                /*
                                // Find the last row with the matching UID
                                var existingMRL = _sql_context.MRLmodel
                                                   .Where(m => m.UID == "313437303139510B00330027")
                                                   .OrderByDescending(m => m.ID)  // Assuming 'Id' is an auto-incremented primary key or timestamp
                                                   .FirstOrDefault();

                                if (existingMRL != null)
                                {
                                    existingMRL.utolsoKapcsolataLifttel = TimeStamp;
                                    _sql_context.SaveChanges();
                                }

                                _sql_context.MRLtelemetryModel.Add(
                                    new MRLtelemetryModel
                                    {
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
                                */
                                //await _sql_context.SaveChangesAsync();
                            }
                            else
                            {
                                Console.WriteLine("Lift ID not found.");
                            }
                        }
                    }

                    Console.WriteLine("Received: " + Encoding.UTF8.GetString(buffer, 0, bytesRead));

                    Thread.Sleep(20);
                }

                /*
                // The IP address and port of the server you want to connect to
                string ipAddress = inEPip.ToString(); // Example IP address
                int port = 22; // Example port

                // The message you want to send
                string message = "Hello, TCP Server!";

                try
                {
                    // Create a TcpClient instance
                    using (TcpClient client2 = new TcpClient())
                    {
                        // Connect to the server
                        client2.Connect(IPAddress.Parse(ipAddress), port);
                        Console.WriteLine($"Connected to {ipAddress}:{port}");

                        // Get the network stream to send data
                        NetworkStream stream = client2.GetStream();

                        // Convert the message to bytes and send it
                        byte[] data = Encoding.ASCII.GetBytes(message);
                        stream.Write(data, 0, data.Length);
                        Console.WriteLine("Message sent.");

                        // Close the connection
                        stream.Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
                */

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Hiba: " + ex.Message);

            Exception inner = ex.InnerException;
            while (inner != null)
            {
                Console.WriteLine("Inner Exception: " + inner.Message);
                inner = inner.InnerException;
            }
        }
        finally
        {
            client.Close();
        }
    }

    private async Task SendMessagesPeriodically(SslStream sslStream)
    {
        int cnt = 100;

        while (true)
        {
            await Task.Delay(10000);  // Send a message every 10 seconds
            byte[] messageToSend = Encoding.UTF8.GetBytes("_Com:AuthTravel," + cnt + "&");

            var data = _sharedData.GetData("313437303139510B00330027", "sendToFloor");

            if (data is not null and > (object)0)
            {
                messageToSend = Encoding.UTF8.GetBytes("_Com:SendToFloor," + (int)data + "&");
                _sharedData.DeleteSubKey("313437303139510B00330027", "sendToFloor");
                // Send the message asynchronously
                await sslStream.WriteAsync(messageToSend, 0, messageToSend.Length);
                Console.WriteLine("Sent periodic message: " + System.Text.Encoding.UTF8.GetString(messageToSend));
            } else
            {
                cnt++;
                await sslStream.WriteAsync(messageToSend, 0, messageToSend.Length);
                Console.WriteLine("Sent periodic message: " + System.Text.Encoding.UTF8.GetString(messageToSend));
            }

            data = _sharedData.GetData("313437303139510B00330027", "GetAllData");
            if (data is not null and > (object)0)
            {
                messageToSend = Encoding.UTF8.GetBytes("_Com:GetAllData," + (int)data + "&");
                _sharedData.DeleteSubKey("313437303139510B00330027", "GetAllData");
                // Send the message asynchronously
                await sslStream.WriteAsync(messageToSend, 0, messageToSend.Length);
                Console.WriteLine("Sent periodic message: " + System.Text.Encoding.UTF8.GetString(messageToSend));
            }


        }
    }

    //------------ Nem használt
    private static void HandleClient(TcpClient client)
    {
        using (var sslStream = new SslStream(client.GetStream(), false))
        {
            try
            {
                // Authenticate the server
                sslStream.AuthenticateAsServer(serverCertificate, clientCertificateRequired: false, enabledSslProtocols: SslProtocols.Tls12, checkCertificateRevocation: true);

                Console.WriteLine("SSL authentication successful.");

                // Read data from client
                byte[] buffer = new byte[4096];
                int bytesRead;
                while ((bytesRead = sslStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine("Received: " + message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("SSL Authentication failed: " + ex.Message);
                // Végigmegyünk az összes inner exception-ön
                Exception inner = ex.InnerException;
                while (inner != null)
                {
                    Console.WriteLine("Inner Exception: " + inner.Message);
                    inner = inner.InnerException;
                }
            }
        }

        client.Close();
    }
}




