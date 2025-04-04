public class FileFinder{
    string path;
    string expression;
    public Queue<string> queue = new Queue<string>();
    public FileFinder(string path, string expression){
        this.path = path;
        this.expression = expression;
    }
    public void findFile(){
        queue.Enqueue(path);

        while(queue.Count > 0){
            string entry = queue.Dequeue();
        
            try{
                foreach(string file in Directory.GetFiles(entry)){
                    DisplayNameOrOmit(new FileInfo(file));
                }

                foreach (string dir in Directory.GetDirectories(entry)){
                    queue.Enqueue(dir);
                    DisplayNameOrOmit(new DirectoryInfo(dir));
                }

            } catch (Exception ex){
                Console.WriteLine($"Error accessing {entry}: {ex.Message}");
            }
        }
    }

    void DisplayNameOrOmit(FileSystemInfo fsi){
        if((fsi.Attributes & FileAttributes.Directory) == FileAttributes.Directory){

        } else{
            if(fsi.Name.Contains(expression)){
                Console.WriteLine(fsi.FullName);
            }
        }
    }
}

public class Task3{
    public string path;
    public string expression;
    public Task3(string path, string expression){
        this.path = path;
        this.expression = expression;
    }

    public void Start(){
        FileFinder ff = new FileFinder(path, expression);

        Thread thread = new( new ThreadStart(ff.findFile));

        thread.Start();
        thread.Join();
    }
}