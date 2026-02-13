using Microsoft.AspNetCore.Mvc;
using RedShirt.Adventure.Realm.Attributes;
using RedShirt.Adventure.Realm.Character.CharacterPartyData;
using RedShirt.Adventure.Realm.Character.CharacterPartyData.Generated;
using RedShirt.Adventure.Realm.Common.Exceptions;
using RedShirt.Adventure.Realm.Models.Characters.CharacterPartyData;

namespace RedShirt.Adventure.Realm.Controllers.Characters;

[ApiController]
[Route("character")]
[ProducesJson]
public class CharacterPartyDataController(ICharacterPartyDataService characterPartyDataService) : ControllerBase
{
    [HttpDelete("{id}/party-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        try
        {
            await characterPartyDataService.DeleteAsync(id);
            return Ok();
        }
        catch (ResourceNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("{id}/party-data")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CharacterPartyDataDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        try
        {
            var model = await characterPartyDataService.GetByIdAsync(id);
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

    [HttpGet("attributes/party-data")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CharacterPartyDataSearchResponse))]
    public async Task<IActionResult> List([FromQuery] Guid? partyId, [FromQuery] bool? isInParty,
        [FromQuery]
        int pageSize,
        [FromQuery]
        Guid? continuationToken)
    {
        var response = await characterPartyDataService.SearchAsync(new CharacterPartyDataServiceSearchRequest
        {
            PageSize = pageSize,
            CreatedBeforeUtc = null,
            CreatedAfterUtc = null,
            UpdatedBeforeUtc = null,
            UpdatedAfterUtc = null,
            PartyId = partyId,
            IsInParty = isInParty
        }, continuationToken);

        return Ok(response);
    }

    [HttpPatch("{id}/party-data")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CharacterPartyDataDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    public async Task<IActionResult> Patch([FromRoute] Guid id, [FromBody] CharacterPartyDataPatchRequest patchRequest)
    {
        try
        {
            var result = await characterPartyDataService.PatchAsync(new CharacterPartyDataServicePatchRequest
            {
                CharacterId = id,
                PartyId = patchRequest.PartyId,
                IsInParty = patchRequest.IsInParty
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

    [HttpPut("{id}/party-data")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CharacterPartyDataDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] CharacterPartyDataPutRequest putRequest)
    {
        try
        {
            var result = await characterPartyDataService.PutAsync(new CharacterPartyDataServicePutRequest
            {
                CharacterId = id,
                PartyId = putRequest.PartyId,
                IsInParty = putRequest.IsInParty
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