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

builder.Services.AddEndpointsApiExplorer();

WebApplication app = builder.Build();

app.MapFeatures();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    await app.ApplyMigrations();
    app.UseDeveloperExceptionPage();
}
app.UseHttpsRedirection();
app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

await app.RunAsync();
