using System.Text.Json;

namespace MiniTwit.Api.Utility;

public static class UpdateLatest
{
    public static async Task UpdateLatestStateAsync(int latest, MiniTwitDbContext db)
    {
        //Update latest
        await db.Latests.Where(l => l.Id == 1)
            .ExecuteUpdateAsync(set => set.SetProperty(l => l.LatestEventId, latest));
    }
}