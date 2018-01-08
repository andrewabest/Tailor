using System;
using Conventional;
using Conventional.Conventions;

namespace Tailor.Test
{
    public class DapperQueriesMustNotDoSelectStar : ConventionSpecification
    {
        private readonly IConnectionFactory _connectionFactory;

        public DapperQueriesMustNotDoSelectStar(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
            
        public override ConventionResult IsSatisfiedBy(Type type)
        {
            var query = (IDapperQuery) Activator.CreateInstance(type, _connectionFactory);

            return query.GetSql().Contains("*")
                ? ConventionResult.NotSatisfied(type.FullName, FailureMessage)
                : ConventionResult.Satisfied(type.FullName);
        }

        protected override string FailureMessage =>
            "Query Sql uses a * parameter. This is bad for ongoing maintainability. Please provide an explicit column list in selects.";
    }
}