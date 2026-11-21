using Passalong.Application.Common;
using Passalong.Application.DTOs.ExchangeRequests;
using Passalong.Core.Entities;

namespace Passalong.Application.Services.Contracts;

public interface IExchangeRequestService
{
    Task<Result<ExchangeRequest>> CreateAsync(Guid userId, CreateExchangeRequest request);

    Task<List<ExchangeRequest>> GetIncomingAsync(Guid userId);

    Task<List<ExchangeRequest>> GetOutgoingAsync(Guid userId);

    Task<Result<ExchangeRequest>> AcceptAsync(Guid userId, Guid id);

    Task<Result<ExchangeRequest>> RejectAsync(Guid userId, Guid id);

    Task<Result<ExchangeRequest>> CancelAsync(Guid userId, Guid id);

    Task<Result<HandoverConfirmationDto>> ConfirmHandoverAsync(Guid userId, Guid id);
}
