using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.WebEncoders;
using System.Text;
using System.Text.Unicode;
using WebApplication2;
using WebApplication2.DB;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IApplicationBuilder, ApplicationBuilder>();
builder.Services.AddMvc();
builder.Services.AddMemoryCache();
builder.Services.AddSession();
builder.Services.AddDbContext<WebappDbContext>();
builder.Services.AddHttpContextAccessor();
builder.Services.Configure<WebEncoderOptions>(options => options.TextEncoderSettings = new System.Text.Encodings.Web.TextEncoderSettings(UnicodeRanges.All));

builder.WebHost.UseIISIntegration();
//builder.WebHost.UseKestrelCore();
//builder.WebHost.UseKestrelHttpsConfiguration();
//builder.WebHost.ConfigureKestrel(options => options.Listen(new System.Net.IPAddress(new byte[] { 192, 168, 0, 8 }), 80));

//builder.WebHost.UseUrls(new string[] { "https://26.127.247.233:80" });
builder.WebHost.UseUrls(new string[] { "https://192.168.0.108:80", "https://26.236.65.40:80" });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//await Host.CreateDefaultBuilder(args)
//    .ConfigureServices(services =>
//    {
//        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
//        services.AddMvc();
//        services.AddMemoryCache();
//        services.AddSession();
//    })
//    .Build()
//    .RunAsync();

app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    //pattern: "{controller=VotingEvents}/{action=VotingEventsList}");
    pattern: "{controller=Login}/{action=RegisterView}");

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

app.Run();
