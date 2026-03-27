using BloomFilterDemo.Models;
using BloomFilterDemo.Services;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

// Config
builder.Services.Configure<BloomFilterOptions>(
    builder.Configuration.GetSection("BloomFilter"));

builder.Services.Configure<BloomMetricsPersistOptions>(
    builder.Configuration.GetSection("BloomMetricsPersist"));


//Connection string for metrics persistence (if using a real database)
//builder.Services.AddDbContext<MetricsDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("MetricsDb")));

// Add services to the container.
builder.Services.AddMemoryCache();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Bloom Filter Demo API",
        Version = "v1"
    });
});


// App services
builder.Services.AddSingleton<IBloomFilterStore, InMemoryBloomFilterStore>();
builder.Services.AddSingleton<IAuthoritativeStore, FakeAuthoritativeStore>();
builder.Services.AddSingleton<BloomMetrics>();
builder.Services.AddSingleton<BloomService>();

builder.Services.AddSingleton<IDbConnectionFactory, SqlConnectionFactory>();
builder.Services.AddScoped<IBloomMetricsHistoryRepository, BloomMetricsHistoryRepository>();

// Background services
builder.Services.AddHostedService<MetricsPrinterService>();
builder.Services.AddHostedService<BloomMetricsBackgroundPersistService>();


var app = builder.Build();




// Seed some data
using (var scope = app.Services.CreateScope())
{
    var store = (FakeAuthoritativeStore)scope.ServiceProvider.GetRequiredService<IAuthoritativeStore>();
    var bloom = scope.ServiceProvider.GetRequiredService<IBloomFilterStore>();

    string[] seedItems =
    {
        "user:1001",
        "user:1002",
        "user:1003",
        "order:A001",
        "order:A002",
        "email:test@example.com"
    };

    await store.SeedAsync(seedItems);

    foreach (var item in seedItems)
    {
        await bloom.AddAsync(item);
    }
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.Run();
