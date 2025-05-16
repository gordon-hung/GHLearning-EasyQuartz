# GHLearning-EasyQuartz
[![GitHub Actions GHLearning-EasyQuartz](https://github.com/gordon-hung/GHLearning-EasyQuartz/actions/workflows/dotnet.yml/badge.svg)](https://github.com/gordon-hung/GHLearning-EasyQuartz/actions/workflows/dotnet.yml) [![Ask DeepWiki](https://deepwiki.com/badge.svg)](https://deepwiki.com/gordon-hung/GHLearning-EasyQuartz)

Quartz.NET 是一個開源的計劃任務排程庫，主要用來在 .NET 應用程式中安排和執行定時任務。它提供了強大且靈活的功能，讓開發者可以設定任務的執行時間、重複頻率、優先順序等等，廣泛應用於需要定時執行任務的場景。

### Quartz.NET 的功能包括：
1. **簡單的排程與觸發器（Triggers）**：可以設定任務的執行時間，像是每天、每週、每月等。
2. **複雜的排程（Cron 表達式）**：支援 Cron 表達式來設定更靈活和精細的排程。
3. **並發處理**：支援多個任務並行執行，並能夠控制執行任務的數量。
4. **持久性（Persistence）**：支持任務狀態的持久化，避免應用程式重啟後丟失任務狀態。
5. **分布式排程**：可以在多個服務之間共享和調度任務，適合於微服務架構中使用。

### 使用契機
1. **定期執行的任務**：例如，每天需要自動備份數據、每週發送報表、每月統計數據等。這些任務可以通過 Quartz.NET 來實現定時調度，減少手動執行的需要。
2. **異步任務的排程**：在有些場景下，可能需要將一些耗時的操作（如數據處理、文件上傳等）異步執行，這時可以使用 Quartz.NET 來排程這些操作。
3. **分布式排程**：如果你的應用程式是分布式架構，Quartz.NET 支援在多個節點間協調任務執行，避免重複執行。
4. **擴展與彈性需求**：需要高度可配置的排程邏輯，像是複雜的 Cron 表達式，或者需要調度不同頻率的任務時，Quartz.NET 提供了足夠的彈性來滿足這些需求。

### 如何使用：
1. **安裝 NuGet 套件**：
   在專案中安裝 Quartz.NET 套件：
   ```
   Install-Package Quartz
   ```

2. **定義 Job 類別**：
   在 Quartz.NET 中，定時執行的任務稱為 `Job`。你可以創建一個類來實現 `IJob` 接口，並定義你需要執行的業務邏輯。
   ```csharp
   public class MyJob : IJob
   {
       public Task Execute(IJobExecutionContext context)
       {
           // 在這裡執行你需要的業務邏輯
           Console.WriteLine("任務執行中...");
           return Task.CompletedTask;
       }
   }
   ```

3. **設置排程與觸發器**：
   接下來，創建一個 `Scheduler` 並設置排程與觸發器：
   ```csharp
   var schedulerFactory = new StdSchedulerFactory();
   var scheduler = await schedulerFactory.GetScheduler();
   
   var job = JobBuilder.Create<MyJob>()
                       .WithIdentity("myJob", "group1")
                       .Build();
   
   var trigger = TriggerBuilder.Create()
                               .WithIdentity("myTrigger", "group1")
                               .StartNow()
                               .WithSimpleSchedule(x => x.WithIntervalInSeconds(60).RepeatForever())
                               .Build();
   
   await scheduler.ScheduleJob(job, trigger);
   await scheduler.Start();
   ```

這樣，`MyJob` 就會每60秒執行一次。

總結來說，Quartz.NET 是一個非常強大的工具，能夠幫助你在 .NET 應用中輕鬆地管理和執行定時任務。如果你的應用程式中需要處理定時執行的工作或排程任務，使用 Quartz.NET 會是很好的選擇。