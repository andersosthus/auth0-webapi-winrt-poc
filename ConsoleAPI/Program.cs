using System;
using Microsoft.Owin.Hosting;

namespace ConsoleAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = WebApp.Start<Startup>("http://localhost:3000/");
            Console.WriteLine("Web API listening at http://localhost:3000/");
            Console.WriteLine("Press ENTER to terminate");
            Console.ReadLine(); 
        }
    }
}
