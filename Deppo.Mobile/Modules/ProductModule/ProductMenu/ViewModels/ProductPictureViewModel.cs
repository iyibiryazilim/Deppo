using System;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.BaseModels;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;

namespace Deppo.Mobile.Modules.ProductModule.ProductMenu.ViewModels;

[QueryProperty(name: nameof(Product), queryId: nameof(Product))]
public partial class ProductPictureViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IProductPictureService _productPictureService;
    private readonly IUserDialogs _userDialogs;

    private readonly ICameraProvider _cameraProvider;

    [ObservableProperty]
    Product _product = null!;

    string imagePath;


    [ObservableProperty]
    byte[] picture = new byte[0];

    [ObservableProperty]
    Image _image = null!;

    [ObservableProperty]
	CameraFlashMode flashMode;

	[ObservableProperty]
	CameraInfo? selectedCamera;

	[ObservableProperty]
	Size selectedResolution;

	[ObservableProperty]
	float currentZoom;

    [ObservableProperty]
	string cameraNameText = "", zoomRangeText = "", currentZoomText = "", flashModeText = "", resolutionText = "";

	public IReadOnlyList<CameraInfo> Cameras => _cameraProvider?.AvailableCameras ?? [];

	public CancellationToken Token => CancellationToken.None;

    public ProductPictureViewModel(
        IProductPictureService productPictureService,
    IHttpClientService httpClientService,
    IUserDialogs userDialogs,
    ICameraProvider cameraProvider)
    {
        _productPictureService = productPictureService;
        _httpClientService = httpClientService;
        _userDialogs = userDialogs;
        _cameraProvider = cameraProvider;

        InsertPictureCommand = new Command(async () => await InsertPictureAsync());
        TakePictureCommand = new Command(async () => await TakePictureAsync());
        LoadCommand = new Command(Load);
        RefreshCamerasCommand = new Command(async () => await RefreshCameras(CancellationToken.None));
        OnImageTappedCommand = new Command(() => { });
        ZoomInCommand = new Command(ZoomIn);
        ZoomOutCommand = new Command(ZoomOut);

        Title = "Product Picture";

        Picture = new byte[0];


    }

    public Page CurrentPage { get; set; }
    public CameraView Camera { get; set; }
    public Command LoadCommand { get; set; }
    public Command InsertPictureCommand { get; set; }
    public Command TakePictureCommand { get; set; }
    public Command OnImageTappedCommand { get; set; }
    public Command RefreshCamerasCommand { get; set; }
    public Command ZoomInCommand { get; set; }
    public Command ZoomOutCommand { get; set; }

    public ICollection<CameraFlashMode> FlashModes { get; } = Enum.GetValues<CameraFlashMode>();


	async Task RefreshCameras(CancellationToken token)
    {
        await _cameraProvider.RefreshAvailableCameras(token);
    }

    private void Load()
    {
        Camera = CurrentPage.FindByName<CameraView>("Camera");
        Image = CurrentPage.FindByName<Image>("image");

        imagePath = Path.Combine(FileSystem.Current.CacheDirectory, "camera-view-image.jpg");


        Camera.MediaCaptured += OnMediaCaptured;
    }

    private async Task InsertPictureAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            _userDialogs.ShowLoading("Loading..");
            await Task.Delay(1000);
            var httpClient = _httpClientService.GetOrCreateHttpClient();

            var result = await _productPictureService.InsertPictureAsync(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                productReferenceId: Product.ReferenceId,
                picture: Picture);

            if (result.IsSuccess)
            {

            }

            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();


        }
        catch (System.Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert($"{ex.Message}", "Error", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    void Cleanup()
    {
        CameraView camera = CurrentPage.FindByName<CameraView>("Camera");

        camera.MediaCaptured -= OnMediaCaptured;
        camera.Handler?.DisconnectHandler();
    }

    private async Task TakePictureAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;


        }
        catch (System.Exception)
        {

            throw;
        }
        finally
        {
            IsBusy = false;
        }
    }

    void OnMediaCaptured(object? sender, MediaCapturedEventArgs e)
    {
        using var localFileStream = File.Create(imagePath);

        e.Media.CopyTo(localFileStream);


        // workaround for https://github.com/dotnet/maui/issues/13858
#if ANDROID
        Image.Source = ImageSource.FromStream(() => File.OpenRead(imagePath));
#else
			Image.Source = ImageSource.FromFile(imagePath);
#endif



    }

    void ZoomIn()
    {
        Camera.ZoomFactor += 1.0f;
    }

    void ZoomOut()
    {
        Camera.ZoomFactor -= 1.0f;
    }

    partial void OnFlashModeChanged(CameraFlashMode value)
	{
		UpdateFlashModeText();
	}

	partial void OnCurrentZoomChanged(float value)
	{
		UpdateCurrentZoomText();
	}

	partial void OnSelectedResolutionChanged(Size value)
	{
		UpdateResolutionText();
	}

	void UpdateFlashModeText()
	{
		if (SelectedCamera is null)
		{
			return;
		}
		FlashModeText = $"{(SelectedCamera.IsFlashSupported ? $"Flash mode: {FlashMode}" : "Flash not supported")}";
	}

	void UpdateCurrentZoomText()
	{
		CurrentZoomText = $"Current Zoom: {CurrentZoom}";
	}

	void UpdateResolutionText()
	{
		ResolutionText = $"Selected Resolution: {SelectedResolution.Width} x {SelectedResolution.Height}";
	}
}
