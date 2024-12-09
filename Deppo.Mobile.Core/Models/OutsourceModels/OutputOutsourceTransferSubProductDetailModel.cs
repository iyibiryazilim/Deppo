using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Core.Models.OutsourceModels
{
    public partial class OutputOutsourceTransferSubProductDetailModel :ObservableObject
    {
        [ObservableProperty]
        int transactionReferenceId;

        [ObservableProperty]
        int transactionFicheReferenceId;

        [ObservableProperty]
        int seriLotReferenceId;

        [ObservableProperty]
        int inTransactionReferenceId;

        [ObservableProperty]
        int inSerilotTransactionReferenceId;

        [ObservableProperty]
        string seriLotCode = string.Empty;

        [ObservableProperty]
        string seriLotName = string.Empty;

        [ObservableProperty]
        int locationReferenceId;

        [ObservableProperty]
        string locationCode = string.Empty;

        [ObservableProperty]
        string locationName = string.Empty;

        [ObservableProperty]
        double quantity;

        [ObservableProperty]
        double remainingQuantity;

        [ObservableProperty]
        double remainingUnitQuantity;
    }
}
