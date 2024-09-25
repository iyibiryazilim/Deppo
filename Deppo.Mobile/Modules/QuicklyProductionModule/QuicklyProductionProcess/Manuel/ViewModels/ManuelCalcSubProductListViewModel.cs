using Deppo.Mobile.Core.Models.QuicklyModels;
using Deppo.Mobile.Helpers.MVVMHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.Manuel.ViewModels
{
    public partial class ManuelCalcSubProductListViewModel : BaseViewModel
    {
        public ManuelCalcSubProductListViewModel()
        {
            BackCommand = new Command(async () => await BackAsync());
            LoadItemsCommand = new Command(async () => await LoadItemsAsync());
            LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
            //ItemTappedCommand = new Command<QuicklyBOMProductModel>(async (parameter) => await ItemTappedAsync(parameter));
            NextViewCommand = new Command(async () => await NextViewAsync());
        }

        public Command LoadItemsCommand { get; }
        public Command LoadMoreItemsCommand { get; }
        public Command ItemTappedCommand { get; }
        public Command ConfirmCommand { get; }
        public Command BackCommand { get; }
        public Command NextViewCommand { get; }

        public async Task LoadItemsAsync()
        {
        }

        public async Task LoadMoreItemsAsync()
        {
        }

        public async Task ItemTappedAsync()
        {
        }

        public async Task ConfirmAsync()
        {
        }

        public async Task BackAsync()
        {
        }

        public async Task NextViewAsync()
        {
        }
    }
}