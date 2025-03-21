namespace MiniTwit.Api.Utility;

public static class HttpHelper
{
    public static async Task<string> GetHttpBodyAsStringAsync(Stream body)
    {
        if (!body.CanSeek) return "No content";
        body.Position = 0; // Reset stream position
        using var reader = new StreamReader(body, leaveOpen: true);
        var content = await reader.ReadToEndAsync();
        body.Position = 0; // Reset again for further processing
        return content;

    }
}