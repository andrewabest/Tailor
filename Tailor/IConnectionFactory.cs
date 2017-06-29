using System.Data.SqlClient;

namespace Tailor
{
    public interface IConnectionFactory
    {
        SqlConnection Connection { get; }
    }
}