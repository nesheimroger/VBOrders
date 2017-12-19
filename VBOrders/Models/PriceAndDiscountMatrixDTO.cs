using System;
using Visma.BusinessModel;

namespace VBOrders.Models
{
    public class PriceAndDiscountMatrixDTO
    {
        //PK, int
        [VismaBusinessColumn((long)C.PriceAndDiscountMatrix.LineNo)]
        public int LineNo { get; set; }

        [VismaBusinessColumn((long)C.PriceAndDiscountMatrix.ProductNo)]
        public string ProductNo { get; set; }

        [VismaBusinessColumn((long)C.PriceAndDiscountMatrix.CostPriceInCurrency)]
        public double CostPriceInCurrency { get; set; }

        [VismaBusinessColumn((long)C.PriceAndDiscountMatrix.CostCurrencyNo)]
        public int CostCurrencyNo { get; set; }

        [VismaBusinessColumn((long)C.PriceAndDiscountMatrix.SalesPrice1InCurrency)]
        public double SalesPrice1InCurrency { get; set; }

        [VismaBusinessColumn((long)C.PriceAndDiscountMatrix.PurchasePriceInCurrency)]
        public double PurchasePriceInCurrency { get; set; }

        [VismaBusinessColumn((long)C.PriceAndDiscountMatrix.PurchaseCurrencyNo)]
        public int PurchaseCurrencyNo { get; set; }

        [VismaBusinessColumn((long)C.PriceAndDiscountMatrix.CreatedDate)]
        public DateTime? CreatedDate { get; set; }

        [VismaBusinessColumn((long)C.PriceAndDiscountMatrix.ChangedDate)]
        public DateTime? ChangedDate { get; set; }

        [VismaBusinessColumn((long)C.PriceAndDiscountMatrix.ChangeTimeInMs)]
        public int ChangedTimeInMs { get; set; }

        [VismaBusinessColumn((long)C.PriceAndDiscountMatrix.CustomerPriceGroup1)]
        public int CustomerPriceGroup1 { get; set; }

        [VismaBusinessColumn((long)C.PriceAndDiscountMatrix.CustomerPriceGroup2)]
        public int CustomerPriceGroup2 { get; set; }

        [VismaBusinessColumn((long)C.PriceAndDiscountMatrix.CustomerPriceGroup3)]
        public int CustomerPriceGroup3 { get; set; }

        [VismaBusinessColumn((long)C.PriceAndDiscountMatrix.ProductPriceGroup1)]
        public int ProductPriceGroup1 { get; set; }

        [VismaBusinessColumn((long)C.PriceAndDiscountMatrix.ProductPriceGroup2)]
        public int ProductPriceGroup2 { get; set; }

        [VismaBusinessColumn((long)C.PriceAndDiscountMatrix.ProductPriceGroup3)]
        public int ProductPriceGroup3 { get; set; }

        [VismaBusinessColumn((long)C.PriceAndDiscountMatrix.EmployeeNo)]
        public int EmployeeNo { get; set; }

        [VismaBusinessColumn((long)C.PriceAndDiscountMatrix.EmployeePriceGroup)]
        public int EmployeePriceGroup { get; set; }

        [VismaBusinessColumn((long)C.PriceAndDiscountMatrix.OrderNo)]
        public int OrderNo { get; set; }

        [VismaBusinessColumn((long)C.PriceAndDiscountMatrix.OrderPriceGroup)]
        public int OrderPriceGroup { get; set; }

        [VismaBusinessColumn((long)C.PriceAndDiscountMatrix.CustomerNo)]
        public int CustomerNo { get; set; }

        [VismaBusinessColumn((long)C.PriceAndDiscountMatrix.SupplierNo)]
        public int SupplierNo { get; set; }

        [VismaBusinessColumn((long)C.PriceAndDiscountMatrix.Volume)]
        public double Volume { get; set; }
    }
}