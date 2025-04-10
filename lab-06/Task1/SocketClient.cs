using System.Net;
using System.Net.Sockets;

namespace ClientServer1024{
    public class SocketClient
        {
            public bool finished = false;
            public static int MainMethod()
            {
                //tworzenie nowego wątku klienta
                SocketClient sc = new SocketClient();
                Task.Run(() => sc.StartClient());
                //główny wątek gotowy jest np. na przyjmowanie
                //komend z klawiatury, w tym wypadku program działa tak
                //długo jak nie rozłączy się z nim serwer
                while (!sc.finished){
                    Thread.Sleep(100);
                }
                return 0;
            }
            //callback do funkcji odbierającej dane
            public void ReceivedMessage(string abc, Client client)
            {
                Console.WriteLine("Klient odebrał wiadomość: " + abc);
            }

            //funkcja do obsługi callbacka kiedy połączenie z serwerem zostanie zakończone
            //ustawiamy koniec == true, co zastopuje główny program
            public void FinishConnection(Client client)
            {
                finished = true;
            }
            //start wątku klienta
            public void StartClient()
            {
                //zakładamy, że wiadomość nie przekroczy 1024 bajtów
                byte[] bytes = new byte[1024];

                try
                {
                    // Łączymy się z serwerm lokalhości na wysokim porcie, np. 11000
                    // jeśli host ma wiele adresów, dostajemy listę adresów
                    IPHostEntry host = Dns.GetHostEntry("localhost");
                    IPAddress ipAddress = host.AddressList[0];
                    IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

                    // Tworzymy socket TCP/IP
                    Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                    // łączymy się ze zdalnym endpointem i przechwytujemy wyjątki
                    try
                    {
                        sender.Connect(remoteEP);
                        Client client = new Client(sender, ReceivedMessage, FinishConnection);
                        client.Start();

                        Task.Run(() =>
                        {
                            while (!client.Finished)
                            {
                                string? message = Console.ReadLine();
                                if(!string.IsNullOrEmpty(message)){
                                    client.SendMessage(message);
                                    if(message == "quit")
                                        client.FinishConnection();
                                }
                                Thread.Sleep(100);
                            }
                        });
                    }
                    catch (ArgumentNullException ane)
                    {
                        Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                    }
                    catch (SocketException se)
                    {
                        Console.WriteLine("SocketException : {0}", se.ToString());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Unexpected exception : {0}", e.ToString());
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }
}