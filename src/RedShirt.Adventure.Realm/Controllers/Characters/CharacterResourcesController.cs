using Microsoft.AspNetCore.Mvc;
using RedShirt.Adventure.Realm.Attributes;
using RedShirt.Adventure.Realm.Character.CharacterResources;
using RedShirt.Adventure.Realm.Character.CharacterResources.Generated;
using RedShirt.Adventure.Realm.Common.Exceptions;
using RedShirt.Adventure.Realm.Models.Characters.CharacterResources;

namespace RedShirt.Adventure.Realm.Controllers.Characters;

[ApiController]
[Route("character")]
[ProducesJson]
public class CharacterResourcesController(ICharacterResourcesService characterResourcesService) : ControllerBase
{
    [HttpDelete("{id}/resources")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        try
        {
            await characterResourcesService.DeleteAsync(id);
            return Ok();
        }
        catch (ResourceNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("{id}/resources")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CharacterResourcesDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        try
        {
            var model = await characterResourcesService.GetByIdAsync(id);
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

    [HttpGet("attributes/resources")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CharacterResourcesSearchResponse))]
    public async Task<IActionResult> List([FromQuery] int pageSize, [FromQuery] Guid? continuationToken)
    {
        var response = await characterResourcesService.SearchAsync(new CharacterResourcesServiceSearchRequest
        {
            PageSize = pageSize,
            CreatedBeforeUtc = null,
            CreatedAfterUtc = null,
            UpdatedBeforeUtc = null,
            UpdatedAfterUtc = null
        }, continuationToken);

        return Ok(response);
    }

    [HttpPatch("{id}/resources")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CharacterResourcesDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    public async Task<IActionResult> Patch([FromRoute] Guid id, [FromBody] CharacterResourcesPatchRequest patchRequest)
    {
        try
        {
            var result = await characterResourcesService.PatchAsync(new CharacterResourcesServicePatchRequest
            {
                CharacterId = id,
                HealthCurrent = patchRequest.HealthCurrent,
                HealthMaximum = patchRequest.HealthMaximum
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

    [HttpPut("{id}/resources")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CharacterResourcesDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] CharacterResourcesPutRequest putRequest)
    {
        try
        {
            var result = await characterResourcesService.PutAsync(new CharacterResourcesServicePutRequest
            {
                CharacterId = id,
                HealthCurrent = putRequest.HealthCurrent,
                HealthMaximum = putRequest.HealthMaximum
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