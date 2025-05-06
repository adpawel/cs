using System.Text;
using Microsoft.Data.Sqlite;
using System.Security.Cryptography;

public class DatabaseService{
    public static bool CreateTables(SqliteConnection connection) {
            try {
                SqliteCommand delTableCmd = connection.CreateCommand();
                delTableCmd.CommandText = "DROP TABLE IF EXISTS Loginy";
                delTableCmd.ExecuteNonQuery();

                delTableCmd.CommandText = "DROP TABLE IF EXISTS Dane";
                delTableCmd.ExecuteNonQuery();

                SqliteCommand createTableCmd = connection.CreateCommand();
                createTableCmd.CommandText = 
                    "CREATE TABLE \"Loginy\" ("
	                + "\"Login\"	TEXT NOT NULL,"
	                + "\"Password\"	TEXT NOT NULL,"
	                + "CONSTRAINT \"PK_Loginy\" PRIMARY KEY(\"Login\"));";                
                createTableCmd.ExecuteNonQuery();

                createTableCmd.CommandText =
                    "CREATE TABLE \"Dane\" ("
                    + "\"Id\" INTEGER NOT NULL,"
                    + "\"Dane\" TEXT NOT NULL,"
                    + "CONSTRAINT \"PK_Dane\" PRIMARY KEY(\"Id\" AUTOINCREMENT));";
                createTableCmd.ExecuteNonQuery();
            } catch (Exception e){ 
                Console.WriteLine(e.Message);
                return false;}
            return true;
    }

    public static bool SeedData(SqliteConnection connection)
        {
            var hashed1 = MD5Hash("efgh");
            var hashed2 = MD5Hash("zzzz");
            var hashed3 = MD5Hash("asdfgh");

            try {
                using (var transaction = connection.BeginTransaction())
                    {
                        SqliteCommand insertCmd = connection.CreateCommand();
                        insertCmd.CommandText =
                        "INSERT INTO Loginy"
                        + "(Login, Password)"
                        + $"VALUES (\"abcd\", \"{hashed1}\"),"
                        + $"(\"xyz\", \"{hashed2}\"),"
                        + $"(\"qwerty\", \"{hashed3}\")";
                        insertCmd.ExecuteNonQuery();

                        insertCmd.CommandText =
                        "INSERT INTO Dane"
                        + "(Id, Dane)"
                        + "VALUES"
                        + "(1, \"dane1\"),"
                        + "(2, \"toster\"),"
                        + "(3, \"dane3\"),"
                        + "(4, \"dane4\"),"
                        + "(5, \"dane5\")";
                        insertCmd.ExecuteNonQuery();
                        transaction.Commit();
                    }
            } catch (Exception e){ 
                Console.WriteLine(e.Message);
                return false;}
            return true;
        }

    public static List<string> GetAll()
    {
        var daneList = new List<string>();

        var connectionStringBuilder = new SqliteConnectionStringBuilder
        {
            DataSource = "./baza.db"
        };

        using (var connection = new SqliteConnection(connectionStringBuilder.ConnectionString))
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT Dane FROM Dane";

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    daneList.Add(reader.GetString(0));
                }
            }
        }

        return daneList;
    }

    public static void AddData(string dane)
    {   
        var connectionStringBuilder = new SqliteConnectionStringBuilder
        {
            DataSource = "./baza.db"
        };

        using (var connection = new SqliteConnection(connectionStringBuilder.ConnectionString))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Dane (Dane) VALUES ($dane)";
            command.Parameters.AddWithValue("$dane", dane);
            command.ExecuteNonQuery();
        }
    }



    public static bool IsValidUser(string login, string password)
    {
        var connectionStringBuilder = new SqliteConnectionStringBuilder
        {
            DataSource = "./baza.db"
        };

        using (var connection = new SqliteConnection(connectionStringBuilder.ConnectionString))
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM Loginy WHERE Login = $login AND Password = $password";
            command.Parameters.AddWithValue("$login", login);
            command.Parameters.AddWithValue("$password", password);

            var count = Convert.ToInt32(command.ExecuteScalar());
            return count > 0;
        }
    }

    public static string MD5Hash(string napis)
    {
        Encoding enc = Encoding.UTF8;
        var hashBuilder = new StringBuilder();
        using var hash = MD5.Create();
        byte[] result = hash.ComputeHash(enc.GetBytes(napis));
        foreach (var b in result)
            hashBuilder.Append(b.ToString("x2"));
        return hashBuilder.ToString();
    }

    public static void Connect()
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder();

            //Połączenie z bazą danych, jeżeli plik bazy nie istnieje zostanie stworzony
            connectionStringBuilder.DataSource = "./baza.db";

            using (var connection = new SqliteConnection(connectionStringBuilder.ConnectionString))
            {
                connection.Open();
                if (CreateTables(connection))
                    SeedData(connection);
            }
        }       
}