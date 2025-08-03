
using Microsoft.EntityFrameworkCore;
using ClienteMS.Aplicacao.Contratos;
using ClienteMS.Aplicacao.Servicos;
using ClienteMS.Infraestrutura.Contexto;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SqlContexto>(options => options.UseSqlite(
    builder.Configuration.GetConnectionString("localDb")));
builder.Services.AddScoped<IClienteApp, ClienteApp>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRabbitMqApp(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
