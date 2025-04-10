using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ClientServer1024{
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
                    Console.WriteLine(e.Message);
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
            
            // ograniczenie do 1024 bajtów
            if (msg.Length > 1024) {
                byte[] trimmedMsg = new byte[1024];
                Array.Copy(msg, trimmedMsg, 1024);
                msg = trimmedMsg;
            }

            clientSocket.Send(msg);
        }

        public void ReceiveMessage(string message, Client client){
                if (receivingCallback != null)
                    receivingCallback(message, client);
                else//altrnatywna obsługa jeśli nie ma callbacka
                {
                }
        }

        public void Start(){
            receivingTask = Task.Run(() =>{
                Console.WriteLine(Name + ": start wątku odbierającego dane" );
                while (!Finished){
                    try{
                        byte[] bytes = new byte[1024];
                        int bytesRec = ClientSocket?.Receive(bytes) ?? 0;
                        if (bytesRec > 0){
                            string data = Encoding.UTF8.GetString(bytes, 0, bytesRec);
                            ReceiveMessage(data, this);
                        }
                    } catch (Exception ex){
                        Console.WriteLine(Name + ": błąd: " + ex.Message);
                        FinishConnection();
                    }
                }
            });

            Task.Run(() =>
            {   
                while (!Finished)
                {
                    if (!IsConnected())
                        FinishConnection();
                    Thread.Sleep(100);
                }
            });
        }
    }
}
