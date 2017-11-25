using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conventional;
using Conventional.Conventions;

namespace Tailor.Test
{
    public class DapperQueriesWithParametersMustExecuteSuccessfully : AsyncConventionSpecification
    {
        private readonly string _connectionString;
        private readonly Type[] _exceptionalExceptions;

        public DapperQueriesWithParametersMustExecuteSuccessfully(string connectionString, Type[] exceptionalExceptions)
        {
            _connectionString = connectionString;
            _exceptionalExceptions = exceptionalExceptions;
        }

        public override async Task<ConventionResult> IsSatisfiedBy(Type type)
        {
            var query = (IDapperQuery) Activator.CreateInstance(type, new ConnectionFactory(_connectionString));
            var queryParametersType = type.GetInterfaces()[0].GetGenericArguments()[0];
            var queryParameters = (IQueryParameters) Activator.CreateInstance(queryParametersType, nonPublic: true);

            foreach (var property in queryParameters.GetType().GetProperties())
            {
                if (property.CanWrite)
                {
                    property.SetValue(queryParameters, property.PropertyType.GetDefault());
                }
            }

            var failures = new List<string>();

            try
            {
                if (query.GetType().GetMethod("Execute").Invoke(query, new object[] {queryParameters}) is Task maybeAsyncResult)
                    await maybeAsyncResult;
            }
            catch (Exception e) when (!_exceptionalExceptions.Contains(e.GetType()))
            {
                failures.Add($"Query {type.FullName} failed to execute with exception: {e.Message}");
            }
            catch (Exception)
            {
                // Not a failure
            }

            return failures.Any()
                ? ConventionResult.NotSatisfied(type.FullName,
                    failures.Aggregate(string.Empty, (s, s1) => s + s1 + Environment.NewLine))
                : ConventionResult.Satisfied(type.FullName);
        }

        protected override string FailureMessage => "Query with parameters did not execute successfully";
    }
}