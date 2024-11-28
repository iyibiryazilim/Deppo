using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Deppo.Sys.Module.BusinessObjects
{
    [DefaultClassOptions]
    [NavigationItem("ProductProcurementManagement")]
    [ImageName("NewOrder")]
    public class ProcurementAudit : BaseObject
    {
        private ApplicationUser _applicationUser;
        private DateTime _createdOn;
        private int _productReferenceId;
        private string _productCode;
        private string _productName;
        private bool _isVariant;
        private double _quantity;
        private ReasonsForRejectionProcurement _reasonsForRejectionProcurement;
        private double _procurementQuantity;
        private int _warehouseNumber;
        private string _warehouseName;
        private int _locationreferenceId;
        private string _locationCode;
        private string _locationName;

        public ProcurementAudit(Session session)
            : base(session)
        {
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }

        [ModelDefault("AllowEdit", "False")]
        public ApplicationUser ApplicationUser { get => _applicationUser; set => SetPropertyValue(nameof(ApplicationUser), ref _applicationUser, value); }

        [ModelDefault("AllowEdit", "False")]
        public DateTime CreatedOn { get => _createdOn; set => SetPropertyValue(nameof(CreatedOn), ref _createdOn, value); }

        [ModelDefault("AllowEdit", "False"), Browsable(false)]
        public int ProductReferenceId { get => _productReferenceId; set => SetPropertyValue(nameof(ProductReferenceId), ref _productReferenceId, value); }

        [ModelDefault("AllowEdit", "False")]
        public string ProductCode { get => _productCode; set => SetPropertyValue(nameof(ProductCode), ref _productCode, value); }

        [ModelDefault("AllowEdit", "False")]
        public string ProductName { get => _productName; set => SetPropertyValue(nameof(ProductName), ref _productName, value); }

        [ModelDefault("AllowEdit", "False")]
        public bool IsVariant { get => _isVariant; set => SetPropertyValue(nameof(IsVariant), ref _isVariant, value); }

        [ModelDefault("AllowEdit", "False")]
        public double Quantity { get => _quantity; set => SetPropertyValue(nameof(Quantity), ref _quantity, value); }

        [ModelDefault("AllowEdit", "False")]
        public ReasonsForRejectionProcurement ReasonsForRejectionProcurement { get => _reasonsForRejectionProcurement; set => SetPropertyValue(nameof(ReasonsForRejectionProcurement), ref _reasonsForRejectionProcurement, value); }

        [ModelDefault("AllowEdit", "False")]
        public double ProcurementQuantity { get => _procurementQuantity; set => SetPropertyValue(nameof(ProcurementQuantity), ref _procurementQuantity, value); }

        [ModelDefault("AllowEdit", "False")]
        public int WarehouseNumber { get => _warehouseNumber; set => SetPropertyValue(nameof(WarehouseNumber), ref _warehouseNumber, value); }

        [ModelDefault("AllowEdit", "False")]
        public string WarehouseName { get => _warehouseName; set => SetPropertyValue(nameof(WarehouseName), ref _warehouseName, value); }

        [ModelDefault("AllowEdit", "False"), Browsable(false)]
        public int LocationreferenceId { get => _locationreferenceId; set => SetPropertyValue(nameof(LocationreferenceId), ref _locationreferenceId, value); }

        [ModelDefault("AllowEdit", "False")]
        public string LocationCode { get => _locationCode; set => SetPropertyValue(nameof(LocationCode), ref _locationCode, value); }

        [ModelDefault("AllowEdit", "False")]
        public string LocationName { get => _locationName; set => SetPropertyValue(nameof(LocationName), ref _locationName, value); }
    }
}