using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace EchoServer_Concurrent
{
    class Program
    {
        static void Main(string[] args)
        {
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
            NetworkStream ns = socket.GetStream();
            StreamReader reader = new StreamReader(ns);
            StreamWriter writer = new StreamWriter(ns);

            string message = reader.ReadLine();
            Console.WriteLine("Client wrote:" + message);
            writer.WriteLine(message);
            writer.Flush();
            socket.Close();

        }
    }
}
