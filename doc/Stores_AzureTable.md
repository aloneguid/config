# Azure Table Store

This store come from a separate NuGet package [Config.Net.Azure](https://www.nuget.org/packages/Config.Net.Azure)

To configure the store:

```csharp
protected override void OnConfigure(IConfigConfiguration configuration)
{
	configuration.UseAzureTable("storage_account_name", "storage_key", "table_name", "application_name");
}
```

The store supports reading and writing and the settings go to a Azure Storage table. To configure the store you need the following:

* **Storage Account Name** which is just a name of your storage account, not a connection string.
* ***Storage Key** can be either primary or secondary access key.
* ***Table Name**. Name of the table you want to use to read and write settings to.
* ***Application Name** which is a name of your application. You can use the same table for storing settings for many applications, and in order to differentiate which app it belongs to the name must be uniqueue per application.

Azure Table has the following format:

| Partition Key    | Row Key    | Value        |
|------------------|------------|--------------|
| application name | option key | option value |

`Value` is a new column added by this store, whereas `PartitionKey` and `RowKey` are built-in table columns.
