using System.Threading.Tasks;

namespace Parakeet.Net.Data;

public interface INetDbSchemaMigrator
{
    Task MigrateAsync();
}
