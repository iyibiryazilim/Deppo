using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Deppo.Core.BaseModels
{
    public class InventoryTurnover : INotifyPropertyChanged, IDisposable
    {
        public InventoryTurnover()
        {
            
        }

        private int _month;
        private double _stockQuantity;
        private double _turnoverRate;


        public int Month
        {
            get => _month;
            set
            {
                if (_month == value) return;
                _month = value;
                NotifyPropertyChanged();
            }
        }

        public string MonthName => Month switch
        {
            1 => "Ocak",
            2 => "Şubat",
            3 => "Mart",
            4 => "Nisan",
            5 => "Mayıs",
            6 => "Haziran",
            7 => "Temmuz",
            8 => "Ağustos",
            9 => "Eylül",
            10 => "Ekim",
            11 => "Kasım",
            12 => "Aralık",
            _ => "Default"
        };

        public double StockQuantity
        {
            get => _stockQuantity;
            set
            {
                if (_stockQuantity == value) return;
                _stockQuantity = value;
                NotifyPropertyChanged();
            }
        }

        public double TurnoverRate
        {
            get => _turnoverRate;
            set
            {
                if (_turnoverRate == value) return;
                _turnoverRate = value;
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
