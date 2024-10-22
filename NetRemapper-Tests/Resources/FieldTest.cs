// Test remapping of field usage, declaration, assignment, definition etc.

using System;

namespace FieldTest;

class Testing
{
    public int i;
    public static int staticI;

    public void Calling()
    {
        i = 1;
        staticI = 2;
    }
}

class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine(Testing.staticI);
        Testing.staticI = 3;
        Console.WriteLine(Testing.staticI);

        var t = new Testing();
        Console.WriteLine(t.i += 1);
        Console.WriteLine(t.i);
        t.Calling();
    }
}