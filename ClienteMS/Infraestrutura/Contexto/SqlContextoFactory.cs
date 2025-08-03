using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
namespace ClienteMS.Infraestrutura.Contexto
{
    public class SqlContextoFactory : IDesignTimeDbContextFactory<SqlContexto>
    {
        public SqlContexto CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SqlContexto>();
            optionsBuilder.UseSqlite("Data Source=database.db");

            return new SqlContexto(optionsBuilder.Options);
        }
    }
}
