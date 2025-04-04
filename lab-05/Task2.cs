public class Task2{
    private static bool stop;
    public string path;

    public Task2(string path){
        this.path = path;
    }
    public void Start(){
        
        using var watcher = new FileSystemWatcher(@path);

        watcher.NotifyFilter = NotifyFilters.FileName |
                                NotifyFilters.DirectoryName |
                                NotifyFilters.LastWrite;
        watcher.Created += OnCreated;
        watcher.Deleted += OnDeleted;
        watcher.EnableRaisingEvents = true;

        Thread inputThread = new Thread(CheckForExit);
        inputThread.Start();

        while (!stop){
            Thread.Sleep(100); 
        }
        Console.WriteLine("q");
    }

    private static void OnCreated(object sender, FileSystemEventArgs e){
        string value = $"Dodano: {e.FullPath.Split("\\")[^1]}";
        Console.WriteLine(value);
    }

    private static void OnDeleted(object sender, FileSystemEventArgs e) =>
        Console.WriteLine($"Usunięto: {e.FullPath.Split("\\")[^1]}");

    static void CheckForExit(){
        while (true){
            if (Console.KeyAvailable){
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Q){
                    stop = true;
                    break;
                }
            }
            Thread.Sleep(100);
        }
    }
}