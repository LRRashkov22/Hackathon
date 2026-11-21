using Passalong.Core.Enums;

namespace Passalong.Application.DTOs.Users;

public sealed record UserListingDto(Guid Id, string Title, ListingStatus Status);
