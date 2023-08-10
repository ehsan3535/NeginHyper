using Data.Repositories;
using Entities;
using Entities.FreeTime;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Services.BackgroundTasks.BirthdayTask
{
    public class PassedSessionCheckerBackGroundTask : BackgroundService
    {
        private readonly IRepository<FreeTime> Repository;
        public PassedSessionCheckerBackGroundTask(IRepository<FreeTime> repository)
        {
            Repository = repository;
        }
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var FreeTimes = Repository.AllTableNoTracking.Where(x => x.Out == false).ToList();
                if (FreeTimes.Any())
                {
                    foreach (var item in FreeTimes)
                    {
                        if (item.DateTime.DayOfWeek == DateTime.Now.DayOfWeek)
                        {
                            if (item.DateTime.Date <= DateTime.Now.Date)
                            {
                                if (DateTime.Now.Hour >= 21)
                                {
                                    item.DateTime = DateTime.Now.AddDays(7);
                                    Repository.Update(item);
                                }
                            }
                        }
                    }
                }
                await Task.Delay(TimeSpan.FromMinutes(45));

            }
        }
    }
}
