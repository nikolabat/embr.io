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
var config = File.ReadAllLines("ip.txt");
string ip = config[0];
Utils.tickrate = Double.Parse(config[1]);
Console.WriteLine(ip);
var rabbit = new RabbitMQKom(ip);


builder.Services.AddHostedService<LeaderboardService>();
builder.Services.AddHostedService<GameService>();
builder.Services.AddSignalR();
builder.Services.AddSingleton<RabbitMQKom>(rabbit);
builder.Services.AddHostedService<LeaderboardService>();

var app = builder.Build();


app.MapHub<GameHub>("gameapi");




app.Run();
