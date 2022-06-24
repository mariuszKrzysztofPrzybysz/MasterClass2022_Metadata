using Metadata.Services;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IMetadatasService, MetadatasService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseRateLimiter(new RateLimiterOptions
{
    DefaultRejectionStatusCode = StatusCodes.Status429TooManyRequests,
    Limiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        return RateLimitPartition.CreateTokenBucketLimiter<string>("TokenBased",
            _ => new TokenBucketRateLimiterOptions(
                tokenLimit: 5,
                queueProcessingOrder: QueueProcessingOrder.OldestFirst,
                queueLimit: 0,
                replenishmentPeriod: TimeSpan.FromSeconds(10),
                tokensPerPeriod: 5,
                autoReplenishment: true));
    }),
    OnRejected = (context, _) =>
    {
        var retryAfterMilliseconds = TimeSpan.FromSeconds(10);
        context.Response.Headers.RetryAfter = retryAfterMilliseconds.ToString();

        return Task.CompletedTask;
    }
});

app.MapControllers();

app.Run();
