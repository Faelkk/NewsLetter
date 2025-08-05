    using System.Text;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.IdentityModel.Tokens;
    using Newsletter.Application.Interfaces;
    using Newsletter.Application.Services;
    using Newsletter.Application.UseCases;
    using Newsletter.Domain.Interfaces;
    using Newsletter.Infrastructure.Context;
    using Newsletter.Infrastructure.Kafka.Producers;
    using Newsletter.Infrastructure.Repository;
    using Newsletter.Infrastructure.Seed;
    using Newsletter.Infrastructure.Services;
    using Newsletter.Infrastructure.Settings;
    using Newsletter.Infrastructure.Stripe;
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
    builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
    builder.Services.AddScoped<ICreateCheckoutSession,CreateCheckoutSession>();
    builder.Services.AddScoped<IStripeCheckoutService,StripeCheckoutService>();
    builder.Services.AddScoped<IStripeWebhookHandler,StripeWebhookHandler>();
    builder.Services.AddScoped<IGetSubscriptionStatus,GetSubscriptionStatus>();
    builder.Services.AddScoped<IConfirmSubscriptionStatus, ConfirmSubscriptionStatus>();
    builder.Services.AddScoped<ISubscriptionStatusProducer,SubscriptionStatusProducer>();
    builder.Services.AddScoped<IJwtService, JwtService>();

    builder.Services.AddScoped<DatabaseSeed>();
    builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
    builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));


    var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();
    builder.Services.AddSingleton(jwtSettings);

    builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
            };
        });


    builder.Services.AddAuthorization(options => {        
        options.AddPolicy("RequireAdminRole", policy =>
        policy.RequireRole("Admin"));
    });

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

    app.UseAuthentication();
    app.UseAuthorization();

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


    
    
