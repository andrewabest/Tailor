using System.Threading.Tasks;

namespace Tailor
{
    public interface IAsyncQuery<TOutput> : IQuery
    {
        Task<TOutput> Execute();
    }

    public interface IAsyncQuery<in TParams, TOutput> : IQuery where TParams : IQueryParameters
    {
        Task<TOutput> Execute(TParams parameters);
    }
}