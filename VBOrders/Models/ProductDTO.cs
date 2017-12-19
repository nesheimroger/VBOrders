using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Visma.BusinessModel;

namespace VBOrders.Models
{
    public class ProductDTO
    {
        [VismaBusinessColumn((long)C.Product.ProductNo)]
        public string ProductNo { get; set; }

        [VismaBusinessColumn((long)C.Product.Description)]
        public string Description { get; set; }

        [VismaBusinessColumn((long)C.Product.CreatedDate)]
        public DateTime? CreatedDate { get; set; }

        [VismaBusinessColumn((long)C.Product.ChangedDate)]
        public DateTime? ChangedDate { get; set; }

        [VismaBusinessColumn((long)C.Product.DefaultSalesUnits)]
        public int DefaultSalesUnits { get; set; }

        [VismaBusinessColumn((long)C.Product.TaxAndAccountingGroup)]
        public int TaxAndAccountingGroup { get; set; }

        [VismaBusinessColumn((long)C.Product.ProductIsStructureHead)]
        public bool IsStructureHead { get; set; }

        public IList<PriceAndDiscountMatrixDTO> PriceLines { get; set; }

        public IList<SubProductDTO> SubProducts { get; set; }

        public IList<DeliveryAlternativeDTO> DeliveryAlternatives { get; set; }
    }
}
