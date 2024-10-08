using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Deppo.Core.Models
{
    public class ProductMeasure : INotifyPropertyChanged, IDisposable
    {
        private int _referenceId;
        private double _width;
        private double _height;
        private double _length;
        private double _weight;
        private double _volume;

        public ProductMeasure()
        {

        }

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

        public double Width
        {
            get => _width;
            set
            {
                if (_width == value) return;
                _width = value;
                NotifyPropertyChanged();
            }
        }

        public double Height
        {
            get => _height;
            set
            {
                if (_height == value) return;
                _height = value;
                NotifyPropertyChanged();
            }
        }
        public double Length
        {
            get => _length;
            set
            {
                if (_length == value) return;
                _length = value;
                NotifyPropertyChanged();
            }
        }

        public double Weight
        {
            get => _weight;
            set
            {
                if (_weight == value) return;
                _weight = value;
                NotifyPropertyChanged();
            }
        }
        public double Volume
        {
            get => _volume;
            set
            {
                if (_volume == value) return;
                _volume = value;
                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

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

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
