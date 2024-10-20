namespace NetRemapper.Tests
{
    public class MappingsReaderTests
    {
        [Fact]
        public void TestSuccessfulMappingsLoad()
        {
            Mappings? mappings = MappingsReader.ReadMappings("Resources/Mappings/valid_mappings.netmap");

            Assert.NotNull(mappings);
            Assert.Equal(3, mappings!.Namespaces.Length);
            Assert.Equal(MappingsFormat.V1, mappings!.Format);
        }

        [Fact]
        public void TestInvalidMappingsLoad()
        {
            // Names != Namespaces counts
            Assert.Throws<ArgumentException>(() =>
            {
                MappingsReader.ReadMappings("Resources/Mappings/invalid_mappings.netmap");
            });

            // Invalid format
            Assert.Throws<ArgumentException>(() =>
            {
                MappingsReader.ReadMappings("Resources/Mappings/invalid_mappings_2.netmap");
            });
        }

        [Fact]
        public void TestFieldLookup()
        {
            Mappings? mappings = MappingsReader.ReadMappings("Resources/Mappings/valid_mappings.netmap");

            Assert.NotNull(mappings);

            Assert.NotNull(mappings.GetType("Creator", "obf"));
            Assert.NotNull(mappings.GetField("Create", "obf", "Creator", "obf"));
            Assert.Null(mappings.GetField("Create", "named", "Creator", "obf2"));
            Assert.Null(mappings.GetField("Create", "obf", "Creator", "named"));
        }

        [Fact]
        public void TestMethodLookup()
        {
            Mappings? mappings = MappingsReader.ReadMappings("Resources/Mappings/valid_mappings.netmap");

            Assert.NotNull(mappings);

            Assert.NotNull(mappings.GetType("Creator", "obf"));
            Assert.NotNull(mappings.GetMethod("Modify", "obf", "Creator", "obf"));
            Assert.Null(mappings.GetField("Modify", "named", "Creator", "obf2"));
            Assert.Null(mappings.GetField("Modify", "obf", "Creator", "named"));
        }
    }
}