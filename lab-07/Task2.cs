using System.Security.Cryptography;
using System.Text;

public class Task2{
    public string filePath;
    public string hashFilePath;
    public string content;

    public Task2(string filePath, string hashFilePath, string hashType){
        this.filePath = filePath;
        this.hashFilePath = hashFilePath;
        string fileContent = File.ReadAllText(filePath);

        if(hashType == "SHA256"){
            content = createSHA256(fileContent);
        } else if(hashType == "SHA512"){
            content = createSHA512(fileContent);
        } else if(hashType == "MD5"){
            content = createMD5(fileContent);
        } else {
            throw new ArgumentException("Nieobsługiwany typ hasha: " + hashType);
        }
    }

    public void Hash(){
        if(File.Exists(hashFilePath)){
            string afterHash = File.ReadAllText(hashFilePath);
            if(afterHash.Equals(content, StringComparison.OrdinalIgnoreCase)){
                Console.WriteLine("Hash jest zgodny");
            } else {
                Console.WriteLine("Hash nie jest zgodny");
            }
        } else {
            using (StreamWriter sw = File.CreateText(hashFilePath))
            {
                sw.Write(content);
            }
            Console.WriteLine("Utworzono nowy plik z hashem.");
        }
    }

    static string createSHA256(string str)
    {
        Encoding enc = Encoding.UTF8;
        var hashBuilder = new StringBuilder();
        using var hash = SHA256.Create();
        byte[] result = hash.ComputeHash(enc.GetBytes(str));
        foreach (var b in result)
            hashBuilder.Append(b.ToString("x2"));
        return hashBuilder.ToString();
    }

    static string createSHA512(string str)
    {
        Encoding enc = Encoding.UTF8;
        var hashBuilder = new StringBuilder();
        using var hash = SHA512.Create();
        byte[] result = hash.ComputeHash(enc.GetBytes(str));
        foreach (var b in result)
            hashBuilder.Append(b.ToString("x2"));
        return hashBuilder.ToString();
    }

    static string createMD5(string str)
    {
        Encoding enc = Encoding.UTF8;
        var hashBuilder = new StringBuilder();
        using var hash = MD5.Create();
        byte[] result = hash.ComputeHash(enc.GetBytes(str));
        foreach (var b in result)
            hashBuilder.Append(b.ToString("x2"));
        return hashBuilder.ToString();
    }
}