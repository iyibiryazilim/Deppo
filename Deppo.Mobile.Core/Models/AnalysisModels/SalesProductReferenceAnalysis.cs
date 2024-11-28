using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Deppo.Mobile.Core.Models.AnalysisModels;

public partial class SalesProductReferenceAnalysis : ObservableObject
{
    string argument = string.Empty;
    int argumentMonth = 0;
    double salesReferenceCount = 0;
    double returnReferenceCount = 0;

    public SalesProductReferenceAnalysis()
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

    public double SalesReferenceCount
    {
        get => salesReferenceCount;
        set => SetProperty(ref salesReferenceCount, value);
    }

    public double ReturnReferenceCount
    {
        get => returnReferenceCount;
        set => SetProperty(ref returnReferenceCount, value);
    }


}
