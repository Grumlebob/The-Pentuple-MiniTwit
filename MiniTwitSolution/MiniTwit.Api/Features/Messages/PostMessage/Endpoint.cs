using MiniTwit.Shared.DTO.Messages;

namespace MiniTwit.Api.Features.Messages.PostMessage;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapPostMessageEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapPost(
            "/message",
            async (PostMessageRequest request, MiniTwitDbContext db) =>
            {
                // Validate that the author exists.
                var author = await db.Users.FindAsync(request.AuthorId);
                if (author == null)
                {
                    return Results.BadRequest("Author not found.");
                }

                // Determine publication date.
                int pubDate = request.PubDate ?? (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                // Create and save the new message.
                var message = new Message
                {
                    AuthorId = request.AuthorId,
                    Text = request.Text,
                    PubDate = pubDate,
                    Flagged = 0,
                };

                db.Messages.Add(message);
                await db.SaveChangesAsync();

                // Build the response DTO.
                var responseDto = new PostMessageResponse(
                    message.MessageId,
                    message.AuthorId,
                    message.Text,
                    message.PubDate
                );
                return Results.Created($"/message/{message.MessageId}", responseDto);
            }
        );

        return routes;
    }
}
