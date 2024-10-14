using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Core.Models.OutsourceModels
{
    public partial class OutsourceDetailModel : ObservableObject
    {
        [ObservableProperty]
        private Outsource outsource = null!;

        [ObservableProperty]
        private double inputQuantity;

        [ObservableProperty]
        private double outputQuantity;

        public OutsourceDetailModel()
        {
        }

        public ObservableCollection<OutsourceTransaction> LastTransactions { get; } = new();
        public ObservableCollection<OutsourceFiche> LastFiches { get; } = new();

        public ObservableCollection<OutsourceDetailInputOutputModel> OutsourceDetailInputOutputModels { get; } = new();
    }
}