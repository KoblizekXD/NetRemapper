using Mono.Cecil;

namespace NetRemapper.Tests
{
    public class NetRemapperTests : IDisposable
    {

        [Fact]
        public void RemapFieldTest()
        {
            NetRemapper remapper = new NetRemapper("Resources/Main.exe", "Resources/Mappings/valid_mappings.netmap");
            remapper.Remap("Resources/Output/Main.exe");

            var assembly = AssemblyDefinition.ReadAssembly("Resources/Output/Main.exe");
            assembly.Dispose();
        }

        public void Dispose()
        {
            File.Delete("Resources/Output/Main.exe");
        }
    }
}
