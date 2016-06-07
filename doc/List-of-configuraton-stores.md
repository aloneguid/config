# Built-in Stores

These configuration stores are available out of the box in the main [Config.Net](https://www.nuget.org/packages/Config.Net) library.

## AppConfigStore (.NET 4.5)

Reads values from the standard app.config file, i.e the settings you would usually read with [System.Configuration.ConfigurationManager](https://msdn.microsoft.com/en-us/library/system.configuration.configurationmanager(v=vs.110).aspx). The store is read-only.

## AssemblyConfigStore (.NET 4.5)

Reads values from the standard .exe.config or .dll.config file, i.e. settings you would usually read with [ConfigurationManager.OpenExeConfiguration Method](https://msdn.microsoft.com/en-us/library/system.configuration.configurationmanager.openexeconfiguration(v=vs.110).aspx). The store is read-only.

## IniFileConfigStore (.NET 4.5)

Reads and writes configuration to a simple INI file. As of version 1.0.3 it supports INI file sections and preserves comments in files when writing back to disk, both inline and comments on separate lines.

Section names can be referred by prefixing keys with a section name and a dot (.), for example this file:

```
GlobalKey=value1

[SectionOne]
GlobalKey=value2

[SectionTwo]
GlobalKey=value3
```

in Config.Net will have three keys:
- GlobalKey
- SectionOne.GlobalKey
- SectionTwo.GlobalKey

## InMemoryConfigStore (.NET 4.5, Portable)

Reads and writes temporary settings in memory.

# Config.Net.Azure

This library is a separate [NuGet Package](https://www.nuget.org/packages/Config.Net.Azure/)

## AzureConfigStore (.NET 4.5)

Reads configuration as you would normally do with [CloudConfigurationManager Class](https://msdn.microsoft.com/en-us/library/microsoft.windowsazure.cloudconfigurationmanager.aspx)

## AzureTableConfigStore (.NET 4.5)

Reads and writes values to a Windows Azure Storage Table. The constructor requires 4 parameters:

* accountName - your storage account name
* storageKey - your storage's primary or secondary key
* tableName - name of table in the storage account
* appName - name of your application

appName is the most important parameter as it allows to use one table for many applications. Internally table record will look similar to this:

* PartitionKey - your application name, set in appName parameter
* RowKey - setting's key
* Value - custom column which stores the key's value.