using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using SwiftCaps.Models.Models;
using Xamariners.Core.Model.Internal;

namespace SwiftCaps.Helpers.Tests.Extensions
{
    public static class DbSetExtensions
    {
        public static DbSet<T> ToDbSet<T>(this IList<T> sourceList) where T : CoreObject
        {
            var queryable = sourceList.AsQueryable();

            var dbSet = new Mock<DbSet<T>>();
            dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
            dbSet.Setup(d => d.Add(It.IsAny<T>())).Callback<T>(sourceList.Add);
            dbSet.Setup(d => d.FindAsync(It.IsAny<object[]>())).ReturnsAsync((object[] param) =>
            {
                if (param.FirstOrDefault() is Guid id)
                {
                    return sourceList.FirstOrDefault(v => v.Id == id);
                }
                return null;
            });
            return dbSet.Object;
        }

        public static DbSet<T> ToDbSetModel<T>(this IList<T> sourceList) where T : ModelBase
        {
            var queryable = sourceList.AsQueryable();

            var dbSet = new Mock<DbSet<T>>();
            dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
            dbSet.Setup(d => d.Add(It.IsAny<T>())).Callback<T>(sourceList.Add);
            dbSet.Setup(d => d.FindAsync(It.IsAny<object[]>())).ReturnsAsync((object[] param) =>
            {
                if (param.FirstOrDefault() is Guid id)
                {
                    return sourceList.FirstOrDefault(v => v.Id == id);
                }
                return null;
            });
            return dbSet.Object;
        }
    }
}
