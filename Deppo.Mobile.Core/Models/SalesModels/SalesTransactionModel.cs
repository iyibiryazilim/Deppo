using Deppo.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Core.Models.SalesModels
{
    public class SalesTransactionModel : SalesTransaction
    {
        private bool _isSelected;
        private string? _image;

        public SalesTransactionModel()
        {
        }

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

        public string? Image
        {
            get => _image;
            set
            {
                if (_image == value) return;
                _image = value;
                NotifyPropertyChanged();
            }
        }
    }
}