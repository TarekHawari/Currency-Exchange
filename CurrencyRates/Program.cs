using DotNetEnv;
using CurrencyRates.Services;

Env.Load(); // loads all environmental variables

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddScoped<GraphApi>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.MapGet("/api/graph/{dropDownCurrency}/{clickedCurrency}", async (string dropDownCurrency, string clickedCurrency, GraphApi graphApi) =>
{
    return await graphApi.GraphCurrencyRateTrends(dropDownCurrency, clickedCurrency);
});

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
