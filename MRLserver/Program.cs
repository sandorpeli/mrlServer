using MRLserver;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, 4243, listenOptions =>
    {
        listenOptions.UseHttps("Certificate/httpscertificate.pfx", "LifonOrcaMRL5687");
    });
});

SharedMRLdata sharedData = new SharedMRLdata();
builder.Services.AddSingleton(sharedData);
/*
builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, 4242); // Listen on all IPs
});
*/

// Add services to the container.
builder.Services.AddRazorPages();

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

SslServer mySslServer = new SslServer(sharedData);

app.Run();


