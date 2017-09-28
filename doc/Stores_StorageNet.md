# Storage.Net Integration

Config.Net can implicitly read configuration from any provider supported by [storage.net](https://github.com/aloneguid/storage) library which provides abstracted access to many storage providers.

## Blobs

Config.Net can be configured to use blob storage from Storage.Net as key-value storage. In this case every setting is treated as a separate blob, for example you can utilise Azure Blob Storage as a backing storage by setting up your config container in the following way:


```csharp

IMySettings settings = new ConfigurationBuilder<IMySettings>()
   .UseStorageNetBlobs(blobs)
   .Build();
```

which when writing 2 settings `testkey1` and `testkey2` will look like:

![Storagenet Azureblob](storagenet-azureblob.png)