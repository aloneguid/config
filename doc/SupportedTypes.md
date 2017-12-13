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

Note that when appropriate types are converted and parsed using an `InvariantCulture` instead of a current culture. This allows to be interoperable between different culture settings between applications.

DateTime is always converted to UTC.

## String Arrays

String arrays are supported, which are encoded using a command-line syntax:

- values are separated by a space i.e. `value1 value2`
- if you need spaces inside values you must take it in quotes i.e. `"value with space" valuewithoutspace`
- quotes inside values must be escaped using a double quote (`""`) and the value itself should be quoted i.e. `"value with ""quotes""""`

## `System.Net.NetworkCredential`

This is a handly built-in .NET class for holding information with username, password and domain. In reality those three fields are almost always enough to hold connection information to remote servers.

Config.Net expects this in the following format:

```
username:password@domain
```

all parts are optional, however there are a few exceptions:

- when `@` character not found, the string is considered not having a domain part but username and password
- when `:` not found the string is considered not having a password. Therefore if you only want to specify a password the string should look like `:password`.