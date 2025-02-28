using System.Text.Json;

namespace MiniTwit.Api.Utility;

public static class UpdateLatest
{
    public static async Task UpdateLatestStateAsync(int latest, MiniTwitDbContext db)
    {
        //Update latest
        await db.Latests.Where(l => l.LatestId == 1)
            .ExecuteUpdateAsync(set => set.SetProperty(l => l.LatestId, latest));
    }
}