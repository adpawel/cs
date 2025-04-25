using System.Security.Cryptography;  
using System.Text;   
class Task1  
{  
    string command;
    string filePathA;
    string filePathB;
    public Task1(string command, string filePathA, string filePathB)  
    {  
        this.command = command;
        this.filePathA = filePathA;
        this.filePathB = filePathB;
    }  

    public void Func(){
        // Stworzenie instancji klasy implementującej algorytm RSA z losową
        // inicjacją klucza prywatnego i publicznego
        RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
        // pliki, w których będą trzymane klucze
        string filePublicKey = "publicKey.dat";
        string filePrivateKey = "privateKey.dat";
        string ?publicKey = null;
        string ?privateKey = null;
        
        if(command == "0"){
            publicKey = rsa.ToXmlString(false); // false aby wziąć klucz publiczny
            File.WriteAllText(filePublicKey, publicKey);
            privateKey = rsa.ToXmlString(true); // true aby wziąć klucz prywatny
            File.WriteAllText(filePrivateKey, privateKey);
        } else if(command == "1"){
            // szyfrowanie danych
            publicKey = File.ReadAllText(filePublicKey);
            string fileContent = File.ReadAllText(filePathA);
            EncryptText(publicKey, fileContent, filePathB);  
        } else if(command == "2"){
            // odszyfrowanie danych
            privateKey = File.ReadAllText(filePrivateKey);
            File.WriteAllText(filePathB, DecryptData(privateKey, filePathA));
        } else {
            throw new ArgumentException("Nieobsługiwany typ komendy: " + command);
        }
    }

    // Utwórz metodę szyfrowania tekstu i zapisywania go do określonego pliku przy użyciu klucza publicznego algorytmu RSA   
    static void EncryptText(string kluczPubliczny, string tekst,string nazwaPliku)  
    {  
        // Zmień text na tablicę bajtów   
        UnicodeEncoding byteConverter = new UnicodeEncoding();  
        byte[] daneDoZaszyfrowania = byteConverter.GetBytes(tekst);  

        // Utwórz tablicę bajtów, aby przechowywać w niej zaszyfrowane dane   
        byte[] zaszyfrowaneDane;   
        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())  
        {  
            // Ustaw publiczny klucz RSA   
            rsa.FromXmlString(kluczPubliczny);  
            // Zaszyfruj dane in wstaw je do tablicy zaszyfrowaneDane
            zaszyfrowaneDane = rsa.Encrypt(daneDoZaszyfrowania, false);   
        }  
        // Zapisz zaszyfrowaną tablicę danych do pliku   
        File.WriteAllBytes(nazwaPliku, zaszyfrowaneDane);  

        Console.WriteLine("Dane zostały zaszyfrowane");   
    }  

    // Metoda odszyfrowania danych w określonym pliku przy użyciu klucza prywatnego algorytmu RSA   
    static string DecryptData(string privateKey,string fileName)  
    {  
        // odczytanie zaszyfrowanych bajtów z pliku   
        byte[] daneDoOdszyfrowania = File.ReadAllBytes(fileName);  

        // Create an array to store the decrypted data in it   
        byte[] odszyfrowaneDane;  
        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())  
        {  
            // Set the private key of the algorithm   
            rsa.FromXmlString(privateKey);  
            odszyfrowaneDane = rsa.Decrypt(daneDoOdszyfrowania, false);   
        }  

        // Get the string value from the decryptedData byte array   
        UnicodeEncoding byteConverter = new UnicodeEncoding();  
        return byteConverter.GetString(odszyfrowaneDane);   
    }  
}