using BuildingBlocks.Core;
using BuildingBlocks.Core.EventBus.Events;
using BuildingBlocks.Core.Mediator;
using CineCloud.Application.Features.Directors.Commands.CreateDirector;
using CineCloud.Application.Features.Directors.Commands.DeleteDirector;
using CineCloud.Application.Features.Directors.Commands.UpdateDirector;
using CineCloud.Queries.Application.Features.Directors.Queries.GetDirector;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CineCloud.WebApi.Controllers;

public class DirectorsController : ApiController
{
    private readonly IMediatorHandler _mediator;
    private readonly IPublishEndpoint _publishEndpoint;

    public DirectorsController(IMediatorHandler mediator, IPublishEndpoint publishEndpoint)
    {
        _mediator = mediator;
        _publishEndpoint = publishEndpoint;
    }


    [HttpGet("[action]/{fullName}", Name = "GetDirector")]
    [ProducesResponseType(typeof(BaseResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult> GetDirector([FromRoute] string fullName)
    {
        var query = new GetDirectorQuery(fullName);

        var response = (GetDirectorResponse)await _mediator.SendQuery(query, HttpContext.RequestAborted);

        if (response is null)
            return CustomResponse((int)HttpStatusCode.NotFound, false);

        return CustomResponse((int)HttpStatusCode.OK, true, response);

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

        var @event = new DirectorCreatedEvent(response.Id, response.FullName, response.CreatedAt, response.UpdatedAt);

        await _publishEndpoint.Publish(@event);

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

        var @event = new DirectorUpdatedEvent(response.Id, response.FullName, response.UpdatedAt);
        await _publishEndpoint.Publish(@event);

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

        var @event = new DirectorDeletedEvent(id.ToString());
        await _publishEndpoint.Publish(@event);

        return CustomResponse((int)HttpStatusCode.OK, response);
    }
}