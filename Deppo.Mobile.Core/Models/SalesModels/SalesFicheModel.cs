using Deppo.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Core.Models.SalesModels
{
    public class SalesFicheModel : SalesFiche
    {
        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value) return;
                _isSelected = value;
                NotifyPropertyChanged(nameof(IsSelected));
            }
        }
    }
}
