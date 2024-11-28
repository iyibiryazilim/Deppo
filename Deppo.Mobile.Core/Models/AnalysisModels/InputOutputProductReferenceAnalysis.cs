using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Core.Models.AnalysisModels
{
	public partial class InputOutputProductReferenceAnalysis : ObservableObject
	{
		string argument = string.Empty;
		int argumentMonth = 0;
		double inputReferenceCount = 0;
		double outputReferenceCount = 0;

		public InputOutputProductReferenceAnalysis()
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

		public double InputReferenceCount
		{
			get => inputReferenceCount;
			set => SetProperty(ref inputReferenceCount, value);
		}

		public double OutputReferenceCount
		{
			get => outputReferenceCount;
			set => SetProperty(ref outputReferenceCount, value);
		}

	}
}
