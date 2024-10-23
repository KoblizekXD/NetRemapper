
namespace NetRemapper;


[Serializable]
public class RemapperException : Exception
{
	public RemapperException() { }
	public RemapperException(string message) : base(message) { }
	public RemapperException(string message, Exception inner) : base(message, inner) { }

	public static RemapperException UnloadedMappings() => new RemapperException("Mappings are not loaded.");
}