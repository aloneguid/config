# INI File Store

## Configuring

### Mapping to file

```csharp
IMySettings settings = new ConfigurationBuilder<IMySettings>()
   .UseIniFile(filePath)
   .Build();
```

This variant supports reading and writing.

### Mapping to file contents

```csharp
IMySettings settings = new ConfigurationBuilder<IMySettings>()
   .UseIniFileContents(contentsOfAnIniFile)
   .Build();
```

This variant only supports reading as you are passing full file content immediately.

## Using

The store fully supports INI file sections.

In the simplest form every key in the INI file corresponds to the name of an option. For instance a definition

```csharp
string MyOption { get; }
```

will correspond to a line in an INI file:

```
MyOption=my fancy value
```

## Using Sections

A section corresponds to a part of option name before the first dot (.), for instance

```
[SectionOne]
MyOption=my fancy value
```

should use the definition

```csharp
[Option(Name = "SectionOne.MyOption")]
string MyOption { get; }
```

### Writing

Writing is straightforward, however note that if an option has a dot in the name a section will be created by default.

Both inline and newline comments are preserved on write:

```
key1=value1 ;this comment is preserved
;this comments is preserved too
```

### Edge Cases

There are a few edge cases when working with INI files you should know about:

* A value can have an equal sign (`=`) and it will be considered a part of the value, because only the first equal sign is considered as a key-value separator.
* Apparently key names cannot contain `=`
* If a value contains semicolon (`;`) which is a comment separator in INI files you should add it also as a last character in the value, because the parser considers only last `;` as a comment separator. For example `key=val;ue` wil be read as `val`, however `key=val;ue;` will be read as `val;ue`.