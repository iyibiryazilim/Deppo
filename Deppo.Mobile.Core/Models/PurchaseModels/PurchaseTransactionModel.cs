using Deppo.Core.Models;

namespace Deppo.Mobile.Core.Models.PurchaseModels
{
    public class PurchaseTransactionModel : PurchaseTransaction
    {
        private bool _isSelected;
        private bool _isVariant;
        private int _trackingType;
        private int _locTracking;
        private string _locTrackingIcon;
        private string _locTrackingIconColor;
        private string _variantIcon;
        private string _variantIconColor;
        private string _trackingTypeIcon;
        private string _trackingTypeIconColor;
        private string? _image;

        public PurchaseTransactionModel()
        {
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value) return;
                _isSelected = value;
                NotifyPropertyChanged(nameof(IsSelected));
            }
        }

        public bool IsVariant
        {
            get => _isVariant;
            set
            {
                if (_isVariant == value) return;
                _isVariant = value;
                NotifyPropertyChanged(nameof(IsVariant));
            }
        }

        public int TrackingType
        {
            get => _trackingType;
            set
            {
                if (_trackingType == value) return;
                _trackingType = value;
                NotifyPropertyChanged(nameof(TrackingType));
            }
        }

        public int LocTracking
        {
            get => _locTracking;
            set
            {
                if (_locTracking == value) return;
                _locTracking = value;
                NotifyPropertyChanged(nameof(LocTracking));
            }
        }

        public string LocTrackingIcon
        {
            get => _locTrackingIcon = _locTrackingIcon ?? "location-dot";
            set
            {
                if (_locTrackingIcon == value) return;
                _locTrackingIcon = value;
                NotifyPropertyChanged();
            }
        }

        public string LocTrackingIconColor => LocTracking == 1 ? "#F5004F" : "#C8C8C8";

        public string VariantIcon
        {
            get => _variantIcon = _variantIcon ?? "bookmark";
            set
            {
                if (_variantIcon == value) return;
                _variantIcon = value;
                NotifyPropertyChanged();
            }
        }

        public string VariantIconColor => IsVariant ? "#F5004F" : "#C8C8C8";

        public string TrackingTypeIcon
        {
            get => _trackingTypeIcon = _trackingTypeIcon ?? "box-archive";
            set
            {
                if (_trackingTypeIcon == value) return;
                _trackingTypeIcon = value;
                NotifyPropertyChanged();
            }
        }

        public string TrackingTypeIconColor => TrackingType == 1 ? "#F5004F" : "#C8C8C8";

        public string? Image
        {
            get => _image;
            set
            {
                if (_image == value) return;
                _image = value;
                NotifyPropertyChanged();
            }
        }

		public byte[] ImageData
		{
			get
			{
				if (string.IsNullOrEmpty(Image))
					return Array.Empty<byte>();
				else
				{
					return Convert.FromBase64String(Image);
				}
			}
		}
	}
}