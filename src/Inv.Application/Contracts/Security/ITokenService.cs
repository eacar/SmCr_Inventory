using System.Security.Claims;
using Inv.Application.Contracts.Injections;

namespace Inv.Application.Contracts.Security
{
    public interface ITokenService : IScopedInstance
    {
        string CreateToken(List<Claim> claims);
    }
}