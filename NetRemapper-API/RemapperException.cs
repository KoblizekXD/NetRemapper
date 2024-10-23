namespace NetRemapper;

public class RemapperException : Exception
{
	public RemapperException() { }
	public RemapperException(string message) : base(message) { }
	public RemapperException(string message, Exception inner) : base(message, inner) { }
	
	public static RemapperException UnloadedMappings() => new("Mappings are not loaded.");
}