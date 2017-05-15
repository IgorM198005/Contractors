using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.EntityFrameworkCore;

namespace Contractors
{
    internal sealed class ContractorsRepository : IRepository<Contractor>
    {
        public async Task Add(Contractor item)
        {
            using (var dataContext = new ContractorsDataContext())
            {
                dataContext.Contractors.Add(item);

                await dataContext.SaveChangesAsync();
            }
        }

        public async Task Delete(Contractor item)
        {
            using (var dataContext = new ContractorsDataContext())
            {
                dataContext.Entry(item).State = EntityState.Deleted;

                await dataContext.SaveChangesAsync();
            }
        }

        public async Task Expand(Contractor item)
        {
            byte[] photoRaw;

            using (var dataContext = new ContractorsDataContext())
            {
                photoRaw = await
                    (from x in dataContext.Contractors.AsNoTracking()
                     where x.Id == item.Id
                     select x.PhotoRaw).FirstAsync(); 
            }

            item.PhotoRaw = photoRaw;
        }

        public async Task<IEnumerable<Contractor>> GetItems()
        {
            using (var dataContext = new ContractorsDataContext())
            {
                return await (from x in dataContext.Contractors.AsNoTracking()
                        select new Contractor
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Description = x.Description,
                            Email = x.Email,
                            Phone = x.Phone,
                            Web = x.Web,
                            Address = x.Address,
                            HasLazyPhoto = x.HasLazyPhoto
                        }).ToListAsync(); 
            }
        }

        public async Task Update(Contractor item)
        {
            using (var dataContext = new ContractorsDataContext())
            {
                dataContext.Entry(item).State = EntityState.Modified;

                await dataContext.SaveChangesAsync();
            }
        }

        private ContractorsRepository()
        {

        }

        private static ContractorsRepository mCurrent;

        private static object mInstanceLock = new object();

        public static IRepository<Contractor> Current
        {
            get
            {
                if (mCurrent != null) return mCurrent;

                Monitor.Enter(mInstanceLock);

                if (mCurrent != null)
                {
                    Monitor.Exit(mInstanceLock);

                    return mCurrent;
                }

                Interlocked.Exchange(ref mCurrent, new ContractorsRepository());

                Monitor.Exit(mInstanceLock);

                return mCurrent;
            }
        }

    }
}
