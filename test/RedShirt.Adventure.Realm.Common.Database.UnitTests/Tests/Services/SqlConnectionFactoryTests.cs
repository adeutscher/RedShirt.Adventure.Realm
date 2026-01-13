using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using Moq;
using RedShirt.Adventure.Realm.Common.Database.Services;

namespace RedShirt.Adventure.Realm.Common.Database.UnitTests.Tests.Services;

public class SqlConnectionFactoryTests
{
    [Fact]
    public async Task CreateConnection()
    {
        var path = Guid.NewGuid().ToString();
        var parameter = string.Empty;

        var ssm = new Mock<IAmazonSimpleSystemsManagement>(MockBehavior.Strict);

        ssm.Setup(s => s.GetParameterAsync(It.IsAny<GetParameterRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetParameterResponse
            {
                Parameter = new Parameter
                {
                    Name = path,
                    Value = parameter
                }
            });

        var factory = new SqlConnectionFactory(ssm.Object, path);

        var conn1 = await factory.GetConnectionAsync();
        Assert.NotNull(conn1);
        var conn2 = await factory.GetConnectionAsync();
        Assert.NotNull(conn2);
        Assert.NotSame(conn1, conn2);

        Assert.Single(ssm.Invocations);
        ssm.Verify(
            s => s.GetParameterAsync(It.Is<GetParameterRequest>(r => r.Name == path && r.WithDecryption == true),
                It.IsAny<CancellationToken>()), Times.Once);
    }
}