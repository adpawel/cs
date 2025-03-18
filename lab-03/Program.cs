using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using static System.Linq.Enumerable;
using System.Xml.Serialization;

public class MainClass{
    public static void Main(string[] args){
        // zad 1
        List<Tweet> tweets = ReadFromJson("data/favorite-tweets.jsonl");
        
        // zad 2
        SerializeToXML(tweets);

        // zad 3
        List<Tweet> tweetsSortedByUsername = SortByUsername(tweets);
        List<Tweet> tweetsSortedByDate = SortByDate(tweets);

        // zad 4
        Console.WriteLine(tweetsSortedByDate[1]);
        Console.WriteLine();
        Console.WriteLine(tweetsSortedByDate[^1]);

        // zad 5
        Dictionary<string, List<Tweet>> tweetsByUser = LinkTweetsToUser(tweets);
        // foreach(Tweet t in tweetsByUser["michaelharriot"]){
        //     Console.WriteLine(t.CreatedAt);
        // }
        // Console.WriteLine(tweetsByUser["michaelharriot"].Count);

        // zad 6
        Dictionary<string, int> wordsCount = CountWordsInText(tweets);
        // Console.WriteLine(wordsCount["get"]);
        
        // zad 7
        List<KeyValuePair<string, int>> list = SortDict(wordsCount);
        for(int i = 0; i < 10; ++i){
            Console.WriteLine(list[i].Key + " " + list[i].Value);
        }

        // zad 8
        List<KeyValuePair<string, double>> idfList = CalculateIDF(wordsCount);
        for(int i = 0; i < 10; ++i){
            Console.WriteLine(idfList[i].Key + " " + idfList[i].Value);
        }

    }

    public static void SerializeToXML(List<Tweet> tweets){
        XmlSerializer x = new XmlSerializer(tweets.GetType());

        using (StreamWriter writer = File.CreateText("data\\tweets.xml"))
        {
            x.Serialize(writer, tweets);
        }

        List<Tweet>? tweets2 = null;
        using (StreamReader reader = new StreamReader("data\\tweets.xml"))
        {
            tweets2 = (List<Tweet>)x.Deserialize(reader);
        }

        // for(int i = 0; i < 3; ++i){
        //     Console.WriteLine(tweets2[i]);
        // }
    }
    public static List<Tweet> ReadFromJson(string path){
        List<Tweet> tweets = new List<Tweet>();

        foreach(string line in File.ReadLines(path))
        {
            Tweet? tweet = JsonSerializer.Deserialize<Tweet>(line);
            if (tweet != null)
                tweets.Add(tweet);
        }
        return tweets;
    }

    public static List<Tweet> SortByUsername(List<Tweet> tweets){
        return tweets.OrderBy(t => t.UserName).ToList();
    }

    public static List<Tweet> SortByDate(List<Tweet> tweets){
        return tweets.Where(t => !string.IsNullOrEmpty(t.CreatedAt))
            .OrderBy(t => DateTime.ParseExact(t.CreatedAt, "MMMM dd, yyyy 'at' hh:mmtt", CultureInfo.InvariantCulture))
            .ToList();
    }

    public static Dictionary<string, List<Tweet>> LinkTweetsToUser(List<Tweet> tweets){
        Dictionary<string, List<Tweet>> dict = new Dictionary<string, List<Tweet>>();
        foreach(Tweet tweet in tweets){
            if(!string.IsNullOrEmpty(tweet.UserName)){
                if(dict.ContainsKey(tweet.UserName)){
                    dict[tweet.UserName].Add(tweet); 
                }
                else{
                    dict.Add(tweet.UserName, new List<Tweet>([tweet]));
                }
            }
        }
        return dict;
    }

    public static Dictionary<string, int> CountWordsInText(List<Tweet> tweets){
        Dictionary<string, int> dict = new Dictionary<string, int>();

        foreach(Tweet t in tweets){
            if(!string.IsNullOrEmpty(t.Text)){
                string textWithoutLink = RemoveLastLink(t.Text);
                string[] words =  Regex.Replace(textWithoutLink, @"[^\w\s]", "").Split(' ', StringSplitOptions.RemoveEmptyEntries);
                
                foreach(string str in words){
                    if(dict.ContainsKey(str.ToLower())){
                        dict[str.ToLower()]++; 
                    }
                    else{
                        dict.Add(str.ToLower(), 1);
                    }
                }
            }
        }
        return dict;
    }

    static string RemoveLastLink(string text)
    {
        return Regex.Replace(text, @"\s*https?:\/\/\S+\s*$", "");
    }

    public static List<KeyValuePair<string, int>> SortDict(Dictionary<string, int> wordsCount){
        List<KeyValuePair<string, int>> list;

        list = wordsCount
            .Where(p => p.Key.Length >= 5)
            .OrderByDescending(p => p.Value)
            .ToList();

        return list;
    }

    public static List<KeyValuePair<string, double>> CalculateIDF(Dictionary<string, int> wordsCount){
        int n = wordsCount.Count;

        Dictionary<string, double> idf = new Dictionary<string, double>();
        foreach(string k in wordsCount.Keys){
            if(!idf.ContainsKey(k)){
                double idfValue = Math.Log(n / (1 + wordsCount[k]));
                idf.Add(k, idfValue);
            }
        }

        List<KeyValuePair<string, double>> idfList;
        idfList = idf
            .OrderByDescending(w => w.Value)
            .ToList();

        return idfList;
    }
}



