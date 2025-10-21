using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ChatClient
{
    class Program
    {
        private const string Ip = "127.0.0.1";
        private const int Port = 8888;

        static void Main(string[] args)
        {
            Console.Title = "TCP Chat İstemcisi";

            try
            {
                IPEndPoint remoteEp = new IPEndPoint(IPAddress.Parse(Ip), Port);
                Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    sender.Connect(remoteEp); 
                    Console.WriteLine($"Sunucuya bağlandı: {sender.RemoteEndPoint}");

                    Thread receiverThread = new Thread(() => Receive(sender));
                    receiverThread.Start();

                    string message;
                    while (true)
                    {
                        Console.Write("Mesajınız: ");
                        message = Console.ReadLine();

                        if (message.ToLower() == "exit")
                        {
                            break;
                        }

                        byte[] msg = Encoding.UTF8.GetBytes(message);
                        sender.Send(msg);
                    }
                }
                catch (ArgumentNullException ex)
                {
                    Console.WriteLine($"ArgumentNullException: {ex.ToString()}");
                }
                catch (SocketException ex)
                {
                    Console.WriteLine($"Sunucuya bağlanılamadı. SocketException: {ex.ToString()}");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Genel Hata: {ex.ToString()}");
            }
            finally
            {
                Console.WriteLine("Bağlantı kapatılıyor...");
            }
        }

        static void Receive(Socket clientSocket)
        {
            byte[] bytes = new byte[1024];

            while (true)
            {
                try
                {
                    int bytesRec = clientSocket.Receive(bytes);
                    if (bytesRec > 0)
                    {
                        string data = Encoding.UTF8.GetString(bytes, 0, bytesRec);

                        
                        Console.SetCursorPosition(0, Console.CursorTop);
                        Console.Write(new string(' ', Console.WindowWidth - 1));
                        Console.SetCursorPosition(0, Console.CursorTop);

                        Console.WriteLine($"\nSunucudan: {data}");

                        Console.Write("Mesajınız: ");
                    }
                    else if (bytesRec == 0)
                    {
                        break;
                    }
                }
                catch (SocketException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    break;
                }
            }
        }
    }
}