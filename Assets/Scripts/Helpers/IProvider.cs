using System.Threading;
using System.Threading.Tasks;

public interface IProvider<T>
{
    Task<T> ProvideAsync(CancellationToken cancellationToken);
}
