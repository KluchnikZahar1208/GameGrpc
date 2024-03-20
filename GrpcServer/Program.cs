using GrpcServer.DataContext;
using GrpcServer.Services;
using GrpcServer.Services.IServices;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc().AddJsonTranscoding();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMatchService, MatchService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddDbContext<DataContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
var app = builder.Build();

app.MapGrpcService<GameService>();

app.Run();
