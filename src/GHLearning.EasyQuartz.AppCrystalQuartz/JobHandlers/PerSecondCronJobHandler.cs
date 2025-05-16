using Quartz;

namespace GHLearning.EasyQuartz.AppCrystalQuartz.JobHandlers;

[DisallowConcurrentExecution]
public class PerSecondCronJobHandler(
	ILogger<PerSecondCronJobHandler> logger,
	TimeProvider timeProvider) : IJob
{
	private int _count = 0;

	public Task Execute(IJobExecutionContext context)
	{
		logger.LogInformation("由Quartz排程發送，時間:{dateAt} Count:{count}", timeProvider.GetUtcNow(), _count++);
		return Task.CompletedTask;
	}
}
