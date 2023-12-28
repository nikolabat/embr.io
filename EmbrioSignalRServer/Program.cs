using aipsfaza2.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.StaticFiles;
using System.Buffers;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
string ip = File.ReadAllText("ip.txt").Trim();
Console.WriteLine(ip);
var rabbit = new RabbitMQKom(ip);

builder.Services.AddHostedService<LeaderboardService>();
builder.Services.AddSignalR();
builder.Services.AddSingleton<RabbitMQKom>(rabbit);

var app = builder.Build();



// Configure the HTTP request pipeline.


app.MapHub<GameHub>("gameapi");




app.Run();
