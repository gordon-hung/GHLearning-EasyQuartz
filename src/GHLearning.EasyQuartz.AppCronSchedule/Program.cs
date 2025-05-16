using GHLearning.EasyQuartz.AppCronSchedule.JobHandlers;
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
	var jobKey = new JobKey(nameof(PerSecondCronJobHandler));
	q.AddJob<PerSecondCronJobHandler>(jobKey);
	//建立 trigger(規則) 來觸發 job
	q.AddTrigger(t => t
		.WithIdentity(nameof(PerSecondCronJobHandler))
		.ForJob(jobKey)
		.StartNow()
		.WithCronSchedule("0 */1 * * * ?", x => x.InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Asia/Shanghai")))
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
