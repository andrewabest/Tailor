using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Reflection;
using Dapper;

namespace Tailor
{
    public static class DapperExtensions
    {
        public static ExpandoObject ToDapperParameters(this object parameters)
        {
            var result = new ExpandoObject();

            foreach (var property in parameters.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty))
            {
                var value = property.GetValue(parameters);

                // If we have a Guid array create a table valued parameter of the SQL Type "Identifier"
                // http://stackoverflow.com/questions/6232978/does-dapper-support-sql-2008-table-valued-parameters
                if (property.PropertyType == typeof(Guid[]))
                {
                    var recordsTable = new DataTable();
                    recordsTable.Columns.Add("Id", typeof(Guid));
                    foreach (var id in (Guid[]) value)
                    {
                        var row = recordsTable.NewRow();
                        row[0] = id;
                        recordsTable.Rows.Add(row);
                    }
                    recordsTable.EndLoadData();

                    value = recordsTable.AsTableValuedParameter("Identifier");
                }

                ((IDictionary<string, object>) result)[property.Name.Camelize()] = value;
            }

            return result;
        }
    }
}