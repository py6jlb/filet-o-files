using FiletOFiles.Api.Features;
using FiletOFiles.Api.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

// using OpenTelemetry;
// using OpenTelemetry.Metrics;
// using OpenTelemetry.Resources;
// using OpenTelemetry.Trace;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));

builder
    .Services.AddIdentityApiEndpoints<IdentityUser>(options =>
        options.SignIn.RequireConfirmedAccount = true
    )
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddFeatures();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// builder
//     .Services.AddOpenTelemetry()
//     .ConfigureResource(r => r.AddService(builder.Environment.ApplicationName))
//     .WithTracing(t => t.AddHttpClientInstrumentation().AddAspNetCoreInstrumentation())
//     .WithMetrics(m =>
//         m.AddHttpClientInstrumentation().AddAspNetCoreInstrumentation().AddRuntimeInstrumentation()
//     )
//     .UseOtlpExporter();

// builder.Logging.AddOpenTelemetry(o =>
// {
//     o.IncludeScopes = true;
//     o.IncludeFormattedMessage = true;
// });

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapIdentityApi<IdentityUser>();
app.UseHttpsRedirection();
app.MapControllers();

await app.RunAsync();
