using MessageService;
using MessageService.Config;
using MessageService.Extensions;
using Api.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// add message service as a singleton
builder.Services.AddSingleton<IProducer<MessageData>, Producer<MessageData>>();

var app = builder.Build();

app.ConfigureMessageService<MessageData>(
    new ProducerConnectionSettings("localhost", "user", "password", 5672),
    new ProducerQueueSettings("events-queue", "events-ex", "save-event")
);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseAuthorization();
app.MapControllers();
app.Run();