namespace Passalong.Application.DTOs.Reviews;

public record CreateReviewRequest(Guid TransferId, int Rating, string? Comment);
