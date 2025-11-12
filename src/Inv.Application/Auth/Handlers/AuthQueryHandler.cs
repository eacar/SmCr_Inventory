using System.Security.Claims;
using FluentValidation;
using Inv.Application.Auth.Queries;
using Inv.Application.Auth.Responses;
using Inv.Application.Base;
using Inv.Application.Contracts.Persistence;
using Inv.Application.Contracts.Security;
using Inv.Domain.Exceptions;
using MediatR;

namespace Inv.Application.Auth.Handlers
{
    public class AuthQueryHandler : HandlerBase
        , IRequestHandler<LoginQuery, LoginResponse>
    {
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly IUserRepository _userRepository;
        private readonly IValidator<LoginQuery> _loginValidator;

        public AuthQueryHandler(
            IPasswordHasher passwordHasher,
            ITokenService tokenService,
            IUserRepository userRepository,
            IValidator<LoginQuery> loginValidator)
        {
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _userRepository = userRepository;
            _loginValidator = loginValidator;
        }

        public async Task<LoginResponse> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            var validation = await _loginValidator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
                throw new ValidationException(validation.Errors);

            var authRsp = await _userRepository.GetAuthAsync(request.Email, cancellationToken);

            if (authRsp == null
                || !_passwordHasher.ValidateHashedPassword(request.Password, authRsp.Value.PasswordHash))
                throw new BusinessException("ErrorCodes.A000077", "Login, wrong credentials");

            var claims = new List<Claim>
            {
                new(AppClaims.UserSub, authRsp.Value.UserId.ToString())
            };

            return new LoginResponse
            {
                Token = _tokenService.CreateToken(claims)
            };
        }
    }
}