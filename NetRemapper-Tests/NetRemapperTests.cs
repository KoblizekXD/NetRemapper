using Mono.Cecil;
using Mono.Cecil.Cil;

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
            
            foreach (var instruction in assembly.MainModule.GetType("Hello.Program").Methods[0].Body.Instructions)
            {
                if (instruction.OpCode == OpCodes.Call)
                {
                    var operand = instruction.Operand as MethodDefinition;
                    Assert.Equal("Create", operand?.Name);
                    Assert.Equal("NamedCreator", operand?.DeclaringType.Name);
                    break;
                }
            }

            assembly.Dispose();
        }

        public void Dispose()
        {
            File.Delete("Resources/Output/Main.exe");
        }
    }
}
