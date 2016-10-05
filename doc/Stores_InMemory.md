# InMemory Store

To configure the store:

```csharp
protected override void OnConfigure(IConfigConfiguration configuration)
{
	configuration.UseInMemoryConfig();
}
```

The store supports reading and writing, and stores configuration in the application memory. When application restarts all the setting values are lost. You may want to use this store for debugging or testing, other that that it has no real applications.