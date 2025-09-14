using System;

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
            .RequireAuthorization();

        return endpointRouteBuilder;
    }
}
