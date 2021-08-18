using Ddd.DynamoDb;

namespace BLambda.HolaMundo.Domain.TemperatureLog
{
    public interface ITemperatureLogRepository : IDocumentRepositoryAsync, IDocumentCrudAsync, IAsyncPageableDocumentRepository
    {

    }
}
