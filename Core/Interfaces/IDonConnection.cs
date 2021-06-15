using System.Data;

namespace Core.Interfaces
{
    public interface IDonConnection
    {
        IDbConnection GetConnection();
    }
}
