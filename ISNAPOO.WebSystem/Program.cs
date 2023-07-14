using Blazored.LocalStorage;
using Data.Models.Data.ProviderData;
using Data.Models.Data.Role;
using Data.Models.DB;
using Data.Models.Framework;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.Services.Common;
using ISNAPOO.WebSystem.Extensions;
using ISNAPOO.WebSystem.Framework;
using ISNAPOO.WebSystem.Seeder;
using ISNAPOO.WebSystem.Shared;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Syncfusion.Blazor;
using System.IO.Compression;
using System.Reflection;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
builder.Logging.ClearProviders();
builder.Logging.AddConsole();


// Add services to the container.
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
//builder.Services.ConfigureApplicationCookie(x => { x.ExpireTimeSpan = TimeSpan.FromDays(1); });

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"c:\SharedCookieApp"))
    .SetApplicationName("NAPOOCookieApp");

builder.Services.ConfigureApplicationCookie(options => {
   
    options.Cookie.SameSite = SameSiteMode.Strict;
     

});



builder.Services.AddSingleton<CircuitHandler, TrackingCircuitHandler>();


builder.Services.AddRazorPages(options => {
    var F = builder.Services.BuildServiceProvider().GetService<IStringLocalizerFactory>();
});

builder.Services.AddServerSideBlazor();
builder.Services.AddControllers();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.Cookie.IsEssential = true;
});


builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = ".AspNet.NAPOOCookie";
    options.LoginPath = "/login";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;
    var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.SessionStore = new CustomTicketStore(builder.Services);
});


Config.RegisterMappings(typeof(ErrorViewModel).GetTypeInfo().Assembly);
builder.Services.AddSyncfusionBlazor();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddApplicationDbContext(builder.Configuration);

//Register the Syncfusion locale service to localize Syncfusion Blazor components.
builder.Services.AddSingleton(typeof(ISyncfusionStringLocalizer), typeof(SyncfusionLocalizer));

builder.Services.AddApplicationServices(builder.Configuration);

var settingService = builder.Services.BuildServiceProvider().GetService<ISettingService>();
var policyService = builder.Services.BuildServiceProvider().GetService<IPolicyService>();

await policyService.MergePoliciesAsync();
await settingService.mergeSettingsAsync();


builder.Services.AddApplicationIdentity();


builder.Services.AddScoped<IdentityErrorDescriber, LocalizedIdentityErrorDescriber>();

Config.RegisterMappings(typeof(ErrorViewModel).GetTypeInfo().Assembly);

builder.Services.AddHttpClient();

//builder.Services.Configure<EmailConfiguration>(builder.Configuration.GetSection("EmailConfiguration"));
builder.Services.Configure<EmailConfiguration>(options =>{options.ToCopy(settingService.SetUpEmailConfiguration().Result);});
builder.Services.Configure<ApplicationSetting>(options =>{options.ToCopy(settingService.SetUpАpplicationSetting().Result);});

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});

builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.SmallestSize;
});


var app = builder.Build();

if (builder.Configuration.GetSection("AppSettings")["MigrateDataBase"].ToString() == "YES") {
    await ISNAPOO.WebSystem.Seeder.MigrateDataBase.UpdateDataBase(app);
}



var scheduleService = builder.Services.BuildServiceProvider().GetService<IScheduleService>();  
scheduleService.OnStart();


app.UseRequestLocalization("bg-BG");

app.UseMiddleware<BlazorCookieLoginMiddleware>();



var loggerFactory = app.Services.GetService<ILoggerFactory>();

loggerFactory.AddFile(settingService.SetUpАpplicationSetting().Result.ResourcesFolderName);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
     
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseResponseCompression();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");



app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});




//TrainingSeeder.TrainingDataSeeder(app).Wait();



app.Run();
