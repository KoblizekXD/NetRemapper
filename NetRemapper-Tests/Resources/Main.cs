using System;

namespace Hello {

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class CustomAttribute : Attribute
    {

    }

    public static class Creator
    {

        public Creator() {

        }

        public static int count = 1;
        public static string Writable { get; set; } = "Test123";

        [CustomAttribute]
        public static void Create()
        {
            count = 69;
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