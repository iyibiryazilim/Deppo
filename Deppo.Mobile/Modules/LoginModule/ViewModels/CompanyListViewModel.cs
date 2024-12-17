using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.LoginModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.LoginModule.Views;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.LoginModule.ViewModels;

public partial class CompanyListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly ICustomQueryService _customQueryService;
    private readonly IUserDialogs _userDialogs;
    public CompanyListViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, ICustomQueryService customQueryService)
    {
        _httpClientService = httpClientService;
        _userDialogs = userDialogs;
        _customQueryService = customQueryService;

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        SaveCommand = new Command(async () => await SaveAsync());
        SelectItemCommand = new Command<CompanyModel>(SelectItemAsync);
        CloseCommand = new Command(async () => await CloseAsync());

    }

    #region Commands
    public Command LoadItemsCommand { get; }
    public Command SaveCommand { get; }
    public Command SelectItemCommand { get; }
    public Command CloseCommand { get; }
    #endregion

    #region Collections
    public ObservableCollection<CompanyModel> Items { get; } = new();
    #endregion

    #region Properties
    [ObservableProperty]
    CompanyModel? selectedCompany;
    #endregion

    async Task LoadItemsAsync()
    {
        try
        {
            IsBusy = true;

            Items.Clear();

			_userDialogs.Loading("Loading Items...");
			await Task.Delay(1000);
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var externalDB = await SecureStorage.GetAsync("ExternalDB");

			var query = @$"SELECT
                [ReferenceId] = LOGICALREF,
                [Number] = NR,
				[Name] = NAME,
                [PeriodNumber] = PERNR   
             FROM {externalDB}L_CAPIFIRM
            ";

			var result = await _customQueryService.GetObjectsAsync(httpClient, query);
            if(result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                    Items.Add(Mapping.Mapper.Map<CompanyModel>(item));

				_userDialogs.Loading().Hide();
			}
			else
			{
				if (_userDialogs.IsHudShowing)
					_userDialogs.Loading().Hide();

				_userDialogs.Alert(message: result.Message, title: "Load Items");
			}
		}
        catch (Exception ex)
        {
			if (_userDialogs.IsHudShowing)
				_userDialogs.Loading().Hide();

			_userDialogs.Alert(message: ex.Message, title: "Load Items Error");
		}
        finally
        {
            IsBusy = false;
        }
    }

    private void SelectItemAsync(CompanyModel item)
    {
        try
        {
            IsBusy = true;

            if(item == SelectedCompany)
            {
                SelectedCompany.IsSelected = false;
                SelectedCompany = null;
            }
            else
            {
                if(SelectedCompany is not null)
                {
                    SelectedCompany.IsSelected = false;
                }
                SelectedCompany = item;
                SelectedCompany.IsSelected = true;
            }
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

    async Task SaveAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

			_userDialogs.Loading("Loading...");

			if (SelectedCompany is not null)
            {
                _httpClientService.FirmNumber = SelectedCompany.Number;
                _httpClientService.PeriodNumber = SelectedCompany.PeriodNumber;
                
                //await SecureStorage.SetAsync("CompanyNumber", SelectedCompany.Number.ToString());
				//await SecureStorage.SetAsync("CompanyPeriod", SelectedCompany.PeriodNumber.ToString());
			}

            await Task.Delay(1000);
            Application.Current.MainPage = new AppShell();

			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();
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

    async Task CloseAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            var alertResult = await _userDialogs.ConfirmAsync("Çıkış yapmak istediğinize emin misiniz?", "Uyarı", "Evet", "Hayır");
            if (!alertResult)
				return;

			_userDialogs.Loading("Loading...");
            await Task.Delay(1000);
            var viewModel = IPlatformApplication.Current.Services.GetRequiredService<LoginViewModel>();
            Application.Current.MainPage = new LoginView(viewModel);
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

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
