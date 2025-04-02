﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Hybrid;
using MiniTwit.Api.Services.Interfaces;
using MiniTwit.Api.Utility;
using MiniTwit.Shared.DTO.Messages;

namespace MiniTwit.Api.Services;

public class MessageService : IMessageService
{
    private readonly MiniTwitDbContext _db;
    private readonly HybridCache _cache;

    public MessageService(MiniTwitDbContext db, HybridCache cache)
    {
        _db = db;
        _cache = cache;
    }

    public async Task<IActionResult> GetPublicMessagesAsync(int no, int latest, CancellationToken cancellationToken)
    {
        var cacheKey = $"publicTimeline:{no}";

        var response = await _cache.GetOrCreateAsync<List<GetMessageResponse>>(
            cacheKey,
            async ct =>
            {
                var messages = await _db.Messages
                    .Where(m => m.Flagged == 0)
                    .Include(m => m.Author)
                    .OrderByDescending(m => m.PubDate)
                    .Take(no)
                    .ToListAsync(ct);

                return messages
                    .Select(m => new GetMessageResponse(
                        m.MessageId,
                        m.PubDate,
                        m.Author!.Username,
                        m.Text))
                    .ToList();
            },
            cancellationToken: cancellationToken,
            tags: new[] { "publicTimeline" });

        await UpdateLatest.UpdateLatestStateAsync(latest, _db, _cache, cancellationToken);
        await _cache.RemoveAsync("latestEvent");

        return new OkObjectResult(response);
    }

    public async Task<IActionResult> GetUserMessagesAsync(string username, int no, int latest, CancellationToken cancellationToken)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
        if (user == null)
        {
            return new NotFoundObjectResult("User not found.");
        }

        var cacheKey = $"userTimeline:{username}:{no}";

        var response = await _cache.GetOrCreateAsync<List<GetMessageResponse>>(
            cacheKey,
            async ct =>
            {
                var messages = await _db.Messages
                    .Where(m => m.AuthorId == user.UserId && m.Flagged == 0)
                    .OrderByDescending(m => m.PubDate)
                    .Take(no)
                    .ToListAsync(ct);

                return messages
                    .Select(m => new GetMessageResponse(
                        m.MessageId,
                        m.PubDate,
                        user.Username,
                        m.Text))
                    .ToList();
            },
            cancellationToken: cancellationToken,
            tags: new[] { $"userTimeline:{username}" });

        await UpdateLatest.UpdateLatestStateAsync(latest, _db, _cache, cancellationToken);

        if (response.Count == 0)
        {
            return new NoContentResult();
        }

        return new OkObjectResult(response);
    }

    public async Task<IActionResult> PostMessageAsync(string username, PostMessageRequest request, int latest, CancellationToken cancellationToken)
    {
        var author = await _db.Users.FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
        if (author == null)
        {
            return new BadRequestObjectResult("Author not found.");
        }

        var pubDate = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        var message = new Message
        {
            AuthorId = author.UserId,
            Text = request.Content,
            PubDate = pubDate,
            Flagged = 0,
        };

        _db.Messages.Add(message);
        await _db.SaveChangesAsync(cancellationToken);

        await _cache.RemoveByTagAsync("publicTimeline", cancellationToken);
        await _cache.RemoveByTagAsync($"userTimeline:{author.Username}", cancellationToken);

        var followerIds = await _db.Followers
            .Where(f => f.WhomId == author.UserId)
            .Select(f => f.WhoId)
            .ToListAsync(cancellationToken);

        foreach (var followerId in followerIds)
        {
            await _cache.RemoveByTagAsync($"privateTimeline:{followerId}", cancellationToken);
        }

        await UpdateLatest.UpdateLatestStateAsync(latest, _db, _cache, cancellationToken);

        return new NoContentResult();
    }
}