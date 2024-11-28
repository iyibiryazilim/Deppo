using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Core.Models.OutsourceModels
{
    public class OutsourceDetailInputOutputModel : ObservableObject
    {
        private string argument = string.Empty;
        private int argumentDay = 0;
        private double salesReferenceQuantity = 0;
        private double returnReferenceQuantity = 0;

        public OutsourceDetailInputOutputModel()
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

        public double SalesReferenceQuantity
        {
            get => salesReferenceQuantity;
            set => SetProperty(ref salesReferenceQuantity, value);
        }

        public double ReturnReferenceQuantity
        {
            get => returnReferenceQuantity;
            set => SetProperty(ref returnReferenceQuantity, value);
        }
    }
}