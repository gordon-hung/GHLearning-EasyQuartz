using CrystalQuartz.AspNetCore;
using GHLearning.EasyQuartz.AppCrystalQuartz.JobHandlers;
using Microsoft.Extensions.Options;
using Quartz;
using Quartz.Impl;
using Quartz.Simpl;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton(TimeProvider.System);

// Learn more about configuring OpenAPI at https://www.quartz-scheduler.net/documentation/quartz-3.x/quick-start.html
builder.Services.AddQuartz(q =>
{
	q.UseSimpleTypeLoader();
	q.UseInMemoryStore();
	q.SchedulerName = builder.Environment.ApplicationName;
});

builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.UseCrystalQuartz(() =>
{
	var schedulerFactory = new StdSchedulerFactory();
	var scheduler = schedulerFactory.GetScheduler().Result;

	// Pass the required 'options' parameter to the constructor
	scheduler.JobFactory = new MicrosoftDependencyInjectionJobFactory(app.Services, app.Services.GetRequiredService<IOptions<QuartzOptions>>());

	var job = JobBuilder.Create<PerSecondCronJobHandler>()
	.WithIdentity(nameof(PerSecondCronJobHandler), "default")
	.Build();

	var trigger = TriggerBuilder.Create()
	.WithIdentity($"{nameof(PerSecondCronJobHandler)} - trigger", "default")
	.ForJob(job)
	.StartNow()
	.WithCronSchedule("*/1 * * * * ?")
	.Build();

	scheduler.ScheduleJob(job, trigger);

	scheduler.Start();

	return scheduler;
});

app.MapStaticAssets();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}")
	.WithStaticAssets();

app.MapGet("/", context =>
{
	context.Response.Redirect("/quartz");
	return Task.CompletedTask;
});

app.Run();
