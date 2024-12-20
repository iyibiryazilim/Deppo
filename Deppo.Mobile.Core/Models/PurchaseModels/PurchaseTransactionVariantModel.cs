using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Core.Models.PurchaseModels;

public class PurchaseTransactionVariantModel : PurchaseTransactionModel
{
	private int variantReferenceId = default;
	private string variantCode = string.Empty;
	private string variantName = string.Empty;

    public PurchaseTransactionVariantModel()
    {
            
    }

	public int VariantReferenceId
	{
		get => variantReferenceId;
		set
		{
			if (variantReferenceId == value) return;
			variantReferenceId = value;
			NotifyPropertyChanged(nameof(VariantReferenceId));
		}
	}

	public string VariantCode
	{
		get => variantCode;
		set
		{
			if (variantCode == value) return;
			variantCode = value;
			NotifyPropertyChanged(nameof(VariantCode));
		}
	}

	public string VariantName
	{
		get => variantName;
		set
		{
			if (variantName == value) return;
			variantName = value;
			NotifyPropertyChanged(nameof(VariantName));
		}
	}

}
