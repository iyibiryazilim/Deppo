namespace Deppo.Core.DTOs.Demand
{
    public class DemandInsert
    {
        public DateTime Date { get; set; } = DateTime.Now;
        public string DocumentNumber { get; set; } = string.Empty;
        public short WarehouseNumber { get; set; } = default;
        public string ProjectCode { get; set; } = string.Empty;
        public IList<DemandLineDto> Lines { get; set; }

        public DemandInsert()
        {
            Lines = new List<DemandLineDto>();
        }
    }
}
