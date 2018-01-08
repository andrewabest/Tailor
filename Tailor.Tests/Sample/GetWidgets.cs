using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace Tailor.Tests.Sample
{
    public class GetWidgets : AsyncDapperQuery<Widget[]>
    {
        private readonly IConnectionFactory _connectionFactory;

        public GetWidgets(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public override async Task<Widget[]> Execute()
        {
            Widget[] result;
            using (var connection = _connectionFactory.Create())
            {
                result =
                    (await connection.QueryAsync<Widget>(GetSql())).ToArray();
            }

            if (result == null)
            {
                throw new NotFoundException();
            }

            return result;
        }

        public override string GetSql()
        {
            return "Select Id, Name From Widget";
        }
    }
}