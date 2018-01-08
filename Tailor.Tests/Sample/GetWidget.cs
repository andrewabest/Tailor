using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace Tailor.Tests.Sample
{
    public class GetWidget : AsyncDapperQuery<GetWidgetParameters, Widget>
    {
        private readonly IConnectionFactory _connectionFactory;

        public GetWidget(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public override async Task<Widget> Execute(GetWidgetParameters parameters)
        {
            Widget result;
            using (var connection = _connectionFactory.Create())
            {
                result =
                    (await connection.QueryAsync<Widget>(GetSql(), parameters.ToDapperParameters()))
                    .SingleOrDefault();
            }

            if (result == null)
            {
                throw new NotFoundException();
            }

            return result;
        }

        public override string GetSql()
        {
            return "Select Id, Name From Widget Where Id = @id";
        }
    }
}