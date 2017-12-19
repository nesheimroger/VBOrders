using System;
using System.Collections.Generic;
using System.Linq;
using VBOrders.Models;
using Visma.BusinessModel;
using Visma.BusinessServices.Generic;
using Visma.BusinessServices.Wrapper;

namespace VBOrders.Services
{
    public class OrderService
    {
        public static OrderDTO GetByOrderId(int orderId)
        {
            return QueryOrder(o => o.IntegerColumnValue((long)C.Order.OrderNo, orderId));
        }

        public static IEnumerable<OrderDTO> QueryOrders(Action<RowsSelection> query)
        {
            GenericServiceClient client = null;
            var success = false;

            List<OrderDTO> orders = new List<OrderDTO>();

            try
            {
                client = ServiceFactory.GetGenericClient();
                var request = new RequestBuilder();
                var context = ServiceFactory.CreateContext(request);

                var orderTable = context.UseTable((long)T.Order);
                var orderSelection = orderTable.SelectRows();

                query(orderSelection);

                var projection = orderSelection.Rows.ProjectColumns();
                projection.AddColumns<OrderDTO>();

                var orderLineProjection = projection.ProjectChildren((long)Fk.OrderLine.Order);
                orderLineProjection.AddColumns<OrderLineDTO>();

                var response = request.Dispatch(client);

                client.Close();
                success = true;

                if (!response.AllSucceeded)
                {
                    throw new Exception("Error from visma getting orders");
                };

                var contextResults = response.ContextResults.FirstOrDefault();
                var tableResult = contextResults?.ContextOperationResults.FirstOrDefault() as TableUsageResult;
                var rowSelectionResult = tableResult?.TableOperationResults.FirstOrDefault() as RowsSelectionResult;
                var multipleRowResults = rowSelectionResult?.MultipleRowOperationResults;
                var projectionResults = multipleRowResults?.FirstOrDefault() as ProjectionResult;
                var rows = projectionResults?.ResultSet.ResultRows;

                if (rows == null) return orders;

                foreach (var row in rows)
                {
                    var item = DTOMapper.MapToDTO<OrderDTO>(row);
                    if (item != null)
                    {
                        item.OrderLines = row.ChildResultSets.First().ResultRows.Select(DTOMapper.MapToDTO<OrderLineDTO>).ToList();
                        orders.Add(item);
                    }
                }
            }
            catch (Exception e)
            {
                //Log errors
                throw;

            }
            finally
            {
                if (!success)
                {
                    client?.Abort();
                }
            }

            return orders;
        }

        public static OrderDTO QueryOrder(Action<RowSelection> query)
        {
            GenericServiceClient client = null;

            var success = false;
            try
            {
                client = ServiceFactory.GetGenericClient();
                var request = new RequestBuilder();
                var context = ServiceFactory.CreateContext(request);

                var orderTable = context.UseTable((long)T.Order);
                var orderSelection = orderTable.SelectRow();

                query(orderSelection);

                var projection = orderSelection.Row.ProjectColumns();
                projection.AddColumns<OrderDTO>();

                var orderLineProjection = projection.ProjectChildren((long)Fk.OrderLine.Order);
                orderLineProjection.AddColumns<OrderLineDTO>();


                var response = request.Dispatch(client);
                client.Close();
                success = true;

                if (response.AllSucceeded)
                {
                    var contextResults = response.ContextResults.FirstOrDefault();
                    var tableResult = contextResults?.ContextOperationResults.FirstOrDefault() as TableUsageResult;
                    var rowSelectionResult = tableResult?.TableOperationResults.FirstOrDefault() as RowSelectionResult;
                    var projectionResults = rowSelectionResult?.SingleRowOperationResults.FirstOrDefault() as ProjectionResult;
                    var rows = projectionResults?.ResultSet.ResultRows;

                    if (rows != null)
                        foreach (var row in rows)
                        {
                            var item = DTOMapper.MapToDTO<OrderDTO>(row);

                            item.OrderLines = row.ChildResultSets.First().ResultRows
                                .Select(DTOMapper.MapToDTO<OrderLineDTO>).ToList();

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
                if (!success)
                {
                    client?.Abort();
                }
            }

            return null;
        }

        public OrderDTO CreateSalesOrder(OrderDTO order)
        {
            GenericServiceClient client = null;
            var success = false;
            try
            {
                client = ServiceFactory.GetGenericClient();
                var request = new RequestBuilder();
                var context = ServiceFactory.CreateContext(request);
                var orderTable = context.UseTable((long) T.Order);
                var orderRow = orderTable.AddRow();

                orderRow.SuggestValue((long) C.Order.OrderNo);
                orderRow.SetIntegerValue((long) C.Order.TransactionType, (long) V.TransactionType.Sales);

                orderRow.Set(order, o => o.CustomerNo);

                orderRow.SetIntegerValue((long) C.Order.CurrencyNo, 47L);

                orderRow.Set(order, o => o.Label);
                orderRow.Set(order, o => o.DeliveryAddress1);
                orderRow.Set(order, o => o.DeliveryAddress2);
                orderRow.Set(order, o => o.DeliveryPostCode);
                orderRow.Set(order, o => o.DeliveryPostalArea);
                orderRow.Set(order, o => o.YourReference);
                orderRow.Set(order, o => o.OurReference);
                orderRow.Set(order, o => o.OrderDate);
                orderRow.Set(order, o => o.RequestedDeliveryDate);
                orderRow.Set(order, o => o.OriginalConfirmedDeliveryDate);
                orderRow.Set(order, o => o.ConfirmedDeliveryDate);

                TableHandler orderLineTable = orderRow.JoinTable((long) Fk.OrderLine.Order);

                // Add orderline row and assign values to the row
                foreach (var orderline in order.OrderLines)
                {
                    Row orderLineRow = orderLineTable.AddRow();

                    orderLineRow.Set(orderline, o => o.ProductNo);
                    orderLineRow.Set(orderline, o => o.ExternalProductNo);
                    orderLineRow.Set(orderline, o => o.ExternalReference);
                    orderLineRow.Set(orderline, o => o.ExternalId);
                    orderLineRow.Set(orderline, o => o.Description);
                    orderLineRow.Set(orderline, o => o.Quantity);
                    orderLineRow.Set(orderline, o => o.UnitType);
                    orderLineRow.Set(orderline, o => o.UnitPrice);
                    orderLineRow.Set(orderline, o => o.Discount);

                    orderLineRow.SetIntegerValue((long) C.OrderLine.SourceType, 16L);
                }

                // Add a projection on the order row and add columns to the projection
                var orderProjection = orderRow.ProjectColumns();
                orderProjection.AddColumns<OrderDTO>();

                var orderLineProjection = orderProjection.ProjectChildren((long) Fk.OrderLine.Order);
                orderLineProjection.AddColumns<OrderLineDTO>();

                var response = request.Dispatch(client);
                client.Close();
                success = true;


                if (response.AllSucceeded)
                {

                    foreach (OperationResult res in response.OperationResultDictionary.Values)
                    {
                        if (res is ProjectionResult)
                        {
                            var projectionResult = res as ProjectionResult;
                            var projectionResultRow = projectionResult.ResultSet.ResultRows[0];
                            var item = DTOMapper.MapToDTO<OrderDTO>(projectionResultRow);

                            item.OrderLines = projectionResultRow.ChildResultSets.First().ResultRows
                                .Select(DTOMapper.MapToDTO<OrderLineDTO>).ToList();

                            return item;
                        }
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
                if (!success)
                {
                    client?.Abort();
                }
            }

            return null;
        }
    }
}