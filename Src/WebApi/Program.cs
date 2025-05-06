using System.Globalization;
using System.Text.Json;
using ByBitApi.Interface;
using ByBitApi.Models;
using ByBitApi.Service;
using Core;
using Infrastructure.Context;
using Infrastructure.Interface;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using TelegramApi.Interface;
using TelegramApi.Service;
using WebApi.Background;
using WebApi.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BitSignalContext>((p, c) =>
{
    c.UseSqlServer(builder.Configuration.GetConnectionString("default"), o => { o.EnableRetryOnFailure(); });
    c.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

builder.Services.AddResponseCompression(options =>
{
    options.Providers.Add<GzipCompressionProvider>();
    options.EnableForHttps = true;
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = Constant.Issuer,

            ValidateAudience = true,
            ValidAudience = Constant.Audience,
            ValidateLifetime = true,

            IssuerSigningKey = new SymmetricSecurityKey(Constant.JwtSecret),
            ValidateIssuerSigningKey = true
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/cost"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Bit Signal Api", Version = "1.0.0" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please insert JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer",
                }
            },
            []
        }
    });
});
builder.Services.AddControllers();
builder.Services.AddSignalR();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddScoped<ICacheRepository, CacheRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IByBitService, ByBitService>();
builder.Services.AddScoped<ITelegramService, TelegramService>();

builder.Services.AddHostedService<PingChecker>();

var app = builder.Build();

app.UseHsts();
app.UseHttpsRedirection();

app.UseResponseCompression();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<CostHub>("/cost");

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    c.DisplayRequestDuration();
    c.DocExpansion(DocExpansion.None);
});


using (var scope = app.Services.CreateScope())
{
    var byBitSvc = scope.ServiceProvider.GetService<IByBitService>();
    var telegramSvc = scope.ServiceProvider.GetService<ITelegramService>();
    var cacheRepo = scope.ServiceProvider.GetService<ICacheRepository>();
    var costHub = scope.ServiceProvider.GetService<IHubContext<CostHub>>();
    var query = ByBitQuery.Create(["tickers.HAEDALUSDT", "tickers.POPCATUSDT"]);
    var minMaxList = new List<ByBitMinMaxSpot>
    {
        new("tickers.HAEDALUSDT", 0.155, 0.175),
        new("tickers.POPCATUSDT", 0.37, 0.396)
    };
    await byBitSvc!.StartAsync(query);
    byBitSvc.ChangeProgress += async cost =>
    {
        try
        {
            Console.WriteLine($"{cost.Topic}:{cost.Data?.LastPrice}");
            if (cost.Topic is null || cost.Data?.LastPrice is null) return;
            _ = costHub!.Clients.All.SendAsync("onchange", JsonSerializer.Serialize(new
            {
                topic = cost.Topic,
                price = cost.Data?.LastPrice
            }));
            var minMax = minMaxList.FirstOrDefault(x => x.Topic == cost.Topic);
            if (minMax is null) return;
            var currentPrice = Convert.ToDouble(cost.Data?.LastPrice, CultureInfo.InvariantCulture);
            if (currentPrice < minMax.PriceMin || currentPrice > minMax.PriceMax)
            {
                if (await cacheRepo!.AnyCoinNotification(cost.Topic)) return;
                _ = telegramSvc!.SendMessage(cost.Topic, cost.Data?.LastPrice);
                _ = cacheRepo.SetCoinNotification(cost.Topic);
            }
        }
        catch
        {
            // ignored
        }
    };
}

app.Run();