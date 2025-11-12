using System.ComponentModel.DataAnnotations;

namespace Inv.Domain.Settings
{
    public class AuthSettings
    {
        public string Secret { get; set; }
        public string Issuer { get; set; }
        [Range(1, 999)] public int Expires { get; set; }
    }
}