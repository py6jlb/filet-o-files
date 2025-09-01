using System;
using CSharpFunctionalExtensions;
using FiletOFiles.Api.Domain.Entities;
using FiletOFiles.Api.Infrastructure;
using FiletOFiles.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FiletOFiles.Api.Features.AddRecipe;

internal sealed class AddRecipeHandler : IAddRecipeHandler
{
    private readonly ILogger<AddRecipeHandler> _logger;
    private readonly AppDbContext _db;

    public AddRecipeHandler(ILogger<AddRecipeHandler> logger, AppDbContext db)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<Result> AddRecipe(AddRecipeRequest request)
    {
        return await Task.FromResult(Result.Success(request));
    }
}
