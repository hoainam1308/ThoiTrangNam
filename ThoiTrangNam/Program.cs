using DinkToPdf.Contracts;
using DinkToPdf;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;
using ThoiTrangNam.Models;
using ThoiTrangNam.Repository;
using System.Runtime.Loader;
using System.Reflection;
using ThoiTrangNam.Helper;
using System.Configuration;


var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json");
builder.Services.AddDbContext<ApplicationDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
 .AddDefaultTokenProviders()
 .AddDefaultUI()
 .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages();

builder.Services.AddControllersWithViews().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
builder.Services.AddScoped<IProductRepository, EFProductRepository>();
builder.Services.AddScoped<ICategoryRepository, EFCategoryRepository>();
builder.Services.AddScoped<ICouponRepository, EFCouponRepository>();
builder.Services.AddScoped<IClassificationRepository, EFClassificationRepository>();
builder.Services.AddScoped<IProductImageRepository, EFProductImageRepository>();
builder.Services.AddScoped<IOrderRepository, EFOrderRepository>();
builder.Services.AddScoped<IOrderDetailRepository, EFOrderDetailRepository>();
builder.Services.AddSingleton<IVnPayService, EFVnPayService>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailSender, EmailSender>();

//use DinktoPDF 
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
// Add services to the container.
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;

});
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 60000000; // 100MB
});
builder.Services.AddSingleton(x =>
    new PaypalClient(
        builder.Configuration["PaypalOptions:AppId"],
        builder.Configuration["PaypalOptions:AppSecret"],
        builder.Configuration["PaypalOptions:Mode"]
    )
);

builder.Services.AddAuthentication().AddFacebook(opt =>
{
    opt.ClientId = "1022879035835933";
    opt.ClientSecret = "bb5d93a484093b624a3ff63efaf8d95b";
    opt.Events.OnRedirectToAuthorizationEndpoint = (context) =>
    {
        context.Response.Redirect(context.RedirectUri + "&prompt=select_account");
        return Task.CompletedTask;
    };

});
builder.Services.AddAuthentication().AddGoogle(opt =>
{
    opt.ClientId = "615825067142-4hpk11pq924ur6ktdoi6tb5q8npuuf33.apps.googleusercontent.com";
    opt.ClientSecret = "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";
    opt.Events.OnRedirectToAuthorizationEndpoint = (context) =>
    {
        context.Response.Redirect(context.RedirectUri + "&prompt=select_account");
        return Task.CompletedTask;
    };
});
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

app.UseSession();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "Admin",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
