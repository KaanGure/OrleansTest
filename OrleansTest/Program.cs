using Microsoft.OpenApi.Models;
using Orleans.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseOrleansClient((context, client) =>
{
    client.UseAzureStorageClustering(configureOptions: options =>
    {
        options.TableServiceClient = new Azure.Data.Tables.TableServiceClient("UseDevelopmentStorage=true;");
    });

    client.Configure<ClusterOptions>(options =>
    {
        options.ClusterId = "OrleansTestAppCluster";
        options.ServiceId = "OrleansTestAppService";
    });
});

// Add services to the container.

builder.Services.AddSwaggerGen();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwagger();

app.UseSwaggerUI();

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
