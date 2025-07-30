using Newsletter.Application.Interfaces;
using Newsletter.Application.Services;
using Newsletter.Domain.Interfaces;
using Newsletter.Infrastructure.Context;
using Newsletter.Infrastructure.Repository;
using Newsletter.Infrastructure.Seed;
using Newsletter.Presentation.Middlewares;

var builder = WebApplication.CreateBuilder(args);
Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

builder.Services.AddScoped<IDatabaseContext, DatabaseContext>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddScoped<INewsletterService, NewsletterService>();
builder.Services.AddScoped<INewsletterRepository, NewsletterRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
builder.Services.AddScoped<DatabaseSeed>();

builder.Services.AddOpenApi();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(
        "v1",
        new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "Newsletters API",
            Version = "v1",
            Description = "Teste",
            Contact = new Microsoft.OpenApi.Models.OpenApiContact
            {
                Name = "Rafael Achtenberg",
                Email = "achtenberg.rafa@gmail.com",
                Url = new Uri("https://github.com/Faelkk"),
            },
        }
    );
});

var port = builder.Configuration["APIPORT"] ?? "5010";
builder.WebHost.UseUrls($"http://*:{port}");

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeed>();
    seeder.Initialize();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Newsletters API v1");
});


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();
