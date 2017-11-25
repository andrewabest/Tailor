using System;
using System.Threading.Tasks;
using Conventional;
using Conventional.Conventions;

namespace Tailor.Test
{
    public class DapperQueriesWithoutParametersMustExecuteSuccessfully : AsyncConventionSpecification
    {
        private readonly string _connectionString;

        public DapperQueriesWithoutParametersMustExecuteSuccessfully(string connectionString)
        {
            _connectionString = connectionString;
        }

        public override async Task<ConventionResult> IsSatisfiedBy(Type type)
        {
            var query = (IDapperQuery) Activator.CreateInstance(type, new ConnectionFactory(_connectionString));

            try
            {
                if (query.GetType().GetMethod("Execute").Invoke(query, new object[0]) is Task maybeAsyncResult)
                    await maybeAsyncResult;
            }
            catch (Exception e)
            {
                return ConventionResult.NotSatisfied(type.FullName,
                    $"Query {type.FullName} failed to execute with exception: {e.Message}");
            }

            return ConventionResult.Satisfied(type.FullName);
        }

        protected override string FailureMessage => "Query without parameters did not execute successfully";
    }
}