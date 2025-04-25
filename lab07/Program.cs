public class MainClass{
    public static void Main(string[] args){
        if(args[0] == "task1"){
            Task1 t;
            if(args.Length > 2){
                t = new Task1(args[1], args[2], args[3]);
            } else {
                t = new Task1(args[1], "", "");
            }
            t.Func();
        } else if(args[0] == "task2"){
            Task2 t = new Task2(args[1], args[2], args[3]);
            t.Hash();
        } else if(args[0] == "task3"){
            Task3 t = new Task3(args[1], args[2]);
            t.Run();
        } else if(args[0] == "task4"){
            Task4 t = new Task4(args[1], args[2], args[3], args[4]);
            t.Run();
        } else {
            Console.WriteLine("Nie ma takiego zadania.");
        }
    }
}
