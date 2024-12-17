using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Final_Project_Web2.Data;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<Final_Project_Web2Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Final_Project_Web2Context") ?? throw new InvalidOperationException("Connection string 'Final_Project_Web2Context' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSession(options => { options.IdleTimeout = TimeSpan.FromMinutes(1); });
var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=usersaccounts}/{action=login}");



app.Run();
