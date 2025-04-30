using System.Security.Cryptography;  
using System.Text;

public class Task3{
    string filePathA;
    string filePathB;
    private const string publicKeyFile = "publicKey.dat";
    private const string privateKeyFile = "privateKey.dat";
    public Task3(string filePathA, string filePathB){
        this.filePathA = filePathA;
        this.filePathB = filePathB;
    }

    public void Run(){
        string contentA = File.ReadAllText(filePathA);
        using SHA256 algSkroto  = SHA256.Create();

        byte[] dane = Encoding.UTF8.GetBytes(contentA);
        // byte[] dane = Encoding.ASCII.GetBytes(contentA);
        //obliczenie skrótu z danych do podpisu
        byte[] hash = algSkroto.ComputeHash(dane);

        if (!File.Exists(publicKeyFile) || !File.Exists(privateKeyFile))
        {
            Console.WriteLine("Brakuje plików z kluczami RSA (publicKey.dat lub privateKey.dat).");
            return;
        }
        byte[] signature;

        if(!File.Exists(filePathB)){
            string privateKeyXml = File.ReadAllText(privateKeyFile);
            //podpisanie danych
            //nowa instancja RSA, która ma losową parę kluczy
            using (RSA rsa = RSA.Create())
            {
                rsa.FromXmlString(privateKeyXml);
                //stworzenie instancji klasy do wykonania podpisu
                RSAPKCS1SignatureFormatter rsaFormatter = new RSAPKCS1SignatureFormatter(rsa);
                //ustawienie algorytmu hashującego
                rsaFormatter.SetHashAlgorithm(nameof(SHA256));
                //podpisanie skrótu i stworzenie podpisu zgodnego z formatem PKCS #1
                signature = rsaFormatter.CreateSignature(hash);

                File.WriteAllBytes(filePathB, signature);
            }
        } else {
            string publicKeyXml = File.ReadAllText(publicKeyFile);
            using (RSA rsa = RSA.Create()){
                rsa.FromXmlString(publicKeyXml);
                signature = File.ReadAllBytes(filePathB);
                //stworzenie instancji algorytmu sprawdzającego podpis
                RSAPKCS1SignatureDeformatter rsaDeformatter = new RSAPKCS1SignatureDeformatter(rsa);
                rsaDeformatter.SetHashAlgorithm(nameof(SHA256));
                //aby tego dokonać potrzebujemy skrót oraz podpisany skrót wiadomości
                //jeżeli podpisany skrót rozszyfrowany przy pomocy klucza publicznego RSA
                //jest identyczny jak skrót wiadomości, oznacza to, że podpisany skrót
                //został zaszyfrowany pasującym do klucza publicznego kluczem prywatnym
                //czyli podpis jest prawidłowy (zweryfikowaliśmy tożsamość posiadacza klucza) 
                if (rsaDeformatter.VerifySignature(hash, signature))
                {
                    Console.WriteLine("Podpis jest prawidłowy");
                }
                else
                {
                    Console.WriteLine("Podpis nie jest prawidłowy");
                }
            }
        }
    }
}