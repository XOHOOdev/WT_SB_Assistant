using Microsoft.EntityFrameworkCore;

namespace WtSbAssistant.Core.Helpers
{
    public static class ExtensionMethods
    {
        public static void RenameKey<TKey, TValue>(this IDictionary<TKey, TValue> dic,
            TKey fromKey, TKey toKey)
        {
            var value = dic[fromKey];
            dic.Remove(fromKey);
            dic[toKey] = value;
        }

        public static List<TDatabaseEntity> BulkInsert<TDatabaseEntity>(this DbSet<TDatabaseEntity> dbSet, List<TDatabaseEntity> entitiesToInsert)
            where TDatabaseEntity : class
        {
            var entitiesInDb = dbSet
                .AsEnumerable()
                .Where(db => entitiesToInsert.Exists(eI => eI.Equals(db)))
                .ToList();


            var entitiesNotInDb = entitiesToInsert
                .AsEnumerable()
                .Where(e => !entitiesInDb.Exists(edb => edb.Equals(e)))
                .ToList();

            dbSet.AddRange(entitiesNotInDb);

            return entitiesInDb.Concat(entitiesNotInDb).ToList();
        }
    }
}
