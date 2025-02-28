using System.Text.Json;

namespace MiniTwit.Api.Utility;

public class UpdateLatest
{
    public async void UpdateLatestState(JsonDocument document, MiniTwitDbContext db)
    {
        var latest = -1;
        if (document.RootElement.TryGetProperty("latest", out var latestElement))
        {
            if (int.TryParse(latestElement.GetString(), out var latestId))
            {
                latest = latestId;
            }
        }
        //Update latest
        await db.Latests.Where(l => l.Id == 1)
            .ExecuteUpdateAsync(set => set.SetProperty(l => l.LatestEventId, latest));

    }
}