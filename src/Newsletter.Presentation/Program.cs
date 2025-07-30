using Newsletter.Application.Interfaces;
using Newsletter.Application.Services;
using Newsletter.Infrastructure.Context;

var builder = WebApplication.CreateBuilder(args);




builder.Services.AddScoped<IDatabaseContext, DatabaseContext>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddScoped<INewsletterService,NewsletterService>();

builder.Services.AddOpenApi();

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


app.Run();

