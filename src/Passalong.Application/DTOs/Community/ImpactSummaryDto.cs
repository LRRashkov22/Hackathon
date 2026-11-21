namespace Passalong.Application.DTOs.Community;

public sealed record ImpactSummaryDto(
    int CompletedTransfers,
    int ItemsReused,
    int ActiveListings,
    int Members);
