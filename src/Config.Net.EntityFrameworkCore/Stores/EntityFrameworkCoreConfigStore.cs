using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Config.Net.EntityFrameworkCore.Stores
{
   public class EntityFrameworkCoreConfigStore<TContext, TEntity> : IConfigStore
      where TContext : DbContext
      where TEntity : class, new()
   {
      private readonly Func<string, Expression<Func<TEntity, bool>>> _predicateBuilder;
      private readonly Func<TEntity, string> _valueGetter;
      private readonly Action<TEntity, string> _valueSetter;
      private readonly Action<TEntity, string> _keySetter;

      private readonly TContext _context;
      private readonly DbSet<TEntity> _dbSet;

      public EntityFrameworkCoreConfigStore(TContext context,
         Func<string, Expression<Func<TEntity, bool>>> predicateBuilder,
         Func<TEntity, string> valueGetter,
         Action<TEntity, string> valueSetter,
         Action<TEntity, string> keySetter)
      {
         _context = context ?? throw new ArgumentNullException(nameof(context));
         _predicateBuilder = predicateBuilder ?? throw new ArgumentNullException(nameof(predicateBuilder));
         _keySetter = keySetter ?? throw new ArgumentNullException(nameof(keySetter));
         _valueGetter = valueGetter ?? throw new ArgumentNullException(nameof(valueGetter));
         _valueSetter = valueSetter ?? throw new ArgumentNullException(nameof(valueSetter));

         _context.Database.EnsureCreated();
         _dbSet = InitSet();
      }

      public bool CanRead => true;

      public bool CanWrite => true;

      public void Dispose()
      {
         _context.Dispose();
      }

      public string Read(string key)
      {
         TEntity entity = FirstOrDefault(key);
         return entity == null ? null : _valueGetter(entity);
      }

      public void Write(string key, string value)
      {
         TEntity entity = FirstOrDefault(key);

         if (entity == null)
         {
            entity = new TEntity();
            _keySetter(entity, key);
            _dbSet.Add(entity);
         }
         
         _valueSetter(entity, value);
         _context.SaveChanges();
      }

      private DbSet<TEntity> InitSet()
      {
         try
         {
            // the documentation says calling Set should already fail if the set doesn't exists but it doesn't
            // so instead we just call Any() once to provoke an exception
            DbSet<TEntity> set = _context.Set<TEntity>();
            bool _ = set.Any();
            return set;
         }
         catch
         {
            throw new InvalidOperationException($"The DbSet for entity type '{typeof(TEntity).Name}' could not be found");
         }
      }

      private TEntity FirstOrDefault(string key)
      {
         Expression<Func<TEntity, bool>> predicate = BuildPredicate(key);
         return _dbSet.FirstOrDefault(predicate);
      }

      private Expression<Func<TEntity, bool>> BuildPredicate(string key)
      {
         return _predicateBuilder(key) ?? throw new InvalidOperationException("The predicate builder returned null");
      }
   }
}