# CommandLine Store

This is in no way a command line framework but is rather an addition allowing you to pass configuration values explicitly on the command line.

To configure the store:

```csharp
protected override void OnConfigure(IConfigConfiguration configuration)
{
    configuration.UseCommandLine(args);
}
```

`args` is a string array usually passed to the `Main` method. This argument is optional and when set to null will get process command line parameters automatically.

## Conventions

This store will recognize any command line parameter which has a key-value delimiter in it (`=` or `:`) and optionally starts with a prefix `/` or `-` (the store trims these characters from the argument start).

If an argument has more than one delimiter the first one will be used.

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

### Ignored Parameters

Any parameter which has no key-value delimiter is completely ignored.