using GHLearning.EasyQuartz.AppSentry.JobHandlers;
using Quartz;
using Serilog;
using Serilog.Exceptions;

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
	var jobPerSecondCronKey = new JobKey(nameof(PerMinuteCronJobHandler));
	q.AddJob<PerMinuteCronJobHandler>(jobPerSecondCronKey);
	//建立 trigger(規則) 來觸發 job
	q.AddTrigger(t => t
		.WithIdentity(nameof(PerMinuteCronJobHandler))
		.ForJob(jobPerSecondCronKey)
		.StartNow()
		.WithCronSchedule("0 */1 * * * ?", x => x.InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Asia/Shanghai")))
	);

	//建立 job
	var jobPerSecondCronExceptioKey = new JobKey(nameof(PerMinuteCronExceptionJobHandler));
	q.AddJob<PerMinuteCronExceptionJobHandler>(jobPerSecondCronExceptioKey);
	//建立 trigger(規則) 來觸發 job
	q.AddTrigger(t => t
		.WithIdentity(nameof(PerMinuteCronExceptionJobHandler))
		.ForJob(jobPerSecondCronExceptioKey)
		.StartNow()
		.WithCronSchedule("0 */1 * * * ?", x => x.InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Asia/Shanghai")))
	);
});

builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

// Learn more about configuring  Serilog at https://github.com/serilog/serilog/wiki/Configuration-Basics
Log.Logger = new LoggerConfiguration()
	.ReadFrom.Configuration(builder.Configuration)
	.Enrich.WithExceptionDetails()
	.Enrich.WithProperty("ApplicationName", builder.Environment.ApplicationName)
	.Enrich.WithProperty("EnvironmentName", builder.Environment.EnvironmentName)
	.Enrich.WithProperty("RuntimeId", SequentialGuid.SequentialGuidGenerator.Instance.NewGuid())
	.Enrich.WithProperty("ApplicationStartAt", DateTimeOffset.UtcNow.ToString("u"))
	.Enrich.WithTraceIdentifier()
	.MinimumLevel.Debug()
	.CreateLogger();

builder.Logging.ClearProviders().AddSerilog();

builder.Host.UseSerilog();

// Learn more about configuring  Sentry at https://docs.sentry.io/platforms/dotnet/guides/aspnetcore/
builder.WebHost.UseSentry();

builder.Services.AddQuartzHostedService(options =>
	// 主程式關閉時，會確保當前任務已經完成
	options.WaitForJobsToComplete = true);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseSentryTracing();

app.MapControllers();

app.Run();
