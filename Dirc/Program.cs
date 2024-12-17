using Dirc.Infrastructure; // Пространство имен вашего репозитория
using Dirc.Domain.Entities; // Пространство имен ваших сущностей
using Dirc.Domain.Interfaces; // Пространство имен вашего интерфейса
using Microsoft.Extensions.Configuration;


var builder = WebApplication.CreateBuilder(args);

// Добавление конфигурации из файла appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Добавление контроллеров и Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Регистрация зависимостей для репозиториев
builder.Services.AddScoped<IRepository<Team>, TeamRepository>();
builder.Services.AddScoped<IRepository<User>, UserRepository>();

// Регистрация конфигурации как Singleton
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

var app = builder.Build();

// Настройка Swagger и среды разработки
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Включение HTTPS перенаправления
app.UseHttpsRedirection();

// Настройка маршрутов контроллеров
app.MapControllers();

// Запуск приложения
app.Run();
