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

```csv
<name>  <revision>
<namespace_1>  <namespace_2>  ...  <namespace_n>
```

### A valid Example

```csv
netmap  V1
obf  intermediary  named
```

#### Explanation

- `netmap` is the name of the mapping format(the only valid value).
- `V1` is the revision of the mapping file, picked up from the `NetRemapper.MappingsFormat` enum.
- `obf`, `intermediary`, `named` are the namespaces that will be used in the mappings.

## Mappings

This part of the specification describes the mappings 
themselves - basically all other lines which are not header or comments.

As of now, following entries are a valid mappable entries:
- `c` - Classes
- `f` - Fields
- `m` - Methods
- `p` - Properties

### Format

```csv
<type>  <namespace value 1> ...  <namespace value n>
```

#### Notes

- If a `<type>` value is not `c`, the current defining type must not be null.
  - To define a new type(and assure it won't be null), use the `c` type before.
- If the current defining type is null, an exception will be thrown.

### A valid Example

```csv
c  qwerty  obf_class  RemappedClass
f  asdf  obf_field  remappedField
```

#### Explanation

- `c` is the type of the mapping, which means that the following line will define a class.
- `qwerty`, `obf_class`, `RemappedClass` are the namespaces values(given header specified 3 namespaces - `obf`, `intermediary`,`named`).
  - If we remapped from the namespace `obf` to `named`, the `RemappedClass` would be the final name of the class and all the `qwerty` references.
- `f` is the type of the mapping, which means that the following line will define a field.
- `asdf`, `obf_field`, `remappedField` are the namespaces values(given header specified 3 namespaces - `obf`, `intermediary`,`named`).
  - Same statement as above applies to the fields and all other types.

## Full Example

(If you're copying this code block, make sure to replace the spaces(&nbsp;&nbsp;) with tabs)

```csv
netmap  V1
obf  intermediary  named
c  qwerty  obf_class  RemappedClass
f  asdf  obf_field  remappedField
```