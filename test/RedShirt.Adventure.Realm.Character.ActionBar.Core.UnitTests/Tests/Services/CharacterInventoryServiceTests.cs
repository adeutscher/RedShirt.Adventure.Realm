using Moq;
using RedShirt.Adventure.Realm.Character.ActionBar.Core.Models;
using RedShirt.Adventure.Realm.Character.ActionBar.Core.Services;
using RedShirt.Adventure.Realm.Common.Exceptions;
using Xunit;

namespace RedShirt.Adventure.Realm.Character.ActionBar.Core.UnitTests.Tests.Services;

public class CharacterActionBarServiceTests
{
    [Fact]
    public async Task Test_Get()
    {
        var characterGuid = Guid.NewGuid();

        var returnObject = new CharacterActionBarModel
        {
            CharacterId = Guid.NewGuid(),
            Actions = []
        };

        var repo = new Mock<ICharacterActionBarRepository>(MockBehavior.Strict);
        repo
            .Setup(r => r.GetAsync(characterGuid, TestContext.Current.CancellationToken))
            .ReturnsAsync(returnObject);
        var service = new CharacterActionBarService(repo.Object);
        var response = await service.GetByCharacterIdAsync(characterGuid, TestContext.Current.CancellationToken);
        Assert.NotNull(response);
        Assert.Same(returnObject, response);

        repo.Verify(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    ///     Happy-path of set with empty ActionBar
    /// </summary>
    [Fact]
    public async Task Test_Set_Happy_A()
    {
        var setObject = new CharacterActionBarWriteBundle
        {
            CharacterId = Guid.NewGuid(),
            Slots = []
        };

        var repo = new Mock<ICharacterActionBarRepository>(MockBehavior.Strict);
        repo
            .Setup(r => r.SetAsync(setObject, TestContext.Current.CancellationToken))
            .Returns(Task.CompletedTask);
        var service = new CharacterActionBarService(repo.Object);
        await service.SetAsync(setObject, TestContext.Current.CancellationToken);

        repo.Verify(r => r.SetAsync(It.IsAny<CharacterActionBarWriteBundle>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    /// <summary>
    ///     Happy-path of set with an action in ActionBar
    /// </summary>
    [Fact]
    public async Task Test_Set_Happy_B()
    {
        var setObject = new CharacterActionBarWriteBundle
        {
            CharacterId = Guid.NewGuid(),
            Slots =
            [
                new CharacterActionBarWriteBundle.Slot
                {
                    SlotId = 0,
                    ActionType = CharacterActionBarActionType.Item,
                    SubjectId = Guid.NewGuid()
                }
            ]
        };

        var repo = new Mock<ICharacterActionBarRepository>(MockBehavior.Strict);
        repo
            .Setup(r => r.SetAsync(setObject, TestContext.Current.CancellationToken))
            .Returns(Task.CompletedTask);
        var service = new CharacterActionBarService(repo.Object);
        await service.SetAsync(setObject, TestContext.Current.CancellationToken);

        repo.Verify(r => r.SetAsync(It.IsAny<CharacterActionBarWriteBundle>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    /// <summary>
    ///     Happy-path of set with multiple items in ActionBar
    /// </summary>
    [Fact]
    public async Task Test_Set_Happy_C()
    {
        var setObject = new CharacterActionBarWriteBundle
        {
            CharacterId = Guid.NewGuid(),
            Slots =
            [
                new CharacterActionBarWriteBundle.Slot
                {
                    SlotId = 0,
                    ActionType = CharacterActionBarActionType.Item,
                    SubjectId = Guid.NewGuid()
                },
                new CharacterActionBarWriteBundle.Slot
                {
                    SlotId = 1,
                    ActionType = CharacterActionBarActionType.Item,
                    SubjectId = Guid.NewGuid()
                }
            ]
        };

        var repo = new Mock<ICharacterActionBarRepository>(MockBehavior.Strict);
        repo
            .Setup(r => r.SetAsync(setObject, TestContext.Current.CancellationToken))
            .Returns(Task.CompletedTask);
        var service = new CharacterActionBarService(repo.Object);
        await service.SetAsync(setObject, TestContext.Current.CancellationToken);

        repo.Verify(r => r.SetAsync(It.IsAny<CharacterActionBarWriteBundle>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    /// <summary>
    ///     Unhappy-path of set with a request that talks about assigning to the same slot more than once in a single request
    /// </summary>
    [Fact]
    public async Task Test_Set_Unhappy_Dupe_Slot()
    {
        var setObject = new CharacterActionBarWriteBundle
        {
            CharacterId = Guid.NewGuid(),
            Slots =
            [
                new CharacterActionBarWriteBundle.Slot
                {
                    SlotId = 1,
                    ActionType = CharacterActionBarActionType.Item,
                    SubjectId = Guid.NewGuid()
                },
                new CharacterActionBarWriteBundle.Slot
                {
                    SlotId = 1,
                    ActionType = CharacterActionBarActionType.Item,
                    SubjectId = Guid.NewGuid()
                }
            ]
        };

        var repo = new Mock<ICharacterActionBarRepository>(MockBehavior.Strict);
        repo
            .Setup(r => r.SetAsync(setObject, TestContext.Current.CancellationToken))
            .Returns(Task.CompletedTask);
        var service = new CharacterActionBarService(repo.Object);
        var ex = await Assert.ThrowsAsync<BadRequestException>(() =>
            service.SetAsync(setObject, TestContext.Current.CancellationToken));
        Assert.Contains("slot", ex.Message.ToLower());

        repo.Verify(r => r.SetAsync(It.IsAny<CharacterActionBarWriteBundle>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    /// <summary>
    ///     Confirm a complaint if the subject Id is Guid.Empty
    /// </summary>
    [Fact]
    public async Task Test_Set_Unhappy_InstanceId()
    {
        var setObject = new CharacterActionBarWriteBundle
        {
            CharacterId = Guid.NewGuid(),
            Slots =
            [
                new CharacterActionBarWriteBundle.Slot
                {
                    SlotId = 0,
                    SubjectId = Guid.Empty,
                    ActionType = CharacterActionBarActionType.Item
                }
            ]
        };

        var repo = new Mock<ICharacterActionBarRepository>(MockBehavior.Strict);
        var service = new CharacterActionBarService(repo.Object);
        var ex = await Assert.ThrowsAsync<BadRequestException>(() =>
            service.SetAsync(setObject, TestContext.Current.CancellationToken));
        Assert.Contains("subject", ex.Message.ToLower());

        repo.Verify(r => r.SetAsync(It.IsAny<CharacterActionBarWriteBundle>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    /// <summary>
    ///     Confirm a complaint if the slot ID is less than 0
    /// </summary>
    [Fact]
    public async Task Test_Set_Unhappy_Slot()
    {
        var setObject = new CharacterActionBarWriteBundle
        {
            CharacterId = Guid.NewGuid(),
            Slots =
            [
                new CharacterActionBarWriteBundle.Slot
                {
                    SlotId = -1,
                    ActionType = CharacterActionBarActionType.Item,
                    SubjectId = Guid.NewGuid()
                }
            ]
        };

        var repo = new Mock<ICharacterActionBarRepository>(MockBehavior.Strict);
        var service = new CharacterActionBarService(repo.Object);
        var ex = await Assert.ThrowsAsync<BadRequestException>(() =>
            service.SetAsync(setObject, TestContext.Current.CancellationToken));
        Assert.Contains("slot", ex.Message.ToLower());

        repo.Verify(r => r.SetAsync(It.IsAny<CharacterActionBarWriteBundle>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}