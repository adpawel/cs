public class Tweet{
    private string? text;
    public string? Text{
        get {return text; }
        set { text = value; }
    }
    private string? userName;
    public string? UserName{
        get {return userName; }
        set { userName = value; }
    }
    private string? linkToTweet;
    public string? LinkToTweet{
        get {return linkToTweet; }
        set { linkToTweet = value; }
    }
    private string? firstLinkUrl;
    public string? FirstLinkUrl{
        get {return firstLinkUrl; }
        set { firstLinkUrl = value; }
    }
    private string? createdAt;
    public string? CreatedAt{
        get {return createdAt; }
        set { createdAt = value; }
    }
    private string? tweetEmbedCode;
    public string? TweetEmbedCode{
        get {return tweetEmbedCode; }
        set { tweetEmbedCode = value; }
    }

    public override string ToString()
    {
        return "Text: "+ text + "\nUsername: " + userName + "\nLink to tweet: " + linkToTweet + "\nFirst link url" + firstLinkUrl + 
        "\nCreated At: " + createdAt + "\nTweet Embed Code: " + tweetEmbedCode;
    }

}