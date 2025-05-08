using Tutorial8.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<ITripsService, TripsService>();
// builder.Services.AddScoped<IClientService, ClientsService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.useSwagger();
    app.useSwaggerUi();
}

app.UseAuthorization();
app.MapControllers();
app.Run();