using System.Text.Json.Serialization;

namespace MiniTwit.Shared.DTO.Followers.FollowUser;

public record FollowOrUnfollowRequest
{
    [JsonPropertyName("follow")]
    public string? Follow { get; init; }

    [JsonPropertyName("unfollow")]
    public string? Unfollow { get; init; }
}
