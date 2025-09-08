using FiletOFiles.Api.Extensions;
using FiletOFiles.Api.Features;
using FiletOFiles.Api.Infrastructure.Database;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
builder.Services.AddSwaggerGen();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    await app.ApplyMigrations();
}
app.UseHttpsRedirection();

app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
