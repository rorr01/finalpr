using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using finalpr.Data;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<finalprContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("finalprContext") ?? throw new InvalidOperationException("Connection string 'finalprContext' not found.")));

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
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=users}/{action=Login}/{id?}");

app.Run();
