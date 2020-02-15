# Entity Framework Core store

## Simple configuration

This is the easiest way to use the Entity Framework Core store.
The built-in `Setting` entity provides the most minimalistic way of storing configuration values.

It looks like this:

```csharp
public class Setting
{
   [Key]
   public long? SettingId { get; set; }

   [Required]
   public string Key { get; set; }

   [Required]
   public string Value { get; set; }
}
```
If you want to use this entity your context class must have a matching `DbSet`:

```csharp
public class MyContext : DbContext
{
   public DbSet<Setting> Settings { get;set; }
}
```

When that is done you can configure the `ConfigurationBuilder`. The only thing you have to supply is an instance of your context.
This instance will be used throughout the lifetime of the configuration object and will be disposed at the end.

```csharp
IMySettings settings = new ConfigurationBuilder<IMySettings>()
   .UseEntityFrameworkCore(new MyContext())
   .Build();
```

## Custom configuration

This second way allows you to use your own context, entity and properties. This approach is very flexible and fits right into any existing code.
For performance reasons no reflection is being used so you will have to supply some delegates that are used to access the entity and it's properties.
The custom entity must have a default constructor.

Let's say your custom context and entity look like this:

```csharp
public class CustomContext : DbContext
{
   public DbSet<CustomSetting> Settings { get;set; }
}
```

```csharp
public class CustomSetting
{
   [Key]
   public long? SettingId { get; set; }

   [Required]
   public string CustomKey { get; set; }

   [Required]
   public string CustomValue { get; set; }
}
```

The configuration would look like this:

```csharp
IMySettings settings = new ConfigurationBuilder<IMySettings>()
   .UseEntityFrameworkCore(
      new MyContext(),                                   // instance of the context

      // function that builds an expression which is passed to EF Core to find the entity by it's key
      // this is necessary since EF Core can't translate nested expressions to SQL
      key => setting => setting.CustomKey == key,

      setting => setting.CustomValue,                    // simple getter for the value
      (setting, value) => setting.CustomValue = value,   // simple setter for the value
      (setting, key) => setting.CustomKey = key,         // simple setter for the key
   )
   .Build();
```
