using Data.Repositories;
using Entities.ShopCards;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Services.BackgroundTasks.BirthdayTask
{
    public class ShopCardCleanerBackgroundTask : BackgroundService
    {
        private readonly IRepository<ShopCard> Repository;
        private readonly IRepository<ShopCardDetail> ShopCardDetailRepo;
        public ShopCardCleanerBackgroundTask(IRepository<ShopCard> repository, IRepository<ShopCardDetail> shopCardDetailRepo)
        {
            Repository = repository;
            ShopCardDetailRepo = shopCardDetailRepo;
        }
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var Shopcards = Repository.AllTableNoTracking.ToList();
                var ShopcardDetails = ShopCardDetailRepo.AllTableNoTracking.ToList();

                if (DateTime.Now.Hour >= 23 && DateTime.Now.Hour <= 6)
                {
                    if (ShopcardDetails.Any())
                    {
                        ShopCardDetailRepo.DeleteRange(ShopcardDetails);
                    }
                    if (Shopcards.Any())
                    {
                        Repository.DeleteRange(Shopcards);
                    }
                }
                await Task.Delay(TimeSpan.FromHours(4));
            }
        }
    }
}
