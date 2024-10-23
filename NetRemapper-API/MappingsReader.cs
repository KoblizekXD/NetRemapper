namespace NetRemapper;

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
    public static Mappings ReadMappings(string mappingsPath)
    {
        Mappings? mappings = null;

        var lines = ReadLines(mappingsPath);

        TypeDefinitionEntry? currentTypeDefinition = null;
        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];

            switch (i)
            {
                case 0: mappings = InitializeByLine(line); 
                    break;
                case 1 when mappings == null:
                    throw RemapperException.HeaderNotInitialized();
                case 1:
                    mappings.Namespaces = [.. line.Split("\t", StringSplitOptions.RemoveEmptyEntries)];
                    break;
                default:
                {
                    if (mappings == null) throw RemapperException.HeaderNotInitialized();

                    var split = line.Split("\t", StringSplitOptions.RemoveEmptyEntries);
                    if (split.Length == 0) continue;

                    switch (split[0])
                    {
                        case "c":
                        {
                            if (currentTypeDefinition != null) mappings.TypeDefinitions.Add(currentTypeDefinition);

                            currentTypeDefinition = new TypeDefinitionEntry([.. mappings.Namespaces], split.Skip(1).ToArray());
                            break;
                        }
                        case "f":
                            currentTypeDefinition?.FieldDefinitions.Add(new FieldDefinitionEntry([.. mappings.Namespaces],
                                split.Skip(1).ToArray()));
                            break;
                        case "m":
                            currentTypeDefinition?.MethodDefinitions.Add(
                                new MethodDefinitionEntry([.. mappings.Namespaces], split.Skip(1).ToArray()));
                            break;
                    }
                    break;
                }
            }
        }

        if (mappings == null) throw RemapperException.HeaderNotInitialized();
        if (currentTypeDefinition is not null)
            mappings.TypeDefinitions.Add(currentTypeDefinition);

        return mappings;
    }
    
    /// <summary>
    /// Reads lines from given path, and ignores all lines starting
    /// with "//" or "#" strings(these are counted as comments and can be therefore ignored).
    /// </summary>
    /// <param name="path">Path to the target mappings</param>
    /// <returns>Read lines</returns>
    /// <exception cref="IOException">If reading failed for some reason</exception>
    private static string[] ReadLines(string path)
    {
        try
        {
            return File.ReadAllLines(path)
                .Where(line => !(line.StartsWith('#') || line.StartsWith("//")))
                .ToArray();
        }
        catch (Exception e)
        {
            throw new IOException("Failed to read mappings file.", e);
        }
    }

    private static Mappings InitializeByLine(string line)
    {
        var split = line.Split("\t", StringSplitOptions.RemoveEmptyEntries);
        if (split[0] == "netmap" && Enum.TryParse<MappingsFormat>(split[1], out var format))
        {
            return new Mappings(format);
        }

        throw new IOException("Invalid mappings file format.");
    }
}