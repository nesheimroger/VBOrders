using Visma.BusinessModel;

namespace VBOrders.Models
{
    public class DeliveryAlternativeDTO
    {
        [VismaBusinessColumn((long)C.DeliveryAlternative.LineNo)]
        public int LineNo { get; set; }

        [VismaBusinessColumn((long)C.DeliveryAlternative.ProductNo)]
        public string ProductNo { get; set; }

        [VismaBusinessColumn((long)C.DeliveryAlternative.SupplierNo)]
        public int SupplierNo { get; set; }
    }
}