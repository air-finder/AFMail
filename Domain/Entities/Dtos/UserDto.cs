using Domain.Entities.Enums;

namespace Domain.Entities.Dtos;

public class UserDto(Guid id, string login, UserRole role)
{
    public Guid Id { get; set; } = id;
    public string Login { get; set; } = login;
    public UserRole Role { get; set; } = role;
}