namespace NetRemapper
{
    public static class MappingsWriter
    {
        public static void Write(Mappings mappings, string output)
        {
            FileStream stream = new(output, FileMode.Create);
            StreamWriter writer = new(stream);

            writer.WriteLine($"netmap\t{mappings.Format}");
            writer.WriteLine(string.Join('\t', mappings.Namespaces));
            mappings.TypeDefinitions.ForEach(entry =>
            {
                writer.WriteLine($"c\t{string.Join('\t', entry.Names.Values)}");
                entry.MethodDefinitions.ForEach(method =>
                {
                    writer.WriteLine($"m\t{string.Join('\t', method.Names.Values)}");
                });
                entry.FieldDefinitions.ForEach(field =>
                {
                    writer.WriteLine($"f\t{string.Join('\t', field.Names.Values)}");
                });
            });

            writer.Close();
        }
    }
}
