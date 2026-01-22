using Microsoft.AspNetCore.Mvc;
using RedShirt.Adventure.Realm.Attributes;
using RedShirt.Adventure.Realm.Character.Character;
using RedShirt.Adventure.Realm.Character.Character.Generated;
using RedShirt.Adventure.Realm.Common.Exceptions;
using RedShirt.Adventure.Realm.Models.Characters.Character;

namespace RedShirt.Adventure.Realm.Controllers.Characters;

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
        string? lastName, [FromQuery] int pageSize, [FromQuery] Guid? continuationToken)
    {
        var response = await characterService.SearchAsync(new CharacterServiceSearchRequest
        {
            AccountId = accountId,
            FirstName = firstName,
            LastName = lastName,
            PageSize = pageSize,
            CreatedBeforeUtc = null,
            CreatedAfterUtc = null,
            UpdatedBeforeUtc = null,
            UpdatedAfterUtc = null,
            AccountIdContains = null,
            FirstNameContains = null,
            LastNameContains = null
        }, continuationToken);

        return Ok(response);
    }

    [HttpPatch("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CharacterDto))]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Patch([FromRoute] Guid id, [FromBody] CharacterPatchRequest patchRequest)
    {
        try
        {
            var result = await characterService.PatchAsync(new CharacterServicePatchRequest
            {
                Id = id,
                AccountId = patchRequest.AccountId,
                FirstName = patchRequest.FirstName,
                LastName = patchRequest.LastName
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
        catch (ConflictException e)
        {
            return Problem(statusCode: StatusCodes.Status409Conflict, title: e.Message);
        }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CharacterDto))]
    public async Task<IActionResult> Post([FromBody] CharacterPostRequest postRequest)
    {
        try
        {
            var response = await characterService.PostAsync(new CharacterServicePostRequest
            {
                AccountId = postRequest.AccountId,
                FirstName = postRequest.FirstName,
                LastName = postRequest.LastName
            });

            return Ok(response);
        }
        catch (BadRequestException e)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, title: e.Message);
        }
        catch (ConflictException e)
        {
            return Problem(statusCode: StatusCodes.Status409Conflict, title: e.Message);
        }
    }
}