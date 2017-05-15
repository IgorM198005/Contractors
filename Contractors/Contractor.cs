using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Media;
using System.ComponentModel.DataAnnotations.Schema;

namespace Contractors
{
    public sealed class Contractor : INotifyPropertyChanged
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ClonableField]
        public int Id { get; set; }

        private string mName;

        [ClonableField]
        [Required(ErrorMessage = "не заполнено")]
        [ExpandedValidationMessage("Заполните поле 'Наименование'", typeof(RequiredAttribute))]
        [MaxLength(128)]
        public string Name
        {
            get
            {
                return this.mName;
            }
            set
            {
                this.mName = value;

                this.OnPropertyChanged();
            }
        }

        private string mNameWarn;

        [NotMapped]
        [NotificationField(nameof(Name))]
        public string NameWarn
        {
            get
            {
                return this.mNameWarn;
            }
            set
            {
                this.mNameWarn = value;

                this.OnPropertyChanged();

                this.OnPropertyChanged(nameof(HasNameWarn));
            }
        }

        [NotMapped]
        public bool HasNameWarn
        {
            get
            {
                return !string.IsNullOrEmpty(this.NameWarn);
            }
        }

        private string mDescription;

        [ClonableField]
        [Required(ErrorMessage = "не заполнено")]
        [ExpandedValidationMessage("Заполните поле 'Описание'", typeof(RequiredAttribute))]
        [MaxLength(255)]
        public string Description
        {
            get
            {
                return this.mDescription;
            }
            set
            {
                this.mDescription = value;

                this.OnPropertyChanged();
            }
        }

        private string mDescriptionWarn;

        [NotMapped]
        [NotificationField(nameof(Description))]
        public string DescriptionWarn
        {
            get
            {
                return this.mDescriptionWarn;
            }
            set
            {
                this.mDescriptionWarn = value;

                this.OnPropertyChanged();

                this.OnPropertyChanged(nameof(HasDescriptionWarn));
            }
        }

        [NotMapped]
        public bool HasDescriptionWarn
        {
            get
            {
                return !string.IsNullOrEmpty(this.DescriptionWarn);
            }
        }

        private string mEmail;

        [ClonableField]
        [EmailAddress(ErrorMessage = "некорректный email")]
        [ExpandedValidationMessage("Укажите корректный email или оставте поле пустым", typeof(EmailAddressAttribute))]
        [MaxLength(100)]
        public string Email
        {
            get
            {
                return this.mEmail;
            }
            set
            {
                this.mEmail = value;

                this.OnPropertyChanged();

                this.OnPropertyChanged(nameof(HasEmail));

                this.OnPropertyChanged(nameof(HasMvContacts));
            }
        }

        [NotMapped]
        public bool HasEmail
        {
            get
            {
                return !string.IsNullOrEmpty(this.Email);
            }
        }

        private string mEmailWarn;

        [NotMapped]
        [NotificationField(nameof(Email))]
        public string EmailWarn
        {
            get
            {
                return this.mEmailWarn;
            }
            set
            {
                this.mEmailWarn = value;

                this.OnPropertyChanged();

                this.OnPropertyChanged(nameof(HasEmailWarn));
            }
        }

        [NotMapped]
        public bool HasEmailWarn
        {
            get
            {
                return !string.IsNullOrEmpty(this.EmailWarn);
            }
        }

        private string mPhone;

        [ClonableField]
        [MaxLength(30)]
        public string Phone
        {
            get
            {
                return this.mPhone;
            }
            set
            {
                this.mPhone = value;

                this.OnPropertyChanged();

                this.OnPropertyChanged(nameof(HasPhone));

                this.OnPropertyChanged(nameof(HasMvContacts));
            }
        }

        [NotMapped]
        public bool HasPhone
        {
            get
            {
                return !string.IsNullOrEmpty(this.Phone);
            }
        }

        private string mPhoneWarn;

        [NotMapped]
        [NotificationField(nameof(Phone))]
        public string PhoneWarn
        {
            get
            {
                return this.mPhoneWarn;
            }
            set
            {
                this.mPhoneWarn = value;

                this.OnPropertyChanged();

                this.OnPropertyChanged(nameof(HasPhoneWarn));
            }
        }

        [NotMapped]
        public bool HasPhoneWarn
        {
            get
            {
                return !string.IsNullOrEmpty(this.PhoneWarn);
            }
        }

        private string mWeb;

        [ClonableField]
        [MaxLength(100)]
        public string Web
        {
            get
            {
                return this.mWeb;
            }
            set
            {
                this.mWeb = value;

                this.OnPropertyChanged();

                this.OnPropertyChanged(nameof(HasWeb));

                this.OnPropertyChanged(nameof(HasMvContacts));
            }
        }

        [NotMapped]
        public bool HasWeb
        {
            get
            {
                return !string.IsNullOrEmpty(this.Web);
            }
        }

        private string mWebWarn;

        [NotMapped]
        [NotificationField(nameof(Web))]
        public string WebWarn
        {
            get
            {
                return this.mWebWarn;
            }
            set
            {
                this.mWebWarn = value;

                this.OnPropertyChanged();

                this.OnPropertyChanged(nameof(HasWebWarn));
            }
        }

        [NotMapped]
        public bool HasWebWarn
        {
            get
            {
                return !string.IsNullOrEmpty(this.WebWarn);
            }
        }

        private string mAddress;

        [ClonableField]
        [MaxLength(255)]
        public string Address
        {
            get
            {
                return this.mAddress;
            }
            set
            {
                this.mAddress = value;

                this.OnPropertyChanged();

                this.OnPropertyChanged(nameof(HasAddress));
            }
        }

        [NotMapped]
        public bool HasAddress
        {
            get
            {
                return !string.IsNullOrEmpty(this.Address);
            }
        }

        private bool mSelected;

        [NotMapped]
        public bool Selected
        {
            get
            {
                return this.mSelected;
            }
            set
            {
                this.mSelected = value;

                this.OnPropertyChanged();
            }
        }

        private ImageSource mPhoto;

        [NotMapped]
        [ClonableField]
        public ImageSource Photo
        {
            get
            {
                return this.mPhoto;
            }
            set
            {
                this.mPhoto = value;

                this.OnPropertyChanged();

                this.OnPropertyChanged(nameof(HasPhoto));
            }
        }

        [ClonableField]
        public byte[] PhotoRaw { get; set; }

        [NotMapped]
        public bool HasPhoto
        {
            get
            {
                return this.mPhoto != null;
            }
        }

        [ClonableField]
        public bool HasLazyPhoto { get; set; }

        [NotMapped]
        public bool HasMvContacts
        {
            get
            {
                return this.HasEmail || this.HasPhone || this.HasWeb;
            }
        }

        private void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (this.PropertyChanged != null) this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
