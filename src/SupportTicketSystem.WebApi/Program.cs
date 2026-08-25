using SupportTicketSystem.Infrastructure;
using SupportTicketSystem.Application;
using SupportTicketSystem.WebApi.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddApplication();

builder.Services.AddControllers();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails(); // ExceptionHandler için gerekli altyapı
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler(); // Hata yönetim ara yazılımını (middleware) aktif et

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
