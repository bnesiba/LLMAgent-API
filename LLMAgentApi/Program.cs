using DnDBeyondConnector.Repository;
using LLMAgentApi;
using LLMAgentApi.DataStorage.StatblockStorage;
using LLMAgentApi.ToolDefinitions;
using OpenAIConnector.Repository;
using ToolManagementFlow.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

//chat
builder.Services.AddSingleton<ChatGPTRepository>();
builder.Services.AddTransient<ChatOrchestrator>();
builder.Services.AddSingleton<SessionStorage>();
builder.Services.AddSingleton<StatblockStorage>();
builder.Services.AddScoped<ChatReferenceService>();//scoped so that all references can be assumed to be new

//repos
builder.Services.AddSingleton<DnDBeyondMonsterSearchRepo>();

//tools
builder.Services.AddTransient<IToolDefinition, ClassicMonsterSearch>();
builder.Services.AddTransient<IToolDefinition, SubmitStatBlock>();

builder.Services.AddCors(options => options.AddPolicy("Everything", policy =>
{
    policy
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowAnyOrigin();
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Everything");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
