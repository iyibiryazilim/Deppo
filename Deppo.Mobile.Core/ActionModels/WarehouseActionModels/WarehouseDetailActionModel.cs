using CommunityToolkit.Mvvm.ComponentModel;
using Kotlin.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Core.ActionModels.WarehouseActionModels
{
    public partial class WarehouseDetailActionModel : ObservableObject
    {
        [ObservableProperty]
        private string actionName = string.Empty;

        [ObservableProperty]
        private string actionUrl = string.Empty;

        [ObservableProperty]
        private int lineNumber = 0;

        [ObservableProperty]
        private string icon = string.Empty;

        [ObservableProperty]
        private bool isSelected = false;

        public WarehouseDetailActionModel()
        {
        }
    }
}