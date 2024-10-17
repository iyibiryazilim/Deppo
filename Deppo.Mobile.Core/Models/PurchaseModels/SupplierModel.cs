using Deppo.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Core.Models.PurchaseModels
{
    public class SupplierModel : Supplier
    {
        private bool _isSelected;
      

        public SupplierModel()
        {
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                NotifyPropertyChanged();
            }
        }

    }
}