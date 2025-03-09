using Microsoft.EntityFrameworkCore;
using syscom.Models;
using syscom.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<DiasFestivosService>();

var conString = builder.Configuration.GetConnectionString("conn") ??
     throw new InvalidOperationException("Connection string 'BloggingContext'" +
    " not found.");

builder.Services.AddDbContext<SyscomdbContext>(options =>
    options.UseSqlServer(conString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Usuarios}/{action=Index}/{id?}");

app.Run();
