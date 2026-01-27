using Microsoft.AspNetCore.Mvc;
using RedShirt.Adventure.Realm.Attributes;
using RedShirt.Adventure.Realm.Character.Inventory.Core.Models;
using RedShirt.Adventure.Realm.Character.Inventory.Core.Services;
using RedShirt.Adventure.Realm.Common.Exceptions;
using RedShirt.Adventure.Realm.Models.Characters.CharacterInventory;

namespace RedShirt.Adventure.Realm.Controllers.Characters;

[ApiController]
[Route("character")]
[ProducesJson]
public class CharacterInventoryController(ICharacterInventoryService characterInventoryService) : ControllerBase
{
    [HttpGet("{characterId}/inventory")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CharacterInventoryModel))]
    public async Task<IActionResult> Get([FromRoute] Guid characterId)
    {
        var model = await characterInventoryService.GetByCharacterIdAsync(characterId);
        return Ok(model);
    }

    [HttpPut("{characterId}/inventory/_save")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Save([FromRoute] Guid characterId,
        [FromBody]
        CharacterInventoryWriteRequest request)
    {
        try
        {
            await characterInventoryService.SetAsync(new CharacterInventoryWriteBundle
            {
                CharacterId = characterId,
                Slots = request.Slots.Select(s => new CharacterInventoryWriteBundle.Slot
                {
                    SlotId = s.Slot,
                    InstanceId = s.InstanceId,
                    ItemId = s.ItemId,
                    Quantity = s.Quantity
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