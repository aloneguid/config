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
   .UseIniString(contentsOfAnIniFile)
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

#### A note on INI  comments

INI files consider semicolon (`;`) as an inline comment separator, therefore you cannot have it a part of a value. For instance a line like `key=value; this is a comment` in ideal INI implementation will be parsed out as

- key: `key`
- value: `value`
- comment: `comment`

However, in my experience, values like secrets, connection strings etc. _do_ often contain semicolons and in order to put them in an INI file you've got to do a trick like put a semicolon at the end of the value so that beforementioned string will become something like this `key=value; this is a comment;` to be parsed out as

- key: `key`
- value: `value; this is a commment`
- comment: *none*

Although this is absolutely valid and this is how INI files should work, it is often really frustrating as when you have a lot of values with semicolons you either have to check that they do contain semicolons and add a semicolon at the end, or just get used to adding semicolon at the end of every value. I believe neither of the solutions are practical, therefore since v**4.8.0** config.net does not parse inline comments by default (comment lines are still processed). This sorts out a lot of confusing questions around "why my value is not parsed correctly by config.net" or "this software is buggy" etc.

If you still want to revert to the old behavior, you can construct INI parser using the new signature:

```csharp
.UseIniFile(string iniFilePath, bool parseInlineComments = false);

// or

.UseIniString<TInterface>(string iniString, bool parseInlineComments = false);
```

and passing `true` to the last argument.


  


* If a value contains semicolon (`;`) which is a comment separator in INI files you should add it also as a last character in the value, because the parser considers only last `;` as a comment separator. For example `key=val;ue` wil be read as `val`, however `key=val;ue;` will be read as `val;ue`.