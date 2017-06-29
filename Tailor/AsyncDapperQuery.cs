using System.Threading.Tasks;

namespace Tailor
{
    public abstract class AsyncDapperQuery<TOutput> : IAsyncQuery<TOutput>, IDapperQuery
    {
        protected AsyncDapperQuery()
        {
        }

        public abstract Task<TOutput> Execute();

        public abstract string GetSql();
    }

    public abstract class AsyncDapperQuery<TParams, TOutput> : IAsyncQuery<TParams, TOutput>, IDapperQuery
        where TParams : IQueryParameters
    {
        protected AsyncDapperQuery()
        {
        }

        public abstract Task<TOutput> Execute(TParams parameters);

        public abstract string GetSql();
    }
}