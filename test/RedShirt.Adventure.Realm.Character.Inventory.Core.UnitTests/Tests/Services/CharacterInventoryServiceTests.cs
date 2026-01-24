using RedShirt.Adventure.Realm.Character.Inventory.Core.Models;
using RedShirt.Adventure.Realm.Character.Inventory.Core.Services;
using RedShirt.Adventure.Realm.Common.Exceptions;

namespace RedShirt.Adventure.Realm.Character.Inventory.Core.UnitTests.Tests.Services;

public class CharacterInventoryServiceTests
{
    [Fact]
    public async Task Test_Get()
    {
        var characterGuid = Guid.NewGuid();

        var returnObject = new CharacterInventoryModel
        {
            CharacterId = Guid.NewGuid(),
            Items = []
        };

        var repo = new Mock<ICharacterInventoryRepository>(MockBehavior.Strict);
        repo
            .Setup(r => r.GetAsync(characterGuid, TestContext.Current.CancellationToken))
            .ReturnsAsync(returnObject);
        var service = new CharacterInventoryService(repo.Object);
        var response = await service.GetByCharacterIdAsync(characterGuid, TestContext.Current.CancellationToken);
        Assert.NotNull(response);
        Assert.Same(returnObject, response);

        repo.Verify(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    ///     Happy-path of set with empty inventory
    /// </summary>
    [Fact]
    public async Task Test_Set_Happy_A()
    {
        var setObject = new CharacterInventoryWriteBundle
        {
            CharacterId = Guid.NewGuid(),
            Slots = []
        };

        var repo = new Mock<ICharacterInventoryRepository>(MockBehavior.Strict);
        repo
            .Setup(r => r.SetAsync(setObject, TestContext.Current.CancellationToken))
            .Returns(Task.CompletedTask);
        var service = new CharacterInventoryService(repo.Object);
        await service.SetAsync(setObject, TestContext.Current.CancellationToken);

        repo.Verify(r => r.SetAsync(It.IsAny<CharacterInventoryWriteBundle>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    /// <summary>
    ///     Happy-path of set with an item in inventory
    /// </summary>
    [Fact]
    public async Task Test_Set_Happy_B()
    {
        var setObject = new CharacterInventoryWriteBundle
        {
            CharacterId = Guid.NewGuid(),
            Slots =
            [
                new CharacterInventoryWriteBundle.Slot
                {
                    SlotId = 0,
                    InstanceId = Guid.NewGuid(),
                    ItemId = Guid.NewGuid(),
                    Quantity = 1
                }
            ]
        };

        var repo = new Mock<ICharacterInventoryRepository>(MockBehavior.Strict);
        repo
            .Setup(r => r.SetAsync(setObject, TestContext.Current.CancellationToken))
            .Returns(Task.CompletedTask);
        var service = new CharacterInventoryService(repo.Object);
        await service.SetAsync(setObject, TestContext.Current.CancellationToken);

        repo.Verify(r => r.SetAsync(It.IsAny<CharacterInventoryWriteBundle>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    /// <summary>
    ///     Happy-path of set with multiple items in inventory
    /// </summary>
    [Fact]
    public async Task Test_Set_Happy_C()
    {
        var setObject = new CharacterInventoryWriteBundle
        {
            CharacterId = Guid.NewGuid(),
            Slots =
            [
                new CharacterInventoryWriteBundle.Slot
                {
                    SlotId = 0,
                    InstanceId = Guid.NewGuid(),
                    ItemId = Guid.NewGuid(),
                    Quantity = 1
                },
                new CharacterInventoryWriteBundle.Slot
                {
                    SlotId = 1,
                    InstanceId = Guid.NewGuid(),
                    ItemId = Guid.NewGuid(),
                    Quantity = 1
                }
            ]
        };

        var repo = new Mock<ICharacterInventoryRepository>(MockBehavior.Strict);
        repo
            .Setup(r => r.SetAsync(setObject, TestContext.Current.CancellationToken))
            .Returns(Task.CompletedTask);
        var service = new CharacterInventoryService(repo.Object);
        await service.SetAsync(setObject, TestContext.Current.CancellationToken);

        repo.Verify(r => r.SetAsync(It.IsAny<CharacterInventoryWriteBundle>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    /// <summary>
    ///     Unhappy-path of set with a request that talks about assigning the same item instance to multiple slots
    /// </summary>
    [Fact]
    public async Task Test_Set_Unhappy_Dupe_Instance()
    {
        var instanceId = Guid.NewGuid();
        var setObject = new CharacterInventoryWriteBundle
        {
            CharacterId = Guid.NewGuid(),
            Slots =
            [
                new CharacterInventoryWriteBundle.Slot
                {
                    SlotId = 0,
                    InstanceId = instanceId,
                    ItemId = Guid.NewGuid(),
                    Quantity = 1
                },
                new CharacterInventoryWriteBundle.Slot
                {
                    SlotId = 1,
                    InstanceId = instanceId,
                    ItemId = Guid.NewGuid(),
                    Quantity = 1
                }
            ]
        };

        var repo = new Mock<ICharacterInventoryRepository>(MockBehavior.Strict);
        repo
            .Setup(r => r.SetAsync(setObject, TestContext.Current.CancellationToken))
            .Returns(Task.CompletedTask);
        var service = new CharacterInventoryService(repo.Object);
        var ex = await Assert.ThrowsAsync<BadRequestException>(() =>
            service.SetAsync(setObject, TestContext.Current.CancellationToken));
        Assert.Contains("instance", ex.Message.ToLower());

        repo.Verify(r => r.SetAsync(It.IsAny<CharacterInventoryWriteBundle>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    /// <summary>
    ///     Unhappy-path of set with a request that talks about assigning to the same slot more than once in a single request
    /// </summary>
    [Fact]
    public async Task Test_Set_Unhappy_Dupe_Slot()
    {
        var setObject = new CharacterInventoryWriteBundle
        {
            CharacterId = Guid.NewGuid(),
            Slots =
            [
                new CharacterInventoryWriteBundle.Slot
                {
                    SlotId = 1,
                    InstanceId = Guid.NewGuid(),
                    ItemId = Guid.NewGuid(),
                    Quantity = 1
                },
                new CharacterInventoryWriteBundle.Slot
                {
                    SlotId = 1,
                    InstanceId = Guid.NewGuid(),
                    ItemId = Guid.NewGuid(),
                    Quantity = 1
                }
            ]
        };

        var repo = new Mock<ICharacterInventoryRepository>(MockBehavior.Strict);
        repo
            .Setup(r => r.SetAsync(setObject, TestContext.Current.CancellationToken))
            .Returns(Task.CompletedTask);
        var service = new CharacterInventoryService(repo.Object);
        var ex = await Assert.ThrowsAsync<BadRequestException>(() =>
            service.SetAsync(setObject, TestContext.Current.CancellationToken));
        Assert.Contains("slot", ex.Message.ToLower());

        repo.Verify(r => r.SetAsync(It.IsAny<CharacterInventoryWriteBundle>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    /// <summary>
    ///     Confirm a complaint if the instance Id is Guid.Empty
    /// </summary>
    [Fact]
    public async Task Test_Set_Unhappy_InstanceId()
    {
        var setObject = new CharacterInventoryWriteBundle
        {
            CharacterId = Guid.NewGuid(),
            Slots =
            [
                new CharacterInventoryWriteBundle.Slot
                {
                    SlotId = 0,
                    InstanceId = Guid.Empty,
                    ItemId = Guid.NewGuid(),
                    Quantity = 1
                }
            ]
        };

        var repo = new Mock<ICharacterInventoryRepository>(MockBehavior.Strict);
        var service = new CharacterInventoryService(repo.Object);
        var ex = await Assert.ThrowsAsync<BadRequestException>(() =>
            service.SetAsync(setObject, TestContext.Current.CancellationToken));
        Assert.Contains("instance", ex.Message.ToLower());

        repo.Verify(r => r.SetAsync(It.IsAny<CharacterInventoryWriteBundle>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    /// <summary>
    ///     Confirm a complaint if the item Id is Guid.Empty
    /// </summary>
    [Fact]
    public async Task Test_Set_Unhappy_ItemId()
    {
        var setObject = new CharacterInventoryWriteBundle
        {
            CharacterId = Guid.NewGuid(),
            Slots =
            [
                new CharacterInventoryWriteBundle.Slot
                {
                    SlotId = 0,
                    InstanceId = Guid.NewGuid(),
                    ItemId = Guid.Empty,
                    Quantity = 1
                }
            ]
        };

        var repo = new Mock<ICharacterInventoryRepository>(MockBehavior.Strict);
        var service = new CharacterInventoryService(repo.Object);
        var ex = await Assert.ThrowsAsync<BadRequestException>(() =>
            service.SetAsync(setObject, TestContext.Current.CancellationToken));
        Assert.Contains("item", ex.Message.ToLower());

        repo.Verify(r => r.SetAsync(It.IsAny<CharacterInventoryWriteBundle>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    /// <summary>
    ///     Confirm a complaint if the quantity is less than 1
    /// </summary>
    [Fact]
    public async Task Test_Set_Unhappy_Quantity()
    {
        var setObject = new CharacterInventoryWriteBundle
        {
            CharacterId = Guid.NewGuid(),
            Slots =
            [
                new CharacterInventoryWriteBundle.Slot
                {
                    SlotId = 0,
                    InstanceId = Guid.NewGuid(),
                    ItemId = Guid.NewGuid(),
                    Quantity = 0
                }
            ]
        };

        var repo = new Mock<ICharacterInventoryRepository>(MockBehavior.Strict);
        var service = new CharacterInventoryService(repo.Object);
        var ex = await Assert.ThrowsAsync<BadRequestException>(() =>
            service.SetAsync(setObject, TestContext.Current.CancellationToken));
        Assert.Contains("quantity", ex.Message.ToLower());

        repo.Verify(r => r.SetAsync(It.IsAny<CharacterInventoryWriteBundle>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    /// <summary>
    ///     Confirm a complaint if the slot ID is less than 0
    /// </summary>
    [Fact]
    public async Task Test_Set_Unhappy_Slot()
    {
        var setObject = new CharacterInventoryWriteBundle
        {
            CharacterId = Guid.NewGuid(),
            Slots =
            [
                new CharacterInventoryWriteBundle.Slot
                {
                    SlotId = -1,
                    InstanceId = Guid.NewGuid(),
                    ItemId = Guid.NewGuid(),
                    Quantity = 1
                }
            ]
        };

        var repo = new Mock<ICharacterInventoryRepository>(MockBehavior.Strict);
        var service = new CharacterInventoryService(repo.Object);
        var ex = await Assert.ThrowsAsync<BadRequestException>(() =>
            service.SetAsync(setObject, TestContext.Current.CancellationToken));
        Assert.Contains("slot", ex.Message.ToLower());

        repo.Verify(r => r.SetAsync(It.IsAny<CharacterInventoryWriteBundle>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}