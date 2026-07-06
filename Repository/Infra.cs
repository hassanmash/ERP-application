namespace Infrastructure
{
    internal class Infra
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            Console.WriteLine(BCrypt.Net.BCrypt.HashPassword("HiliteOSAdmin@1101", workFactor: 12));
        }
    }
}
