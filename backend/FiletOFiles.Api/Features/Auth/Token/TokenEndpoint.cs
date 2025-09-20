using System.Security.Claims;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace FiletOFiles.Api.Features.Auth.Token;

public static class TokenEndpoint
{
    public static IEndpointRouteBuilder MapToken(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder
            .MapPost("/token", HandleAsync)
            .WithName(nameof(TokenEndpoint))
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
        if (request.IsPasswordGrantType())
        {
            var user = await userManager.FindByEmailAsync(request.Username!);
            if (user == null)
            {
                var properties = new AuthenticationProperties(
                    new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] =
                            Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                            "The username/password couple is invalid.",
                    }
                );

                return TypedResults.Forbid(
                    properties,
                    [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme]
                );
            }

            var result = await signInManager.CheckPasswordSignInAsync(
                user,
                request.Password!,
                lockoutOnFailure: true
            );

            if (!result.Succeeded)
            {
                var properties = new AuthenticationProperties(
                    new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] =
                            Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                            "The username/password couple is invalid.",
                    }
                );

                return TypedResults.Forbid(
                    properties,
                    [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme]
                );
            }

            var identity = new ClaimsIdentity(
                authenticationType: TokenValidationParameters.DefaultAuthenticationType,
                nameType: Claims.Name,
                roleType: Claims.Role
            );

            identity
                .SetClaim(Claims.Subject, await userManager.GetUserIdAsync(user))
                .SetClaim(Claims.Email, await userManager.GetEmailAsync(user))
                .SetClaim(Claims.Name, await userManager.GetUserNameAsync(user))
                .SetClaim(Claims.PreferredUsername, await userManager.GetUserNameAsync(user))
                .SetClaims(Claims.Role, [.. await userManager.GetRolesAsync(user)]);

            identity.SetScopes(
                new[]
                {
                    Scopes.OpenId,
                    Scopes.Email,
                    Scopes.Profile,
                    Scopes.Roles,
                    Scopes.OfflineAccess,
                }.Intersect(request.GetScopes())
            );

            identity.SetDestinations(claim => [Destinations.AccessToken]);

            return Results.SignIn(
                new ClaimsPrincipal(identity),
                properties: null,
                OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
            );
        }

        if (request.IsRefreshTokenGrantType())
        {
            var result = await context.AuthenticateAsync(
                OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
            );
            var user = await userManager.FindByIdAsync(result.Principal!.GetClaim(Claims.Subject)!);
            if (user == null)
            {
                var properties = new AuthenticationProperties(
                    new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] =
                            Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                            "The refresh token is no longer valid.",
                    }
                );

                return TypedResults.Forbid(
                    properties,
                    [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme]
                );
            }

            if (!await signInManager.CanSignInAsync(user))
            {
                var properties = new AuthenticationProperties(
                    new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] =
                            Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                            "The user is no longer allowed to sign in.",
                    }
                );

                return TypedResults.Forbid(
                    properties,
                    [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme]
                );
            }

            var identity = new ClaimsIdentity(
                result.Principal!.Claims,
                authenticationType: TokenValidationParameters.DefaultAuthenticationType,
                nameType: Claims.Name,
                roleType: Claims.Role
            );

            identity
                .SetClaim(Claims.Subject, await userManager.GetUserIdAsync(user))
                .SetClaim(Claims.Email, await userManager.GetEmailAsync(user))
                .SetClaim(Claims.Name, await userManager.GetUserNameAsync(user))
                .SetClaim(Claims.PreferredUsername, await userManager.GetUserNameAsync(user))
                .SetClaims(Claims.Role, [.. await userManager.GetRolesAsync(user)]);

            identity.SetDestinations(AuthHelpers.GetDestinations);

            return TypedResults.SignIn(
                new ClaimsPrincipal(identity),
                null,
                OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
            );
        }

        throw new NotImplementedException("The specified grant type is not implemented.");
    }
}
