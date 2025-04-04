using System.Text.Json.Serialization;

namespace MiniTwit.Shared.DTO.Followers.FollowUser;

public record UnfollowRequest([property: JsonPropertyName("unfollow")] string Unfollow);
