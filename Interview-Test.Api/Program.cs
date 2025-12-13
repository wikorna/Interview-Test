using Interview_Test.Infrastructure;
using Interview_Test.Middlewares;
using Interview_Test.Repositories;
using Interview_Test.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddMvc(options => options.EnableEndpointRouting = false);

var connection = builder.Configuration.GetConnectionString("DefaultConnection")
                      ?? "<your database connection string>";
builder.Services.AddDbContext<InterviewTestDbContext>(options =>
    {
        options.UseSqlServer(connection,
            sqlOptions =>
            {
                sqlOptions.UseCompatibilityLevel(110);
                sqlOptions.CommandTimeout(30);
                sqlOptions.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), errorNumbersToAdd: null);
            });
    }
);

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddTransient<AuthenMiddleware>();

var app = builder.Build();
app.UseRouting();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseMiddleware<AuthenMiddleware>();

//app.UseMvc();
app.MapControllers();

app.Run();