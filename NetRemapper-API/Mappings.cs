using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return TypeDefinitions.FirstOrDefault(type => type.Names[ns] == name);
        }

        /// <param name="name">The name symbol</param>
        /// <param name="ns">The namespace to look in</param>
        /// <returns>The first entry which contains name in the specified namespace</returns>
        public MethodDefinitionEntry? GetMethod(string name, string ns)
        {
            return TypeDefinitions.SelectMany(type => type.MethodDefinitions).FirstOrDefault(method => method.Names[ns] == name);
        }

        /// <param name="name">The name symbol</param>
        /// <param name="ns">The namespace to look in</param>
        /// <returns>The first entry which contains name in the specified namespace</returns>
        public FieldDefinitionEntry? GetField(string name, string ns)
        {
            return TypeDefinitions.SelectMany(type => type.FieldDefinitions).FirstOrDefault(field => field.Names[ns] == name);
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
    }

    public class MethodDefinitionEntry(string[] namespaces, string[] names) : GenericMappingEntry(namespaces, names);
    public class FieldDefinitionEntry(string[] namespaces, string[] names) : GenericMappingEntry(namespaces, names);

    public class TypeDefinitionEntry(string[] namespaces, string[] names) : GenericMappingEntry(namespaces, names)
    {
        public List<MethodDefinitionEntry> MethodDefinitions { get; set; } = [];
        public List<FieldDefinitionEntry> FieldDefinitions { get; set; } = [];
    }

    public enum MappingsFormat
    {
        V1
    }
}
