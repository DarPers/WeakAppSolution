namespace DataIngestorService.APIs;

using DataIngestorService.Contracts.Contracts;
using Refit;

public interface IWeakAppApi
{
    [Get("/meters")]
    Task<ApiResponse<List<SensorEvent>>> GetDataAsync(CancellationToken cancellationToken);
}
