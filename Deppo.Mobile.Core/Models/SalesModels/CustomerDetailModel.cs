using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Core.Models.SalesModels
{
    public partial class CustomerDetailModel : ObservableObject
    {
        [ObservableProperty]
        private Customer customer = null!;

        [ObservableProperty]
        private double inputQuantity;

        [ObservableProperty]
        private double outputQuantity;

        public CustomerDetailModel()
        {
        }

        public ObservableCollection<CustomerTransaction> LastTransactions { get; } = new();
        public ObservableCollection<SalesFiche> Transactions { get; } = new();
    }
}