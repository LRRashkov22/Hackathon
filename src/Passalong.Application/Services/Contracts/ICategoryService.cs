using Passalong.Application.DTOs.Categories;

namespace Passalong.Application.Services.Contracts;

public interface ICategoryService
{
    Task<List<CategoryDto>> GetAllAsync();
}
