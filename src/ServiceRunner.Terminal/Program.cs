using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ServiceRunner.Terminal.Tests")]

namespace ServiceRunner.Terminal
{
    internal class Program
    {
        internal static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}