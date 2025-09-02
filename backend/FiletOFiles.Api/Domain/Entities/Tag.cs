using CSharpFunctionalExtensions;

namespace FiletOFiles.Api.Domain.Entities;

public sealed class Tag : Entity<long>
{
    public required string Name { get; set; }
    public required string Color { get; set; }
    public List<Recipe> Recipes { get; set; } = [];
}
