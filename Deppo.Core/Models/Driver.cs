using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Deppo.Core.Models
{
    public class Driver : INotifyPropertyChanged,IDisposable
    {
        private int _referenceId;
        private string _name = string.Empty;
        private string _surname = string.Empty;
        private string _tckn = string.Empty;
        private string _plateNumber = string.Empty;

        [Key]
        public int ReferenceId
        {
            get => _referenceId;
            set
            {
                if (_referenceId == value) return;
                _referenceId = value;
                NotifyPropertyChanged();
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                if (_name == value) return;
                _name = value;
                NotifyPropertyChanged();
            }
        }

        public string Surname
        {
            get => _surname;
            set
            {
                if (_surname == value) return;
                _surname = value;
                NotifyPropertyChanged();
            }
        }

        public string IdentityNumber
        {
            get => _tckn;
            set
            {
                if (_tckn == value) return;
                _tckn = value;
                NotifyPropertyChanged();
            }
        }
        public string PlateNumber
        {
            get => _plateNumber;
            set
            {
                if (_plateNumber == value) return;
                _plateNumber = value;
                NotifyPropertyChanged();
            }
        }

        public string FullName => $"{Name} {Surname}";

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                PropertyChanged = null;
            }
        }

    }
}
