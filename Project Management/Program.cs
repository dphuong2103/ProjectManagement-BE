using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Project_Management.Database;
using Project_Management.Handlers;

var builder = WebApplication.CreateBuilder(args);
const string FirebaseAuthentication = "FirebaseAuthentication";

var dbHost =Environment.GetEnvironmentVariable("DB_HOST");
var dbName = Environment.GetEnvironmentVariable("DB_NAME");
var dbPassword = Environment.GetEnvironmentVariable("DB_SA_PASSWORD");
var connectionStringFromEnvironmentVariable = $"Data Source={dbHost};Initial Catalog={dbName};User ID=sa;Password={dbPassword}";
//var connectionStringFromEnvironmentVariable = $"Data Source={dbHost};Initial Catalog={dbName}; User ID=sa;Password={dbPassword};ApplicationIntent=ReadWrite;MultiSubnetFailover=False;Integrated Security=True;MultipleActiveResultSets=true;TrustServerCertificate=True";
builder.Host.ConfigureLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
});

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
// Add services to the container.
builder.Services.AddSingleton(FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile("Credentials/project-management-a129e-firebase-adminsdk-tc07n-588af4f467.json"),
}));
builder.Services.AddAuthentication("FirebaseAuthentication")
    .AddScheme<AuthenticationSchemeOptions, FirebaseUserAuthenticationHandler>(FirebaseAuthentication, null);

builder.Services.AddAuthorization(options =>
{
    var defaultauthorizationpolicybuilder = new AuthorizationPolicyBuilder(FirebaseAuthentication);
    defaultauthorizationpolicybuilder =
        defaultauthorizationpolicybuilder.RequireAuthenticatedUser();
    options.DefaultPolicy = defaultauthorizationpolicybuilder.Build();
});

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      builder =>
                      {
                          builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin();
                      });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
string ConnectionString = builder.Configuration.GetValue<string>("ConnectionStrings:SQLConnectionString");
builder.Services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(connectionStringFromEnvironmentVariable));
var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{

//}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors(MyAllowSpecificOrigins);
app.UseAuthorization();
//using (var scope = app.Services.CreateScope())
//{
//    var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
//    try
//    {
//        dbContext.Database.Migrate();
//        Console.WriteLine("Database connection test successful.");
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine("An error occurred while testing the database connection: " + ex.Message);
//    }
//}
app.MapControllers();
app.Run();
