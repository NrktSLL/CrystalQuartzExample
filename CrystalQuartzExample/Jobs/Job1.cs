using System;
using System.Threading;
using System.Threading.Tasks;
using Quartz;

namespace CrystalQuartzExample.Jobs
{
    public class Job1 : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                Console.WriteLine("Hello World");
                return Task.CompletedTask;
            }
            catch (JobExecutionException exception)
            {
                Console.WriteLine($"The Job Execute Error! {exception.Message}");
                return Task.FromCanceled(CancellationToken.None);
            }
            catch (TaskCanceledException exception)
            {
                Console.WriteLine($"The operation has been canceled {exception.Message} ");
                return Task.FromCanceled(CancellationToken.None);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                throw;
            }
        }
    }
}
