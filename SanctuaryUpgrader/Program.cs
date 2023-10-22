using System;

namespace SanctuaryUpgrader
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Publishing...");
            bool fFull = false;
            bool f = Publisher.PublishWebProject(fFull).Result;
        }
    }
}
