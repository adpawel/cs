using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ClientServer3{
    public class Client{
        private bool finished = false;
        public bool Finished
        { 
            get { return finished; }
            set { finished = value; }    
        }
        private Task? receivingTask = null;
        public Task? ReceivingTask{
            get { return receivingTask; }
            set { receivingTask = value; }    
        }
        private Socket? clientSocket = null;
        public Socket? ClientSocket{
            get { return clientSocket; }
            set { clientSocket = value; }    
        }
        private Action<string, Client>? receivingCallback = null;
        private Action<Client>? finishCallback = null;
        private string firstName;
        public string FirstName{
            get{ return firstName; }
            set{ firstName = value; }
        }

        public string Name { get{
            IPEndPoint? remoteIpEndPoint = clientSocket?.RemoteEndPoint as IPEndPoint;
            if (remoteIpEndPoint != null)
                {
                    return "" + remoteIpEndPoint.Address + ":" + remoteIpEndPoint.Port;
                }
                else return "";
        }}

        private static volatile int count = 0;
        public Client(Socket clientSocket, Action<string, Client>? receivingCallback = null, Action<Client>? finishCallback = null){
            this.clientSocket = clientSocket;
            this.receivingCallback = receivingCallback;
            this.finishCallback = finishCallback;
            firstName = $"Klient{Interlocked.Increment(ref count)}";      
        }

        public void FinishConnection(){
                Console.WriteLine("Klient: zakończenie połączenia");
                finished = true;
                //jeśli gniazdo byłoby już zamknięte, łapiemy wyjątek
                try {
                    clientSocket?.Shutdown(SocketShutdown.Both);
                    clientSocket?.Close();
                    if (finishCallback != null)
                        finishCallback(this);
                }
                catch (Exception e){ 
                    Console.WriteLine(e.Message  + "59");
                }
        }

        public bool IsConnected(){
            if (clientSocket == null) return false;
            try
            {
                return !(clientSocket.Poll(1, SelectMode.SelectRead) && clientSocket.Available == 0);
            } catch(SocketException){
                return false;
            }
        }

        public void SendMessage(string message, bool isServer = false){
            if (clientSocket == null) return;

            if (isServer)
                Console.WriteLine($"Serwer: {message}");
            else
                Console.WriteLine($"Klient wysyła wiadomość: {message}");

            byte[] msg = Encoding.UTF8.GetBytes(message);

            int size = msg.Count();
            byte[] sizeMsg = Encoding.UTF8.GetBytes(size.ToString() + "#");
            clientSocket.Send(sizeMsg);
            clientSocket.Send(msg);
        }

        public void ReceiveMessage(string message, Client client){
                if (receivingCallback != null)
                    receivingCallback(message, client);
                else//altrnatywna obsługa jeśli nie ma callbacka
                {
                    //zrób coś :-)
                }
        }

        public void Start(){
            receivingTask = Task.Run(() =>{
                Console.WriteLine(Name + ": start wątku odbierającego dane");
                while (!finished)
                {
                    try
                    {
                        // 1. Odczytaj rozmiar wiadomości zakończony separatorem '#'
                        List<byte> sizeBytes = new List<byte>();
                        while (true)
                        {
                            byte[] oneByte = new byte[1];
                            int read = clientSocket?.Receive(oneByte, 0, 1, SocketFlags.None) ?? 0;

                            if (read == 0)
                                throw new Exception("Rozłączono klienta (podczas odczytu rozmiaru)");
                            
                            if (oneByte[0] == (byte)'#')
                                break;

                            sizeBytes.Add(oneByte[0]);
                        }

                        // 2. Sparsuj długość
                        string sizeStr = Encoding.UTF8.GetString(sizeBytes.ToArray());
                        if (!int.TryParse(sizeStr, out int messageSize))
                            throw new Exception("Nieprawidłowy rozmiar wiadomości: " + sizeStr);

                        // 3. Odbierz dokładnie messageSize bajtów treści wiadomości
                        byte[] messageBytes = new byte[messageSize];
                        int totalReceived = 0;
                        while (totalReceived < messageSize)
                        {
                            int read = clientSocket?.Receive(messageBytes, totalReceived, messageSize - totalReceived, SocketFlags.None) ?? 0;
                            if (read == 0)
                                throw new Exception("Rozłączono klienta (podczas odbioru wiadomości)");

                            totalReceived += read;
                        }

                        // 4. Zdekoduj i przekaż dalej
                        string message = Encoding.UTF8.GetString(messageBytes);
                        ReceiveMessage(message, this);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Klient: błąd: " + ex.Message);
                        FinishConnection();
                        break; 
                    }
                }
            });
        }
    }
}