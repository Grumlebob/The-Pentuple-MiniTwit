namespace MiniTwit.Api.Features.Timeline
{
    public class TimelineViewModel
    {
        // For private timeline, this is the logged-in user’s ID.
        public int? CurrentUserId { get; set; }

        // The list of messages to display.
        public List<MessageViewModel> Messages { get; set; } = new List<MessageViewModel>();

        // For a user timeline, this holds the profile being viewed.
        // If null, it means you’re viewing your own (private) timeline or the public timeline.
        public UserViewModel ProfileUser { get; set; }

        // Indicates if the logged-in user is following the profile (only relevant for a user timeline).
        public bool Followed { get; set; }
    }

    public class MessageViewModel
    {
        public int MessageId { get; set; }
        public string Text { get; set; }
        public int? PubDate { get; set; }
        public UserViewModel Author { get; set; }
    }

    public class UserViewModel
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
    }
}