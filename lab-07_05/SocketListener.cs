using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ClientServer{
    public class SocketListener{
        public bool finished = false;
        Socket ?listener = null;
        ConcurrentDictionary<string, Client> clients = new ConcurrentDictionary<string, Client>();
        public void Finish(){
            try{
                Console.WriteLine("Kończenie pracy serwera");
                foreach(string name in clients.Keys)
                {
                    clients[name].FinishConnection();
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

            while (!sl.finished)
            {
                Thread.Sleep(100);
            }
            return 0;
        }

        //funkcja do obsługi callbacka wątków klienta, kiedy klient się rozłącza
        //usuwamy jego obiekt z listy
        public void RemoveWorker(string clientName){
            lock(clients)
            {
                if (clients.ContainsKey(clientName))
                    clients.Remove(clientName, out _);
            }
        }
        //callback obsługujący odbieranie wiadomości
        public void ReceivedMessage(string message, Client client){
            if (client.FirstName == null) {
                if (message.StartsWith("NAME ")) {
                    string desiredName = message.Substring(5).Trim();
                    if (string.IsNullOrWhiteSpace(desiredName) || clients.ContainsKey(desiredName)) {
                        client.SendMessage("ERROR: Nazwa zajęta lub nieprawidłowa", true);
                        client.FinishConnection();
                    } else {
                        client.FirstName = desiredName;
                        clients.TryAdd(desiredName, client);
                        client.SendMessage("OK: Zarejestrowano jako " + desiredName, true);
                    }
                } else {
                    client.SendMessage("ERROR: Najpierw podaj nazwę (NAME <twoja_nazwa>)", true);
                }
                return;
            }

            // Obsługa komend
            if (message.StartsWith("check ")) {
                string nameToCheck = message.Substring(6).Trim();
                if (clients.ContainsKey(nameToCheck)) {
                    client.SendMessage("OK: Klient " + nameToCheck + " jest online", true);
                } else {
                    client.SendMessage("ERROR: Klient " + nameToCheck + " nie istnieje", true);
                }
            } else if (message.StartsWith("send ")) {
                int firstSpace = message.IndexOf(' ', 5);
                if (firstSpace == -1) {
                    client.SendMessage("ERROR: Błędny format SEND <nazwa> <wiadomość>", true);
                    return;
                }
                string targetName = message.Substring(5, firstSpace - 5).Trim();
                string msg = message.Substring(firstSpace + 1).Trim();
                if (clients.TryGetValue(targetName, out var targetClient)) {
                    targetClient.SendMessage($"[Od {client.FirstName}]: {msg}", true);
                    client.SendMessage("OK: Wiadomość wysłana", true);
                } else {
                    client.SendMessage("ERROR: Klient " + targetName + " nie istnieje", true);
                }
            } else if (message == "quit") {
                client.SendMessage("OK: Rozłączanie", true);
                client.FinishConnection();
            } else {
                client.SendMessage("ERROR: Nieznana komenda", true);
            }
        }

        public void StartServer(){
            // Startujemy serwer na 127.0.0.1 na wysokim porcie, np. 11000
            // jeśli host ma wiele adresów, dostajemy listę adresów
            IPHostEntry host = Dns.GetHostEntry("localhost");
            //wybieramy pierwszy adres z listy
            IPAddress ipAddress = host.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

            try {
                listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listener.Bind(localEndPoint);
                listener.Listen(10);

                Console.WriteLine("Serwer czeka na nowe połączenia...");
                while (!finished) {
                    Socket handler = listener.Accept();
                    Console.WriteLine("Nowe połączenie");

                    Client client = new Client(handler, ReceivedMessage, (c) => {
                        if (!string.IsNullOrEmpty(c.FirstName)) {
                            RemoveWorker(c.FirstName);
                        }
                    });

                    client.Start();
                }
            } catch (Exception e) {
                Console.WriteLine("Błąd serwera: " + e.Message);
            }
        }
    }
}