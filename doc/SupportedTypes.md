# Supported Types

This page lists types supported by Config.Net out of the box. To create a custom type please follow these [instructions](CustomParsers.md)

## CLR Types

Config.Net supports the following CLR types:
- `bool`
- `double`
- `int`
- `long`
- `string`
- `System.TimeSpan`
- `System.DateTime`
- `System.Uri`
- `System.Guid`
- `System.Collections.Generic.Dictionary`

## Arrays

String arrays are supported. Config.Net splits an input string by comma `,` and space character ` ` and builds an array from it. When writing back array values are joined by `, `.

## `System.Net.NetworkCredential`

This is a handly built-in .NET class for holding information with username, password and domain. In reality those three fields are almost always enough to hold connection information to remote servers.

Config.Net expects this in the following format:

```
username:password@domain
```

all parts are optional, however there are a few exceptions:

- when `@` character not found, the string is considered not having a domain part but username and password
- when `:` not found the string is considered not having a password. Therefore if you only want to specify a password the string should look like `:password`.