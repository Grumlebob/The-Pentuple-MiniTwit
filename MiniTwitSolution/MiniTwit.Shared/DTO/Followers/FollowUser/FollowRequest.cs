using System.Text.Json.Serialization;

namespace MiniTwit.Shared.DTO.Followers.FollowUser;

public record FollowRequest([property: JsonPropertyName("follow")] string Follow);
