namespace Passalong.Application.DTOs.Community;

public sealed record LeaderboardEntryDto(
    Guid Id,
    string DisplayName,
    int ReputationScore,
    string? Location);
