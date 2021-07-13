using System.Threading.Tasks;

namespace Tyts.Abstractions.Data
{
    /// <summary>
    /// Represents a data provider
    /// </summary>
    public partial interface IDataProvider
    {
        string ConfigurationName { get; }

        int SupportedLengthOfBinaryHash { get; }

        bool BackupSupported { get; }

        void CreateDatabase(string collation, int triesToConnect = 10);

        void InitializeDatabase();
        
        string CreateForeignKeyName(string foreignTable, string foreignColumn, string primaryTable, string primaryColumn);

        string GetIndexName(string targetTable, string targetColumn);
        
        Task<int?> GetTableIdentAsync<TEntity>() where TEntity : BaseEntity;

        Task<bool> DatabaseExistsAsync();

        bool DatabaseExists();

        Task BackupDatabaseAsync(string fileName);

        Task RestoreDatabaseAsync(string backupFileName);

        Task ReIndexTablesAsync();

        string BuildConnectionString(IConnectionStringInfo connectionString);

        Task SetTableIdentAsync<TEntity>(int ident) where TEntity : BaseEntity;
    }
}
