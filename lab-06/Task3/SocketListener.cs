using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ClientServer3{
    public class SocketListener{
        //jeżeli koniec == true kończy działanie programu
        public bool finished = false;
        private string path = Directory.GetCurrentDirectory();
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
            if(abc == "!end"){  
                finished = true;
            } else if(abc == "list"){
                string response = GetLocalFiles();
                client.SendMessage(response, isServer:true);
            } else if(abc.StartsWith("in ")){
                string name = abc.Split(" ")[1];
                bool dirExists = false;
                foreach(string dir in Directory.GetDirectories(path)){
                    FileSystemInfo fsi = new FileInfo(dir);
                    if(fsi.Name == name)
                        dirExists = true; 
                }

                if(dirExists){
                    path += "\\" + name;
                    string response = GetLocalFiles();
                    client.SendMessage(response, isServer:true);
                } else {
                    if(name == ".."){
                        DirectoryInfo di = new DirectoryInfo(path);
                        var p = di.Parent;
                        if(p != null){
                            path = p.FullName;
                            string r = GetLocalFiles();
                            client.SendMessage(r, isServer:true);
                        } else {
                            client.SendMessage("Nie można przejśc do katalogu nadrzędnego.", isServer:true);
                        }
                    } else{
                        client.SendMessage("Katalog nie istnieje.", isServer:true);
                    } 
                }
                
            } else {
                client.SendMessage("Nieznane polecenie", isServer:true);
            }
        }

        private string GetLocalFiles(){
            StringBuilder response = new StringBuilder();
            foreach(string file in Directory.GetFiles(path)){
                FileSystemInfo fsi = new FileInfo(file);
                response.Append(fsi.Name).Append(", "); 
            }
            foreach(string dir in Directory.GetDirectories(path)){
                FileSystemInfo fsi = new FileInfo(dir);
                response.Append(fsi.Name).Append(", "); 
            }
            if(response.Length > 2)
                response.Remove(response.Length - 2, 2);
            else
                response.Append(' ');
            return response.ToString();
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
                Console.WriteLine(e.ToString() + "168");
            }
        }
    }
}