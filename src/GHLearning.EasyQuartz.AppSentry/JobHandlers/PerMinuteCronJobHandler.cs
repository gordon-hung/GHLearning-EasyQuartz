using Quartz;

namespace GHLearning.EasyQuartz.AppSentry.JobHandlers;

public class PerMinuteCronJobHandler(
	ILogger<PerMinuteCronJobHandler> logger,
	TimeProvider timeProvider) : IJob
{
	public async Task Execute(IJobExecutionContext context)
	{
		var checkInId = SentrySdk.CaptureCheckIn("second-cron", CheckInStatus.InProgress);
		try
		{
			logger.LogInformation("由Quartz排程發送，時間:{dateAt}，Nameof:{nameof}", timeProvider.GetUtcNow(), nameof(PerMinuteCronJobHandler));
			await Task.Delay(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
			SentrySdk.CaptureCheckIn("second-cron", CheckInStatus.Ok, checkInId);
		}
		catch
		{
			SentrySdk.CaptureCheckIn("second-cron", CheckInStatus.Error, checkInId);
		}
	}
}
