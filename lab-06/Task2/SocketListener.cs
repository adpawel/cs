using System.Net;
using System.Net.Sockets;

namespace ClientServer{
    public class SocketListener{
        //jeżeli koniec == true kończy działanie programu
        public bool finished = false;
        //kończy działanie serwera - zamyka wszystkie wątki oraz wyłącza gniazdo
        //nasłuchujące
        //lista z obiektami obsługującymi połączenia full duplex z klientem
        List<Client> workerThreads = new List<Client>();
        Socket ?listener = null;
        public void Finish(){
            try{
                Console.WriteLine("Kończenie pracy serwera");
                lock (workerThreads)
                {
                    foreach(Client client in workerThreads)
                    {
                        client.FinishConnection();
                    }
                }
                listener?.Shutdown(SocketShutdown.Both);
                listener?.Close();
            }
            catch {}
            finished = true;
        }

        public static int MainMethod()
        {
            //tworzenie nowego obiektu serwera, program napisany jest obiektowo 
            //a nie na metodach statycznych
            SocketListener sl = new SocketListener();
            //puszczamy serwer w tle
            Task.Run(sl.StartServer);
            //ponieważ Task jest wątkiem w tle nie kończymy głównego wątku tylko
            //trzymamy go w pętli, jeśli chcemy możemy zrobić obsługę klawiatury itp.

            /* zamyka serwer po 30 sekundach
            Task.Delay(30000).Wait();
            sl.Zakoncz();
            */
            while (!sl.finished)
            {
                Thread.Sleep(100);
            }
            return 0;
        }

        //funkcja do obsługi callbacka wątków klienta, kiedy klient się rozłącza
        //usuwamy jego obiekt z listy
        public void RemoveWorker(Client client){
            lock(workerThreads)
            {
                if (workerThreads.Contains(client))
                    workerThreads.Remove(client);
            }
        }
        //callback obsługujący odbieranie wiadomości
        public void ReceivedMessage(string abc, Client client){
            Console.Write("Serwer: " + client.FirstName + " wysłał: ");
            Console.WriteLine(abc);
            client.SendMessage("odczytałem: " + abc, isServer:true);
            // Console.WriteLine("Serwer: Odczytałem: " + abc);
        }
        public void StartServer(){
            // Startujemy serwer na 127.0.0.1 na wysokim porcie, np. 11000
            // jeśli host ma wiele adresów, dostajemy listę adresów
            IPHostEntry host = Dns.GetHostEntry("localhost");
            //wybieramy pierwszy adres z listy
            IPAddress ipAddress = host.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

            try {
                // tworzymy socket na protokole TCP
                listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                // łączymy socket z adresem metodą BIND
                listener.Bind(localEndPoint);
                // ustawiamy socket w stan nasłuchu, w tym wypadku na maksimum 10 połączeń,
                // które zostaną obsłużone, jeśli będzie ich więcej serwer odpowie, że jest zajęty
                listener.Listen(1);

                // symulacja komunikacji klient - serwer
                // w losowych odstępach czasu wysyłamy wiadomość do każdego wątku klienta 
                Random rand = new Random();
                Task.Run(() =>
                {
                    while (!finished)
                    {
                        // sekcja krytyczna - nie możemy modyfikować kolekcji workerThreads
                        // z innego miejsca programu, jeśli idzie po niej pętla foreach
                        // ta sekcja wyklucza się z sekcją dodawania nowego połączenia do listy
                        // workerThreads
                        lock (workerThreads)
                        {
                            workerThreads.RemoveAll(c => c.Finished);
                        }
                        Thread.Sleep(100);
                    }
                });

                Console.WriteLine("Serwer czeka na nowe połączenia");
                //wątek czeka do zakończenia programu, tak naprawdę program serwera
                while (!finished){
                    Socket handler = listener.Accept();

                    lock(workerThreads){
                        if (workerThreads.Count >= 1) {
                            Console.WriteLine("Zbyt wielu klientów. Rozłączam.");
                            handler.Close();
                            continue;
                        }

                        Console.WriteLine("Odebrano połączenie");
                        Client client = new Client(handler, ReceivedMessage, RemoveWorker);
                        client.Start();
                        workerThreads.Add(client);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}