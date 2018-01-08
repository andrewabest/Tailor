using System;
using System.Data;
using System.Data.SqlClient;

namespace Tailor
{
    public class SqlConnectionFactory : IConnectionFactory, IDisposable
    {
        private readonly string _connectionString;
        private SqlConnection _connection;

        public SqlConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection Create()
        {
            _connection = new SqlConnection(_connectionString);
            _connection.Open();
            return _connection;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _connection.Close();
            }
        }
    }
}