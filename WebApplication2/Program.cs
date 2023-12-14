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
    pattern: "{controller=VotingEvents}/{action=VotingEventsList}");
//pattern: "{controller=VotingEvents}/{action=VotingEventsList}");

app.Run();
