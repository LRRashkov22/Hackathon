using Passalong.Application.Common;
using Passalong.Application.DTOs.Reviews;

namespace Passalong.Application.Services.Contracts;

public interface IReviewService
{
    Task<Result> CreateAsync(Guid userId, CreateReviewRequest request);
}
