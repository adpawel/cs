public class Producer{
    public string Name;
    public bool finished = false;
    MainClass parent;

    public Producer(string name, MainClass parent){
        Name = name;
        this.parent = parent;
    }

    public void Start(){
        Console.WriteLine("Producent " + Name + " wystartował.");

        while(!finished){
            Console.WriteLine("Producent " + Name + " czeka");
            parent.semaphore.WaitOne();
            Console.WriteLine("Producent " + Name + " ma zasób");

            parent.data.Enqueue(new Data(Name, "content"));
            parent.semaphore.Release();

            Thread.Sleep(parent.random.Next(3000));
        }
        Console.WriteLine("Producent " + Name + " zatrzymany.");
    }
}

public class Consumer{
    public string Name;
    public bool finished = false;
    public List<string> producersNames = new List<string>();
    MainClass parent;
    public Consumer(string name, MainClass parent){
        Name = name;
        this.parent = parent;
    }

    public void Start(){
        Console.WriteLine("Konsument " + Name + " wystartował.");
        while(!finished){
            Console.WriteLine("Konsument " + Name + " czeka");
            parent.semaphore.WaitOne();
            Console.WriteLine("Konsument " + Name + " ma zasób");
            if(parent.data.Count > 0){
                Data d = parent.data.Dequeue();
                producersNames.Add(d.ProducerID);
            }
            parent.semaphore.Release();
            Thread.Sleep(parent.random.Next(3000));
        }
        Console.WriteLine("Konsument " + Name + " zatrzymał się.");
    }

    public void printProducers(){
        Dictionary<string, int> producersCount = new Dictionary<string, int>();

        foreach(string name in producersNames){
            if(producersCount.ContainsKey(name)){
                producersCount[name] += 1;
            } else {
                producersCount.Add(name, 1);
            }
        }
        Console.WriteLine("Konsument " + Name + ": ");

        foreach(string name in producersCount.Keys){
            Console.WriteLine("     Producent " + name + ": " + producersCount[name].ToString());
        }
    }
}

public class Data{
    public string ProducerID;
    public int Id;
    private static int Count = 0;
    public string Content;

    public Data(string producerID, string content){
        ProducerID = producerID;
        Content = content;
        Id = Interlocked.Increment(ref Count);
    }
}

public class MainClass{
    public int n;
    public int m;
    // public Semaphore producerSemaphore;
    // public Semaphore consumerSemaphore;
    public Semaphore semaphore;
    public List<Consumer> consumers = new List<Consumer>();
    public List<Producer> producers = new List<Producer>();
    public Queue<Data> data = new Queue<Data>();
    public Random random = new Random(Environment.TickCount);
    static bool stop = false;

    public void Start(){
        List<Thread> producerThreads = new List<Thread>();
        List<Thread> consumerThreads = new List<Thread>();

        for (int i = 0; i < n; i++){
            Producer p = new Producer(i.ToString(), this);
            producers.Add(p);
            Thread t = new Thread(new ThreadStart(p.Start));
            producerThreads.Add(t);
            t.Start();
        }

        for (int i = 0; i < m; i++){
            Consumer c = new Consumer(i.ToString(), this);
            consumers.Add(c);
            Thread t = new Thread(new ThreadStart(c.Start));
            consumerThreads.Add(t);
            t.Start();
        }

        Thread inputThread = new Thread(CheckForExit);
        inputThread.Start();

        while (!stop){
            Thread.Sleep(100); 
        }

        foreach (Producer p in producers) p.finished = true;
        foreach (Consumer c in consumers) c.finished = true;

        foreach (Thread t in producerThreads) t.Join();
        foreach (Thread t in consumerThreads) t.Join();
        
        foreach (Consumer c in consumers) c.printProducers();
    }

    static void CheckForExit(){
        while (true)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Q)
                {
                    stop = true;
                    break;
                }
            }
            Thread.Sleep(100);
        }
    }

    public MainClass(int n, int m){
        this.n = n;
        this.m = m;
        semaphore = new Semaphore(1, 1);
    }
    public static void Main(string[] args){
        if(args[0] == "task1"){
            MainClass m = new MainClass(int.Parse(args[1]), int.Parse(args[2]));
            m.Start();
        } else if(args[0] == "task2"){
            Task2 t = new Task2(args[1]);
            t.Start();
        } else if(args[0] == "task3"){
            Task3 t = new Task3(args[1], args[2]);
            t.Start();
        } else if(args[0] == "task4"){
            Task4 t = new Task4(int.Parse(args[1]));
            t.Start();
        } 
    }
}
