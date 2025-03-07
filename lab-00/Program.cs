using System.Data;
using System.Runtime.InteropServices.Marshalling;
using System.Text;

namespace lab0{
    public class MainClass{
        public static void Main(string[] args){
            string task = args[0];
            switch(task){
                case "task1":
                    task1.RepeatingString.Execute();
                    break;
                case "task2":
                    task2.FileWriting.Execute();
                    break;
                case "task3":
                    task3.FileReading.Execute();
                    break;
                case "task4":
                    task4.Music.Execute();
                    break;
                default:
                    Console.WriteLine("Nie ma takiego zadania.");
                    break;
            }
        }
    }
}

namespace task1{
    class RepeatingString{
        public static void Execute(){            
            Console.Write("Podaj zestaw napisów i liczbę powtórzeń: ");
            string ?input = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(input)) {
                Console.WriteLine("Błędne dane wejściowe.");
                return;
            }
            
            List<string> words = input.Split(" ").ToList();
            
            string lastString = words[^1];
            if (!int.TryParse(lastString, out int n) || n <= 0) {
                Console.WriteLine($"'{lastString}' nie jest poprawną liczbą.");
                return;
            }

            words.RemoveAt(words.Count - 1);

            StringBuilder output = new StringBuilder();

            foreach (string word in words) {
                for (int j = 0; j < n; j++) {
                    output.Append(word).Append(' ');
                }
            }

            Console.WriteLine(output.ToString().TrimEnd()); 
        }
    }
}

namespace task2{
    class FileWriting{
        public static void Execute(){
            int count = 0;
            int sum = 0;

            while(true){
                string? line = Console.ReadLine();
        
                if(!string.IsNullOrEmpty(line)){
                    int a = int.Parse(line);
                    if(a == 0) break;
                    sum += a;
                }
                count++;
            }
            
            float mean = (float) sum / count;
            string filePath = "output.txt";
            File.WriteAllText(filePath, "" + mean);
            Console.WriteLine("Dane zapisane do pliku: {0}", filePath);
        }
    }
}

namespace task3{
    class FileReading{
        public static void Execute(){
            Console.Write("Podaj nazwę pliku: ");
            string? filePath = Console.ReadLine();
            
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Plik nie istnieje!");
                return;
            }

            StreamReader streamReader = new StreamReader(filePath);
            
            int lineNumber = 1;
            int currentLineNumber = 1;
            int maxNumber = int.MinValue;

            while(!streamReader.EndOfStream){
                string ?line = streamReader.ReadLine();
                if(line == null) continue;

                List<string> list = line.Split(" ").ToList();
                foreach(string s in list){
                    if (int.TryParse(s, out int n) && n > maxNumber) {
                        maxNumber = n;
                        lineNumber = currentLineNumber;
                    }
                }
                currentLineNumber++;
            }
            streamReader.Close();

            if (maxNumber == int.MinValue)
            {
                Console.WriteLine("Brak liczb w pliku.");
            } else{
                Console.WriteLine("{0}, linijka: {1}", maxNumber, lineNumber);
            }
        }
    }
}


namespace task4{
    public class Music{
        public static void Execute(){
            string[] arr = new string[]{"C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "B", "H"};
            string? input = Console.ReadLine();
            int[] leaps = new int[]{2, 2, 1, 2, 2, 2, 1};

            if (Array.IndexOf(arr, input) == -1)
            {
                Console.WriteLine("Niepoprawny dźwięk!");
                return;
            }

            int startIndex = Array.IndexOf(arr, input); 

            StringBuilder sb = new StringBuilder();
            sb.Append(arr[startIndex]).Append(' ');  

            foreach (int n in leaps)
            {
                startIndex = (startIndex + n) % arr.Length;  
                sb.Append(arr[startIndex]).Append(' ');      
            }

            Console.WriteLine(sb.ToString().TrimEnd());
        }
    }
}