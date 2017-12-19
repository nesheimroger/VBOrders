using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Visma.BusinessModel;

namespace VBOrders.Models
{
    public class OrderDTO
    {
        [VismaBusinessColumn((long)C.Order.Name)]
        public string OrderName { get; set; }

        [VismaBusinessColumn((long)C.Order.OrderNo)]
        public int OrderNo { get; set; }

        [VismaBusinessColumn((long)C.Order.CustomerOrSupplierOrderNo)]
        public string ExternalOrderNo { get; set; }

        [VismaBusinessColumn((long)C.Order.CustomerNo)]
        public int CustomerNo { get; set; }

        [VismaBusinessColumn((long)C.Order.SupplierNo)]
        public int SupplierNo { get; set; }

        [VismaBusinessColumn((long)C.Order.OrderDate)]
        public DateTime? OrderDate { get; set; }

        [VismaBusinessColumn((long)C.Order.OriginalConfirmedDeliveryDate)]
        public DateTime? OriginalConfirmedDeliveryDate { get; set; }

        [VismaBusinessColumn((long)C.Order.ConfirmedDeliveryDate)]
        public DateTime? ConfirmedDeliveryDate { get; set; }

        [VismaBusinessColumn((long)C.Order.RequiredDeliveryDate)]
        public DateTime? RequestedDeliveryDate { get; set; }

        [VismaBusinessColumn((long)C.Order.YourReference)]
        public string YourReference { get; set; }

        [VismaBusinessColumn((long)C.Order.OurReference)]
        public string OurReference { get; set; }

        [VismaBusinessColumn((long)C.Order.Label)]
        public string Label { get; set; }

        [VismaBusinessColumn((long)C.Order.InvoicedAmountTotalInCurrency)]
        public double PriceTotal { get; set; }

        [VismaBusinessColumn((long)C.Order.DeliveryAddress1)]
        public string DeliveryAddress1 { get; set; }

        [VismaBusinessColumn((long)C.Order.DeliveryAddress2)]
        public string DeliveryAddress2 { get; set; }

        [VismaBusinessColumn((long)C.Order.DeliveryPostCode)]
        public string DeliveryPostCode { get; set; }

        [VismaBusinessColumn((long)C.Order.DeliveryPostalArea)]
        public string DeliveryPostalArea { get; set; }

        [VismaBusinessColumn((long)C.Order.TransactionType)]
        public int TransactionType { get; set; }

        public IList<OrderLineDTO> OrderLines { get; set; }
    }
}
