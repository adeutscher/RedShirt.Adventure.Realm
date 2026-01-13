using Microsoft.AspNetCore.Mvc;
using RedShirt.Adventure.Realm.Attributes;
using RedShirt.Adventure.Realm.Characters.Core.Models;
using RedShirt.Adventure.Realm.Characters.Core.Models.Requests;
using RedShirt.Adventure.Realm.Characters.Core.Models.Responses;
using RedShirt.Adventure.Realm.Characters.Core.Services;
using RedShirt.Adventure.Realm.Common.Exceptions;
using RedShirt.Adventure.Realm.Models.Characters;

namespace RedShirt.Adventure.Realm.Controllers;

[ApiController]
[Route("character")]
[ProducesJson]
public class CharacterController(ICharacterService characterService) : ControllerBase
{
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        try
        {
            await characterService.DeleteAsync(id);
            return Ok();
        }
        catch (ResourceNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CharacterDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        try
        {
            var model = await characterService.GetByIdAsync(id);
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

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CharacterSearchResponse))]
    public async Task<IActionResult> GetList([FromQuery] string? accountId, [FromQuery] string? firstName,
        [FromQuery]
        string? lastName, [FromQuery] int? mapId, [FromQuery] string? continuationToken)
    {
        var response = await characterService.SearchAsync(new CharacterSearchRequest
        {
            AccountId = accountId,
            FirstName = firstName,
            LastName = lastName,
            MapId = mapId,
            ContinuationToken = continuationToken
        });

        return Ok(response);
    }

    [HttpPatch("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CharacterDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    public async Task<IActionResult> Patch([FromRoute] Guid id, [FromBody] CharacterPatchRequest patchRequest)
    {
        try
        {
            var result = await characterService.PatchAsync(new CharacterDtoPatchRequest
            {
                Id = id,
                AccountId = patchRequest.AccountId,
                FirstName = patchRequest.FirstName,
                LastName = patchRequest.LastName,
                MapId = patchRequest.MapId,
                PositionX = patchRequest.PositionX,
                PositionY = patchRequest.PositionY,
                PositionZ = patchRequest.PositionZ,
                Rotation = patchRequest.Rotation,
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

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CharacterDto))]
    public async Task<IActionResult> Post([FromQuery] string accountId, [FromQuery] string firstName,
        [FromQuery]
        string lastName)
    {
        try
        {
            var response = await characterService.PostAsync(new CharacterPostRequest
            {
                AccountId = accountId,
                FirstName = firstName,
                LastName = lastName
            });

            return Ok(response);
        }
        catch (BadRequestException e)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, title: e.Message);
        }
    }
}