using Microsoft.EntityFrameworkCore;
using Passalong.Application.Abstractions.Persistence;
using Passalong.Application.DTOs.Categories;
using Passalong.Application.Services.Contracts;

namespace Passalong.Application.Services.Implementation;

public class CategoryService(IApplicationDbContext db) : ICategoryService
{
    public Task<List<CategoryDto>> GetAllAsync()
    {
        return db.Categories
            .OrderBy(x => x.Name)
            .Select(x => new CategoryDto(x.Id, x.Name, x.Description, x.Icon))
            .ToListAsync();
    }
}
