namespace Passalong.Application.DTOs.Categories;

public sealed record CategoryDto(Guid Id, string Name, string? Description, string? Icon);
