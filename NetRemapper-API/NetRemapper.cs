using Mono.Cecil;
using Mono.Cecil.Cil;

namespace NetRemapper;

public class NetRemapper
{
    public NetRemapper(string assemblyPath, string mappings)
    {
        Assembly = AssemblyDefinition.ReadAssembly(assemblyPath);
        Module = Assembly.MainModule;
        Mappings = MappingsReader.ReadMappings(mappings);
    }

    public NetRemapper(Stream stream, string mappings)
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
    public bool Verbose { get; init; }

    public bool MappingsLoaded => Mappings is not null;

    public ModuleDefinition? GetModule(string name)
    {
        return Assembly.Modules.FirstOrDefault(m => m.Name == name);
    }

    /// <summary>
    ///     Remaps the <see cref="Assembly"/> according to the loaded <see cref="Mappings"/>.
    ///     If the mappings are not loaded, an exception is thrown.
    ///     The process will use <see cref="DefaultNamespace"/> as the namespace to remap from and <see cref="TargetNamespace"/> as the namespace to remap to.
    /// </summary>
    /// <param name="output">The path where generate the remapped assembly.</param>
    /// <exception cref="Exception">Thrown when no mappings are loaded.</exception>
    public void Remap(string output)
    {
        if (Mappings is null) throw RemapperException.UnloadedMappings();

        foreach (var type in Module.Types)
        {
            if (Mappings.GetType(type.Name, DefaultNamespace) is { } entry1)
            {
                WriteIfVerbose($"Remapping type {type.Name} -> {entry1[TargetNamespace]}");
                type.Name = entry1.Names[TargetNamespace];
            }

            foreach (var property in type.Properties)
            {
                if (Mappings.GetProperty(property.Name, DefaultNamespace, type.Name, DefaultNamespace) is not 
                    { } entry) continue;
                WriteIfVerbose($"Remapping property {property.Name} -> {entry[TargetNamespace]}");
                property.Name = entry.Names[TargetNamespace];
            }

            foreach (var field in type.Fields)
            {
                if (Mappings.GetField(field.Name, DefaultNamespace, type.Name, DefaultNamespace) is not
                    { } entry) continue;
                WriteIfVerbose($"Remapping field {field.Name} -> {entry[TargetNamespace]}");
                field.Name = entry.Names[TargetNamespace];
            }

            foreach (var method in type.Methods)
            {
                ModifyMethod(method);
            }
        }

        Directory.CreateDirectory(Path.GetDirectoryName(output)!);
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

    private void ModifyMethod(MethodDefinition method)
    {
        MethodDefinitionEntry? entry = Mappings!.GetMethod(method.Name, DefaultNamespace);

        if (entry is not null)
        {
            WriteIfVerbose($"Remapping method {method.Name} -> {entry[TargetNamespace]}");
            method.Name = entry[TargetNamespace];
        }
        
        foreach (var parameter in method.Parameters)
        {
            if (Mappings!.GetType(parameter.ParameterType.Name, DefaultNamespace) is not { } typeDefinition) return;
        
            WriteIfVerbose($"Remapping reference to parameter {parameter.Name}'s type {parameter.ParameterType.Name} -> {typeDefinition[TargetNamespace]}");
            parameter.ParameterType = new TypeReference(parameter.ParameterType.Namespace, typeDefinition.Names[TargetNamespace], parameter.ParameterType.Module, parameter.ParameterType.Scope);
        }

        if (method.HasBody)
        {
            ILProcessor processor = method.Body.GetILProcessor();

            foreach (var instruction in method.Body.Instructions)
            {
                if (instruction.Operand is MemberReference member) RemapMemberReference(member);

                switch (instruction.Operand)
                {
                    case MethodReference mRef:
                        RemapMethodReference(processor, instruction, mRef);
                        break;
                    case FieldReference fRef:
                        RemapFieldReference(processor, instruction, fRef);
                        break;
                }
            }
        }
    }

    private void RemapMemberReference(MemberReference member)
    {
        if (Mappings!.GetType(member.DeclaringType.Name, DefaultNamespace) is not { } typeDefinition) return;
        
        WriteIfVerbose($"Remapping reference to {member.Name}'s type {member.DeclaringType.Name} -> {typeDefinition[TargetNamespace]}");
        member.DeclaringType = new TypeReference(member.DeclaringType.Namespace, typeDefinition.Names[TargetNamespace], member.DeclaringType.Module, member.DeclaringType.Scope);
    }

    private void RemapMethodReference(ILProcessor processor, Instruction instruction, MethodReference mRef)
    {
        if (Mappings!.GetMethod(mRef.Name, DefaultNamespace, mRef.Name, TargetNamespace) is not { } methodEntry) return;
        
        WriteIfVerbose($"Remapping method reference {mRef.Name} -> {methodEntry[TargetNamespace]}");
        mRef.Name = methodEntry.Names[TargetNamespace];
        processor.Replace(instruction, processor.Create(instruction.OpCode, mRef));
    }

    private void RemapFieldReference(ILProcessor processor, Instruction instruction, FieldReference fRef)
    {
        if (Mappings!.GetField(fRef.Name, DefaultNamespace, fRef.Name, TargetNamespace) is not { } fieldEntry) return;
        
        WriteIfVerbose($"Remapping field reference {fRef.Name} -> {fieldEntry[TargetNamespace]}");
        fRef.Name = fieldEntry.Names[TargetNamespace];
        processor.Replace(instruction, processor.Create(instruction.OpCode, fRef));
    }

    public void ExportMappings(string output)
    {
        if (Mappings is null) throw RemapperException.UnloadedMappings();

        MappingsWriter.Write(Mappings, output);
    }
}
