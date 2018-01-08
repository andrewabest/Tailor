using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conventional;

namespace Tailor.Test
{
    public class TheTailor
    {
        private readonly Func<IConnectionFactory> _connectionFactoryBuilder;
        private readonly IEnumerable<Type> _dapperQueries;
        private readonly IEnumerable<Type> _dapperQueriesThatHaveNoParameters;
        private readonly IEnumerable<Type> _dapperQueryParameters;

        protected TheTailor(string connectionString, IEnumerable<Type> dapperQueries, IEnumerable<Type> dapperQueriesThatHaveNoParameters, IEnumerable<Type> dapperQueryParameters)
            : this(() => new ConnectionFactory(connectionString), dapperQueries, dapperQueriesThatHaveNoParameters, dapperQueryParameters)
        {
        }

        protected TheTailor(Func<IConnectionFactory> connectionFactoryBuilder, IEnumerable<Type> dapperQueries, IEnumerable<Type> dapperQueriesThatHaveNoParameters, IEnumerable<Type> dapperQueryParameters)
        {
            _connectionFactoryBuilder = connectionFactoryBuilder;
            _dapperQueries = dapperQueries;
            _dapperQueriesThatHaveNoParameters = dapperQueriesThatHaveNoParameters;
            _dapperQueryParameters = dapperQueryParameters;
        }

        public static TheTailor Create(string connectionString, Type[] exportedAssemblyTypes)
        {
            var dapperQueries = exportedAssemblyTypes
                .Where(x => typeof(IDapperQuery).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .ThatHaveParameters();

            var  dapperQueriesThatHaveNoParameters = exportedAssemblyTypes
                .Where(x => typeof(IDapperQuery).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .ThatHaveNoParameters();

            var dapperQueryParameters = exportedAssemblyTypes
                .Where(x => typeof(IQueryParameters).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);

            return new TheTailor(connectionString, dapperQueries, dapperQueriesThatHaveNoParameters,
                dapperQueryParameters);
        }

        public Task<WrappedConventionResult>[] Measure(params Type[] exceptionalExceptions)
        {
            return new[]
            {
                Task.FromResult(_dapperQueryParameters.MustConformTo(Convention.MustHaveAppropriateConstructors)),
                Task.FromResult(
                    _dapperQueries.Union(_dapperQueriesThatHaveNoParameters)
                    .MustConformTo(new DapperQueriesMustNotDoSelectStar(_connectionFactoryBuilder()))),
                Task.FromResult(_dapperQueries.MustConformTo(new DapperQueriesWithParametersMustHaveAParametersListThatMatchesTheDefinedSql(_connectionFactoryBuilder()))),
                _dapperQueries.MustConformTo(new DapperQueriesWithParametersMustExecuteSuccessfully(_connectionFactoryBuilder(), exceptionalExceptions)),
                _dapperQueriesThatHaveNoParameters.MustConformTo(new DapperQueriesWithoutParametersMustExecuteSuccessfully(_connectionFactoryBuilder())),
            };
        }
    }
}
