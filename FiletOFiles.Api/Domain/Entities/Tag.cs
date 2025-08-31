using CSharpFunctionalExtensions;

namespace FiletOFiles.Api.Domain.Entities;

public class Tag : Entity<long>
{
    public string Name { get; set; }
    public List<Recipe> Recipes { get; set; } = [];
}
