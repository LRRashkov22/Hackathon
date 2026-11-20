using Microsoft.AspNetCore.Mvc; using Microsoft.EntityFrameworkCore; using Passalong.Infrastructure.Persistence;
namespace Passalong.Api.Controllers;
[ApiController, Route("api/categories")] public class CategoriesController(PassalongDbContext db):ControllerBase { [HttpGet] public Task<List<object>> Get()=>db.Categories.OrderBy(x=>x.Name).Select(x=>(object)new{x.Id,x.Name,x.Description,x.Icon}).ToListAsync(); }
