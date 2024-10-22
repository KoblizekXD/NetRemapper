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

        private static ModuleDefinition PrepareTester(string assemblyName)
        {
            NetRemapper remapper = new($"Resources/{assemblyName}.dll", $"Resources/Mappings/{assemblyName}.netmap")
            {
                Verbose = true
            };
            remapper.Remap($"Resources/Output/{assemblyName}.dll");

            return AssemblyDefinition.ReadAssembly($"Resources/Output/{assemblyName}.dll").MainModule;
        }

        [Fact]
        public void TestFieldRemap()
        {
            ModuleDefinition def = PrepareTester("FieldTest");

            var testingType = def.GetType("FieldTest.Testing");
            var programType = def.GetType("FieldTest.Program");

            Assert.Equal("renamedI", testingType.Fields[0].Name);
            Assert.Equal("renamedStaticI", testingType.Fields[1].Name);

            testingType.Methods.First(md => md.Name == "Calling").Body.Instructions
                .Where(i => i.Operand is FieldDefinition)
                .ToList()
                .ForEach(i =>
                {
                    Assert.True((i.Operand as FieldDefinition)?.Name.StartsWith("renamed"));
                });

            programType.Methods.First(md => md.Name == "Main").Body.Instructions
                .Where(i => i.Operand is FieldDefinition)
                .Select(i => (i.Operand as FieldDefinition)!)
                .ToList()
                .ForEach(f =>
                {
                    f.Name.StartsWith("renamed");
                });

            def.Assembly.Dispose();
        }

        [Fact] // Doesn't use dll yet -> can't test
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
            Directory.Delete("Resources/Output", true);
        }
    }
}
