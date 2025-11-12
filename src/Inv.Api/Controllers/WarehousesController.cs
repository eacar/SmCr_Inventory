using Asp.Versioning;
using AutoMapper;
using Inv.Api.Base;
using Inv.Api.Requests.Warehouses;
using Inv.Application.Base;
using Inv.Application.Warehouses.Commands;
using Inv.Application.Warehouses.Queries;
using Inv.Application.Warehouses.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Inv.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route(RouteBase.BaseApiRoute)]
    [ApiController]
    [Authorize]
    public class WarehousesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public WarehousesController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("{id:Guid}")]
        [ProducesResponseType(typeof(WarehouseDetailResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        //We could restrict the access even further with some roles here as well
        public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
        {
            var item = await _mediator.Send(new WarehouseGetQuery
            {
                Id = id
            }, cancellationToken);

            if (item is null) return NotFound();
            
            return Ok(item);
        }

        [HttpPost]
        [ProducesResponseType(typeof(AddResponseBase<Guid>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Post([FromBody] WarehouseAddRequest request, CancellationToken cancellationToken)
        {
            var command = _mapper.Map<WarehouseAddCommand>(request);

            var rsp = await _mediator.Send(command, cancellationToken);

            //This could be written more dynamic
            return Created($"/api/v1/warehouses/{rsp.Id}", rsp);
        }

        [HttpPut]
        [Route("{id:Guid}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Put(Guid id, [FromBody] WarehouseUpdateRequest request, CancellationToken cancellationToken)
        {
            var command = _mapper.Map<WarehouseUpdateCommand>(request);
            command.Id = id;

            await _mediator.Send(command, cancellationToken);

            return NoContent();
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _mediator.Send(new WarehouseDeleteCommand
            {
                Id = id
            }, cancellationToken);

            return NoContent();
        }
    }
}