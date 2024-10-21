# NetRemapper

C# IL remapper, utilizing Mono.Cecil package.
Inspired by Fabric's Tiny Remapper.

> [!WARNING]
> This is very early state of development, there are no releases so far, and some things might be done a bit differently than they are supposed to be done.

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

## Goals & Targets  

- [x] Field Remapping
- [ ] Method Remapping
	- [ ] Method Body Remapping
	- [x] Method Signature Remapping
- [x] Class Remapping
- [ ] Property Remapping
- [ ] Attribute Remapping
- [ ] Assembly Remapping
	- Allow to remap the whole assembly - experimental
- [ ] JSON Mapping format

## License & Contribution  

Project is licensed under MIT License.  
For contribution, please create an Issue & Pull Request. Contributions are always welcome!