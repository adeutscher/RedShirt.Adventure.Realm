namespace RedShirt.Adventure.Realm.Character.Character.Services;

internal interface ICharacterNameValidator
{
    Task<CharacterNameValidator.ValidationResult> ValidateAsync(string name, string label = "Name",
        CancellationToken cancellationToken = default);
}

internal class CharacterNameValidator : ICharacterNameValidator
{
    public Task<ValidationResult> ValidateAsync(string name, string label = "Name",
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Task.FromResult(new ValidationResult
            {
                IsValid = false,
                ErrorMessage = $"{label} is required."
            });
        }

        return Task.FromResult(new ValidationResult
        {
            IsValid = true,
            ErrorMessage = string.Empty
        });
    }

    internal class ValidationResult
    {
        public required bool IsValid { get; init; }
        public required string ErrorMessage { get; init; }
    }
}