using ClientServer;

public class MainClass{
    public static void Main(string[] args){
        if(args[0] == "task1"){
            if(args[1] == "client"){
                SocketClient.MainMethod();
            } else if(args[1] == "server"){
                SocketListener.MainMethod();
            } else {
                Console.WriteLine("Niepoprawna komenda");
            }
        }
    } 
}