using System;
using Deppo.Mobile.Core.Models.ProductModels;
using DevExpress.Maui.Controls;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.Converters;

public class SelectProductModel
{
    public int ItemReferenceId { get; set; } = default;
    public BottomSheet? BottomSheet { get; set; }
}
