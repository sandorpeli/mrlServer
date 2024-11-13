using System;
using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Security.Authentication;
using System.Text;
using MRLserver;
using System.Text.RegularExpressions;

public class SslServer
{
    private static X509Certificate2 serverCertificate;
    private SharedMRLdata _sharedData;

    public SslServer(SharedMRLdata sharedData)
	{
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

                // Olvasás/írás a klienssel (példaként)
                byte[] buffer = new byte[4096];
                while (true) { 
                    int bytesRead = await sslStream.ReadAsync(buffer, 0, buffer.Length);
                    // Ha a bytesRead 0, akkor a kliens lecsatlakozott
                    if (bytesRead == 0)
                    {
                        Console.WriteLine("Client disconnected.");
                        break;
                    } else
                    {
                        string fullText = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                        if (fullText.Contains("HTTP"))
                        {
                            break;
                        }
                        else
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
                            }
                            else
                            {
                                Console.WriteLine("Lift ID not found.");
                            }
                        }
                    }

                    Console.WriteLine("Received: " + Encoding.UTF8.GetString(buffer, 0, bytesRead));

                    Thread.Sleep(10);
                }
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




