using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SwiftCaps.Data.Context;

namespace SwiftCaps.Admin.Services.Quiz.Tests
{
    public abstract class CRUDTestBase : IDisposable
    {
        private DbContextOptions<SwiftCapsContext> _dbContextOptions;
        private readonly DbConnection _connection;
        private SwiftCapsContext _dbContext;

        public CRUDTestBase()
        {
            _dbContextOptions = new DbContextOptionsBuilder<SwiftCapsContext>()
                              .UseSqlite(CreateInMemoryDatabase())
                              .Options;
            _connection = RelationalOptionsExtension.Extract(_dbContextOptions).Connection;
            _dbContext = new SwiftCapsContext(_dbContextOptions);
            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();
        }

        private DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();
            return connection;
        }

        public SwiftCapsContext GetDbContext() => _dbContext;

        public async ValueTask Seed<T>(IEnumerable<T> data)
            where T : class
        {
            var dbSet = _dbContext.Set<T>();
            await dbSet.AddRangeAsync(data);
        }

        public void Dispose() => _connection.Close();
    }
}
