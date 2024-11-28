using Deppo.Core.BaseModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.Models
{
    public class SupplierTransaction : BaseTransaction
    {
        private int _supplierReferenceId;
        private string _supplierCode = string.Empty;
        private string _supplierName = string.Empty;
        private string? _image;

        public SupplierTransaction()
        {

        }

        [Browsable(false)]
        public int SupplierReferenceId
        {
            get => _supplierReferenceId;
            set
            {
                if (_supplierReferenceId == value) return;
                _supplierReferenceId = value;
                NotifyPropertyChanged();
            }
        }

        public string SupplierCode
        {
            get => _supplierCode;
            set
            {
                if (_supplierCode == value) return;
                _supplierCode = value;
                NotifyPropertyChanged();
            }
        }

        public string SupplierName
        {
            get => _supplierName;
            set
            {
                if (_supplierName == value) return;
                _supplierName = value;
                NotifyPropertyChanged();
            }
        }

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