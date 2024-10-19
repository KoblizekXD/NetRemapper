using Mono.Cecil;
using Mono.Cecil.Cil;

namespace NetRemapper
{
    public class NetRemapper(string assemblyPath, string? mappings)
    {
        public AssemblyDefinition Assembly { get; private set; } = AssemblyDefinition.ReadAssembly(assemblyPath);
        public Mappings? Mappings { get; private set; } = MappingsReader.ReadMappings(mappings);
        public string DefaultNamespace { get; private set; } = "obf";
        public string TargetNamespace { get; private set; } = "named";

        /// <summary>
        ///     Remaps the <see cref="assemblyPath"/> according to the loaded mappings(<see cref="mappings"/>).
        ///     If the mappings are not loaded, an exception is thrown.
        ///     The process will use <see cref="DefaultNamespace"/> as the namespace to remap from and <see cref="TargetNamespace"/> as the namespace to remap to.
        /// </summary>
        /// <param name="output">The path where generate the remapped assembly.</param>
        /// <exception cref="Exception">Thrown when no mappings are loaded.</exception>
        public void Remap(string output)
        {
            if (Mappings is null) throw new Exception("Mappings are not loaded.");

            foreach (TypeDefinition type in Assembly.MainModule.Types)
            {
                foreach (var field in type.Fields)
                {
                    if (Mappings.GetField(field.Name, DefaultNamespace) is FieldDefinitionEntry entry)
                    {
                        field.Name = entry.Names[TargetNamespace];
                    }
                }

                foreach (var method in type.Methods)
                {
                    ModifyMethod(method);
                }
            }
            Assembly.Write(output);
        }

        #pragma warning disable CS8602 // Dereference of a possibly null reference.
        private void ModifyMethod(MethodDefinition method)
        {
            MethodDefinitionEntry? entry = Mappings.GetMethod(method.Name, DefaultNamespace);

            if (entry is not null)
            {
                method.Name = entry.Names[TargetNamespace];
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

        public void ExportMappings(string output)
        {
            if (Mappings is null) throw new Exception("Mappings are not loaded.");

            MappingsWriter.Write(Mappings, output);
        }

        public static void Main()
        {
            var m = MappingsReader.ReadMappings("./maps.netmap");
        }
    }
}
