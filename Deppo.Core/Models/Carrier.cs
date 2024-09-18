using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Deppo.Core.Models
{
    public class Carrier : INotifyPropertyChanged,IDisposable
    {
        private int _referenceId;
        private string _code = string.Empty;
        private string _name = string.Empty;

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

        public string Code
        {
            get => _code;
            set
            {
                if (_code == value) return;
                _code = value;
                NotifyPropertyChanged();
            }
        }

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
