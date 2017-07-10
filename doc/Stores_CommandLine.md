# CommandLine Store

This is in no way a command line framework but is rather an addition allowing you to pass configuration values explicitly on the command line.

To configure the store:

```csharp
protected override void OnConfigure(IConfigConfiguration configuration)
{
    configuration.UseCommandLine();
}
```

## Conventions

This store will recognize any command line parameter which has a key-value delimiter in it (`=` or `:`) and optionally starts with a prefix `/` or `-` (the store trims these characters from the argument start).

If an argument has more than one delimiter the first one will be used.

## Unnamed parameters

Parameters which are not named (don't have a delimiter) are skipped by default. If you wish to map a positional parameter to an option value you can specify an optional dictionary in configuration (see examples below).

## Examples

### Recognizable Parameters

`program.exe arg1=value1 arg2:value2 arg3:value:3 -arg4:value4` `--arg5:value5` `/arg6:value6`

all the parameters are valid and essentially will become the following:

- `arg1`:`value1`
- `arg2`:`value2`
- `arg3`:`value:3` - first delimiter used
- `arg4`:`value4`
- `arg5`:`value5`
- `arg6`:`value6`

### Positional parameters

Given a settings container:

```csharp
class MySettings : SettingsContainer
{
   public readonly Option<string> CommandName;

   public readonly Option<int> Delay;

   protected override void OnConfigure(IConfigConfiguration configuration)
   {
      configuration.UseCommandLine(new Dictionary<int, Option>
      {
         {0, CommandName}
      });
   }
}
```

the following command line `set Delay=5` will map to:

- `CommandName`:`set`
- `Delay`:`5`

