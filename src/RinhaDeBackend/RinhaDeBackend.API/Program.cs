using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using RinhaDeBackend.API.Data;
using RinhaDeBackend.API.Data.Repositories;
using RinhaDeBackend.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
        options.SuppressModelStateInvalidFilter = true
    );

builder.Services.AddSingleton<IDatabaseSession, DatabaseSession>();

builder.Services.AddSingleton<ICustomerRepository, CustomerRepository>();
builder.Services.AddSingleton<ITransactionRepository, TransactionRepository>();

builder.Services.AddSingleton<ILockService, LockService>();

builder.Services.AddSingleton<ICustomerService, CustomerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
