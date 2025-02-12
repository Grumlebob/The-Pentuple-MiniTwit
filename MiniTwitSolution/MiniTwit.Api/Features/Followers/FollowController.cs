using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniTwit.Api.Infrastructure; // adjust namespace if needed

// if you have view models or entity models here

namespace MiniTwit.Api.Features.Followers
{
    // The route prefix uses the {username} parameter.
    [Route("{username}")]
    public class FollowController : Controller
    {
        private readonly MiniTwitDbContext _db;

        public FollowController(MiniTwitDbContext db)
        {
            _db = db;
        }

        // GET /{username}/follow
        [HttpGet("follow")]
        public async Task<IActionResult> Follow(string username)
        {
            // Check that the user is authenticated.
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            // Assume the current user's id is stored in a claim called "UserId"
            if (!int.TryParse(User.FindFirst("UserId")?.Value, out int currentUserId))
            {
                return Unauthorized();
            }

            // Find the user to be followed by username.
            var targetUser = await _db.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (targetUser == null)
            {
                return NotFound($"User '{username}' not found.");
            }

            // Optional: Check if already following.
            bool alreadyFollowing = await _db.Followers.AnyAsync(f => f.WhoId == currentUserId && f.WhomId == targetUser.UserId);
            if (!alreadyFollowing)
            {
                // Add a new follower record.
                _db.Followers.Add(new Follower
                {
                    WhoId = currentUserId,
                    WhomId = targetUser.UserId
                });
                await _db.SaveChangesAsync();

                // Flash a message using TempData.
                TempData["FlashMessage"] = $"You are now following {username}.";
            }
            else
            {
                TempData["FlashMessage"] = $"You are already following {username}.";
            }

            // Redirect to the target user's timeline.
            return Redirect($"/{username}");
        }

        // GET /{username}/unfollow
        [HttpGet("unfollow")]
        public async Task<IActionResult> Unfollow(string username)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            if (!int.TryParse(User.FindFirst("UserId")?.Value, out int currentUserId))
            {
                return Unauthorized();
            }

            var targetUser = await _db.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (targetUser == null)
            {
                return NotFound($"User '{username}' not found.");
            }

            // Find the follower record.
            var follower = await _db.Followers.FirstOrDefaultAsync(
                f => f.WhoId == currentUserId && f.WhomId == targetUser.UserId
            );

            if (follower != null)
            {
                _db.Followers.Remove(follower);
                await _db.SaveChangesAsync();
                TempData["FlashMessage"] = $"You are no longer following {username}.";
            }
            else
            {
                TempData["FlashMessage"] = $"You were not following {username}.";
            }

            // Redirect to the target user's timeline.
            return Redirect($"/{username}");
        }
    }
}
