using ImagePassAPI.Repositories.Implementations;
using ImagePassAPI.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "LocalOriginPolicy",
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:5046", 
                                             "https://localhost:5046", 
                                             "https://localhost:5000",
                                             "https://localhost:7148")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IImageRepository, ImageRepository>();
builder.Services.AddScoped<ISentenseRepository, SentenseRepository>();
builder.Services.AddScoped<IPasswordRepository, PasswordRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("LocalOriginPolicy");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
