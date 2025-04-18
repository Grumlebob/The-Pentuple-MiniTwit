﻿using Microsoft.Extensions.Caching.Hybrid;
using MiniTwit.Api.Services.Interfaces;
using MiniTwit.Api.Utility;
using MiniTwit.Shared.DTO.Messages;

namespace MiniTwit.Api.Services;

public class MessageService(MiniTwitDbContext db, HybridCache cache) : IMessageService
{
    public async Task<IResult> GetPublicMessagesAsync(
        int no,
        int latest,
        CancellationToken cancellationToken
    )
    {
        var cacheKey = $"publicTimeline:{no}";

        var response = await cache.GetOrCreateAsync<List<GetMessageResponse>>(
            cacheKey,
            async ct =>
            {
                var messages = await db
                    .Messages.Where(m => m.Flagged == 0)
                    .Include(m => m.Author)
                    .OrderByDescending(m => m.PubDate)
                    .Take(no)
                    .ToListAsync(ct);

                return messages
                    .Select(m => new GetMessageResponse(
                        m.MessageId,
                        m.PubDate,
                        m.Author!.Username,
                        m.Text
                    ))
                    .ToList();
            },
            cancellationToken: cancellationToken,
            tags: ["publicTimeline"]
        );

        await UpdateLatest.UpdateLatestStateAsync(latest, db, cache, cancellationToken);
        await cache.RemoveAsync("latestEvent", cancellationToken);

        return Results.Ok(response);
    }

    public async Task<IResult> GetUserMessagesAsync(
        string username,
        int no,
        int latest,
        CancellationToken cancellationToken
    )
    {
        var user = await db.Users.FirstOrDefaultAsync(
            u => u.Username == username,
            cancellationToken
        );

        if (user == null)
        {
            return Results.NotFound("User not found.");
        }

        var cacheKey = $"userTimeline:{username}:{no}";

        var response = await cache.GetOrCreateAsync<List<GetMessageResponse>>(
            cacheKey,
            async ct =>
            {
                var messages = await db
                    .Messages.Where(m => m.AuthorId == user.UserId && m.Flagged == 0)
                    .OrderByDescending(m => m.PubDate)
                    .Take(no)
                    .ToListAsync(ct);

                return messages
                    .Select(m => new GetMessageResponse(
                        m.MessageId,
                        m.PubDate,
                        user.Username,
                        m.Text
                    ))
                    .ToList();
            },
            cancellationToken: cancellationToken,
            tags: [$"userTimeline:{username}"]
        );

        await UpdateLatest.UpdateLatestStateAsync(latest, db, cache, cancellationToken);

        return response.Count == 0 ? Results.NoContent() : Results.Ok(response);
    }

    public async Task<IResult> PostMessageAsync(
        string username,
        PostMessageRequest request,
        int latest,
        CancellationToken cancellationToken
    )
    {
        var author = await db.Users.FirstOrDefaultAsync(
            u => u.Username == username,
            cancellationToken
        );

        if (author == null)
        {
            return Results.BadRequest("Author not found.");
        }

        var pubDate = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        var message = new Message
        {
            AuthorId = author.UserId,
            Text = request.Content,
            PubDate = pubDate,
            Flagged = 0,
        };

        db.Messages.Add(message);
        await db.SaveChangesAsync(cancellationToken);

        await cache.RemoveByTagAsync("publicTimeline", cancellationToken);
        await cache.RemoveByTagAsync($"userTimeline:{author.Username}", cancellationToken);

        var followerIds = await db
            .Followers.Where(f => f.WhomId == author.UserId)
            .Select(f => f.WhoId)
            .ToListAsync(cancellationToken);

        foreach (var followerId in followerIds)
        {
            await cache.RemoveByTagAsync($"privateTimeline:{followerId}", cancellationToken);
        }

        await UpdateLatest.UpdateLatestStateAsync(latest, db, cache, cancellationToken);

        return Results.NoContent();
    }
}
