using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace Contractors
{
    public interface IContractorsContext
    {
        ObservableCollection<Contractor> Items { get; }

        Contractor CreateForEdit();

        Task Add(Contractor contractor);

        Task Expand(Contractor contractor);

        void Collapse(Contractor contractor);

        Contractor CopyForEdit(Contractor contractor);

        Task Update(Contractor contractorOld, Contractor contractorNew);

        Task Delete(Contractor contractor);

        BitmapImage NoImage { get; }
    }
}
