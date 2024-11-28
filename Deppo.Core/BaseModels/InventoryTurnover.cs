using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Deppo.Core.BaseModels
{
    public class InventoryTurnover : INotifyPropertyChanged, IDisposable
    {

        private double _firstStockQuantity;
        private double _lastStockQuantity;
        private double _salesQuantity;
        private double _turnoverRate;

        public InventoryTurnover()
        {
            
        }


        public double FirstStockQuantity
        {
            get => _firstStockQuantity;
            set
            {
                if (_firstStockQuantity == value)
                    return;
                _firstStockQuantity = value;
                NotifyPropertyChanged();
            }
        }

        public double LastStockQuantity
        {
            get => _lastStockQuantity;
            set
            {
                if (_lastStockQuantity == value)
                    return;
                _lastStockQuantity = value;
                NotifyPropertyChanged();
            }
        }

        public double SalesQuantity
        {
            get => _salesQuantity;
            set
            {
                if (_salesQuantity == value)
                    return;
                _salesQuantity = value;
                NotifyPropertyChanged();
            }
        }

        public double TurnoverRate
        {
            get => _turnoverRate;
            set
            {
                if (_turnoverRate == value)
                    return;
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
