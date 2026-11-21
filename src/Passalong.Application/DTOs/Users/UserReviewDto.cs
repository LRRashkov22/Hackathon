namespace Passalong.Application.DTOs.Users;

public sealed record UserReviewDto(int Rating, string? Comment, DateTime CreatedAt);
