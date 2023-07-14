using System.Reflection;
using Data.Models.DB;
using Data.Models.Framework;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.Services.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NapooMigration.Data;
using NapooMigration.Models;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddTransient<ImportService>();

builder.Services.AddDbContext<napoo_jessieContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("NpgsqlConnection")), ServiceLifetime.Transient);
//builder.Services.AddDbContext<napoo_jessieContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection1")), ServiceLifetime.Transient);
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Transient);



var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

Config.RegisterMappings(typeof(ErrorViewModel).GetTypeInfo().Assembly);


using (var scope = app.Services.CreateScope())
{
    var loggerFactory = scope.ServiceProvider.GetService<ILoggerFactory>();
    var context = scope.ServiceProvider.GetService<ApplicationDbContext>();

    var pathSetting = context.Settings.Where(x => x.SettingIntCode.Equals("ResourcesFolderName")).First();
    loggerFactory.AddFile($"{pathSetting.SettingValue}\\log.txt");
}

app.UseHttpsRedirection();

app.UseStaticFiles();


app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();