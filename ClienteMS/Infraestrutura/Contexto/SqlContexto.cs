﻿using Microsoft.EntityFrameworkCore;
using Shared.Modelos;

namespace ClienteMS.Infraestrutura.Contexto
{
    public class SqlContexto : DbContext
    {
        public SqlContexto(DbContextOptions<SqlContexto> option ): base( option ) {
        
        }
        public DbSet<Cliente> Cliente { get; set; }
        public DbSet<Erro> Erro { get; set; }
    }
}
