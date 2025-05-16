using GHLearning.EasyQuartz.AppDisallowConcurrentExecution.JobHandlers;
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

	//建立 job
	var jobKey1 = new JobKey(nameof(PerSecondCronJobHandler));
	q.AddJob<PerSecondCronJobHandler>(jobKey1);
	//建立 trigger(規則) 來觸發 job
	q.AddTrigger(t => t
		.WithIdentity(nameof(PerSecondCronJobHandler))
		.ForJob(jobKey1)
		.StartNow()
		.WithCronSchedule("* * * * * ?", x => x.InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Asia/Shanghai")))
	);

	//建立 job
	var jobKey2 = new JobKey(nameof(PerSecondCronV2JobHandler));
	q.AddJob<PerSecondCronV2JobHandler>(jobKey2);
	//建立 trigger(規則) 來觸發 job
	q.AddTrigger(t => t
		.WithIdentity(nameof(PerSecondCronV2JobHandler))
		.ForJob(jobKey2)
		.StartNow()
		.WithCronSchedule("* * * * * ?", x => x.InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Asia/Shanghai")))
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
