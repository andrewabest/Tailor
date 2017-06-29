using System;
using Conventional;
using Conventional.Conventions;

namespace Tailor.Test
{
    public class DapperQueriesMustNotDoSelectStar : ConventionSpecification
    {
        private readonly string _connectionString;

        public DapperQueriesMustNotDoSelectStar(string connectionString)
        {
            _connectionString = connectionString;
        }
            
        public override ConventionResult IsSatisfiedBy(Type type)
        {
            var query = (IDapperQuery) Activator.CreateInstance(type, new ConnectionFactory(_connectionString));

            return query.GetSql().Contains("*")
                ? ConventionResult.NotSatisfied(type.FullName, FailureMessage)
                : ConventionResult.Satisfied(type.FullName);
        }

        protected override string FailureMessage =>
            "Query Sql uses a * parameter. This is bad for ongoing maintainability. Please provide an explicit column list in selects.";
    }
}