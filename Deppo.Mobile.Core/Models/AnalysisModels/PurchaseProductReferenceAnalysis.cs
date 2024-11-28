using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Deppo.Mobile.Core.Models.AnalysisModels;

public partial class PurchaseProductReferenceAnalysis: ObservableObject
{
string argument = string.Empty;
    int argumentMonth = 0;
    double purchaseReferenceCount = 0;
    double returnReferenceCount = 0;

    public PurchaseProductReferenceAnalysis()
    {
        
    }

    public string Argument
    {
        get => argument;
        set => SetProperty(ref argument, value);
    }

    public int ArgumentMonth
    {
        get => argumentMonth;
        set => SetProperty(ref argumentMonth, value);
    }

    public double PurchaseReferenceCount
    {
        get => purchaseReferenceCount;
        set => SetProperty(ref purchaseReferenceCount, value);
    }

    public double ReturnReferenceCount
    {
        get => returnReferenceCount;
        set => SetProperty(ref returnReferenceCount, value);
    }

}
