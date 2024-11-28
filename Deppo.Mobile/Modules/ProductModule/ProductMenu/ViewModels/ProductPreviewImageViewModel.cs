using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Mobile.Helpers.MVVMHelper;

namespace Deppo.Mobile.Modules.ProductModule.ProductMenu.ViewModels;

[QueryProperty(name: nameof(ImageSource), queryId: nameof(ImageSource))]
public partial class ProductPreviewImageViewModel : BaseViewModel
{
    [ObservableProperty]
    ImageSource imageSource = null!;

    [ObservableProperty]
    Stream stream = null!;
    public ProductPreviewImageViewModel()
    {
        Title = "Ürün Resmi";

        TryAgainImageCommand = new Command(TryAgainImageAsync);
        ConfirmImageCommand = new Command(ConfirmImageAsync);
    }

    public Page CurrentPage { get; set; } = null!;

    public Command LoadCommand { get; set; }
    public Command TryAgainImageCommand { get; set; }
    public Command ConfirmImageCommand { get; set; }

    

    private void TryAgainImageAsync()
    {
        throw new NotImplementedException();
    }

    private void ConfirmImageAsync()
    {
        throw new NotImplementedException();
    }
}
