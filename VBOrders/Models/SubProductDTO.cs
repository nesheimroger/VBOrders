using Visma.BusinessModel;

namespace VBOrders.Models
{
    public class SubProductDTO  
    {
        [VismaBusinessColumn((long)C.Structure.SubProductNo)]
        public string ProductNo { get; set; }

        [VismaBusinessColumn((long)C.Structure.QuantityPerStructure)]
        public double Quantity { get; set; }
    }
}