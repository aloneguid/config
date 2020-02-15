using System;
using System.Linq.Expressions;
using Config.Net.EntityFrameworkCore;
using Config.Net.EntityFrameworkCore.Stores;
using Microsoft.EntityFrameworkCore;

namespace Config.Net
{
   /// <summary>
   /// Configuration extensions
   /// </summary>
   public static class EntityFrameworkCoreConfigurationExtensions
   {
      /// <summary>
      /// Uses Entity Framework Core as a builder storage.
      /// This method allows you to use a custom configuration entity.
      /// </summary>
      /// <typeparam name="TContext">The type of DbContext to use.</typeparam>
      /// <typeparam name="TEntity">The type of your custom entity</typeparam>
      /// <param name="builder">Configuration object.</param>
      /// <param name="context">The DbContext that will be used.</param>
      /// <param name="predicateBuilder">Builds the query for EF Core.</param>
      /// <param name="valueGetter">The getter for the config value.</param>
      /// <param name="valueSetter">The setter for the config value.</param>
      /// <param name="keySetter">The setter for the config key.</param>
      /// <returns></returns>
      public static ConfigurationBuilder<TInterface> UseEntityFrameworkCore<TInterface, TContext, TEntity>(
         this ConfigurationBuilder<TInterface> builder,
         TContext context,
         Func<string, Expression<Func<TEntity, bool>>> predicateBuilder,
         Func<TEntity, string> valueGetter,
         Action<TEntity, string> valueSetter,
         Action<TEntity, string> keySetter)
         where TInterface : class
         where TContext : DbContext
         where TEntity : class, new()
      {
         IConfigStore configStore = new EntityFrameworkCoreConfigStore<TContext, TEntity>(
            context,
            predicateBuilder,
            valueGetter,
            valueSetter,
            keySetter);
         return builder.UseConfigStore(configStore);
      }

      /// <summary>
      /// Uses Entity Framework Core as a builder storage.
      /// To use this you have to declare a DbSet&lt;Setting&gt; in your DbContext.
      /// </summary>
      /// <typeparam name="TContext">The type of DbContext to use.</typeparam>
      /// <param name="builder">Configuration object.</param>
      /// <param name="context">The DbContext that will be used.</param>
      /// <returns>Changed builder.</returns>
      public static ConfigurationBuilder<TInterface> UseEntityFrameworkCore<TInterface, TContext>(
         this ConfigurationBuilder<TInterface> builder,
         TContext context)
         where TInterface : class
         where TContext : DbContext
      {
         IConfigStore configStore = new EntityFrameworkCoreConfigStore<TContext, Setting>(
            context,
            key => setting => setting.Key == key,
            setting => setting.Value,
            (setting, value) => setting.Value = value,
            (setting, key) => setting.Key = key);
         return builder.UseConfigStore(configStore);
      }
   }
}