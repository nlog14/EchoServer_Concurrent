using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace EchoServer_Concurrent
{
    class Program
    {
        //Updating server to use SSL
        static string serverCertificateFile = "c:/certificates/ServerSSL.pfx";
        static bool clientCertificateRequired = false;
        static bool checkCertificateRevocation = true;
        static SslProtocols enabledSSLProtocols = SslProtocols.Tls;
        static bool leaveInnerStreamOpen = false;
        static X509Certificate serverCertificate;

        static void Main(string[] args)
        {
            //To use SSL
            serverCertificate = new X509Certificate(serverCertificateFile, "mysecret");

            Console.WriteLine("Server");
            
            TcpListener listener = new TcpListener(System.Net.IPAddress.Loopback, 7);
            listener.Start();

            //while loop keeps the socket running continuously 
            while(true)
            {
                //Accepts clients 
                TcpClient socket = listener.AcceptTcpClient();

                //Creates threads to run "HandleClient" method for each client
                Task.Run(() => { HandleClient(socket); });
            }
        }

        //Method to handle clients 
        public static void HandleClient(TcpClient socket)
        {
            //Updating code so server uses SSL
            Stream unsecureStream = socket.GetStream();
            SslStream sslStream = new SslStream(unsecureStream, leaveInnerStreamOpen);
            sslStream.AuthenticateAsServer(serverCertificate, clientCertificateRequired, enabledSSLProtocols, checkCertificateRevocation);
            StreamReader reader = new StreamReader(sslStream);
            StreamWriter writer = new StreamWriter(sslStream);

            //NetworkStream ns = socket.GetStream();
            //StreamReader reader = new StreamReader(ns);
            //StreamWriter writer = new StreamWriter(ns);




            string message = reader.ReadLine();
            Console.WriteLine("Client wrote:" + message);
            writer.WriteLine(message);
            writer.Flush();
            socket.Close();

        }
    }
}
