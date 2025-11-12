using System.Net;
using Asp.Versioning;
using AutoMapper;
using Inv.Api.Base;
using Inv.Api.Requests.Auth;
using Inv.Application;
using Inv.Application.Auth.Queries;
using Inv.Application.Auth.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;

namespace Inv.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route(RouteBase.BaseApiRoute)]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public AuthController(IMapper mapper, IMediator mediator)
        {
            _mapper = mapper;
            _mediator = mediator;
        }

        [EnableRateLimiting(AppPolicies.Login)]
        [HttpPost]
        [SwaggerOperation(
            Summary = "Authenticate and get a token",
            Description = "Demo credentials → **username:** `admin@admin.com`  **password:** `password`"
        )]
        [ProducesResponseType(typeof(LoginResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {
            var query = _mapper.Map<LoginQuery>(request);

            var item = await _mediator.Send(query, cancellationToken);

            return Ok(item);
        }
    }
}