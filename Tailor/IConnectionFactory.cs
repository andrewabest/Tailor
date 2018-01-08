using System.Data;

namespace Tailor
{
    public interface IConnectionFactory
    {
        IDbConnection Connection { get; }
    }
}