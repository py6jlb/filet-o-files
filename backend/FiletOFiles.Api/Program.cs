using FiletOFiles.Api.Extensions;
using FiletOFiles.Api.Features;
using FiletOFiles.Api.Infrastructure.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(connectionString);
    options.UseSnakeCaseNamingConvention();
});

builder
    .Services.AddIdentityApiEndpoints<IdentityUser>(options =>
        options.SignIn.RequireConfirmedAccount = true
    )
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddFeatures();
builder.Services.AddControllers(o =>
{
    o.ReturnHttpNotAcceptable = true;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddOpenTelemetry();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    await app.ApplyMigrations();
}
app.MapIdentityApi<IdentityUser>();
app.UseHttpsRedirection();
app.MapControllers();

await app.RunAsync();
