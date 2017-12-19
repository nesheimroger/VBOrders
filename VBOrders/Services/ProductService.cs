using System;
using System.Collections.Generic;
using System.Linq;
using VBOrders.Models;
using Visma.BusinessModel;
using Visma.BusinessServices.Generic;
using Visma.BusinessServices.Wrapper;

namespace VBOrders.Services
{
    public class ProductService
    {
        public ProductDTO GetProduct(string productNo)
        {
            GenericServiceClient client = null;
            var requestSuccess = false;
            try
            {
                client = ServiceFactory.GetGenericClient();
                var request = new RequestBuilder();
                var context = ServiceFactory.CreateContext(request);

                var productTable = context.UseTable((long) T.Product);
                var productSelection = productTable.SelectRow();
                productSelection.StringColumnValue((long) C.Product.ProductNo, productNo);

                var productProjection = productSelection.Row.ProjectColumns();
                productProjection.AddColumns<ProductDTO>();

                var priceProjection = productProjection.ProjectChildren((long) Fk.PriceAndDiscountMatrix.Product);
                priceProjection.AddColumns<PriceAndDiscountMatrixDTO>();

                var structureProjection = productProjection.ProjectChildren((long) Fk.Structure.Product);
                structureProjection.AddColumns<SubProductDTO>();

                var deliveryProjection = productProjection.ProjectChildren((long)Fk.DeliveryAlternative.Product);
                deliveryProjection.AddColumns<DeliveryAlternativeDTO>();

                var response = request.Dispatch(client);
                requestSuccess = true;

                if (!response.AllSucceeded)
                {
                    throw new Exception("Error from visma");
                }

                foreach (OperationResult res in response.OperationResultDictionary.Values)
                {
                    if (res is ProjectionResult)
                    {
                        var projectionResult = res as ProjectionResult;
                        var projectionResultRow = projectionResult.ResultSet.ResultRows[0];
                        var item = DTOMapper.MapToDTO<ProductDTO>(projectionResultRow);
                        item.PriceLines = new List<PriceAndDiscountMatrixDTO>();
                        item.SubProducts = new List<SubProductDTO>();
                        item.DeliveryAlternatives = new List<DeliveryAlternativeDTO>();

                        for (int i = 0; i < projectionResultRow.ChildResultSets.Count; i++)
                        {
                            var childRows = projectionResultRow.ChildResultSets[i].ResultRows;
                            foreach (var childRow in childRows)
                            {
                                switch (i)
                                {
                                    case 0:
                                        var price = DTOMapper.MapToDTO<PriceAndDiscountMatrixDTO>(childRow);
                                        item.PriceLines.Add(price);
                                        break;
                                    case 1:
                                        var subProduct = DTOMapper.MapToDTO<SubProductDTO>(childRow);
                                        item.SubProducts.Add(subProduct);
                                        break;
                                    case 2:
                                        var delivery = DTOMapper.MapToDTO<DeliveryAlternativeDTO>(childRow);
                                        item.DeliveryAlternatives.Add(delivery);
                                        break;
                                }
                            }
                        }
                        return item;
                    }
                }
            }
            catch (Exception e)
            {
                //Log error
                throw;
            }
            finally
            {
                if(!requestSuccess)
                    client?.Abort();
            }

            return null;
        }

        public ProductDTO CreateProduct(ProductDTO product)
        {
            GenericServiceClient client = null;
            var requestSuccess = false;

            try
            {
                client = ServiceFactory.GetGenericClient();
                var request = new RequestBuilder();
                var context = ServiceFactory.CreateContext(request);

                var productTable = context.UseTable((long)T.Product);
                var productRow = productTable.AddRow();

                productRow.Set(product, p => p.ProductNo);
                productRow.Set(product, p => p.Description);
                productRow.Set(product, p => p.DefaultSalesUnits);
                productRow.Set(product, p => p.TaxAndAccountingGroup);

                if (product.IsStructureHead)
                {
                    productRow.Set(product, p => p.IsStructureHead);

                    productRow.SetIntegerValue((long)C.Product.ProcessingMethod1, 131072);
                    productRow.SetIntegerValue((long)C.Product.ProcessingMethod1b, 1073872896);
                    productRow.SetIntegerValue((long)C.Product.ProcessingMethod1c, 262144);
                    productRow.SetIntegerValue((long)C.Product.ProcessingMethod2, 16384);
                    //productRow.SetIntegerValue((long)C.Product.ProcessingMethod2b, 16384);
                    //productRow.SetIntegerValue((long)C.Product.ProcessingMethod2c, 16384);
                    productRow.SetIntegerValue((long)C.Product.ProcessingMethod4, 262144);
                    productRow.SetIntegerValue((long)C.Product.ProcessingMethod4b, 2097152);
                    //productRow.SetIntegerValue((long)C.Product.ProcessingMethod4c, 1048576);
                    //productRow.SetIntegerValue((long)C.Product.ProcessingMethod6, 0);

                    var structureTable = productRow.JoinTable((long)Fk.Structure.Product);

                    // Add structure row and assign values to the row
                    foreach (var item in product.SubProducts)
                    {
                        var structureRow = structureTable.AddRow();
                        structureRow.Set(item, p => p.ProductNo);
                        structureRow.Set(item, p => p.Quantity);
                    }

                    //var deliveryTable = productRow.JoinTable((long)Fk.DeliveryAlternative.Product);
                    //var deliveryRow = deliveryTable.AddRow();
                    ////rowSelection.StringColumnValue((long) C.DeliveryAlternative.ProductNo, structure.ProductNo);
                    //deliveryRow.SetOn((long)C.DeliveryAlternative.Production, 1);
                }

                if (product.PriceLines != null && product.PriceLines.Any())
                {
                    var priceTable = productRow.JoinTable((long) Fk.PriceAndDiscountMatrix.Product);

                    foreach (var priceLine in product.PriceLines)
                    {
                        var priceRow = priceTable.AddRow();
                        priceRow.SuggestValue((long)C.PriceAndDiscountMatrix.LineNo);
                        priceRow.Set(priceLine, p => p.CostPriceInCurrency);
                        priceRow.Set(priceLine, p => p.SupplierNo);
                        priceRow.Set(priceLine, p => p.SalesPrice1InCurrency);

                        //Handle more fields?
                    }
                }

                if (product.DeliveryAlternatives != null && product.DeliveryAlternatives.Any())
                {
                    var priceTable = productRow.JoinTable((long)Fk.DeliveryAlternative.Product);

                    foreach (var deliveryAlternative in product.DeliveryAlternatives)
                    {
                        var priceRow = priceTable.AddRow();
                        priceRow.SuggestValue((long)C.DeliveryAlternative.LineNo);
                        priceRow.Set(deliveryAlternative, p => p.SupplierNo);

                        //Handle more fields?
                    }
                }

                // Add a projection on the product row and add columns to the projection
                var productProjection = productRow.ProjectColumns();
                productProjection.AddColumns<ProductDTO>();

                var priceProjection = productProjection.ProjectChildren((long) Fk.PriceAndDiscountMatrix.Product);
                priceProjection.AddColumns<PriceAndDiscountMatrixDTO>();

                var structureProjection = productProjection.ProjectChildren((long) Fk.Structure.Product);
                structureProjection.AddColumns<SubProductDTO>();

                var deliveryProjection = productProjection.ProjectChildren((long)Fk.DeliveryAlternative.Product);
                deliveryProjection.AddColumns<DeliveryAlternativeDTO>();

                var response = request.Dispatch(client);

                client.Close();
                requestSuccess = true;

                if (!response.AllSucceeded)
                {
                    throw new Exception("Error from visma");
                }

                foreach (OperationResult res in response.OperationResultDictionary.Values)
                {
                    if (res is ProjectionResult)
                    {
                        var projectionResult = res as ProjectionResult;
                        var projectionResultRow = projectionResult.ResultSet.ResultRows[0];
                        var item = DTOMapper.MapToDTO<ProductDTO>(projectionResultRow);
                        item.PriceLines = new List<PriceAndDiscountMatrixDTO>();
                        item.SubProducts = new List<SubProductDTO>();
                        item.DeliveryAlternatives = new List<DeliveryAlternativeDTO>();

                        for (int i = 0; i < projectionResultRow.ChildResultSets.Count; i++)
                        {
                            var childRows = projectionResultRow.ChildResultSets[i].ResultRows;
                            foreach (var childRow in childRows)
                            {
                                switch (i)
                                {
                                    case 0:
                                        var price = DTOMapper.MapToDTO<PriceAndDiscountMatrixDTO>(childRow);
                                        item.PriceLines.Add(price);
                                        break;
                                    case 1:
                                        var subProduct = DTOMapper.MapToDTO<SubProductDTO>(childRow);
                                        item.SubProducts.Add(subProduct);
                                        break;
                                    case 2:
                                        var delivery = DTOMapper.MapToDTO<DeliveryAlternativeDTO>(childRow);
                                        item.DeliveryAlternatives.Add(delivery);
                                        break;
                                }
                            }
                        }
                        return item;
                    }
                }
            }
            catch (Exception e)
            {
                //Log error
                throw;
            }
            finally
            {
                if (!requestSuccess)
                    client?.Abort();

            }
            return null;
        }
    }
}