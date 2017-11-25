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

        // Copied from https://github.com/StackExchange/Dapper/blob/071a3fd5a0e9a3ebb9b598737b94b49c56446722/Dapper/SqlMapper.cs#L3653
        // Embedding in Tailor until Dapper goes netstandard 2.0 and DataTable support returns.

        private const string DataTableTypeNameKey = "dapper:TypeName";

        public static SqlMapper.ICustomQueryParameter AsTableValuedParameter(this DataTable table, string typeName = null) =>
            new TableValuedParameter(table, typeName);

        public static string GetTypeName(this DataTable table) =>
            table?.ExtendedProperties[DataTableTypeNameKey] as string;
    }

    /// <summary>
    /// Copied from https://github.com/StackExchange/Dapper/blob/9076086b312b9576df0c137b50f5a356cac87546/Dapper/TableValuedParameter.cs
    /// Embedding in Tailor until Dapper goes netstandard 2.0 and DataTable support returns.
    /// </summary>
    internal sealed class TableValuedParameter : SqlMapper.ICustomQueryParameter
    {
        private readonly DataTable table;
        private readonly string typeName;

        /// <summary>
        /// Create a new instance of <see cref="TableValuedParameter"/>.
        /// </summary>
        /// <param name="table">The <see cref="DataTable"/> to create this parameter for</param>
        public TableValuedParameter(DataTable table) : this(table, null) { /* run base */ }

        /// <summary>
        /// Create a new instance of <see cref="TableValuedParameter"/>.
        /// </summary>
        /// <param name="table">The <see cref="DataTable"/> to create this parameter for.</param>
        /// <param name="typeName">The name of the type this parameter is for.</param>
        public TableValuedParameter(DataTable table, string typeName)
        {
            this.table = table;
            this.typeName = typeName;
        }

        private static readonly Action<System.Data.SqlClient.SqlParameter, string> setTypeName;
        static TableValuedParameter()
        {
            var prop = typeof(System.Data.SqlClient.SqlParameter).GetProperty("TypeName", BindingFlags.Instance | BindingFlags.Public);
            if (prop != null && prop.PropertyType == typeof(string) && prop.CanWrite)
            {
                setTypeName = (Action<System.Data.SqlClient.SqlParameter, string>)
                    Delegate.CreateDelegate(typeof(Action<System.Data.SqlClient.SqlParameter, string>), prop.GetSetMethod());
            }
        }

        void SqlMapper.ICustomQueryParameter.AddParameter(IDbCommand command, string name)
        {
            var param = command.CreateParameter();
            param.ParameterName = name;
            Set(param, table, typeName);
            command.Parameters.Add(param);
        }

        internal static void Set(IDbDataParameter parameter, DataTable table, string typeName)
        {
#pragma warning disable 0618
            parameter.Value = SqlMapper.SanitizeParameterValue(table);
#pragma warning restore 0618
            if (string.IsNullOrEmpty(typeName) && table != null)
            {
                typeName = table.GetTypeName();
            }
            if (!string.IsNullOrEmpty(typeName) && (parameter is System.Data.SqlClient.SqlParameter sqlParam))
            {
                setTypeName?.Invoke(sqlParam, typeName);
                sqlParam.SqlDbType = SqlDbType.Structured;
            }
        }
    }
}