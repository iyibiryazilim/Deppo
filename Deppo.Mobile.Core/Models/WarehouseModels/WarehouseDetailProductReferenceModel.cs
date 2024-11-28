using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Core.Models.WarehouseModels
{
    public partial class WarehouseDetailProductReferenceModel : ObservableObject
    {
        private string argument = string.Empty;
        private int argumentDay = 0;
        private double inputQuantity = 0;
        private double outputQuantity = 0;

        public WarehouseDetailProductReferenceModel()
        {
        }

        public string Argument
        {
            get => argument;
            set => SetProperty(ref argument, value);
        }

        public int ArgumentDay
        {
            get => argumentDay;
            set => SetProperty(ref argumentDay, value);
        }

        public double InputQuantity
        {
            get => inputQuantity;
            set => SetProperty(ref inputQuantity, value);
        }

        public double OutputQuantity
        {
            get => outputQuantity;
            set => SetProperty(ref outputQuantity, value);
        }
    }
}