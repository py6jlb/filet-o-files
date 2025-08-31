using CSharpFunctionalExtensions;

namespace FiletOFiles.Api.Domain.Entities;

public class Recipe : Entity<long>
{
    public DateTime Created { get; set; }
    public string Title { get; set; }
    public string? Descriptions { get; set; }
    public List<Tag> Tags { get; set; } = [];
    public ICollection<File> Files { get; } = [];
}
