using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Reflection;

namespace NetRemapper
{
    public class NetRemapper
    {
        public NetRemapper(string assemblyPath, string? mappings)
        {
            Assembly = AssemblyDefinition.ReadAssembly(assemblyPath);
            Module = Assembly.MainModule;
            Mappings = MappingsReader.ReadMappings(mappings);
        }

        public NetRemapper(Stream stream, string? mappings)
        {
            Assembly = AssemblyDefinition.ReadAssembly(stream);
            Module = Assembly.MainModule;
            Mappings = MappingsReader.ReadMappings(mappings);
        }

        public AssemblyDefinition Assembly { get; private set; }
        public ModuleDefinition Module { get; set; }
        public Mappings? Mappings { get; private set; }
        public string DefaultNamespace { get; set; } = "obf";
        public string TargetNamespace { get; set; } = "named";
        public bool Verbose { get; set; } = false;

        public bool MappingsLoaded => Mappings is not null;

        public ModuleDefinition? GetModule(string name)
        {
            return Assembly.Modules.FirstOrDefault(m => m.Name == name);
        }

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

            foreach (TypeDefinition type in Module.Types)
            {
                if (Mappings.GetType(type.Name, DefaultNamespace) is TypeDefinitionEntry entry1)
                {
                    WriteIfVerbose($"Remapping type {type.Name} -> {entry1[TargetNamespace]}");
                    type.Name = entry1.Names[TargetNamespace];
                }

                foreach (var property in type.Properties)
                {
                    if (Mappings.GetProperty(property.Name, DefaultNamespace, type.Name, DefaultNamespace) is PropertyDefinitionEntry entry)
                    {
                        WriteIfVerbose($"Remapping property {property.Name} -> {entry[TargetNamespace]}");
                        property.Name = entry.Names[TargetNamespace];
                    }
                }

                foreach (var field in type.Fields)
                {
                    if (Mappings.GetField(field.Name, DefaultNamespace, type.Name, DefaultNamespace) is FieldDefinitionEntry entry)
                    {
                        WriteIfVerbose($"Remapping field {field.Name} -> {entry[TargetNamespace]}");
                        field.Name = entry.Names[TargetNamespace];
                    }
                }

                foreach (var method in type.Methods)
                {
                    ModifyMethod(method);
                }
            }

#pragma warning disable CS8604 // Possible null reference argument.
            Directory.CreateDirectory(Path.GetDirectoryName(output));
            Assembly.Write(output);
        }

        void WriteIfVerbose(string message)
        {
            if (Verbose)
            {
                ConsoleColor originalColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("[NetRemapper] " + message);
                Console.ForegroundColor = originalColor;
            }
        }

        #pragma warning disable CS8602 // Dereference of a possibly null reference.
        private void ModifyMethod(MethodDefinition method)
        {
            MethodDefinitionEntry? entry = Mappings.GetMethod(method.Name, DefaultNamespace);

            if (entry is not null)
            {
                WriteIfVerbose($"Remapping method {method.Name} -> {entry[TargetNamespace]}");
                method.Name = entry[TargetNamespace];
            }

            if (method.HasBody)
            {
                ILProcessor processor = method.Body.GetILProcessor();

                foreach (var instruction in method.Body.Instructions)
                {
                    if (instruction.Operand is MemberReference member)
                    {
                        if (Mappings.GetType(member.DeclaringType.Name, DefaultNamespace) is TypeDefinitionEntry typeDefinition)
                        {
                            WriteIfVerbose($"Remapping reference to {member.Name}'s type {member.DeclaringType.Name} -> {typeDefinition[TargetNamespace]}");
                            member.DeclaringType = new(member.DeclaringType.Namespace, typeDefinition.Names[TargetNamespace], member.DeclaringType.Module, member.DeclaringType.Scope);
                            // member.DeclaringType.Name = typeDefinition.Names[TargetNamespace];
                        }
                    }

                    if (instruction.Operand is MethodReference mRef)
                    {
                        if (Mappings.GetMethod(mRef.Name, DefaultNamespace, mRef.Name, TargetNamespace) is MethodDefinitionEntry e)
                        {
                            WriteIfVerbose($"Remapping method reference {mRef.Name} -> {e[TargetNamespace]}");
                            mRef.Name = e.Names[TargetNamespace];
                            processor.Replace(instruction, processor.Create(instruction.OpCode, mRef));
                        }
                    } else if (instruction.Operand is FieldReference fRef)
                    {
                        if (Mappings.GetField(fRef.Name, DefaultNamespace, fRef.Name, TargetNamespace) is FieldDefinitionEntry e)
                        {
                            WriteIfVerbose($"Remapping field reference {fRef.Name} -> {e[TargetNamespace]}");
                            fRef.Name = e.Names[TargetNamespace];
                            processor.Replace(instruction, processor.Create(instruction.OpCode, fRef));
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
    }
}
