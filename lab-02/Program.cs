public class MainClass{
    public static void Main(string[] args){
        OsobaFizyczna osFizyczna = new OsobaFizyczna("Jan", "Kowalski", "Marek", "98786756171", null);
        OsobaPrawna osPrawna = new OsobaPrawna("Polfarma", "Słoneczny dwór");
        OsobaFizyczna osFizyczna1 = new OsobaFizyczna("Kamil", "Nowak", "Jan", null, "AB1234567");
        try{
            OsobaFizyczna osFizyczna2 = new OsobaFizyczna("Kamil", "Nowak", "Jan", null, null);
        } catch(Exception e){
            Console.WriteLine(e.Message);
        }

        RachunekBankowy src = new RachunekBankowy("123456789", 45.98m, false, new List<PosiadaczRachunku>([osFizyczna, osPrawna]));
        RachunekBankowy goal = new RachunekBankowy("987654321", 1223.34m, true, new List<PosiadaczRachunku>([osFizyczna1, osPrawna]));

        Console.WriteLine($"Stan konta źródłowego przed przelewem: {src.StanRachunku}");
        Console.WriteLine($"Stan konta docelowego przed przelewem: {goal.StanRachunku}");

        RachunekBankowy.DokonajTransakcji(src, goal, 45m, "Przykładowy przelew za pizzę");
        
        Console.WriteLine($"Stan konta źródłowego: {src.StanRachunku}");
        Console.WriteLine($"Stan konta docelowego: {goal.StanRachunku}");
        Console.WriteLine(src.ToString());
        Console.WriteLine(goal.ToString());

        src = src - new OsobaFizyczna("Kacper", "Nowak", "Michał", "12345671728", null);
        Console.Write(src.ToString());
    }
}
