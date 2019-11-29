# INotifyPropertyChanged Support

INotifyPropertyChanged is part of .NET Framework and is often ised in situations when you want to monitor changes to a class' property. It is also an essential part of **Xamarin**, **WPF**, **UWP**, and **Windows Forms** data binding systems.

Config.Net totally supports `INPC` interface out of the box, and all you need to do is derive your interface from `INPC`:

```csharp
public interface IMyConfiguration : INotifyPropertyChanged
{
   string Name { get; set; }
}
```

then build your configuration as usual and subscribe to property changed event:

```csharp
IMyConfiguration config = new ConfigurationBuilder<IMyConfiguration>()
   //...
   .Build();

config.PropertyChanged += (sender, e) =>
{
   Assert.Equal("Name", e.PropertyName);
};

config.Name = "test";   //this will trigger PropertyChanged delegate
```