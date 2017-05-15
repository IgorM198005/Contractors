using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contractors
{
    public interface IRepository<T>
    {
        Task Add(T item);

        Task Update(T item);

        Task Delete(T item);

        Task Expand(T item);

        Task<IEnumerable<T>> GetItems(); 
    }
}
