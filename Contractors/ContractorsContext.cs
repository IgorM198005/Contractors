using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace Contractors
{
    public sealed class ContractorsContext : IContractorsContext
    {
        public ObservableCollection<Contractor> Items { get; private set; }

        public Contractor CreateForEdit()
        {
            Contractor contractor = new Contractor();

            contractor.Photo = this.NoImage;

            return contractor; 
        }

        public async Task Add(Contractor contractor)
        {
            this.SetPhotoStatus(contractor);

            await ContractorsRepository.Current.Add(contractor);

            this.Items.Add(contractor);  
        }

        public async Task Expand(Contractor contractor)
        {
            if (contractor.HasLazyPhoto && (contractor.PhotoRaw == null))
            {
                await ContractorsRepository.Current.Expand(contractor);

                contractor.Photo = await UIFunctions.RawToImage(contractor.PhotoRaw);
            }
        }

        public void Collapse(Contractor contractor)
        {
            contractor.PhotoRaw = null;

            contractor.Photo = null;
        }

        public Contractor CopyForEdit(Contractor contractor)
        {
            var clone = (Contractor)DataTypeFactory.Current.Clone(typeof(Contractor), contractor);

            if (clone.Photo == null) clone.Photo = this.NoImage;

            return clone;
        }

        public async Task Update(Contractor contractorOld, Contractor contractorNew)
        {
            this.SetPhotoStatus(contractorNew);          

            await ContractorsRepository.Current.Update(contractorNew);

            DataTypeFactory.Current.CloneTo(typeof(Contractor), contractorNew, contractorOld);
        }

        private void SetPhotoStatus(Contractor contractor)
        {
            if (contractor.PhotoRaw == null)
            {
                contractor.Photo = null;
            }
            else
            {
                contractor.HasLazyPhoto = true;
            }
        }

        public async Task Delete(Contractor contractor)
        {
            await ContractorsRepository.Current.Delete(contractor);

            this.Items.Remove(contractor);
        }

        public BitmapImage NoImage
        {
            get
            {
                return this.noImage;
            }
        }

        private BitmapImage noImage;

        private static async Task<BitmapImage> GetNoImageSplash()
        {
            using (IRandomAccessStream raStream = await RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/NoImage.png")).OpenReadAsync())
            {
                BitmapImage bitmapImage = new BitmapImage();

                await bitmapImage.SetSourceAsync(raStream);

                return bitmapImage;
            }
        }

        private static ContractorsContext mCurrent;

        private static object mCurrentLock = new object();

        public static IContractorsContext Current
        {
            get
            {
                if (mCurrent == null) throw new ArgumentNullException("Текущий контекст не существует, прежде выполните SetCurrent однократно");

                return mCurrent;
            }
        }

        public static async Task SetCurrentAsync()
        {
            Monitor.Enter(mCurrentLock);

            if (mCurrent == null) await SetCurrentContext(); 

            Monitor.Exit(mCurrentLock);
        }

        private static async Task SetCurrentContext()
        {
            await DataTypeFactory.SetCurrentAsync(new Type[] { typeof(Contractor) });

            mCurrent = new ContractorsContext();

            mCurrent.noImage = await GetNoImageSplash();

            mCurrent.Items = new ObservableCollection<Contractor>();  

            foreach (var item in await ContractorsRepository.Current.GetItems())
                mCurrent.Items.Add(item);            
        }
    }
}
