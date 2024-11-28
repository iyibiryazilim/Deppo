using DevExpress.Persistent.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Sys.Module.BusinessObjects
{
    public enum TransactionType
    {
        Consumable = 0,
        Wastage = 1,
        Sales = 2,
        SalesByOrder = 3,
        SalesByProcurement = 4,
    }

    public enum OutProcessType
    {
        Manuel = 0,
        FIFO = 1,
        LIFO = 2,
    }

    public enum Priority
    {
        High = 0,
        Normal = 1,
        Low = 2,
        Critic = 3
    }

    public enum TaskStatus
    {
        NotStarted = 0,
        InProgress = 1,
        Waiting = 2,
        Completed = 3,
        Cancelled = 4,
    }

    public enum ProcessType
    {
    }

    public enum IntegrationResult
    {
        [ImageName("Action_Deny")]
        Nothing = 0,

        [ImageName("Action_Grant")]
        Integration = 1
    }

    public enum CustomerType
    {
        Group = 4,
        SupplierSeller = 3,
        Supplier = 2,
        Seller = 1,
        Other = 0,
    }



}