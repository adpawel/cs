using System.Text;
using System.Globalization;
using Microsoft.Data.Sqlite;

public class Program{
    string[]? colTypes;
    // List<List<string>> file = new List<List<string>>();
    List<string> header = new List<string>();
    bool[]? isNullable;
    public List<List<string>> Read(string fileName, string sep){
        int i = 0;
        List<List<string>> file = new List<List<string>>();

        using(var reader = new StreamReader(fileName)){
            
            while(!reader.EndOfStream){
                var line = reader.ReadLine();
                
                if(string.IsNullOrEmpty(line)) 
                    continue;
                var values = line.Split(sep);

                if(i == 0){
                    ++i;
                    header = [.. values];
                    continue;
                }
            
                file.Add([.. values]);
            }
        }
        return file;
    }

    public string[] CheckColumnTypes(List<List<string>> content){
        string[] columnTypes = new string[content[0].Count];
        isNullable = new bool[content[0].Count];

        for(int i = 0; i < content[0].Count; ++i){
            List<string> column = new List<string>();

            foreach(List<string> row in content){
                column.Add(row[i]);
            }
            
            // Filtrowanie niepustych wartości (czyli ignorujemy null lub "")
            if(column.Where(string.IsNullOrEmpty).ToList().Count() > 0){
                isNullable[i] = true;
            }
            var nonEmptyValues = column.Where(x => !string.IsNullOrEmpty(x)).ToList();
            
            if(nonEmptyValues.Count == 0){
                columnTypes[i] = "TEXT";
                continue;
            }
            
            bool allInts = nonEmptyValues.All(x => int.TryParse(x, out _));
            if(allInts){
                columnTypes[i] = "INTEGER";
            } else if(nonEmptyValues.All(x => double.TryParse(x, NumberStyles.Any, CultureInfo.InvariantCulture, out _))){
                columnTypes[i] = "REAL";
            } else {
                columnTypes[i] = "TEXT";
            }
        }
        colTypes = columnTypes;
        return columnTypes;
    }

    public bool CreateTable(string[] columnTypes, string name, SqliteConnection connection){
        try {
                SqliteCommand delTableCmd = connection.CreateCommand();
                delTableCmd.CommandText = "DROP TABLE IF EXISTS " + name;
                delTableCmd.ExecuteNonQuery();

                SqliteCommand createTableCmd = connection.CreateCommand();
                    
                StringBuilder sb = new StringBuilder();
                sb.Append("CREATE TABLE \"").Append(name).Append("\" (");
                for(int i = 0; i < header.Count; ++i){
                    sb.Append("\"").Append(header[i]).Append("\" ").Append(columnTypes[i]);
                    if(!isNullable[i]){
                        sb.Append(" NOT NULL");
                    }
                    sb.Append(",");
                } 
                string cmd = sb.ToString();
                cmd = cmd.Substring(0, sb.Length - 1);
                cmd += ");";
                createTableCmd.CommandText = cmd;

                createTableCmd.ExecuteNonQuery();
            } catch (Exception e){ 
                Console.WriteLine(e.Message);
                return false;}
            return true;
    }

    public bool FillTable(List<List<string>> content, string name, SqliteConnection connection){
        try {
            using (var transaction = connection.BeginTransaction())
            {
                SqliteCommand insertCmd = connection.CreateCommand();

                StringBuilder sb = new StringBuilder();
                sb.Append("INSERT INTO ").Append(name).Append(" (");
                foreach(string colName in header){
                    sb.Append("\"").Append(colName).Append("\"").Append(",");
                }
                sb.Length--; 
                sb.Append(") VALUES ");

                foreach(List<string> row in content){
                    sb.Append("(");
                    for(int i = 0; i < header.Count; ++i){
                        string val = i < row.Count ? row[i] : "";

                        if (string.IsNullOrWhiteSpace(val)) {
                            sb.Append("NULL,");
                        }
                        else if(colTypes[i] == "INTEGER" && int.TryParse(val, out int intVal)){
                            sb.Append(intVal).Append(",");
                        }
                        else if(colTypes[i] == "REAL" && double.TryParse(val, NumberStyles.Any, CultureInfo.InvariantCulture, out double dblVal)){
                            sb.Append(dblVal.ToString(CultureInfo.InvariantCulture)).Append(",");
                        }
                        else {
                            string safeVal = val.Replace("\"", "\"\"");
                            sb.Append("\"").Append(safeVal).Append("\",");
                        }
                    }
                    sb.Length--; 
                    sb.Append("),");
                }
                // Console.WriteLine(sb.ToString());
                
                sb.Length--;
                insertCmd.CommandText = sb.ToString();
                insertCmd.ExecuteNonQuery();
                transaction.Commit();
            }
        } catch (Exception e){ 
            Console.WriteLine("Błąd podczas wypełniania tabeli: " + e.Message);
            return false;
        }
        return true;
    }


    public void Test(string name, SqliteConnection connection){
        SqliteCommand selectCmd = connection.CreateCommand();
        //Przeglądanie danych przy użyciu data reader                
        selectCmd = connection.CreateCommand();
        selectCmd.CommandText = $"SELECT * FROM {name}";

        using (SqliteDataReader reader = selectCmd.ExecuteReader())
        {
            bool firstRow = true;
            while (reader.Read())
            {
                //pobranie nazw kolumn
                if (firstRow)
                {
                    for (int a = 0; a < reader.FieldCount; a++)
                    {
                        Console.Write(reader.GetName(a));
                        Console.Write(",");
                    }
                    firstRow = false;
                    Console.WriteLine("");
                }
                //Można pobierać kolumny po ich nazwach
                //Console.Write(reader["Id"] + ",");
                //lub przeiterować po nich w kolejności
                for (int a = 0; a < reader.FieldCount; a++)
                {
                    string?val = null;
                    //jeżeli wartość pola równa się null, to GetString rzuci wyjątkiem,
                    //dlatego przechwytujemy wyjątek
                    try {
                        val = reader.GetString(a);
                    } catch {}
                    Console.Write(val != null ? val : "NULL");
                    Console.Write(",");
                }
                Console.WriteLine("");
            }
            //readera po zakończeniu pracy należy zamknąć nim będziemy mogli wykonać
            //nowe polecenie na tym samym obiekcie SqliteCommand
            reader.Close();
        }
    }
}


public class MainClass{
     public static void Main(string[] args){
        Program p = new Program();
        
        // Zadanie 1
        List<List<string>> file = p.Read(args[0], args[1]);
        // foreach(List<string> row in file){
        //     StringBuilder sb = new StringBuilder();
        //     foreach(string cell in row){
        //         sb.Append(cell).Append(',');
        //     }
        //     Console.WriteLine(sb.ToString().Substring(0, sb.Length-1));
        // }

        // Zadanie 2
        string[] columnTypes = p.CheckColumnTypes(file);

        // Console.WriteLine();

        // foreach(string type in columnTypes){
        //     Console.WriteLine(type);
        // }

        var connectionStringBuilder = new SqliteConnectionStringBuilder();

        connectionStringBuilder.DataSource = "./baza.db";

        using (var connection = new SqliteConnection(connectionStringBuilder.ConnectionString))
        {
            connection.Open();
            // Zadanie 3
            if (p.CreateTable(columnTypes, "tabela1", connection)){
                // Zadanie 4
                if (p.FillTable(file, "tabela1", connection)){
                    // Zadanie 5
                    p.Test("tabela1", connection);
                }
            }
        }
    }
}