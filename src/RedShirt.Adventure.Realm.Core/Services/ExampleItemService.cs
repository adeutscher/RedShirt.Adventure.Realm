using RedShirt.Adventure.Realm.Core.Exceptions;
using RedShirt.Adventure.Realm.Core.Models;
using RedShirt.Adventure.Realm.Core.Repositories;

namespace RedShirt.Adventure.Realm.Core.Services;

public interface IExampleItemService
{
    Task DeleteAsync(string name, CancellationToken cancellationToken = default);
    Task<ExampleItemModel> GetAsync(string name, CancellationToken cancellationToken = default);
    Task<ExampleItemListModel> GetListAsync(string? continuationToken, CancellationToken cancellationToken = default);
    Task<ExampleItemModel> PutAsync(ExampleItemModel model, CancellationToken cancellationToken = default);
}

internal class ExampleItemService(IExampleItemRepository repository) : IExampleItemService
{
    public Task<ExampleItemModel> GetAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new BadRequestException("Name is required");
        }

        return repository.GetByName(name, cancellationToken);
    }

    public Task<ExampleItemListModel> GetListAsync(string? continuationToken,
        CancellationToken cancellationToken = default)
    {
        return repository.GetListAsync(continuationToken, cancellationToken);
    }

    public async Task<ExampleItemModel> PutAsync(ExampleItemModel model, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(model.Name))
        {
            throw new BadRequestException("Name is required");
        }

        await repository.Put(model, cancellationToken);

        return model;
    }

    public Task DeleteAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new BadRequestException("Name is required");
        }

        return repository.DeleteByName(name, cancellationToken);
    }
}