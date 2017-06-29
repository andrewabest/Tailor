using System;
using System.Linq;
using System.Text.RegularExpressions;
using Conventional;
using Conventional.Conventions;

namespace Tailor.Test
{
    public class DapperQueriesWithParametersMustHaveAParametersListThatMatchesTheDefinedSql : ConventionSpecification
    {
        private readonly string _connectionString;

        public DapperQueriesWithParametersMustHaveAParametersListThatMatchesTheDefinedSql(string connectionString)
        {
            _connectionString = connectionString;
        }

        public override ConventionResult IsSatisfiedBy(Type type)
        {
            var query = (IDapperQuery) Activator.CreateInstance(type, new ConnectionFactory(_connectionString));
            var queryParametersType = type.GetInterfaces()[0].GetGenericArguments()[0];
            var queryParameters = queryParametersType.GetProperties()
                .Select(x => "@" + x.Name.ToLower())
                .ToArray();
            var sqlParameters = Regex.Matches(query.GetSql(), @"\@[a-z]+", RegexOptions.IgnoreCase)
                .Cast<Match>()
                .Select(m => m.Value.ToLower())
                .ToArray();

            var queryParamNotInSql = queryParameters.Where(qp => !sqlParameters.Contains(qp));
            var failures = queryParamNotInSql
                .Select(
                    parameter =>
                        $"Query {type.FullName} Sql should specify parameter {parameter} found in Parameters Collection, but does not.")
                .ToList();

            var sqlParamNotInQuery = sqlParameters.Where(sqlp => !queryParameters.Contains(sqlp));
            failures.AddRange(sqlParamNotInQuery.Select(
                parameter =>
                    $"Query {type.FullName} Parameters Collection should contain parameter {parameter} found in Sql, but does not."));

            return failures.Any()
                ? ConventionResult.NotSatisfied(type.FullName,
                    failures.Aggregate(string.Empty, (s, s1) => s + s1 + Environment.NewLine))
                : ConventionResult.Satisfied(type.FullName);
        }

        protected override string FailureMessage => "Query parameters do not match SQL parameters";
    }
}