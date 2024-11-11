var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient(); // Register HttpClient for making requests to external pages

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

var microWebsitePaths = new List<string>
{
    "/en/corolla",
    "/en/crown",
    "/en/nabd",
    "/en/camry",
    "/en/urban-cruiser",
    "/en/almost-toyota",
    "/club/en",
    "/en/dream",

    "/ar/corolla",
    "/ar/crown",
    "/ar/nabd",
    "/ar/camry",
    "/ar/urban-cruiser",
    "/ar/almost-toyota",
    "/club/ar",
    "/ar/dream",

    "/ku/corolla",
    "/ku/crown",
    "/ku/nabd",
    "/ku/camry",
    "/ku/urban-cruiser",
    "/ku/almost-toyota",
    "/club/ku",
    "/ku/dream",
};

foreach (var microWebsitePath in microWebsitePaths)
{
    app.MapGet(microWebsitePath, async (HttpContext context, IHttpClientFactory clientFactory) =>
    {
        var client = clientFactory.CreateClient();
        var externalUrl = $"https://tiq-website.azurewebsites.net{microWebsitePath}";

        // Fetch the content from the external URL
        var response = await client.GetAsync(externalUrl);

        if (!response.IsSuccessStatusCode)
        {
            // If the external URL is not found, return 404
            context.Response.StatusCode = (int)response.StatusCode;
            await context.Response.WriteAsync("Content not found");
            return;
        }

        // Read the response content
        var content = await response.Content.ReadAsStringAsync();

        // Set the content type (this example assumes HTML; adapt if you need to handle other content types)
        context.Response.ContentType = response.Content.Headers.ContentType?.ToString() ?? "text/html";
        await context.Response.WriteAsync(content);
    });
}

app.Run();
