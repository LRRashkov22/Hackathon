using Microsoft.AspNetCore.Mvc;
using Passalong.Application.DTOs.Categories;
using Passalong.Application.Services.Contracts;

namespace Passalong.Api.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController(ICategoryService categories) : ApiControllerBase
{
    [HttpGet]
    public Task<List<CategoryDto>> Get()
    {
        return categories.GetAllAsync();
    }
}
