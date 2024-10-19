using Mono.Cecil;
using Mono.Cecil.Cil;

namespace NetRemapper
{
    public class NetRemapper(string assemblyPath, string? mappings)
    {
        public AssemblyDefinition Assembly { get; private set; } = AssemblyDefinition.ReadAssembly(assemblyPath);
        public Mappings? Mappings { get; private set; } = MappingsReader.ReadMappings(mappings);

        public void Remap(string from, string target)
        {
            foreach (TypeDefinition type in Assembly.MainModule.Types)
            {
                foreach (var method in type.Methods)
                {
                    if (method.Name == "Create")
                    {
                        method.Name = "NamedCreate";
                    }

                    if (method.HasBody)
                    {
                        ILProcessor processor = method.Body.GetILProcessor();

                        foreach (var instruction in method.Body.Instructions)
                        {

                            if (instruction.OpCode == OpCodes.Call || instruction.OpCode == OpCodes.Callvirt)
                            {
                                MethodReference? calledMethod = instruction.Operand as MethodReference;

                                if (calledMethod is not null && calledMethod.Name == "Create")
                                {
                                    calledMethod.Name = "NamedCreate";

                                    processor.Replace(instruction, processor.Create(instruction.OpCode, calledMethod));
                                }
                            }
                        }
                    }
                }
            }
            Assembly.Write("C:\\Users\\Koblizkac\\Desktop\\New.exe");
        }

        public static void Main()
        {
            NetRemapper remapper = new("C:\\Users\\Koblizkac\\Desktop\\Main.exe", "C:\\Users\\Koblizkac\\Documents\\Dev\\CXX\\NetRemapper\\NetRemapper-API\\mappings.netmap");
            remapper.Remap("obf", "named");
        }
    }
}
