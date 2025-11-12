using Inv.Domain.Base;

namespace Inv.Domain.Users
{
    public class User : SoftDeleteBase<Guid>
    {
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        //We could also have PasswordExpiresOn, IsActivationRequired, MustChangePassword, PasswordHistory and so on
    }
}
