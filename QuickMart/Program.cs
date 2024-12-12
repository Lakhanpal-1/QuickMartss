using QuickMart.Extension;
using QuickMart.Services.Services;
using QuickMart.Services.Services.IServices;

var builder = WebApplication.CreateBuilder(args);

// ===================== Register services =====================
builder.Services.AddQuickMartServices(builder.Configuration);

// ===================== Add Controllers and Swagger =====================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ===================== Configure the HTTP request pipeline =====================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

// Ensure Authentication middleware is before Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
