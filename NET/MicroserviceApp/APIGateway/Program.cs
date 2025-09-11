using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Microsoft.Extensions.Hosting;


var builder = WebApplication.CreateBuilder(args);

// Configure App Configuration to load configuration.json
builder.Configuration.AddJsonFile("configuration.json", optional: false, reloadOnChange: true);

// Configure services
builder.Services.AddOcelot();

var app = builder.Build();

// Configure HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting(); // Ocelot needs routing

// Use Ocelot middleware, .Wait() is important for async config loading
app.UseOcelot().Wait();

app.Run();