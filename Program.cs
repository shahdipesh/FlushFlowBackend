using FlushFlow.Data;
using FlushFlow.Models;
using FlushFlow.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;


var builder = WebApplication.CreateBuilder(args);

// ---------------------------------PRODUCTION--------------------------------------------------------
// var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// builder.Services.AddDbContext<ApplicationDbContext>(options =>
//     options.UseMySql(
//         builder.Configuration.GetConnectionString("DefaultConnection"),
//         new MySqlServerVersion(new Version(5, 5, 54)), // Provide the correct server version here
//         sqlOptions => sqlOptions.EnableRetryOnFailure()));

//------------------------------------DEVELOPMENT---------------------------------------------------

var connectionString = builder.Configuration.GetConnectionString("localConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("localConnection")));

//---------------------------------------------------------------------------------------------------


builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ScheduleService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<TransactionService>();
builder.Services.AddScoped<SplitwiseService>();
builder.Services.AddScoped<UserGroupsService>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout value
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", new CorsPolicyBuilder()
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()
        .Build());
});

builder.Services.ConfigureApplicationCookie(options =>
{
    // other options...

    options.Cookie.SecurePolicy = CookieSecurePolicy.None; // This line removes the secure flag
    options.Cookie.SameSite = SameSiteMode.Lax; // This line removes the SameSite attribute
});


var app = builder.Build();

app.UseSession();
app.UseRouting();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
