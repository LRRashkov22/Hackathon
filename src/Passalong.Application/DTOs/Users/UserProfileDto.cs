namespace Passalong.Application.DTOs.Users;

public sealed record UserProfileDto(
    Guid Id,
    string DisplayName,
    string? Bio,
    string? Location,
    string? AvatarUrl,
    int ReputationScore,
    List<UserListingDto> Listings,
    List<UserReviewDto> Reviews);
