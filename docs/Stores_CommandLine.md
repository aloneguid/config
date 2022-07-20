# CommandLine Store

This is in no way a command line framework but is rather an addition allowing you to pass configuration values explicitly on the command line.

To configure the store:

```csharp
IMySettings settings = new ConfigurationBuilder<IMySettings>()
   .UseCommandLine()
   .Build();
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

In many cases command line parameters do not have a name but still need to be captured, consider this example:

`myutil upload file1.txt`

this is much shorter than forcing user to specify command line like

`myutil /action=upload /filepath=file1.txt`

You can express the configuration to capture this in the following form:

```csharp
public interface IConsoleCommands
{
   [Option(DefaultValue = "download")]
   string Action { get; }

   string FilePath { get; }
}

//...


IConsoleCommands settings =
   new ConfigurationBuilder<IConsoleCommands>()
   .UseCommandLineArgs(
      new KeyValuePair<string, int>(nameof(IConsoleCommands.Action), 1),
      new KeyValuePair<string, int>(nameof(IConsoleCommands.FilePath), 2))
   .Build();
```

Note that the first command-line parameter starts with `1` not `0`.

## Collections

Command line store also supports collections by using the [flatline syntax](flatline.md).