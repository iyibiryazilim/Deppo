using Deppo.Core.BaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.Models
{
    public class CustomerTransaction : BaseTransaction
    {
        private string _currentName = string.Empty;

        public string CurrentName
        {
            get => _currentName;
            set
            {
                if (_currentName == value) return;
                _currentName = value;
                NotifyPropertyChanged();
            }
        }

        private string _currrentCode = string.Empty;

        public string CurrentCode
        {
            get => _currrentCode;
            set
            {
                if (_currrentCode == value) return;
                _currrentCode = value;
                NotifyPropertyChanged();
            }
        }
    }
}