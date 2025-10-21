
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ChatServer
{
    class Program
    {
        private const string Ip = "127.0.0.1"; 
        private const int Port = 8888;

        static void Main(string[] args)
        {
            Console.Title = "TCP Chat Server";

            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(Ip), Port);

            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(ipEndPoint);
                listener.Listen(10); 
                Console.WriteLine($"Sunucu çalışıyor. Dinlenen adres: {ipEndPoint}");

                while (true)
                {
                    Console.WriteLine("Yeni bir bağlantı bekleniyor...");
                    Socket handler = listener.Accept(); 

                    Console.WriteLine($"İstemci bağlandı: {handler.RemoteEndPoint}");

                    Thread clientThread = new Thread(() => HandleClient(handler));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                listener.Close();
            }
        }

        static void HandleClient(Socket clientSocket)
        {
            byte[] bytes = new byte[1024];

            while (true)
            {
                try
                {
                    int bytesRec = clientSocket.Receive(bytes);
                    string data = Encoding.UTF8.GetString(bytes, 0, bytesRec);

                    if (string.IsNullOrEmpty(data) || data.ToLower() == "exit")
                    {
                        Console.WriteLine($"İstemci ayrıldı: {clientSocket.RemoteEndPoint}");
                        break; 
                    }

                    Console.WriteLine($"Alınan mesaj ({clientSocket.RemoteEndPoint}): {data}");

                    string reply = $"Sunucu aldı: {data}";
                    byte[] msg = Encoding.UTF8.GetBytes(reply);
                    clientSocket.Send(msg);
                }
                catch (SocketException)
                {
                    Console.WriteLine($"İstemci bağlantısı kesildi: {clientSocket.RemoteEndPoint}");
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    break;
                }
            }

            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();

        }
    }
}