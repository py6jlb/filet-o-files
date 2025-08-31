    using System;
using CSharpFunctionalExtensions;

namespace FiletOFiles.Api.Domain.Entities;

public class Recipe : Entity<long>
{
    DateTime Created { get; set; }
    string? Title { get; set; }
    
}
