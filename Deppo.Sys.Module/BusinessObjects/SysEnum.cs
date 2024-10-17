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
}