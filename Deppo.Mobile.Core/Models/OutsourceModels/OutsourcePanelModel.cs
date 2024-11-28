using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Core.Models;

namespace Deppo.Mobile.Core.Models.OutsourceModels;

public partial class OutsourcePanelModel : ObservableObject
{
    [ObservableProperty]
    private int totalProductCount;

    [ObservableProperty]
    private double inProductCount;

    [ObservableProperty]
    private double outProductCount;

    [ObservableProperty]
    private double outProductCountTotalRate = default;

    [ObservableProperty]
    private double inProductCountTotalRate = default;

    [ObservableProperty]
    private ObservableCollection<OutsourceModel> outsources = new();

    [ObservableProperty]
    private ObservableCollection<OutsourceFiche> lastOutsourceFiche = new();

    [ObservableProperty]
    private ObservableCollection<OutsourceTransaction> lastOutsourceTransaction = new();

    // Image property with direct notification of ImageData change
    private string? _image;

    public string? Image
    {
        get => _image;
        set
        {
            if (_image == value) return;
            _image = value;
            OnPropertyChanged(); // Notify that Image has changed
            OnPropertyChanged(nameof(ImageData)); // Notify that ImageData has also changed
        }
    }

    // ImageData property
    public byte[] ImageData
    {
        get
        {
            if (string.IsNullOrEmpty(Image))
                return Array.Empty<byte>();
            else
                return Convert.FromBase64String(Image);
        }
    }
}