using Visma.BusinessModel;

namespace VBOrders.Models
{
    public class OrderLineDTO
    {
        [VismaBusinessColumn((long) C.OrderLine.ProductNo)]
        public string ProductNo { get; set; }

        [VismaBusinessColumn((long) C.OrderLine.TransactionInformation2)]
        public string ExternalProductNo { get; set; }

        [VismaBusinessColumn((long) C.OrderLine.TransactionInformation3)]
        public string ExternalReference { get; set; }

        [VismaBusinessColumn((long) C.OrderLine.Description)]
        public string Description { get; set; }

        [VismaBusinessColumn((long) C.OrderLine.PriceInCurrency)]
        public double UnitPrice { get; set; }

        [VismaBusinessColumn((long) C.OrderLine.Unit)]
        public int UnitType { get; set; }

        [VismaBusinessColumn((long) C.OrderLine.Quantity)]
        public double Quantity { get; set; }

        [VismaBusinessColumn((long) C.OrderLine.DiscountPercent1)]
        public double Discount { get; set; }

        [VismaBusinessColumn((long) C.OrderLine.Finished)]
        public double Finished { get; set; }

        [VismaBusinessColumn((long) C.OrderLine.FinishNow)]
        public double FinishNow { get; set; }

        [VismaBusinessColumn((long) C.OrderLine.LineNo)]
        public int LineNo { get; set; }

        [VismaBusinessColumn((long) C.OrderLine.ExternalId)]
        public int ExternalId { get; set; }

        [VismaBusinessColumn((long) C.OrderLine.AmountInCurrency)]
        public double SubTotalAmount { get; set; }

        [VismaBusinessColumn((long) C.OrderLine.AllocatedOrderNo)]
        public int AllocatedOrderNo { get; set; }

        [VismaBusinessColumn((long) C.OrderLine.AllocatedOrderLineNo)]
        public int AllocatedLineNo { get; set; }

    }
}