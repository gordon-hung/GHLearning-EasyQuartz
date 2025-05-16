using GHLearning.EasyQuartz.AppSimpleSchedule.JobHandlers;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSingleton(TimeProvider.System);

// Learn more about configuring OpenAPI at https://www.quartz-scheduler.net/documentation/quartz-3.x/quick-start.html
builder.Services.AddQuartz(q =>
{
	q.UseSimpleTypeLoader();
	q.UseInMemoryStore();
	q.ScheduleJob<PerSecondSimpleJobHandler>(trigger => trigger
		.WithIdentity(nameof(PerSecondSimpleJobHandler))
		.StartNow()
		.WithSimpleSchedule(x => x.WithIntervalInSeconds(1).RepeatForever())
	);
});

builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
