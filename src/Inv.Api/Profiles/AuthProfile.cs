using AutoMapper;
using Inv.Api.Requests.Auth;
using Inv.Application.Auth.Queries;

namespace Inv.Api.Profiles
{
    public class AuthProfile : Profile
    {
        public AuthProfile()
        {
            this.CreateMap<LoginRequest, LoginQuery>();
        }
    }
}