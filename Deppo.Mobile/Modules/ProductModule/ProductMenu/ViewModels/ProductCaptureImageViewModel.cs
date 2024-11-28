using System;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.BaseModels;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ProductModule.ProductMenu.Views;

namespace Deppo.Mobile.Modules.ProductModule.ProductMenu.ViewModels;


[QueryProperty(name: nameof(Product), queryId: nameof(Product))]
public partial class ProductCaptureImageViewModel : BaseViewModel
{

    private readonly IUserDialogs _userDialogs;

    [ObservableProperty]
    Product product = null!;

    CommunityToolkit.Maui.Views.CameraView cameraView = null!;

    [ObservableProperty]
    CameraFlashMode cameraFlashMode = CameraFlashMode.Off;


    [ObservableProperty]
    CameraInfo selectedCamera;

    [ObservableProperty]
    float currentZoom;

    public ProductCaptureImageViewModel(IUserDialogs userDialogs)
    {
        _userDialogs = userDialogs;
        Title = "Ürün Resmi";

        LoadCommand = new Command(LoadAsync);
        CaptureImageCommand = new Command<CameraView>(async (camera) => await CaptureImageAsync(camera));
        CameraFlashModeCommand = new Command(CameraFlashModeAsync);
        CloseCommand = new Command(async () => await CloseAsync());
        PreviewImageCommand = new Command<ImageSource>(async (imageSource) => await PreviewImageAsync(imageSource));
    }

    public Page CurrentPage { get; set; } = null!;

    public Command LoadCommand { get; set; }
    public Command<CameraView> CaptureImageCommand { get; set; }
    public Command PreviewImageCommand { get; set; }
    public Command RotateCameraCommand { get; set; }
    public Command CameraFlashModeCommand { get; set; }
    public Command CloseCommand { get; set; }

    private void LoadAsync()
    {
        cameraView = CurrentPage.FindByName<CommunityToolkit.Maui.Views.CameraView>("cameraView");

        if (cameraView is not null)
        {
            SelectedCamera = cameraView.SelectedCamera;
            //cameraView.MediaCaptured += CameraView_MediaCaptured;
        }


    }

    private async Task CaptureImageAsync(CameraView camera)
    {
        try
        {
            if (camera is not null)
                await camera.CaptureImage(CancellationToken.None);
        }
        catch (System.Exception ex)
        {
          await  _userDialogs.AlertAsync(ex.Message);
        }
    }

    private async Task PreviewImageAsync(ImageSource imageSource)
    {
        await Shell.Current.GoToAsync($"{nameof(ProductPreviewImageView)}", new Dictionary<string, object>
        {
            [nameof(ImageSource)] = imageSource
        });
    }

    private void CameraFlashModeAsync()
    {
        cameraView = CurrentPage.FindByName<CommunityToolkit.Maui.Views.CameraView>("cameraView");

        if (cameraView is not null)
        {
            CameraFlashMode = CameraFlashMode switch
            {
                CameraFlashMode.Off => CameraFlashMode.On,
                CameraFlashMode.On => CameraFlashMode.Auto,
                CameraFlashMode.Auto => CameraFlashMode.Off,
                _ => CameraFlashMode.Off
            };

        }
    }



    private async Task CloseAsync()
    {
        //cameraView.MediaCaptured -= CameraView_MediaCaptured;
        //cameraView.Handler?.DisconnectHandler();
        await Shell.Current.GoToAsync("..");
    }


}
