public class InfoThread{
    public int id;
    Task4 parent;
    public bool stop = false;
    public InfoThread(Task4 parent, int number){
        id = number;
        this.parent = parent;
    }

    public void Start(){
        Interlocked.Increment(ref parent.numberOfActiveThreads);
        while(!stop){
            Thread.Sleep(50);
        }
    }
}

public class Task4{
    public long numberOfActiveThreads = 0;
    private int numberOfThreads;
    private List<Thread> threads = new List<Thread>();
    private List<InfoThread> infoThreads = new List<InfoThread>();
    public Task4(int n){
        numberOfThreads = n;
    }

    public void Start(){
        for(int i = 0; i < numberOfThreads; ++i){
            InfoThread it = new InfoThread(this, i);
            infoThreads.Add(it);
            Thread t = new Thread(new ThreadStart(it.Start));
            threads.Add(t);
            t.Start();
        }

        while (Interlocked.Read(ref numberOfActiveThreads) != numberOfThreads)
        {
            Thread.Sleep(100);
        }
        Console.WriteLine("Wszystkie wątki się rozpoczęły");
        // Thread.Sleep(4000);
        
        foreach(InfoThread it in infoThreads)
        {
            it.stop = true;
        }

        foreach(Thread t in threads)
        {
            t.Join();
        }
        Console.WriteLine("Wszystkie wątki się zakończyły");
    }
}