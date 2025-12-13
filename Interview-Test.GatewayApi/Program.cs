using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole(); // or Serilog/NLog

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("configurationOcelot.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddOcelot(builder.Configuration);

var allowedOrigins = builder.Configuration
    .GetSection("AllowedOrigins")
    .Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.AllowAnyOrigin()
        //policy.WithOrigins(allowedOrigins!) 
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

app.MapGet("/health", () => Results.Ok("OK"));

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();
app.UseCors("AllowFrontend");
app.UseHttpsRedirection();

await app.UseOcelot();
app.Run();