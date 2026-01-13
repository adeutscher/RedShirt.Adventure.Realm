using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace RedShirt.Adventure.Realm.Common.Database.Services;

public interface ISqlConnectionFactory
{
    Task<IDbConnection> GetConnectionAsync();
}

internal class SqlConnectionFactory(IAmazonSimpleSystemsManagement ssm, string connectionStringPath)
    : ISqlConnectionFactory
{
    private readonly Lazy<Task<string>> _connectionStringTask = new(async () => (await ssm.GetParameterAsync(
        new GetParameterRequest
        {
            Name = connectionStringPath,
            WithDecryption = true
        })).Parameter.Value);

    public async Task<IDbConnection> GetConnectionAsync()
    {
        var connectionString = await _connectionStringTask.Value;
        var connection = new MySqlConnection(connectionString);
        return connection;
    }
}