using FiletOFiles.Api.Extensions;
using FiletOFiles.Api.Features;
using FiletOFiles.Api.Helpers;
using FiletOFiles.Api.Infrastructure.Database;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

//builder.AddOpenTelemetry();
builder.AddDatabase();
builder.AddApplicationServices();
builder.AddAuthenticationServices();

builder.AddErrorHandling();
builder.Services.AddEndpointsApiExplorer();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(opt =>
    {
        opt.Layout = ScalarLayout.Modern;
    });
    await app.ApplyMigrations();
    app.UseDeveloperExceptionPage();
}
await app.SeedOpenidData();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapFeatures();
//app.UseHttpsRedirection();
app.UseExceptionHandler();

await app.RunAsync();
