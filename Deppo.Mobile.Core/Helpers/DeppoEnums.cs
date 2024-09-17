using System;
using System.ComponentModel;

namespace Deppo.Mobile.Core.Helpers;

public static class DeppoEnums
{
    public enum InputProductProcessType
    {
        [Description("Üretimden Giriş İşlemi")]
        ProductionInputProcess = 3,

        [Description("Sayım Fazlası İşlemi")]
        OverCountProcess = 3,

        [Description("Ambar Transfer İşlemi")]
        DevirProcess = 3
    }

    public enum OutputProductProcessType
    {
        [Description("Sarf Fişi")]
        ConsumableProcess = 0,

        [Description("Sayım Eksiği")]
        UnderCountProcess = 1,

        [Description("Fire Fişi")]
        WasteProcess = 3
    }

    public enum TransferProductProcessType
    {
        [Description("Ambar Transferi")]
        TransferProcess = 0,

        [Description("Malzeme Virmanı")]
        OtherTransferProcess = 1
    }

    public enum SalesReturnEnumType
    {
        [Description("Perakende Satış İade")]
        Retail = 0,

        [Description("Toptan İade")]
        Whole= 1
    }
   
}
