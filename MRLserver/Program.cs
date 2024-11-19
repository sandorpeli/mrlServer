using MRLserver;
using System.Net;
using System.Net.Sockets;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MRLserver.Data;
using MRLserver.Pages.Shared;
using MRLserver.Pages.menutexts;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    //options.Listen(IPAddress.Parse("172.20.0.119"), 4243, listenOptions =>
    options.Listen(IPAddress.Any, 4243, listenOptions =>
    {
        listenOptions.UseHttps("Certificate/httpscertificate.pfx", "LifonOrcaMRL5687");
    });
    /*
    options.Listen(IPAddress.Any, 4242, listenOptions =>
    {
        listenOptions.UseHttps("Certificate/cacertificate.pfx", "LifonOrcaMRL5687");
    });
    */
});

SharedMRLdata sharedData = new SharedMRLdata();
builder.Services.AddSingleton(sharedData);

/// csv feldolgozás
// singleton version
builder.Services.AddSingleton<csv_text_data>(new csv_text_data());  // Így csak egyszer fut le

// Register CsvTextData as a transient service
//builder.Services.AddTransient<csv_text_data>(provider => new csv_text_data());    // Minden weboldal nyitásánál lefutna
/// EOF csv feldolgozás

/*
builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, 4242); // Listen on all IPs
});
*/

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<MRLservContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MRLservContext") ?? throw new InvalidOperationException("Connection string 'MRLservContext' not found.")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

var runner = app.RunAsync();

var optionsBuilder = new DbContextOptionsBuilder<MRLservContext>();
optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("MRLservContext"));
using var context = new MRLservContext(optionsBuilder.Options);

SslServer mySslServer = new SslServer(sharedData, context);

// Register MyService
//builder.Services.AddScoped<SslServer>();

await runner;

