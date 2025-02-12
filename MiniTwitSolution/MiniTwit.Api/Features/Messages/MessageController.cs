using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniTwit.Api.Infrastructure; // Adjust this namespace to where your DbContext and entity models are defined
using MiniTwit.Api.Features.Messages; // If your PostMessageRequest model is defined here
using MiniTwit.Shared.DTO.Messages;   // Assuming your DTO or request model is here
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Hybrid;

namespace MiniTwit.Api.Features.Messages
{
    public class MessageController : Controller
    {
        private readonly MiniTwitDbContext _db;
        private readonly HybridCache _hybridCache;

        // Inject both the DbContext and the HybridCache via constructor injection.
        public MessageController(MiniTwitDbContext db, HybridCache hybridCache)
        {
            _db = db;
            _hybridCache = hybridCache;
        }

        // POST /add_message
        [HttpPost("/add_message")]
        public async Task<IActionResult> AddMessage(PostMessageRequest model, CancellationToken cancellationToken)
        {
            // Ensure the user is authenticated.
            if (!User.Identity.IsAuthenticated)
            {
                TempData["FlashMessage"] = "You must be logged in to post a message.";
                return Redirect("/login");
            }

            // Optionally check if the message text is provided.
            if (string.IsNullOrWhiteSpace(model.Text))
            {
                TempData["FlashMessage"] = "Message text cannot be empty.";
                // Redirect back to the timeline. You can adjust the URL as needed.
                return Redirect("/");
            }

            // Determine the publication date. If none provided, use the current Unix timestamp.
            int pubDate = model.PubDate ?? (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            // Create a new message entity.
            var message = new Message
            {
                AuthorId = model.AuthorId, // Ensure this matches the logged-in user in production.
                Text = model.Text,
                PubDate = pubDate,
                Flagged = 0,
            };

            // Add the message and save changes.
            _db.Messages.Add(message);
            await _db.SaveChangesAsync(cancellationToken);

            // Invalidate caches that are impacted by a new message.
            await _hybridCache.RemoveAsync("publicTimeline:0", cancellationToken);
            await _hybridCache.RemoveAsync($"userTimeline:{model.AuthorId}:0", cancellationToken);

            // Invalidate private timelines of all followers of the author.
            var followerIds = await _db.Followers
                .Where(f => f.WhomId == model.AuthorId)
                .Select(f => f.WhoId)
                .ToListAsync(cancellationToken);
            foreach (var followerId in followerIds)
            {
                await _hybridCache.RemoveAsync($"privateTimeline:{followerId}:0", cancellationToken);
            }

            // Flash a success message.
            TempData["FlashMessage"] = "Your message was recorded.";

            // Redirect to the timeline (or any other appropriate page).
            return Redirect("/");
        }
    }
}
