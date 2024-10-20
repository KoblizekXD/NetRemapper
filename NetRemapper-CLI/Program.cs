using CommandLine;
using NetRemapper;

class Program
{

    class Options
    {
        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.", Default = false)]
        public bool Verbose { get; set; }

        [Option('m', "mappings", Required = true, HelpText = "Mappings file required to remap the source assembly")]
        public string Mappings { get; set; }

        [Option('s', "source", Required = true, HelpText = "Source assembly to remap")]
        public string Source { get; set; }

        [Option('o', "output", Required = true, HelpText = "Output path for the remapped assembly")]
        public string Output { get; set; }

        [Option('s', "sourceNamespace", Required = false, HelpText = "Namespace to remap in the source assembly", Default = "obf")]
        public string SourceNamespace { get; set; }

        [Option('t', "targetNamespace", Required = false, HelpText = "Namespace to remap in the target assembly", Default = "named")]
        public string TargetNamespace { get; set; }
    }

    static void Main(string[] args)
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.Write("NetRemapper");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write(" CLI");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(" v1");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("Remap .NET Assemblies with ease.");
        Console.ResetColor();

        Parser.Default.ParseArguments<Options>(args)
            .WithParsed<Options>(opts => RunOptions(opts))
            .WithNotParsed<Options>((errs) => HandleParseError(errs));
    }

    static void RunOptions(Options opts)
    {
        
    }

    static void HandleParseError(IEnumerable<Error> errs)
    {
        foreach (var error in errs)
        {
            
        }
    }
}