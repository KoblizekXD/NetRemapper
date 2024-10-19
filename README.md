# NetRemapper

C# IL remapper, utilizing Mono.Cecil package.
Inspired by Fabric's Tiny Remapper.

## Mappings Format  

NetRemapper uses a custom mappings format called "netmap".

### NetMap specifications:  

NetMap will always split by `\t` character, and not spaces(which are ignored).
Lines starting with `#` or `//` are ignored.

1) First line always starts with header:
```regex
netmap   <version>
```

2) Second line always contains the mapping namespaces:
```regex
obf   intermediary   named
```

3) Other lines contain keys and values for each namespace and specified class:
```python
c  seklfj   creator  NamedCreator
# In order to define a field/method translation, you need to be inside class definition
f  coklrkjf create   NamedCreate
```