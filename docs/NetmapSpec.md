# Netmap Spec V1  

- Revision: V1  

The Netmap Spec is a set of specification for the Netmap mappings file format.
Every mapping file consists of:
- A header
- The mappings itself

The format uses the `\t`(tab) character as a primary delimiter, any space
you see in the examples is for readability purposes only, and tab character
is supposed to be used instead.  

## Comments  

Comments are supported. Lines starting with `#` or `//` will be ignored
and considered as comments.

## Header

### Format

The first 2 lines(not counting comments) of the file are always the header.
`<name>` indicates the name of the mapping format(in this case, it will always be
equal to the `netmap`), and `<revision>` indicates
the revision of the mapping file - This value will be looked up from the
`NetRemapper.MappingsFormat` enum. In case of an invalid name or revision, exception
will be thrown.

```
<name>  <revision>
<namespace_1>  <namespace_2>  ...  <namespace_n>
```

### A valid Example

```
netmap  V1
obf  intermediary  named
```

#### Explanation

- `netmap` is the name of the mapping format(the only valid value).
- `V1` is the revision of the mapping file, picked up from the `NetRemapper.MappingsFormat` enum.
- `obf`, `intermediary`, `named` are the namespaces that will be used in the mappings.