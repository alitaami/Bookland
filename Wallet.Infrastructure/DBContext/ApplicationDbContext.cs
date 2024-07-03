using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

public class ApplicationDbContext
{
    private readonly IConfiguration _configuration;

    public ApplicationDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IDbConnection Connection
    {
        get
        {
            return new NpgsqlConnection(_configuration.GetConnectionString("BookLandDB"));
        }
    }
}
