using Microsoft.AspNetCore.Mvc;
using RedShirt.Adventure.Realm.Attributes;
using RedShirt.Adventure.Realm.Character.ActionBar.Core.Models;
using RedShirt.Adventure.Realm.Character.ActionBar.Core.Services;
using RedShirt.Adventure.Realm.Common.Exceptions;
using RedShirt.Adventure.Realm.Models.Characters.CharacterActionBar;

namespace RedShirt.Adventure.Realm.Controllers.Characters;

[ApiController]
[Route("character")]
[ProducesJson]
public class CharacterActionBarController(ICharacterActionBarService characterActionBarService) : ControllerBase
{
    [HttpGet("{characterId}/action-bar")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CharacterActionBarModel))]
    public async Task<IActionResult> Get([FromRoute] Guid characterId)
    {
        var model = await characterActionBarService.GetByCharacterIdAsync(characterId);
        return Ok(model);
    }

    [HttpPut("{characterId}/action-bar/_save")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Save([FromRoute] Guid characterId,
        [FromBody]
        CharacterActionBarWriteRequest request)
    {
        try
        {
            await characterActionBarService.SetAsync(new CharacterActionBarWriteBundle
            {
                CharacterId = characterId,
                Slots = request.Slots.Select(s => new CharacterActionBarWriteBundle.Slot
                {
                    SlotId = s.Slot,
                    ActionType = s.ActionType,
                    SubjectId = s.SubjectId,
                    ItemId = s.ItemId
                }).ToList()
            });
        }
        catch (BadRequestException e)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, title: "Bad Request", detail: e.Message);
        }

        return Ok();
    }
}