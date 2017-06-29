namespace Tailor
{
    public interface IQuery
    {
    }

    public interface IQuery<out TOutput> : IQuery
    {
        TOutput Execute();
    }

    public interface IQuery<in TParams, out TOutput> : IQuery where TParams : IQueryParameters
    {
        TOutput Execute(TParams parameters);
    }
}