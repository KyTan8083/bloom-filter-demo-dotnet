using System.Data;

namespace BloomFilterDemo.Services
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
