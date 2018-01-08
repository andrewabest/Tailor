using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using NUnit.Framework;
using Shouldly;
using Tailor.Test;
using Tailor.Tests.Sample;

namespace Tailor.Tests
{
    public class DapperConventionScenarios : DbTest
    {
        private class SelectStarQuery : DapperQuery<string>
        {
            public SelectStarQuery(IConnectionFactory connectionFactory)
            {
            }

            public override string Execute()
            {
                throw new System.NotImplementedException();
            }

            public override string GetSql()
            {
                return "Select * From Widget";
            }
        }

        [Test]
        public void DapperQueriesMustNotDoSelectStar_FailsWhenStarIsPresent()
        {
            new DapperQueriesMustNotDoSelectStar(new ConnectionFactory(TestDbConnectionString))
                .IsSatisfiedBy(typeof(SelectStarQuery))
                .Failures
                .ShouldBe(new [] { "Query Sql uses a * parameter. This is bad for ongoing maintainability. Please provide an explicit column list in selects." });
        }

        private class NonSelectStarQuery : DapperQuery<string>
        {
            public NonSelectStarQuery(IConnectionFactory connectionFactory)
            {
            }

            public override string Execute()
            {
                throw new System.NotImplementedException();
            }

            public override string GetSql()
            {
                return "Select Id, Name From Widget";
            }
        }

        [Test]
        public void DapperQueriesMustNotDoSelectStar_Success()
        {
            new DapperQueriesMustNotDoSelectStar(new ConnectionFactory(TestDbConnectionString))
                .IsSatisfiedBy(typeof(NonSelectStarQuery))
                .IsSatisfied
                .ShouldBeTrue();
        }

        public class FailingQuery : AsyncDapperQuery<Widget[]>
        {
            private readonly IConnectionFactory _connectionFactory;

            public FailingQuery(IConnectionFactory connectionFactory)
            {
                _connectionFactory = connectionFactory;
            }

            public override async Task<Widget[]> Execute()
            {
                using (var connection = _connectionFactory.Connection)
                {
                    return (await connection.QueryAsync<Widget>(GetSql())).ToArray();
                }
            }

            public override string GetSql()
            {
                return "Select ColumnThatDoesNotExist, Name From Widget";
            }
        }

        [Test]
        public async Task DapperQueriesWithoutParametersMustExecuteSuccessfully_FailsWhenQueryFailsToExecute()
        {
            var result = await new DapperQueriesWithoutParametersMustExecuteSuccessfully(new ConnectionFactory(TestDbConnectionString))
                .IsSatisfiedBy(typeof(FailingQuery));

            result.Failures.ShouldBe(new [] { "Query Tailor.Tests.DapperConventionScenarios+FailingQuery failed to execute with exception: Invalid column name 'ColumnThatDoesNotExist'." });
        }

        public class SucceedingQuery : AsyncDapperQuery<Widget[]>
        {
            private readonly IConnectionFactory _connectionFactory;

            public SucceedingQuery(IConnectionFactory connectionFactory)
            {
                _connectionFactory = connectionFactory;
            }

            public override async Task<Widget[]> Execute()
            {
                using (var connection = _connectionFactory.Connection)
                {
                    return (await connection.QueryAsync<Widget>(GetSql())).ToArray();
                }
            }

            public override string GetSql()
            {
                return "Select Id, Name From Widget";
            }
        }

        [Test]
        public async Task DapperQueriesWithoutParametersMustExecuteSuccessfully_Success()
        {
            var result = await new DapperQueriesWithoutParametersMustExecuteSuccessfully(new ConnectionFactory(TestDbConnectionString))
                .IsSatisfiedBy(typeof(SucceedingQuery));

            result.IsSatisfied.ShouldBeTrue();
        }

        public class QueryParameter : IQueryParameters
        {
            public Guid Id { get; set; }
        }

        public class FailingQueryWithParameter : AsyncDapperQuery<QueryParameter, Widget>
        {
            private readonly IConnectionFactory _connectionFactory;

            public FailingQueryWithParameter(IConnectionFactory connectionFactory)
            {
                _connectionFactory = connectionFactory;
            }

            public override async Task<Widget> Execute(QueryParameter parameter)
            {
                using (var connection = _connectionFactory.Connection)
                {
                    return (await connection.QueryAsync<Widget>(GetSql(), parameter.ToDapperParameters())).SingleOrDefault();
                }
            }

            public override string GetSql()
            {
                return "Select ColumnThatDoesNotExist, Name From Widget Where Id = @id";
            }
        }

        [Test]
        public async Task DapperQueriesWithParametersMustExecuteSuccessfully_FailsWhenQueryFailsToExecute()
        {
            var result = await new DapperQueriesWithParametersMustExecuteSuccessfully(new ConnectionFactory(TestDbConnectionString), new Type[0])
                .IsSatisfiedBy(typeof(FailingQueryWithParameter));

            result.Failures.ShouldBe(new[] { "Query Tailor.Tests.DapperConventionScenarios+FailingQueryWithParameter failed to execute with exception: Invalid column name 'ColumnThatDoesNotExist'.\r\n" });
        }

        public class SucceedingQueryWithParameter : AsyncDapperQuery<QueryParameter, Widget>
        {
            private readonly IConnectionFactory _connectionFactory;

            public SucceedingQueryWithParameter(IConnectionFactory connectionFactory)
            {
                _connectionFactory = connectionFactory;
            }

            public override async Task<Widget> Execute(QueryParameter parameter)
            {
                using (var connection = _connectionFactory.Connection)
                {
                    return
                        (await connection.QueryAsync<Widget>(GetSql(), parameter.ToDapperParameters()))
                        .SingleOrDefault();
                }
            }
            public override string GetSql()
            {
                return "Select Id, Name From Widget Where Id = @id";
            }
        }

        [Test]
        public async Task DapperQueriesWithParametersMustExecuteSuccessfully_Success()
        {
            var result = await new DapperQueriesWithParametersMustExecuteSuccessfully(new ConnectionFactory(TestDbConnectionString), new Type[0])
                .IsSatisfiedBy(typeof(SucceedingQueryWithParameter));

            result.IsSatisfied.ShouldBeTrue();
        }

        public class SucceedingQueryWithParameterAndExceptionalException : AsyncDapperQuery<QueryParameter, Widget>
        {
            private readonly IConnectionFactory _connectionFactory;

            public SucceedingQueryWithParameterAndExceptionalException(IConnectionFactory connectionFactory)
            {
                _connectionFactory = connectionFactory;
            }

            public override async Task<Widget> Execute(QueryParameter parameter)
            {
                Widget result;
                using (var connection = _connectionFactory.Connection)
                {
                    result =
                        (await connection.QueryAsync<Widget>(GetSql(), parameter.ToDapperParameters()))
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

        [Test]
        public async Task DapperQueriesWithParametersMustExecuteSuccessfully_ShouldIgnoreExceptionalExceptions()
        {
            var result = await new DapperQueriesWithParametersMustExecuteSuccessfully(new ConnectionFactory(TestDbConnectionString), new [] { typeof(NotFoundException) })
                .IsSatisfiedBy(typeof(SucceedingQueryWithParameterAndExceptionalException));

            result.IsSatisfied.ShouldBeTrue();
        }

        public class WrongParameter : IQueryParameters
        {
            public bool ShouldNotWork { get; set; }
        }

        public class QueryWithWrongParameter : DapperQuery<WrongParameter, Widget>
        {
            public QueryWithWrongParameter(IConnectionFactory connectionFactory)
            {
            }

            public override Widget Execute(WrongParameter parameters)
            {
                throw new NotImplementedException();
            }

            public override string GetSql()
            {
                return "Select Id, Name From Widget Where Id = @id";
            }
        }

        [Test]
        public void DapperQueriesWithParametersMustHaveAParametersListThatMatchesTheDefinedSql_FailsWhenQueryParametersDoNotMatchSqlParameters()
        {
            new DapperQueriesWithParametersMustHaveAParametersListThatMatchesTheDefinedSql(new ConnectionFactory(TestDbConnectionString))
                .IsSatisfiedBy(typeof(QueryWithWrongParameter))
                .Failures
                .ShouldBe(new [] { "Query Tailor.Tests.DapperConventionScenarios+QueryWithWrongParameter Sql should specify parameter @shouldnotwork found in Parameters Collection, but does not.\r\nQuery Tailor.Tests.DapperConventionScenarios+QueryWithWrongParameter Parameters Collection should contain parameter @id found in Sql, but does not.\r\n" });
        }

        public class CorrectParameter : IQueryParameters
        {
            public Guid Id { get; set; }
        }

        public class QueryWithCorrectParameter : DapperQuery<CorrectParameter, Widget>
        {
            public QueryWithCorrectParameter(IConnectionFactory connectionFactory)
            {
            }

            public override Widget Execute(CorrectParameter parameters)
            {
                throw new NotImplementedException();
            }

            public override string GetSql()
            {
                return "Select Id, Name From Widget Where Id = @id";
            }
        }

        [Test]
        public void DapperQueriesWithParametersMustHaveAParametersListThatMatchesTheDefinedSql_Success()
        {
            new DapperQueriesWithParametersMustHaveAParametersListThatMatchesTheDefinedSql(new ConnectionFactory(TestDbConnectionString))
                .IsSatisfiedBy(typeof(QueryWithCorrectParameter))
                .IsSatisfied
                .ShouldBeTrue();
        }
    }
}