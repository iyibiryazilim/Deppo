using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.OutsourceModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceMenu.ViewModels
{
    [QueryProperty(name: nameof(OutsourceDetailModel), queryId: nameof(OutsourceDetailModel))]
    public partial class OutsourceInputTransactionViewModel : BaseViewModel
    {
        private readonly IHttpClientService _httpClientService;
        private readonly IOutsourceDetailInputProductService _outsourceDetailInputProductService;
        private readonly IUserDialogs _userDialogs;

        [ObservableProperty]
        private OutsourceDetailModel outsourceDetailModel = null!;

        public ObservableCollection<ProductModel> Items { get; } = new();

        [ObservableProperty]
        public SearchBar searchText;

        public OutsourceInputTransactionViewModel(IHttpClientService httpClientService, IOutsourceDetailInputProductService outsourceDetailInputProductService, IUserDialogs userDialogs)
        {
            _httpClientService = httpClientService;
            _userDialogs = userDialogs;
            _outsourceDetailInputProductService = outsourceDetailInputProductService;

            Title = "Fason Giriş Hareketleri";

            LoadItemsCommand = new Command(async () => await LoadItemsAsync());
            LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
            GoToBackCommand = new Command(async () => await GoToBackAsync());
            PerformSearchCommand = new Command(async () => await PerformSearchAsync());
            PerformEmptySearchCommand = new Command(async () => await PerformEmptySearchAsync());
        }

        public Command LoadItemsCommand { get; }
        public Command LoadMoreItemsCommand { get; }
        public Command PerformSearchCommand { get; }
        public Command PerformEmptySearchCommand { get; }
        public Command GoToBackCommand { get; }

        private async Task LoadItemsAsync()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;

                _userDialogs.Loading("Load Items");
                Items.Clear();
                await Task.Delay(1000);

                var httpClient = _httpClientService.GetOrCreateHttpClient();

                var result = await _outsourceDetailInputProductService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, OutsourceDetailModel.Outsource.ReferenceId, SearchText.Text, 0, 20);

                if (result.IsSuccess)
                {
                    if (result.Data is null)
                        return;

                    foreach (var item in result.Data)
                    {
                        Items.Add(Mapping.Mapper.Map<ProductModel>(item));
                    }
                }

                if (_userDialogs.IsHudShowing)
                    _userDialogs.HideHud();
            }
            catch (Exception ex)
            {
                if (_userDialogs.IsHudShowing)
                    _userDialogs.Loading().Hide();

                _userDialogs.Alert(message: ex.Message, title: "Hata", "Tamam");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LoadMoreItemsAsync()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;

                _userDialogs.Loading("Load More Items");

                var httpClient = _httpClientService.GetOrCreateHttpClient();

                var result = await _outsourceDetailInputProductService.GetObjects(
                    httpClient: httpClient,
                    firmNumber: _httpClientService.FirmNumber,
                    periodNumber: _httpClientService.PeriodNumber,
                    outsourceReferenceId: OutsourceDetailModel.Outsource.ReferenceId,
                    search: SearchText.Text,
                    skip: Items.Count,
                    take: 20
                    );

                if (result.IsSuccess)
                {
                    if (result.Data is null)
                        return;

                    foreach (var item in result.Data)
                    {
                        Items.Add(Mapping.Mapper.Map<ProductModel>(item));
                    }
                }

                if (_userDialogs.IsHudShowing)
                    _userDialogs.HideHud();
            }
            catch (Exception ex)
            {
                if (_userDialogs.IsHudShowing)
                    _userDialogs.Loading().Hide();

                _userDialogs.Alert(message: ex.Message, title: "Hata", "Tamam");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task PerformSearchAsync()
        {
            if (IsBusy)
                return;

            try
            {
                if (string.IsNullOrWhiteSpace(SearchText.Text))
                {
                    await LoadItemsAsync();
                    SearchText.Unfocus();
                    return;
                }
                IsBusy = true;

                var httpClient = _httpClientService.GetOrCreateHttpClient();

                var result = await _outsourceDetailInputProductService.GetObjects(
                    httpClient: httpClient,
                    firmNumber: _httpClientService.FirmNumber,
                    periodNumber: _httpClientService.PeriodNumber,
                    outsourceReferenceId: OutsourceDetailModel.Outsource.ReferenceId,
                    search: SearchText.Text,
                    skip: 0,
                    take: 20
                );

                if (result.IsSuccess)
                {
                    if (result.Data is null)
                        return;

                    Items.Clear();
                    foreach (var item in result.Data)
                    {
                        Items.Add(Mapping.Mapper.Map<ProductModel>(item));
                    }
                }
            }
            catch (System.Exception ex)
            {
                _userDialogs.Alert(ex.Message, "Hata");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task PerformEmptySearchAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchText.Text))
            {
                await PerformSearchAsync();
            }
        }

        private async Task GoToBackAsync()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;

                await Task.Delay(300);
                await Shell.Current.GoToAsync("..");
                //SearchText.Text = string.Empty;
            }
            catch (Exception ex)
            {
                if (_userDialogs.IsHudShowing)
                    _userDialogs.Loading().Hide();

                _userDialogs.Alert(message: ex.Message, title: "Hata");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}