using System.Net.Sockets;
using System.Text;

namespace ClientServer {
    public class Client {
        private Socket _socket;
        private Action<string, Client> _receivedMessage;
        private Action<Client> _finishConnection;
        private Thread _thread;
        private byte[] _buffer;
        public bool Finished { get; private set; } = false;
        public string? FirstName { get; set; }

        public Client(Socket socket, Action<string, Client> receivedMessage, Action<Client> finishConnection) {
            _socket = socket;
            _receivedMessage = receivedMessage;
            _finishConnection = finishConnection;
            _buffer = new byte[1024];
            _thread = new Thread(HandleCommunication);
        }

        public void Start() {
            _thread.Start();
        }

        private void HandleCommunication() {
            try {
                while (!Finished) {
                    int bytesRec = _socket.Receive(_buffer);
                    if (bytesRec > 0) {
                        string data = Encoding.UTF8.GetString(_buffer, 0, bytesRec);
                        _receivedMessage(data, this);
                    } else {
                        // połączenie zamknięte
                        FinishConnection();
                    }
                }
            } catch (SocketException) {
                // Wystąpił błąd połączenia
                FinishConnection();
            } catch (Exception ex) {
                Console.WriteLine("Błąd klienta: " + ex.Message);
                FinishConnection();
            }
        }

        public void SendMessage(string message, bool addNewLine = false) {
            try {
                if (addNewLine)
                    message += "\n";
                byte[] msg = Encoding.UTF8.GetBytes(message);
                _socket.Send(msg);
            } catch (Exception) {
                FinishConnection();
            }
        }

        public void FinishConnection() {
            if (Finished)
                return;
            try {
                Finished = true;
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
            } catch {}
            _finishConnection(this);
        }
    }
}
