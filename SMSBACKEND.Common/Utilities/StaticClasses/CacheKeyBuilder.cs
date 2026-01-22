using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Common.Utilities.StaticClasses
{
    public static class CacheKeyBuilder<TEntity> where TEntity : class
    {
        public static string FirstOrDefaultKey(Expression<Func<TEntity, bool>> predicate)
        {
            // Optional: Hash or serialize predicate for safety
            var entityName = typeof(TEntity).Name;
            var predicateString = predicate?.ToString() ?? "nopredicate";
            var hashedPredicate = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(predicateString)));

            return $"cache:{entityName}:first:{hashedPredicate}";
        }
        public static string GetKey(
         Expression<Func<TEntity, bool>> predicate)
        {
            var entityName = typeof(TEntity).Name;
            var predicateString = predicate?.ToString() ?? "nopredicate";
            var hashedPredicate = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(predicateString)));

            return $"cache:{entityName}:{hashedPredicate}";
        }
        public static string GetAllKey(
         Expression<Func<TEntity, bool>> predicate)
        {
            var entityName = typeof(TEntity).Name;
            var predicateString = predicate?.ToString() ?? "nopredicate";
            var hashedPredicate = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(predicateString)));

            return $"cache:{entityName}:{hashedPredicate}";
        }
    }

}
