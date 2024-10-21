using Mono.Cecil;
using Mono.Cecil.Cil;
using Xunit.Abstractions;

namespace NetRemapper.Tests
{
    public class NetRemapperTests : IDisposable
    {
        private readonly ITestOutputHelper testOutputHelper;
        private readonly StringWriter stringReader;
        private readonly TextWriter textWriter;

        public NetRemapperTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
            stringReader = new StringWriter();
            textWriter = Console.Out;
            Console.SetOut(stringReader);
        }

        [Fact]
        public void TestFieldRemapping()
        {
            NetRemapper remapper = new("Resources/Main.exe", "Resources/Mappings/valid_mappings.netmap")
            {
                Verbose = true
            };
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
            Console.SetOut(textWriter);
            testOutputHelper.WriteLine(stringReader.ToString());
            File.Delete("Resources/Output/Main.exe");
        }
    }
}
