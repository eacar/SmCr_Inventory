using Inv.Application.Contracts.Injections;

namespace Inv.Application.Contracts.Security
{
    public interface IPasswordHasher : ISingletonInstance
    {
        string ToHashedPassword(string input);
        bool ValidateHashedPassword(string input, string correctHash);
    }
}