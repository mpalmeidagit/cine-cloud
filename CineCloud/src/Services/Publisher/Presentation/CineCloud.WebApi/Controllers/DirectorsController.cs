using BuildingBlocks.Core.Mediator;
using CineCloud.Application.Features.Directors.Commands.CreateDirector;
using CineCloud.Application.Features.Directors.Commands.DeleteDirector;
using CineCloud.Application.Features.Directors.Commands.UpdateDirector;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CineCloud.WebApi.Controllers;

public class DirectorsController : ApiController
{
    private readonly IMediatorHandler _mediator;

    public DirectorsController(IMediatorHandler mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("create-director")]
    [ProducesResponseType(typeof(CreateDirectorResponse), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<CreateDirectorResponse>> CreateDirector(
        [FromBody] CreateDirectorCommand command)
    {
        var response = (CreateDirectorResponse)await _mediator.SendCommand(command, HttpContext.RequestAborted);

        if (response is null)
            return CustomResponse((int)HttpStatusCode.BadRequest, false); 

        return CustomResponse((int)HttpStatusCode.Created, true, response);
    }

    [HttpPut("update-director")]
    [ProducesResponseType(typeof(UpdateDirectorResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<UpdateDirectorResponse>> UpdateDirector(
        [FromBody] UpdateDirectorCommand command)
    {
        var response = (UpdateDirectorResponse)await _mediator.SendCommand(command, HttpContext.RequestAborted);

        if (response is null)
            return CustomResponse((int)HttpStatusCode.BadRequest, false);

        return CustomResponse((int)HttpStatusCode.OK, true, response);
    }

    [HttpDelete("delete-director/{id:guid}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> DeleteDirector([FromRoute] Guid id)
    {
        var command = new DeleteDirectorCommand(id);
        var response = await _mediator.SendCommandAndReturnBool(command, HttpContext.RequestAborted);

        if (!response)
            return CustomResponse((int)HttpStatusCode.BadRequest, response);


        return CustomResponse((int)HttpStatusCode.OK, response);
    }
}