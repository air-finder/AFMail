using Domain.Entities.Dtos;

namespace Infra.Security;

public interface IJwtService
{
    string CreateToken(UserDto user);
}