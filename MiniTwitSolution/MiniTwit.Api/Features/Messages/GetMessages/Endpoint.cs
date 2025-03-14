﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using MiniTwit.Api.Utility;
using MiniTwit.Shared.DTO.Messages;

namespace MiniTwit.Api.Features.Messages.GetMessages
{
    public static class Endpoint
    {
        public static IEndpointRouteBuilder MapGetMessagesEndpoints(
            this IEndpointRouteBuilder routes
        )
        {
            routes.MapGet(
                "/msgs",
                async (
                    MiniTwitDbContext db,
                    HybridCache hybridCache,
                    CancellationToken cancellationToken,
                    [FromQuery] int no = 100,
                    [FromQuery] int latest = -1
                ) =>
                {
                    // Cache key includes the limit
                    var cacheKey = $"publicTimeline:{no}";

                    // Retrieve or create the cached response.
                    var response = await hybridCache.GetOrCreateAsync<List<GetMessageResponse>>(
                        cacheKey,
                        async ct =>
                        {
                            var messages = await db
                                .Messages.Where(m => m.Flagged == 0)
                                .Include(m => m.Author)
                                .OrderByDescending(m => m.PubDate)
                                .Take(no)
                                .ToListAsync(ct);

                            var dto = messages
                                .Select(m => new GetMessageResponse(
                                    m.MessageId,
                                    m.PubDate,
                                    m.Author!.Username,
                                    m.Text
                                ))
                                .ToList();

                            return dto;
                        },
                        cancellationToken: cancellationToken,
                        tags: new[] { "publicTimeline" }
                    );

                    await UpdateLatest.UpdateLatestStateAsync(
                        latest,
                        db,
                        hybridCache,
                        cancellationToken
                    );
                    // Remove the latestEvent tag from the cache
                    await hybridCache.RemoveAsync("latestEvent");

                    return Results.Ok(response);
                }
            );

            return routes;
        }
    }
}
