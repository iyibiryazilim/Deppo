using Deppo.Mobile.Core.Models.LocationModels;

namespace Deppo.Mobile.Core.Models.ProductModels
{
    public class ProductActionLocationTransactionModel : LocationTransactionModel
    {
        private int _warehouseReferenceId;
        private int _warehouseNumber;
        private string _warehouseName = string.Empty;

        public ProductActionLocationTransactionModel()
        {
        }

        public int WarehouseReferenceId
        {
            get => _warehouseReferenceId;
            set
            {
                if (_warehouseReferenceId == value) return;
                _warehouseReferenceId = value;
                NotifyPropertyChanged();
            }
        }

        public int WarehouseNumber
        {
            get => _warehouseNumber;
            set
            {
                if (_warehouseNumber == value) return;
                _warehouseNumber = value;
                NotifyPropertyChanged();
            }
        }

        public string WarehouseName
        {
            get => _warehouseName;
            set
            {
                if (_warehouseName == value) return;
                _warehouseName = value;
                NotifyPropertyChanged();
            }
        }
    }
}
