using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Visma.BusinessServices.Generic;
using Visma.BusinessServices.Wrapper;

namespace VBOrders
{
    public static class DTOMapper
    {
        public static IEnumerable<long> GetColumnIds<T>()
        {
            var tt = typeof(T);
            foreach (var prop in tt.GetProperties())
            {
                var customAttribute = prop.GetCustomAttribute<VismaBusinessColumnAttribute>();
                if (customAttribute != null)
                {
                    yield return customAttribute.ColumnId;
                }
            }
        }

        public static void AddColumns<T>(this Projection projection)
        {
            var ids = GetColumnIds<T>();
            foreach (var id in ids)
            {
                if (id == 0)
                {
                    throw new Exception("ColumnId cannot be 0");
                }
                projection.AddColumn(id);
            }
        }

        public static void AddColumns<T>(this ChildProjection projection)
        {
            var ids = GetColumnIds<T>();
            foreach (var id in ids)
            {
                projection.AddColumn(id);
            }
        }

        public static T MapToDTO<T>(ResultRow row)
        {
            var myDict = new Dictionary<Type, Func<ResultRow, long, object>>()
            {
                {typeof(string), (rr, col) => rr.GetStringValue(col)},
                {typeof(int), (rr, col) => rr.GetIntegerValue(col)},
                {typeof(long), (rr, col) => rr.GetInt64Value(col)},
                {typeof(byte[]), (rr, col) => rr.GetBinaryValue(col)},
                {typeof(decimal), (rr, col) => rr.GetDecimalValue(col)},
                {typeof(double), (rr, col) => (double)rr.GetDecimalValue(col)},
                {typeof(float), (rr, col) => (float)rr.GetDecimalValue(col)},
                {typeof(DateTime?), (rr, col) =>
                {
                    var time = rr.GetInt64Value(col);
                    if (time != 0)
                    {
                        return DateTime.ParseExact(time.ToString(), "yyyyMMdd", CultureInfo.CurrentCulture);
                    }
                    return null;
                }},
                {typeof(bool), (rr, col) => rr.GetIntegerValue(col) == 1 }
            };

            var dto = Activator.CreateInstance<T>();
            var myType = typeof(T);
            var myProps = myType.GetProperties().ToList();

            foreach (var prop in myProps)
            {
                var customAttribute = prop.GetCustomAttribute<VismaBusinessColumnAttribute>();
                if (customAttribute != null)
                {
                    try
                    {
                        prop.SetValue(dto,
                            myDict.ContainsKey(prop.PropertyType)
                                ? myDict[prop.PropertyType](row, customAttribute.ColumnId)
                                : null);

                    }
                    catch (Exception e)
                    {
                        //Most likely wrong prop type in DTO
                        throw;
                    }
                }
            }

            return dto;
        }

        private static readonly Dictionary<Type, Func<Row, long, object, Assignment>> RopOperationDictionary = new Dictionary<Type, Func<Row, long, object, Assignment>>()
        {

            {typeof(bool), (rr, col, val) => (bool)val ? rr.SetOn(col,1) : rr.SetOff(col, 0)},
            {typeof(string), (rr, col, val) => rr.SetStringValue(col,(string)val)},
            {typeof(int), (rr, col, val) => rr.SetIntegerValue(col, (int)val)},
            {typeof(long), (rr, col, val) => rr.SetIntegerValue(col, (long)val)},
            {typeof(byte[]), (rr, col, val) => rr.SetBinaryValue(col, (byte[]) val)},
            {typeof(decimal), (rr, col, val) => rr.SetDecimalValue(col, (decimal)val)},
            {typeof(double), (rr, col, val) => rr.SetDecimalValue(col, (decimal)Convert.ToDecimal(val))},
            {typeof(float), (rr, col, val) => rr.SetDecimalValue(col, (decimal)val)},
            {typeof(DateTime?), (rr, col, val) => {
                    var date = (DateTime?) val;
                    return rr.SetIntegerValue(col, date.HasValue ? int.Parse(date.Value.ToString("yyyyMMdd")) : 0);
                }
            }
        };

        public static void Set<T1, T2>(this Row row, T1 dto, Expression<Func<T1, T2>> valueExpression)
        {
            var member = valueExpression.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException($"Expression '{valueExpression}' refers to a method, not a property.");

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException($"Expression '{valueExpression}' refers to a field, not a property.");

            var attribute = propInfo.GetCustomAttribute<VismaBusinessColumnAttribute>();
            if (attribute == null)
                throw new ArgumentException("Property is not mapped with attribute");
            
            if(!RopOperationDictionary.ContainsKey(propInfo.PropertyType))
                throw new ArgumentException("Unknown property type");

            var operation = RopOperationDictionary[propInfo.PropertyType];

            var value = valueExpression.Compile()(dto);

            var assignment = operation(row, attribute.ColumnId, value);
            assignment.Operation.EscalateErrorToOperation = row;
        }


    }
}