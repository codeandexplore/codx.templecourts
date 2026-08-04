namespace Codx.Temple.Application.DTOs.Users;

public record UserProfileDto(Guid Id, string Email, string DisplayName, List<string> Roles, string Status);
