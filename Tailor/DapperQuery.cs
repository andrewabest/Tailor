namespace Tailor
{
    public abstract class DapperQuery<TOutput> : IQuery<TOutput>, IDapperQuery
    {
        protected DapperQuery()
        {
        }

        public abstract TOutput Execute();

        public abstract string GetSql();
    }

    public abstract class DapperQuery<TParams, TOutput> : IQuery<TParams, TOutput>, IDapperQuery where TParams : IQueryParameters
    {
        protected DapperQuery()
        {
        }

        public abstract TOutput Execute(TParams parameters);

        public abstract string GetSql();
    }
}