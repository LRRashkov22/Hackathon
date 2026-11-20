using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Passalong.Core.Entities;
using Passalong.Infrastructure.Persistence;
DotNetEnv.Env.TraversePath().Load();
var builder = WebApplication.CreateBuilder(args);
var connection = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Host=localhost;Database=passalong;Username=postgres;Password=postgres";
builder.Services.AddDbContext<PassalongDbContext>(o => o.UseNpgsql(connection));
builder.Services.AddIdentityCore<ApplicationUser>(o => { o.Password.RequiredLength = 8; o.User.RequireUniqueEmail = true; }).AddRoles<IdentityRole<Guid>>().AddEntityFrameworkStores<PassalongDbContext>();
var jwtKey = builder.Configuration["Jwt:Key"] ?? "development-only-change-this-key-to-a-secure-secret-32";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o => o.TokenValidationParameters = new() { ValidateIssuer = true, ValidateAudience = true, ValidateLifetime = true, ValidateIssuerSigningKey = true, ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "passalong", ValidAudience = builder.Configuration["Jwt:Audience"] ?? "passalong", IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)) });
builder.Services.AddAuthorization(); builder.Services.AddControllers().AddJsonOptions(o => { o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; }); builder.Services.AddEndpointsApiExplorer(); builder.Services.AddSwaggerGen(); builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<PassalongDbContext>();
    await db.Database.MigrateAsync();
    await DatabaseSeeder.SeedAsync(db, services.GetRequiredService<UserManager<ApplicationUser>>());
}
app.UseSwagger(); app.UseSwaggerUI(); app.UseCors(); app.UseAuthentication(); app.UseAuthorization(); app.MapControllers(); app.Run();
