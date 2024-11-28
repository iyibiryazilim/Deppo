using Deppo.Sys.Module.LogoBusinessObjects;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.ReportsV2;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deppo.Sys.Module.Controllers.LOGO_ProcurementFicheControllers;


public partial class Logo_ProcurementFicheListViewController : ObjectViewController<ListView, LOGO_Dispatch>
{
	SimpleAction PrintProductBarcode;
	const string reportName = "Malzeme Etiketi";
	public Logo_ProcurementFicheListViewController()
	{
		InitializeComponent();
		TargetObjectType = typeof(LOGO_Dispatch);

		PrintProductBarcode = new SimpleAction(this, nameof(PrintProductBarcode), PredefinedCategory.Reports)
		{
			SelectionDependencyType = SelectionDependencyType.RequireSingleObject,
			ImageName = "BO_Report",
			ToolTip = "Print Product Barcode"
		};
		PrintProductBarcode.Execute += PrintProductBarcode_Execute;

	}

	private void PrintProductBarcode_Execute(object sender, SimpleActionExecuteEventArgs e)
	{
		var reportOsProvider = ReportDataProvider.GetReportObjectSpaceProvider(this.Application.ServiceProvider);
		var reportStorage = ReportDataProvider.GetReportStorage(this.Application.ServiceProvider);
		IObjectSpace objectSpace = reportOsProvider.CreateObjectSpace(typeof(ReportDataV2));
		IReportDataV2 reportData = objectSpace.FirstOrDefault<ReportDataV2>(data => data.DisplayName == reportName);
		string handle = reportStorage.GetReportContainerHandle(reportData);
		ReportServiceController controller = Frame.GetController<ReportServiceController>();
		if (controller != null)
		{
			controller.ShowPreview(handle);
		};
	}
}
