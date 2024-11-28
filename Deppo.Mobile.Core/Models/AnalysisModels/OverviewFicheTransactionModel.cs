using Deppo.Core.BaseModels;

namespace Deppo.Mobile.Core.Models.AnalysisModels
{
	public class OverviewFicheTransactionModel : BaseTransaction
	{
		private int _currentReferenceId;
		private string _currentCode;
		private string _currentName = string.Empty;

		public int CurrentReferenceId
		{
			get => _currentReferenceId;
			set
			{
				if (_currentReferenceId == value) return;
				_currentReferenceId = value;
				NotifyPropertyChanged();
			}
		}
		public string CurrentCode
		{
			get => _currentCode;
			set
			{
				if (_currentCode == value) return;
				_currentCode = value;
				NotifyPropertyChanged();
			}
		}

		public string CurrentName
		{
			get => _currentName;
			set
			{
				if (_currentName == value) return;
				_currentName = value;
				NotifyPropertyChanged();
			}
		}


	}
}
