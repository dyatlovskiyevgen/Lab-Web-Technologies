using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using OSS.Blazor.Components;
using OSS.Blazor.Services;
using OSS30333.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services
    .AddHttpClient<IProductService<Product>, ApiProductService>(c =>
        c.BaseAddress = new Uri("https://localhost:7002/api/ProductsAPI"));

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
