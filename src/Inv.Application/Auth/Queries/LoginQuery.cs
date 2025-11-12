using Inv.Application.Auth.Responses;
using MediatR;

namespace Inv.Application.Auth.Queries
{
    public class LoginQuery : IRequest<LoginResponse>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}