using Deppo.Mobile.Helpers.MVVMHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.WorkOrder.ViewModels
{
    public partial class WorkOrderFormViewModel : BaseViewModel
    {
        public WorkOrderFormViewModel()
        {
            Title = "Form Sayfası";

            LoadPageCommand = new Command(async () => await LoadPageAsync());
            ShowBasketItemCommand = new Command(async () => await ShowBasketItemAsync());
            SaveCommand = new Command(async () => await SaveAsync());
        }

        public Page CurrentPage { get; set; }

        public Command LoadPageCommand { get; }
        public Command BackCommand { get; }
        public Command SaveCommand { get; }
        public Command ShowBasketItemCommand { get; }

        public async Task ShowBasketItemAsync()
        {
        }

        private async Task LoadPageAsync()
        {
        }

        private async Task SaveAsync()
        {
        }


    }
}