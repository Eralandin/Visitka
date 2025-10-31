using Microsoft.EntityFrameworkCore;
using Visitka.Data;

var builder = WebApplication.CreateBuilder(args);

// ���������� ��������
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ��������� Entity Framework � PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ��������� CORS (��� �������������� � ����������)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();


app.UseCors("AllowAll");
app.UseRouting();
app.UseAuthorization();

app.MapControllers();

// ������ ���������� �� ����� 8218
app.Run("http://0.0.0.0:8218");