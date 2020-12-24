using System;
using Wasmtime;

namespace WasmTest2
{
    class Program
    {
        static void Main(string[] args)
        {
            using var engine = new Engine();
            using var module = Module.FromTextFile(engine, "ffmpeg-webm.wat");

            using var host = new Host(engine);
            //host.DefineFunction("", "DYNAMICTOP_PTR", () => Console.WriteLine("hello world"));
            using dynamic instance = host.Instantiate(module);

            Console.WriteLine($"gcd(27, 6) = {instance.gcd(27, 6)}");
        }
    }
}
