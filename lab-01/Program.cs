using System.Globalization;
using System.Text;

namespace lab1{
    public class MainClass{
        public static void Main(string[] args){
            string task = args[0];
            args = args.Skip(1).ToArray();
            switch(task){
                case "task2":
                    task2.FileStringWriting.Execute();
                    break;
                case "task3":
                    task3.StringFinder.Execute(args);
                    break;
                case "task4":
                    task4.RNG.Execute(args);
                    break;
                case "task5":
                    task5.FileOverview.Execute();
                    break;
                default:
                    Console.WriteLine("Nie ma takiego zadania.");
                    break;
            }
        }
    }
}

namespace task2{
    class FileStringWriting{
        public static void Execute(){
            Console.WriteLine("Podawaj napisy: ");
            string result = "";

            StreamWriter sw = new StreamWriter("output_task2.txt", append:false);
            while(true){
                string? napis = Console.ReadLine();
                if(napis == "koniec!"){
                       break;
                }

                if(!string.IsNullOrEmpty(napis)){
                    sw.WriteLine(napis);
                    if(string.Compare(napis, result) > 0){
                        result = napis;
                    }
                }
            }
            sw.Close();

            Console.WriteLine("Pomyślnie zapisano do pliku. Szukany wyraz: {0}", result);
        }
    }
}


namespace task3{
    public class StringFinder{
        public static void Execute(string[] args){
            string filePath = args[0];
            string toFind = args[1];

            int lineNumber = 1;
            StringBuilder sb = new StringBuilder();
            if(!File.Exists(filePath)){
                Console.WriteLine($"Plik {filePath} nie istnieje");
                return;
            }

            StreamReader sr = new StreamReader(filePath);
            while(!sr.EndOfStream){
                string? line = sr.ReadLine();
                if(!string.IsNullOrEmpty(line)){
                    int position = line.IndexOf(toFind);

                    while(position != -1){
                        sb.Append($"linijka: {lineNumber}, pozycja: {position}\n");
                        position = line.IndexOf(toFind, position + 1);
                    }
                }
                lineNumber++;
            }
            sr.Close();

            if(sb.ToString().Length > 0){
                Console.WriteLine("Wystąpienia podanego ciągu: ");
                Console.Write(sb.ToString());
            }
            else Console.WriteLine("Brak wystąpień podanego ciągu w tekście.");
        }
    }
}


namespace task4{
    public class RNG{
        public static void Execute(string[] args){
            string fileName = args[0];
            int n = int.Parse(args[1]);
            string range = args[2];
            int seed = int.Parse(args[3]);
            bool isInt = Boolean.Parse(args[4]); 

            int start = int.Parse(range.Split(":")[0]);
            int end = int.Parse(range.Split(":")[1]);

            Random random = new Random(seed);
            StreamWriter sw = new StreamWriter(fileName, append:false);
            for(int i = 0; i < n; i++){
                if(isInt) sw.WriteLine("" + random.Next(start, end + 1));
                else sw.WriteLine((random.NextDouble()*(end - start) + start).ToString());
            }
            sw.Close();

            Console.WriteLine($"Pomyślnie wygenerowano {n} liczb losowych.");
        }
    }
}


namespace task5{
    public class FileOverview{
        public static void Execute(){
            string filename = "output_task4.txt";
            string[] lines = File.ReadAllLines(filename);
            int numberOfLines = lines.Length;
            
            int numberOfChars = 0;
            foreach(string line in lines){
                numberOfChars += line.Length;
            }

            if(numberOfLines == 0){
                Console.WriteLine("Plik jest pusty.");
                return;
            }

            List<double> nums = new List<double>();
            foreach(string line in lines){
                nums.Add(double.Parse(line));
            }
            double minNumber = nums.Min();
            double maxNumber = nums.Max();
            double mean = nums.Average();

            StringBuilder sb = new StringBuilder();
            sb.Append($"Liczba linii: {numberOfLines}");
            sb.Append($"\nLiczba znaków: {numberOfChars}");
            sb.Append($"\nNajwiększa liczba: {maxNumber}");
            sb.Append($"\nNajmniejsza liczba: {minNumber}");
            sb.Append($"\nŚrednia: {mean}");
            Console.WriteLine(sb.ToString());
        }
    }
}