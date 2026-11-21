namespace Passalong.Application.DTOs.Community;

public sealed record TransferHistoryDto(
    Guid Id,
    DateTime CompletedAt,
    string PreviousOwner,
    string NewOwner);
