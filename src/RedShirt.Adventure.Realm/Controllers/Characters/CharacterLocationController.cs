using Microsoft.AspNetCore.Mvc;
using RedShirt.Adventure.Realm.Attributes;
using RedShirt.Adventure.Realm.Character.CharacterLocation;
using RedShirt.Adventure.Realm.Character.CharacterLocation.Generated;
using RedShirt.Adventure.Realm.Common.Exceptions;
using RedShirt.Adventure.Realm.Models.Characters.CharacterLocation;

namespace RedShirt.Adventure.Realm.Controllers.Characters;

[ApiController]
[Route("character")]
[ProducesJson]
public class CharacterLocationController(ICharacterLocationService characterLocationService) : ControllerBase
{
    [HttpDelete("{id}/location")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        try
        {
            await characterLocationService.DeleteAsync(id);
            return Ok();
        }
        catch (ResourceNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("{id}/location")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CharacterLocationDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        try
        {
            var model = await characterLocationService.GetByIdAsync(id);
            return Ok(model);
        }
        catch (BadRequestException e)
        {
            return BadRequest(e.Message);
        }
        catch (ResourceNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("attributes/location")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CharacterLocationSearchResponse))]
    public async Task<IActionResult> List([FromQuery] int? mapId, [FromQuery] int pageSize,
        [FromQuery]
        Guid? continuationToken)
    {
        var response = await characterLocationService.SearchAsync(new CharacterLocationServiceSearchRequest
        {
            PageSize = pageSize,
            CreatedBeforeUtc = null,
            CreatedAfterUtc = null,
            UpdatedBeforeUtc = null,
            UpdatedAfterUtc = null,
            MapId = mapId,
            MapInstanceId = null,
            MapIdGreaterThan = null,
            MapIdLessThan = null,
            PositionXGreaterThan = null,
            PositionXLessThan = null,
            PositionYGreaterThan = null,
            PositionYLessThan = null,
            PositionZGreaterThan = null,
            PositionZLessThan = null,
            RotationGreaterThan = null,
            RotationLessThan = null
        }, continuationToken);

        return Ok(response);
    }

    [HttpPatch("{id}/location")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CharacterLocationDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    public async Task<IActionResult> Patch([FromRoute] Guid id, [FromBody] CharacterLocationPatchRequest patchRequest)
    {
        try
        {
            var result = await characterLocationService.PatchAsync(new CharacterLocationServicePatchRequest
            {
                CharacterId = id,
                MapId = patchRequest.MapId,
                MapInstanceId = patchRequest.MapInstanceId,
                PositionX = patchRequest.PositionX,
                PositionY = patchRequest.PositionY,
                PositionZ = patchRequest.PositionZ,
                Rotation = patchRequest.Rotation
            });
            return Ok(result);
        }
        catch (BadRequestException e)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, title: e.Message);
        }
        catch (NoChangesToModifyException)
        {
            return StatusCode(StatusCodes.Status304NotModified);
        }
        catch (ResourceNotFoundException)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, title: "Resource not found");
        }
    }

    [HttpPut("{id}/location")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CharacterLocationDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] CharacterLocationPutRequest putRequest)
    {
        try
        {
            var result = await characterLocationService.PutAsync(new CharacterLocationServicePutRequest
            {
                CharacterId = id,
                MapId = putRequest.MapId,
                MapInstanceId = putRequest.MapInstanceId,
                PositionX = putRequest.PositionX,
                PositionY = putRequest.PositionY,
                PositionZ = putRequest.PositionZ,
                Rotation = putRequest.Rotation
            });
            return Ok(result);
        }
        catch (BadRequestException e)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, title: e.Message);
        }
        catch (NoChangesToModifyException)
        {
            return StatusCode(StatusCodes.Status304NotModified);
        }
        catch (ResourceNotFoundException)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, title: "Resource not found");
        }
    }
}