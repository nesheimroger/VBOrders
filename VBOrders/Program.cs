using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VBOrders.Models;
using VBOrders.Services;

namespace VBOrders
{
    class Program
    {
        static void Main(string[] args)
        {
            /* TODO
             * 
             *  Make sure of the following or replace values in code with something appropriate:
             *   - Unit with id 1 exists
             *   - Tax and accounting group with id 1 exists
             *   - Customer with customerno 10000 exists
             *   - Supplier with supplierno 50000 exists
             *   
             *   Run the program and follow instructions. When it is done go to visma business and the purchase suggestion tab.
             *   - Locate the product numbers that have been created in the purchase estimates
             *   - Notice that the decrease field is not updated
             *   
             *   
             *   The problem seems to be that subproducts in a structure does not count towards the stock balance / purchase suggestions.
             *   We have tried with various prosessing methods, but unless we set the structure to expand on sales transactions (causing multiple order lines) it does work.
             *   
             */

            var orderService = new OrderService();
            var productService = new ProductService();


            var product1 = new ProductDTO()
            {
                ProductNo = "test-1",
                DefaultSalesUnits = 1,
                Description = "Test part no 1",
                TaxAndAccountingGroup = 1,
                PriceLines = new List<PriceAndDiscountMatrixDTO>()
                {
                    new PriceAndDiscountMatrixDTO()
                    {
                        SupplierNo = 50000,
                        CostPriceInCurrency = 750.0
                    }
                },
                DeliveryAlternatives = new List<DeliveryAlternativeDTO>()
                {
                    new DeliveryAlternativeDTO()
                    {
                        SupplierNo = 50000
                    }
                }

            };

            var product2 = new ProductDTO()
            {
                ProductNo = "test-2",
                DefaultSalesUnits = 1,
                Description = "Test part no 2",
                TaxAndAccountingGroup = 1,
                PriceLines = new List<PriceAndDiscountMatrixDTO>()
                {
                    new PriceAndDiscountMatrixDTO()
                    {
                        SupplierNo = 50000,
                        CostPriceInCurrency = 750.0
                    }
                },
                DeliveryAlternatives = new List<DeliveryAlternativeDTO>()
                {
                    new DeliveryAlternativeDTO()
                    {
                        SupplierNo = 50000
                    }
                }
            };



            var created = false;
            var productFromVisma1 = productService.GetProduct(product1.ProductNo);
            if (productFromVisma1 == null)
            {
                productFromVisma1 = productService.CreateProduct(product1);
                if (productFromVisma1 != null)
                {
                    created = true;
                    Console.WriteLine(
                        $"Created product: {productFromVisma1.ProductNo}. Go to visma and create stock balance for it.");
                }
                else
                {
                    throw new Exception("Error creating product");
                }
            }

            var productFromVisma2 = productService.GetProduct(product2.ProductNo);
            if (productFromVisma2 == null)
            {
                productFromVisma2 = productService.CreateProduct(product2);
                if (productFromVisma2 != null)
                {
                    created = true;
                    Console.WriteLine($"Created product: {productFromVisma2.ProductNo}. Go to visma and create stock balance for it.");
                }
                else
                {
                    throw new Exception("Error creating product");
                }
            }


            var structureProduct = new ProductDTO()
            {
                ProductNo = "test-structure-1",
                Description = "Test structure no 1",
                TaxAndAccountingGroup = 1,
                IsStructureHead = true,
                DefaultSalesUnits = 1, //Stk
                SubProducts = new List<SubProductDTO>()
                {
                    new SubProductDTO(){ProductNo = productFromVisma1.ProductNo, Quantity = 3},
                    new SubProductDTO(){ProductNo = productFromVisma2.ProductNo, Quantity = 5},
                }
            };

            var structureFromVisma = productService.GetProduct(structureProduct.ProductNo);

            if (structureFromVisma == null)
            {
                structureFromVisma = productService.CreateProduct(structureProduct);
                if (structureFromVisma != null)
                {
                    Console.WriteLine($"Created structure product: {structureFromVisma.ProductNo}.");
                }
                else
                {
                    throw new Exception("Error creating structure product");
                }
            }

            if (created)
            {
                Console.WriteLine("Press enter when stock balance is created");
                Console.ReadLine();
            }

            var order = new OrderDTO()
            {
                CustomerNo = 10000,
                RequestedDeliveryDate = DateTime.Now.AddDays(7),
                OriginalConfirmedDeliveryDate = DateTime.Now.AddDays(7),
                ConfirmedDeliveryDate = DateTime.Now.AddDays(7),
                Label = $"Test ordre - {Guid.NewGuid()}",
                DeliveryAddress1 = "Bankgata 1",
                DeliveryPostCode = "6770",
                DeliveryPostalArea = "Nordfjordeid",
                OrderDate = DateTime.Now,
                OrderLines = new List<OrderLineDTO>()
                {
                    new OrderLineDTO()
                    {
                        ProductNo = structureFromVisma.ProductNo,
                        Description = structureFromVisma.Description,
                        UnitType = structureFromVisma.DefaultSalesUnits,
                        Quantity = 5,
                        UnitPrice = 500,
                    }
                }
                
            };

            var orderFromVisma = orderService.CreateSalesOrder(order);
            if (orderFromVisma == null)
            {
                throw new Exception("Error creating order");
            }

            Console.WriteLine($"Created order: {orderFromVisma.OrderNo}");
            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
        }
    }
}
