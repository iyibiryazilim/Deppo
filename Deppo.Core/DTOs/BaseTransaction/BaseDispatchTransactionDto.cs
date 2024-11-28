using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.DTOs.BaseTransaction;

public class BaseDispatchTransactionDto
{
    public string? Code { get; set; }
    public int FirmNumber { get; set; }
    public DateTime TransactionDate { get; set; }
    public int? WarehouseNumber { get; set; }
    public string? CurrentCode { get; set; }
    public string? Description { get; set; }
    public short? DispatchType { get; set; }
    public string? CarrierCode { get; set; }
    public string? DriverFirstName { get; set; }
    public string? DriverLastName { get; set; }
    public string? IdentityNumber { get; set; }
    public string? Plaque { get; set; }
    public string? ShipInfoCode { get; set; }
    public string? SpeCode { get; set; }
    public short? DispatchStatus { get; set; }
	public short? Status { get; set; }
	public short? IsEDispatch { get; set; }
    public string? DoCode { get; set; }
    public string? DocTrackingNumber { get; set; }
    public short? EDispatchProfileId { get; set; }
}