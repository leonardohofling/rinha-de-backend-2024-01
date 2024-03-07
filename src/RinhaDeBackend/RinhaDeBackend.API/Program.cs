#if DEBUG
using Npgsql;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
#endif

using RinhaDeBackend.API;
using RinhaDeBackend.API.Data;
using RinhaDeBackend.API.Data.Repositories;
using RinhaDeBackend.API.Services;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

// Add services to the container.

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
        options.SuppressModelStateInvalidFilter = true
    );

builder.Services.AddSingleton<IConnectionFactory, ConnectionFactory>();

builder.Services.AddSingleton<ICustomerRepository, CustomerRepository>();
builder.Services.AddSingleton<ITransactionRepository, TransactionRepository>();

builder.Services.AddSingleton<ILockService, LockService>();

builder.Services.AddSingleton<ICustomerService, CustomerService>();

builder.Services.AddSingleton(new DiagnosticsConfig());

#if DEBUG
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService(serviceName: builder.Environment.ApplicationName))
    .WithTracing(tracing => tracing
        .AddSource(DiagnosticsConfig.SourceName)
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddNpgsql()
        .AddConsoleExporter()
        .AddOtlpExporter());
#endif

builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

#if DEBUG
app.MapGet("/", () => $"Hello World! OpenTelemetry Trace: {Activity.Current?.Id}");
#endif

app.Run();
