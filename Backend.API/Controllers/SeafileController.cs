using Backend.API.Common;
using Backend.API.Features.Seafile.CreateSeafileTrack;
using Backend.API.Features.Seafile.DeleteSeafileTrack;
using Backend.API.Features.Seafile.GetSeafileFolders;
using Backend.API.Features.Seafile.GetSeafileTrack;
using Backend.API.Features.Seafile.GetSeafileTracks;
using Backend.API.Features.Seafile.Models;
using Backend.API.Features.Seafile.UpdateSeafileTrack;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeafileController : ControllerBase
{
    private readonly IMediator _mediator;

    public SeafileController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [ProducesResponseType(typeof(Paged<SeafileTrackDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetSeafileTracks([FromQuery] SeafileTrackFilter filter, CancellationToken ct)
        => Ok(await _mediator.Send(new GetSeafileTracksQuery(filter), ct));

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SeafileTrackDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSeafileTrack(string id, CancellationToken ct)
    {
        var track = await _mediator.Send(new GetSeafileTrackQuery(id), ct);
        return track == null ? NotFound() : Ok(track);
    }

    [HttpGet("folders")]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSeafileFolders(CancellationToken ct)
        => Ok(await _mediator.Send(new GetSeafileFoldersQuery(), ct));

    [HttpPost]
    [ProducesResponseType(typeof(SeafileTrackDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSeafileTrack([FromBody] CreateSeafileTrackRequest request, CancellationToken ct)
    {
        var track = await _mediator.Send(new CreateSeafileTrackCommand(request.Title, request.Src, request.Folder), ct);
        return CreatedAtAction(nameof(GetSeafileTrack), new { id = track.Id }, track);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(SeafileTrackDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateSeafileTrack(string id, [FromBody] UpdateSeafileTrackRequest request, CancellationToken ct)
    {
        var track = await _mediator.Send(new UpdateSeafileTrackCommand(id, request.Title, request.Src, request.Folder), ct);
        return track == null ? NotFound() : Ok(track);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSeafileTrack(string id, CancellationToken ct)
    {
        var deleted = await _mediator.Send(new DeleteSeafileTrackCommand(id), ct);
        return deleted ? NoContent() : NotFound();
    }
}

