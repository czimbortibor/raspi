using RaspiInterface.Server.Hubs;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddSignalR()
                .AddAzureSignalR(builder.Configuration.GetConnectionString("SignalR"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();


app.MapRazorPages();
app.MapControllers();
app.MapHub<Interface>("/interface");
app.MapFallbackToFile("index.html");

app.Run();
