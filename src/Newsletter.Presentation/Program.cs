    using System.Text;
    using Hangfire;
    using Hangfire.PostgreSql;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.OpenApi.Models;
    using Newsletter.Application.Interfaces;
    using Newsletter.Application.Jobs;
    using Newsletter.Application.Jobs.Interfaces;
    using Newsletter.Application.Services;
    using Newsletter.Application.UseCases;
    using Newsletter.Domain.Interfaces;
    using Newsletter.Infrastructure.Config;
    using Newsletter.Infrastructure.Context;
    using Newsletter.Infrastructure.Email;
    using Newsletter.Infrastructure.Interfaces;
    using Newsletter.Infrastructure.Kafka.Producers;
    using Newsletter.Infrastructure.Repository;
    using Newsletter.Infrastructure.Seed;
    using Newsletter.Infrastructure.Services;
    using Newsletter.Infrastructure.Settings;
    using Newsletter.Infrastructure.Stripe;
    using Newsletter.Presentation.Hangfire;
    using Newsletter.Presentation.Jobs.Wrappers;
    using Newsletter.Presentation.Middlewares;
    using Scalar.AspNetCore;

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
    builder.Services.AddScoped<ICheckExpiredSubscriptionJob,CheckExpiredSubscriptionJob>();
    builder.Services.AddScoped<IStripeSubscriptionService, StripeSubscriptionService>();
    builder.Services.AddScoped<ISendMonthlyEmailsJob,SendMonthlyEmailsJob>();
    builder.Services.AddScoped<IEmailService, EmailService>();
    builder.Services.AddScoped<IGenerateNewsLetters, GenerateNewsLetters>();


    builder.Services.AddScoped<DatabaseSeed>();
    
    builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
    builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
    builder.Services.Configure<GeminiOptions>(builder.Configuration.GetSection("Gemini"));
    builder.Services.AddHttpClient<GeminiService>();
    builder.Services.AddHangfire(config =>
        config.UsePostgreSqlStorage(builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddHangfireServer();
    
    
    var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();
    builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

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
    

    builder.Services.AddOpenApi(options =>
    {
        options.AddDocumentTransformer((document, context, cancellationToken) =>
        {
            document.Info = new OpenApiInfo
            {
                Title = "Newsletter API",
                Version = "v1",
                Description = "API de newsletters, assinaturas e envio de e-mails automáticos.",
                Contact = new OpenApiContact
                {
                    Name = "Rafael Achtenberg",
                    Email = "rafael@example.com",
                    Url = new Uri("https://github.com/Faelkk/NewsLetter")
                },
                License = new OpenApiLicense
                {
                    Name = "MIT",
                    Url = new Uri("https://opensource.org/licenses/MIT")
                }
            };

            return Task.CompletedTask;
        });
    });



    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();


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
    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization = new[] { new HangfireAuthorizationFilter() }
    });


    app.MapControllers();

     RecurringJob.AddOrUpdate<SendMonthlyEmailsJobWrapper>(
         "daily-emails",
         job => job.Execute(),
        Cron.Hourly
     );
    
    RecurringJob.AddOrUpdate<CheckExpiredSubscriptionsJobWrapper>(
        "check-expired-subscriptions",
        job => job.Execute(),
        Cron.Daily
    );




    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Newsletter API")
            .WithTheme(ScalarTheme.Mars);
    });

        
    app.UseHttpsRedirection();

    app.Run();


    
    
