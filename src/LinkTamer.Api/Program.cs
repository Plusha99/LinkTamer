using LinkTamer.Application.Interfaces;
using LinkTamer.Infrastructure.Extensions;
using LinkTamer.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("CustomPolicy", policy =>
        policy.WithOrigins()
               .AllowAnyMethod()
               .AllowAnyHeader());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Подключаем Redis
builder.Services.AddInfrastructure(builder.Configuration.GetConnectionString("Redis"));
builder.Services.AddScoped<IUrlShortenerService, UrlShortenerService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CustomPolicy");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
