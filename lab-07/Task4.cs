using System.Text;
using System.Security.Cryptography;

public class Task4 {
    string filePathA;
    string filePathB;
    string password;
    string opType;
    public Task4(string filePathA, string filePathB, string password, string opType){
        this.filePathA = filePathA;
        this.filePathB = filePathB;
        this.password = password;
        this.opType = opType;
    }

    public void Run(){
        int liczbaIteracji = 2000;
        if (opType == "0") {
            // szyfrowanie
            string content = File.ReadAllText(filePathA);
            byte[] dane = Encoding.UTF8.GetBytes(content);

            byte[] salt = RandomNumberGenerator.GetBytes(8);
            byte[] initVector = RandomNumberGenerator.GetBytes(16);

            byte[] zaszyfrowane = Szyfruj(password, salt, initVector, liczbaIteracji, dane);

            // Zapisz [salt][iv][ciphertext] do jednego pliku
            using FileStream fs = new FileStream(filePathB, FileMode.Create, FileAccess.Write);
            fs.Write(salt, 0, salt.Length);
            fs.Write(initVector, 0, initVector.Length);
            fs.Write(zaszyfrowane, 0, zaszyfrowane.Length);

            Console.WriteLine("Plik został zaszyfrowany.");
        } 
        else if (opType == "1") {
            // deszyfrowanie
            byte[] allBytes = File.ReadAllBytes(filePathA);

            byte[] salt = new byte[8];
            byte[] initVector = new byte[16];
            byte[] ciphertext = new byte[allBytes.Length - 24];

            Array.Copy(allBytes, 0, salt, 0, 8);
            Array.Copy(allBytes, 8, initVector, 0, 16);
            Array.Copy(allBytes, 24, ciphertext, 0, ciphertext.Length);

            byte[] rozszyfrowane = Rozszyfruj(password, salt, initVector, liczbaIteracji, ciphertext);
            string plaintext = Encoding.UTF8.GetString(rozszyfrowane);

            File.WriteAllText(filePathB, plaintext);
            Console.WriteLine("Plik został odszyfrowany.");
        } 
        else {
            throw new ArgumentException("Nieobsługiwany typ operacji: " + opType);
        }

    }

    public static byte[]? Rozszyfruj(String haslo, byte[]salt, 
        byte[]initVector, int iteracje, byte[]dane)
    {
        Rfc2898DeriveBytes k1 = new Rfc2898DeriveBytes(haslo, salt, iteracje,
            HashAlgorithmName.SHA256);
        Aes decAlg = Aes.Create();
        decAlg.Key = k1.GetBytes(16);
        decAlg.IV = initVector;
        MemoryStream decryptionStreamBacking = new MemoryStream();
        CryptoStream decrypt = new CryptoStream(
            decryptionStreamBacking, decAlg.CreateDecryptor(), CryptoStreamMode.Write);
        decrypt.Write(dane, 0, dane.Length);
        decrypt.Flush();
        decrypt.Close();
        k1.Reset();
        return decryptionStreamBacking.ToArray();
    }

    public static byte[]? Szyfruj(string password, byte[]salt, byte[]initVector,
                    int iteracje, byte[]daneDoZaszyfrowania)
    {
        Rfc2898DeriveBytes k1 = new Rfc2898DeriveBytes(password, salt, iteracje, 
            HashAlgorithmName.SHA256);
        Aes encAlg = Aes.Create();
        encAlg.IV = initVector;
        encAlg.Key = k1.GetBytes(16);
        MemoryStream encryptionStream = new MemoryStream();
        CryptoStream encrypt = new CryptoStream(encryptionStream,
            encAlg.CreateEncryptor(), CryptoStreamMode.Write);
        encrypt.Write(daneDoZaszyfrowania, 0, daneDoZaszyfrowania.Length);
        encrypt.FlushFinalBlock();
        encrypt.Close();
        byte[] edata1 = encryptionStream.ToArray();
        k1.Reset();
        return edata1;
    }
}