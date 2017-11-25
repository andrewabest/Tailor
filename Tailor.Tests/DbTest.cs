using System;
using System.Data;
using System.Data.SqlClient;
using NUnit.Framework;

namespace Tailor.Tests
{
    public abstract class DbTest
    {
#if DEBUG
        protected const string TestDbConnectionString = @"Server=.\SQLEXPRESS;Database=Tailor;Integrated Security=true;";
#else
        protected const string TestDbConnectionString = @"Server=(local)\SQL2014;Database=Tailor;User ID=sa;Password=Password12!";
#endif

        [SetUp]
        public void Setup()
        {
            CreateDatabase();
            CreateTables();
        }

        [TearDown]
        public void Teardown()
        {
            DropDatabase();
        }

        private static void CreateDatabase()
        {
            var sb = new SqlConnectionStringBuilder(TestDbConnectionString);
            var dbName = sb.InitialCatalog;
            sb.InitialCatalog = "master";

            using (IDbConnection dbConnection = new SqlConnection(sb.ConnectionString))
            {
                dbConnection.Open();
                var command = dbConnection.CreateCommand();
                command.CommandText = SqlScripts.CreateDb(dbName);
                command.ExecuteNonQuery();
            }
        }

        private static void CreateTables()
        {
            var sb = new SqlConnectionStringBuilder(TestDbConnectionString);

            using (IDbConnection dbConnection = new SqlConnection(sb.ConnectionString))
            {
                dbConnection.Open();

                var command = dbConnection.CreateCommand();
                command.CommandText = @"
CREATE TABLE [dbo].[Widget](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_Widget] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
))";
                command.ExecuteNonQuery();
            }
        }

        private static void DropDatabase()
        {
            var sb = new SqlConnectionStringBuilder(TestDbConnectionString);
            var dbName = sb.InitialCatalog;
            sb.InitialCatalog = "master";

            using (IDbConnection dbConnection = new SqlConnection(sb.ConnectionString))
            {
                dbConnection.Open();
                var command = dbConnection.CreateCommand();
                command.CommandText = SqlScripts.DropDb(dbName);
                command.ExecuteNonQuery();
            }
        }
    }
}