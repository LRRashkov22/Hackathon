using Passalong.Core.Enums;

namespace Passalong.Application.DTOs.ExchangeRequests;

public sealed record HandoverConfirmationDto(
    Guid Id,
    ExchangeRequestStatus Status,
    bool RequesterConfirmed,
    bool OwnerConfirmed);
