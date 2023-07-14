using Data.Models.Common;
using Data.Models.Data.ProviderData;
using Data.Models.DB;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Services.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using SoapCore;
using SoapCore.Extensibility;
using SoapServiceNAPOOweb;
using SoapServiceNAPOOweb.Models.EGOV;
using SoapServiceNAPOOweb.Services;
using System.Net;
using System.ServiceModel;

var builder = WebApplication.CreateBuilder(args);


 
 
builder.Services.AddScoped<IWebIntegrationService, WebIntegrationService>();
builder.Services.AddScoped<IAZService, AZService>();
builder.Services.AddScoped<IData, SoapServiceNAPOOweb.Services.Data>();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionWeb")));
builder.Services.AddMvc();
//builder.Services.AddSingleton<IFaultExceptionTransformer, DefaultFaultExceptionTransformer<CustomNamespaceMessage>>();
 




var app = builder.Build();

var azSafeList = builder.Configuration["AZSafeList"];
var webServiceSafeList = builder.Configuration["WebServiceSafeList"];
var egovSafeList = builder.Configuration["EGOVSafeList"];
var egovSafeListRange = builder.Configuration["EGOVSafeListRange"];


var loggerFactory = app.Services.GetService<ILoggerFactory>();

string resourcesFolderName = @"C:\\Resources_NAPOO";
//using (var scope = app.Services.CreateScope())
//{
//    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//    resourcesFolderName = dbContext.Settings.First(x => x.SettingIntCode == "ResourcesFolderName").SettingValue;
//}



//loggerFactory.AddFile("C:\\Resources_NAPOO\\SoapService\\log-{Date}.txt");
loggerFactory.AddFile(resourcesFolderName + "\\SoapService\\log-{Date}.txt");

app.UseMiddleware<AdminSafeListMiddleware>(azSafeList, webServiceSafeList, egovSafeList, egovSafeListRange);




app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.UseSoapEndpoint<IWebIntegrationService>("/Service.asmx", new SoapEncoderOptions(), SoapSerializer.XmlSerializer);
    endpoints.UseSoapEndpoint<IAZService>("/AZService.asmx", new SoapEncoderOptions(), SoapSerializer.XmlSerializer);
    endpoints.UseSoapEndpoint<IData>("/EGOVService.asmx", new SoapEncoderOptions(), SoapSerializer.XmlSerializer);
    //endpoints.UseSoapEndpoint<IData, CustomNamespaceMessage>("/EGOVService.asmx", new SoapEncoderOptions(), SoapSerializer.XmlSerializer);
    //endpoints.UseSoapEndpoint<IData, CustomNamespaceMessage>(opt =>
    //{
    //    opt.Path = "/EGOVService.asmx";
    //    opt.Binding = new BasicHttpBinding();
    //    opt.OmitXmlDeclaration = true;
    //    opt.SoapSerializer = SoapSerializer.XmlSerializer;
    //});
});


app.MapGet("/", () => "Web Service");

app.Run();



