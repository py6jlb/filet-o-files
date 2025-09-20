using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace FiletOFiles.Api.Features.Auth.Authorize;

public static class AuthorizeEndpoint
{
    public static IEndpointRouteBuilder MapAuthorize(
        this IEndpointRouteBuilder endpointRouteBuilder
    )
    {
        endpointRouteBuilder
            .MapPost("/authorize", HandleAsync)
            .WithName(nameof(AuthorizeEndpoint))
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
        ;
        return endpointRouteBuilder;
    }

    public static async Task<IResult> HandleAsync(
        HttpContext context,
        IOpenIddictScopeManager manager,
        SignInManager<IdentityUser> signInManager,
        UserManager<IdentityUser> userManager,
        CancellationToken cancellationToken = default
    )
    {
        var request =
            context.GetOpenIddictServerRequest()
            ?? throw new InvalidOperationException(
                "The OpenID Connect request cannot be retrieved."
            );

        var properties = new AuthenticationProperties(
            new Dictionary<string, string?>
            {
                [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                    "The username/password couple is invalid.",
            }
        );

        return TypedResults.Forbid(
            properties,
            [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme]
        );
    }
}
