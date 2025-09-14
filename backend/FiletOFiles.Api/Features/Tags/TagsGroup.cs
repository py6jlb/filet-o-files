using System;
using FiletOFiles.Api.Features.Tags.AddTag;
using FiletOFiles.Api.Features.Tags.DeleteTag;
using FiletOFiles.Api.Features.Tags.GetTag;
using FiletOFiles.Api.Features.Tags.GetTags;
using FiletOFiles.Api.Features.Tags.UpdateTag;

namespace FiletOFiles.Api.Features.Tags;

public static class TagsGroup
{
    public static IEndpointRouteBuilder MapTagsGroup(
        this IEndpointRouteBuilder endpointRouteBuilder
    )
    {
        endpointRouteBuilder
            .MapGroup("/tags")
            .WithOpenApi()
            .WithTags("Tags")
            .RequireAuthorization()
            .MapAddTag()
            .MapDeleteTag()
            .MapGetTag()
            .MapGetTags()
            .MapUpdateTag();

        return endpointRouteBuilder;
    }
}
