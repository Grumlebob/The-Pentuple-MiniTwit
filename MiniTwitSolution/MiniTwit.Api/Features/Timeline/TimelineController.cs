using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniTwit.Api.Infrastructure; // Contains your MiniTwitDbContext and entities.
// Contains TimelineViewModel, MessageViewModel, UserViewModel.
using System.Linq;
using System.Threading.Tasks;

namespace MiniTwit.Api.Features.Timeline
{
    public class TimelineController : Controller
    {
        private readonly MiniTwitDbContext _db;

        // DbContext is injected via the constructor.
        public TimelineController(MiniTwitDbContext db)
        {
            _db = db;
        }

        // GET "/" - Private timeline
        [HttpGet("/")]
        public async Task<IActionResult> Index(int? offset)
        {
            // In our Python version, if there is no "userId", we redirect to the public timeline.
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("PublicTimeline");
            }


            const int perPage = 30;
            int skip = offset ?? 0;

            // Retrieve the IDs of users that the current user follows.
            var followedUserIds = await _db.Followers
                .Where(f => f.WhoId == userId)
                .Select(f => f.WhomId)
                .ToListAsync();

            followedUserIds.Add((int)userId); // Include self

            // Query the messages where flagged is 0 and the author is in the followed list.
            var messages = await _db.Messages
                .Where(m => (m.Flagged ?? 0) == 0 && followedUserIds.Contains(m.AuthorId))
                .OrderByDescending(m => m.PubDate)
                .Skip(skip)
                .Take(perPage)
                .Include(m => m.Author)
                .ToListAsync();

            // Map the messages to your view model.
            var viewModel = new TimelineViewModel
            {
                CurrentUserId = userId,
                Messages = messages.Select(m => new MessageViewModel
                {
                    MessageId = m.MessageId,
                    Text = m.Text,
                    PubDate = m.PubDate,
                    Author = new UserViewModel
                    {
                        UserId = m.Author.UserId,
                        Username = m.Author.Username,
                        Email = m.Author.Email
                    }
                }).ToList()
            };

            // Render the "Timeline" view (i.e. Views/Timeline/Timeline.cshtml) with the view model.
            return View("Timeline", viewModel);
        }

// GET "/public" - Public timeline
        [HttpGet("/public")]
        public async Task<IActionResult> PublicTimeline(int? offset)
        {
            const int perPage = 30;
            int skip = offset ?? 0;

            // Retrieve public messages (all messages where flagged is 0).
            var messages = await _db.Messages
                .Where(m => (m.Flagged ?? 0) == 0)
                .OrderByDescending(m => m.PubDate)
                .Skip(skip)
                .Take(perPage)
                .Include(m => m.Author)
                .ToListAsync();

            var viewModel = new TimelineViewModel
            {
                Messages = messages.Select(m => new MessageViewModel
                {
                    MessageId = m.MessageId,
                    Text = m.Text,
                    PubDate = m.PubDate,
                    Author = new UserViewModel
                    {
                        UserId = m.Author.UserId,
                        Username = m.Author.Username,
                        Email = m.Author.Email
                    }
                }).ToList()
            };

            return View("Timeline", viewModel);
        }

// GET /{username}
        [HttpGet("{username}", Order = 1)] // order =1 makes it prioritize /login etc who has order 0
        public async Task<IActionResult> UserTimeline(string username, int? offset)
        {
            // Look up the user by username.
            var profileUser = await _db.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (profileUser == null)
            {
                return NotFound($"User '{username}' not found.");
            }

            const int perPage = 30;
            int skip = offset ?? 0;

            // Query for messages posted by this user.
            var messages = await _db.Messages
                .Where(m => m.AuthorId == profileUser.UserId && (m.Flagged ?? 0) == 0)
                .OrderByDescending(m => m.PubDate)
                .Skip(skip)
                .Take(perPage)
                .Include(m => m.Author)
                .ToListAsync();

            // Determine if the currently logged in user is following this user.
            bool followed = false;
            int? currentUserId = null;
            if (User.Identity.IsAuthenticated)
            {
                if (int.TryParse(User.FindFirst("UserId")?.Value, out int id))
                {
                    currentUserId = id;
                    followed = await _db.Followers
                        .AnyAsync(f => f.WhoId == id && f.WhomId == profileUser.UserId);
                }
            }

            // Build the view model.
            var viewModel = new TimelineViewModel
            {
                CurrentUserId = currentUserId, // Set the logged-in user's ID here.
                ProfileUser = new UserViewModel
                {
                    UserId = profileUser.UserId,
                    Username = profileUser.Username,
                    Email = profileUser.Email
                },
                Messages = messages.Select(m => new MessageViewModel
                {
                    MessageId = m.MessageId,
                    Text = m.Text,
                    PubDate = m.PubDate,
                    Author = new UserViewModel
                    {
                        UserId = m.Author.UserId,
                        Username = m.Author.Username,
                        Email = m.Author.Email
                    }
                }).ToList(),
                Followed = followed
            };

            // Render the same view ("Timeline") with the populated view model.
            return View("Timeline", viewModel);
        }
    }
}