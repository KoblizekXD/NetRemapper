namespace NetRemapper
{
    public class Mappings(MappingsFormat format)
    {
        public MappingsFormat Format { get; private set; } = format;
        public string[] Namespaces { get; internal set; } = [];
        public List<TypeDefinitionEntry> TypeDefinitions { get; internal set; } = [];

        /// <param name="name">The name symbol</param>
        /// <param name="ns">The namespace to look in</param>
        /// <returns>The first entry which contains name in the specified namespace</returns>
        public TypeDefinitionEntry? GetType(string name, string ns)
        {
            if (!Namespaces.Contains(ns)) return null;
            return TypeDefinitions.FirstOrDefault(type => type.Names[ns] == name);
        }

        /// <param name="name">The name symbol</param>
        /// <param name="ns">The namespace to look in</param>
        /// <returns>The first entry which contains name in the specified namespace</returns>
        public MethodDefinitionEntry? GetMethod(string name, string ns)
        {
            if (!Namespaces.Contains(ns)) return null;
            return TypeDefinitions.SelectMany(type => type.MethodDefinitions).FirstOrDefault(method => method.Names[ns] == name);
        }

        /// <param name="name">The name symbol</param>
        /// <param name="ns">The namespace to look in</param>
        /// <returns>The first entry which contains name in the specified namespace</returns>
        public FieldDefinitionEntry? GetField(string name, string ns)
        {
            if (!Namespaces.Contains(ns)) return null;
            return TypeDefinitions.SelectMany(type => type.FieldDefinitions).FirstOrDefault(field => field.Names[ns] == name);
        }

        /// <summary>
        /// Finds a field corresponding to specified class in the mappings
        /// </summary>
        /// <param name="name">The name of the field</param>
        /// <param name="ns">The namespace the name is located in</param>
        /// <param name="type">The type to look for the field in</param>
        /// <param name="typeNs">The namespace the <see cref="type"/> name is in</param>
        /// <returns>The entry corresponding to parameters or null, if none was found</returns>
        public FieldDefinitionEntry? GetField(string name, string ns, string type, string typeNs)
        {
            if (!Namespaces.Contains(ns) || !Namespaces.Contains(typeNs)) return null;
            return GetType(type, typeNs)?.FieldDefinitions.FirstOrDefault(field => field.Names[ns] == name);
        }

        /// <summary>
        /// Finds a method corresponding to specified class in the mappings
        /// </summary>
        /// <param name="name">The name of the method</param>
        /// <param name="ns">The namespace the name is located in</param>
        /// <param name="type">The type to look for the method in</param>
        /// <param name="typeNs">The namespace the <see cref="type"/> name is in</param>
        /// <returns>The entry corresponding to parameters or null, if none was found</returns>
        public MethodDefinitionEntry? GetMethod(string name, string ns, string type, string typeNs)
        {
            return GetType(type, typeNs)?.MethodDefinitions.FirstOrDefault(method => method.Names[ns] == name);
        }

        /// <summary>
        /// Finds a property corresponding to specified class in the mappings
        /// </summary>
        /// <param name="name">The name of the property</param>
        /// <param name="ns">The namespace the name is located in</param>
        /// <param name="type">The type to look for the method in</param>
        /// <param name="typeNs">The namespace the <see cref="type"/> name is in</param>
        /// <returns>The entry corresponding to parameters or null, if none was found</returns>
        public PropertyDefinitionEntry? GetProperty(string name, string ns, string type, string typeNs)
        {
            return GetType(type, typeNs)?.PropertyDefinitions.FirstOrDefault(method => method.Names[ns] == name);
        }
    }

    public class GenericMappingEntry
    {
        public Dictionary<string, string> Names { get; } = [];

        public GenericMappingEntry(string[] namespaces, string[] names)
        {
            if (namespaces.Length != names.Length) throw new ArgumentException("Namespaces and names must have the same length. The mappings might be incorrectly formatted.");

            for (int i = 0; i < namespaces.Length; i++)
            {
                Names.Add(namespaces[i], names[i]);
            }
        }

        public string this[string ns] => Names[ns];
    }

    public class MethodDefinitionEntry(string[] namespaces, string[] names) : GenericMappingEntry(namespaces, names);
    public class FieldDefinitionEntry(string[] namespaces, string[] names) : GenericMappingEntry(namespaces, names);
    public class PropertyDefinitionEntry(string[] namespaces, string[] names) : GenericMappingEntry(namespaces, names);

    public class TypeDefinitionEntry(string[] namespaces, string[] names) : GenericMappingEntry(namespaces, names)
    {
        public List<MethodDefinitionEntry> MethodDefinitions { get; set; } = [];
        public List<FieldDefinitionEntry> FieldDefinitions { get; set; } = [];
        public List<PropertyDefinitionEntry> PropertyDefinitions { get; set; } = [];
    }

    public enum MappingsFormat
    {
        V1
    }
}
