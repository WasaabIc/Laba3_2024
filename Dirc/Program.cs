using Dirc.Infrastructure; // ������������ ���� ������ �����������
using Dirc.Domain.Entities; // ������������ ���� ����� ���������
using Dirc.Domain.Interfaces; // ������������ ���� ������ ����������
using Microsoft.Extensions.Configuration;


var builder = WebApplication.CreateBuilder(args);

// ���������� ������������ �� ����� appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// ���������� ������������ � Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ����������� ������������ ��� ������������
builder.Services.AddScoped<IRepository<Team>, TeamRepository>();
builder.Services.AddScoped<IRepository<User>, UserRepository>();

// ����������� ������������ ��� Singleton
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

var app = builder.Build();

// ��������� Swagger � ����� ����������
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ��������� HTTPS ���������������
app.UseHttpsRedirection();

// ��������� ��������� ������������
app.MapControllers();

// ������ ����������
app.Run();
