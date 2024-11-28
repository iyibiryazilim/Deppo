using Deppo.Sys.Module.BusinessObjects;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deppo.Sys.Module.Controllers.ProductControllers;


public partial class ProductDetailViewController : ViewController<DetailView>
{
    public ProductDetailViewController()
    {
        InitializeComponent();
        TargetObjectType = typeof(Product);
    }
    
}
