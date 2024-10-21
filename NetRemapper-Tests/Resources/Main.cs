using System;

namespace Hello {
    public static class Creator
    {
	public static int count = 1;
	public static string Writable { get; set; } = "Test123";

        public static void Create()
        {

        }
    }

    public static class Program {
        public static void Main(string[] args) {
	    Creator.Writable = "Nope";
            Creator.Create();
            Console.WriteLine(Creator.Writable);
	    Console.WriteLine(Creator.count);
        }
    }
}