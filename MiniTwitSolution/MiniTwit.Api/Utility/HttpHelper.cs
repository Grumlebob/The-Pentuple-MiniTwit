namespace MiniTwit.Api.Utility;

public static class HttpHelper
{
    public static string GetHttpBodyAsString(Stream body)
    {
        if (!body.CanSeek) return "No content";
        body.Position = 0; // Reset stream position
        using var reader = new StreamReader(body, leaveOpen: true);
        var content = reader.ReadToEnd();
        body.Position = 0; // Reset again for further processing
        return content;

    }
}