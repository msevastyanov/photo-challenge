using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using PhotoChallenge.DataAccess.Configuration;
using PhotoChallenge.DataAccess.Context;
using PhotoChallenge.Domain.Models;
using PhotoChallenge.BusinessLogic.Infrastructure;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using PhotoChallenge.Server.Extensions;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
DbConfig.ConfigureDb(builder.Services, builder.Configuration.GetConnectionString("DefaultConnection"));
WebConfig.ConfigureServices(builder.Services);
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<DataContext>();

builder.Services.AddIdentityServer()
    .AddApiAuthorization<ApplicationUser, DataContext>(options =>
    {
        options.IdentityResources["openid"].UserClaims.Add("role");
        options.ApiResources.Single().UserClaims.Add("role");
    });

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("role");

builder.Services.AddAuthentication()
    .AddIdentityServerJwt();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseExceptionHandler(c => c.Run(async context =>
{
    var exception = context?.Features
        .Get<IExceptionHandlerPathFeature>()
        .Error;
    var response = new { error = exception?.Message };
    await context.Response.WriteAsJsonAsync(response);
}));

app.UseRouting();

app.UseIdentityServer();
app.UseAuthentication();
app.UseAuthorization();

var serviceScope = app.Services.CreateScope();
var dbContext = serviceScope.ServiceProvider.GetService<DataContext>();
var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

dbContext?.EnsureSeeds(roleManager);

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();