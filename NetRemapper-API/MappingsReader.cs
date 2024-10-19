namespace NetRemapper
{
    public static class MappingsReader
    {
        /// <summary>
        ///     Reads the mappings from the specified mappings file into <c>Mappings</c> object.
        /// </summary>
        /// <param name="mappingsPath">
        ///     The path to the mappings file.
        /// </param>
        /// <returns>
        ///     Returns the newly read mappings(modifiable).
        /// </returns>
        /// <exception cref="IOException">
        ///     Thrown when the mappings format is invalid or was incorrectly defined or
        ///     if the mappings file could not be read for some reason(file was not found etc.).
        /// </exception>
        /// <seealso cref="Mappings"/>
        public static Mappings? ReadMappings(string? mappingsPath)
        {
            Mappings? mappings = null;

            if (mappingsPath == null) return null;

            string[] lines = [];
            try
            {
                lines = File.ReadAllLines(mappingsPath);
            }
            catch (Exception e) { throw new IOException("Failed to read mappings file.", e); };

            lines = lines.Where(line => !(line.StartsWith('#') || line.StartsWith("//"))).ToArray();

            TypeDefinitionEntry? currentTypeDefinition = null;
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                if (i == 0)
                {
                    string[] splitted = line.Split("\t", StringSplitOptions.RemoveEmptyEntries);
                    if (splitted[0] == "netmap")
                    {
                        mappings = new(Enum.Parse<MappingsFormat>(splitted[1]));
                    }
                    else
                    {
                        throw new IOException("Invalid mappings file format.");
                    }
                } else if (i == 1)
                {
                    if (mappings == null) throw new Exception("Invalid mappings file format(header not initialized).");
                    mappings.Namespaces = [.. line.Split("\t", StringSplitOptions.RemoveEmptyEntries)];
                } else
                {
                    if (mappings == null) throw new Exception("Invalid mappings file format(header not initialized).");

                    string[] splitted = line.Split("\t", StringSplitOptions.RemoveEmptyEntries);
                    if (splitted.Length == 0) continue;

                    if (splitted[0] == "c")
                    {
                        if (currentTypeDefinition != null)
                        {
                            mappings.TypeDefinitions.Add(currentTypeDefinition);
                        }
                        currentTypeDefinition = new([.. mappings.Namespaces], splitted.Skip(1).ToArray());
                    } else if (splitted[0] == "f")
                    {
                        currentTypeDefinition?.FieldDefinitions.Add(new([.. mappings.Namespaces], splitted.Skip(1).ToArray()));
                    } else if (splitted[0] == "m")
                    {
                        currentTypeDefinition?.MethodDefinitions.Add(new([.. mappings.Namespaces], splitted.Skip(1).ToArray()));
                    }
                    else continue;
                }
                
            }

            if (currentTypeDefinition != null) mappings?.TypeDefinitions.Add(currentTypeDefinition);

            if (mappings == null) throw new IOException("Invalid mappings file format(header not initialized).");

            return mappings;
        }
    }

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
            if (namespaces.Length != names.Length) throw new ArgumentException("Namespaces and names arrays must have the same length.");

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
