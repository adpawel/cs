public class MainClass{
    public static void Main(string[] args){
        if(args[0] == "task1"){
            Task1 t1 = new Task1(args[1]);
        } else if(args[0] == "task2"){
            Task2 t2 = new Task2(args[1]);
        } else if(args[0] == "task3"){
            Task3 t3 = new Task3(args[1]);
        }
    }   
}

