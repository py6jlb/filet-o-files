using FiletOFiles.Api.Extensions;
using FiletOFiles.Api.Features;
using FiletOFiles.Api.Infrastructure.Database;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddOpenTelemetry();
builder.AddDatabase();
builder.AddApplicationServices();
builder.AddAuthenticationServices();

builder.AddErrorHandling();
builder.AddFeatures();

builder.Services.AddControllers(o =>
{
    o.ReturnHttpNotAcceptable = true;
});
builder.Services.AddEndpointsApiExplorer();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    await app.ApplyMigrations();
}
app.UseHttpsRedirection();
app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();
