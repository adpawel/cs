public class Task1{

    public Task1(string type){
        if(type == "client"){
            ClientServer1024.SocketClient.MainMethod();
        } else {
            ClientServer1024.SocketListener.MainMethod();
        }
    }
}

public class Task2{
    public Task2(string type){
        if(type == "client"){
            ClientServer.SocketClient.MainMethod();
        } else {
            ClientServer.SocketListener.MainMethod();
        }
    }
}

public class Task3{
    public Task3(string type){
        if(type == "client"){
            ClientServer3.SocketClient.MainMethod();
        } else {
            ClientServer3.SocketListener.MainMethod();
        }
    }
}