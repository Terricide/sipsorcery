using System;
using System.Data;
using System.IO;
using System.Text;
using WasmerSharp;
using WebAssembly.Instructions;
//using WebAssembly; // Acquire from https://www.nuget.org/packages/WebAssembly
//using WebAssembly.Instructions;
//using WebAssembly.Runtime;

// We need this later to call the code we're generating.
public abstract class Sample
{
    // Sometimes you can use C# dynamic instead of building an abstract class like this.
    public abstract int Demo(int value);
}

static class Program
{
    static void Main()
    {
        //var tbl = new FunctionTable(2);
        //var instance = Compile.FromBinary<dynamic>(Environment.CurrentDirectory + "\\ogv-decoder-audio-vorbis-wasm.wasm")(new ImportDictionary());
        // This creates a memory block with a minimum of 256 64k pages
        // and a maxium of 256 64k pages
        var memory = Memory.Create(minPages: 256, maxPages: 256);

        //
        // Creates the imports for the instance
        //
        var func = new Import("env", "_print_str",
            new ImportFunction((PrintDel)(Print)));

        //Module.asmLibraryArg = { "abort": abort, "assert": assert, "enlargeMemory": enlargeMemory, "getTotalMemory": getTotalMemory, "setTempRet0": setTempRet0, "getTempRet0": getTempRet0, "abortOnCannotGrowMemory": abortOnCannotGrowMemory, "invoke_dd": invoke_dd, "invoke_dddd": invoke_dddd, "invoke_i": invoke_i, "invoke_ii": invoke_ii, "invoke_iii": invoke_iii, "invoke_iiii": invoke_iiii, "invoke_iiiiii": invoke_iiiiii, "invoke_iiiiiiii": invoke_iiiiiiii, "invoke_iiiiiiiii": invoke_iiiiiiiii, "invoke_iiiij": invoke_iiiij, "invoke_iiiijj": invoke_iiiijj, "invoke_jiiiiii": invoke_jiiiiii, "invoke_vi": invoke_vi, "invoke_vii": invoke_vii, "invoke_viii": invoke_viii, "invoke_viiii": invoke_viiii, "invoke_viiiii": invoke_viiiii, "invoke_viiiiii": invoke_viiiiii, "invoke_viiiiiiiii": invoke_viiiiiiiii, "invoke_vijjjid": invoke_vijjjid, "___setErrNo": ___setErrNo, "___syscall140": ___syscall140, "___syscall146": ___syscall146, "___syscall54": ___syscall54, "___syscall6": ___syscall6, "__exit": __exit, "_abort": _abort, "_emscripten_memcpy_big": _emscripten_memcpy_big, "_exit": _exit, "_gettimeofday": _gettimeofday, "_llvm_log10_f32": _llvm_log10_f32, "_llvm_log10_f64": _llvm_log10_f64, "_longjmp": _longjmp, "_pthread_cond_destroy": _pthread_cond_destroy, "_pthread_cond_init": _pthread_cond_init, "_pthread_cond_signal": _pthread_cond_signal, "_pthread_cond_wait": _pthread_cond_wait, "_pthread_create": _pthread_create, "_pthread_join": _pthread_join, "_pthread_mutex_destroy": _pthread_mutex_destroy, "_pthread_mutex_init": _pthread_mutex_init, "_pthread_once": _pthread_once, "_sched_yield": _sched_yield, "_sem_destroy": _sem_destroy, "_sem_init": _sem_init, "_sem_post": _sem_post, "_sem_wait": _sem_wait, "_sysconf": _sysconf, "flush_NO_FILESYSTEM": flush_NO_FILESYSTEM, "DYNAMICTOP_PTR": DYNAMICTOP_PTR, "tempDoublePtr": tempDoublePtr, "STACKTOP": STACKTOP, "STACK_MAX": STACK_MAX };

        //
        // Creates the imports for the instance
        //
        var abort = new Import("env", "abort",
            new ImportFunction((AbortDel)(Abort)));

        //
        // Creates the imports for the instance
        //
        var enlargeMemory = new Import("env", "enlargeMemory",
            new ImportFunction((EnlargeMemoryDel)(EnlargeMemory)));

        // Now we surface the memory as an import
        var memoryImport = new Import("env", "memory", memory);

        // We load a webassembly file
        var wasm = File.ReadAllBytes("libffmpeg.wasm");

        // Now we create an instance based on the WASM file, and the memory provided:
        var instance = new Instance(wasm, memoryImport, func, abort, enlargeMemory);

        // And now you can invoke some code from WebAssembly:
        var ret = instance.Call("hello_world");
        if (ret == null)
        {
            Console.WriteLine("Error calling the method hello_world, status:" + instance.LastError);
        }
        else
        { 
            Console.WriteLine("The method returned: " + ret);
        }
    }

    public static int PAGE_SIZE = 16384;
    public static int WASM_PAGE_SIZE = 65536;
    public static int ASMJS_PAGE_SIZE = 16777216;
    public static int MIN_TOTAL_MEMORY = 16777216;

    public delegate void PrintDel(InstanceContext ctx, int ptr, int len);
    public delegate void AbortDel(InstanceContext ctx, int ptr);

    public static void Abort(InstanceContext ctx, int ptr)
    {
        Console.WriteLine(".NET abort called");        
    }

    // This method is invoked by the WebAssembly code.
    public static void Print(InstanceContext ctx, int ptr, int len)
    {
        Console.WriteLine(".NET Print called");
        var memoryBase = ctx.GetMemory(0).Data;
        unsafe
        {
            var str = Encoding.UTF8.GetString((byte*)memoryBase + ptr, len);

            Console.WriteLine("Received this utf string: [{0}]", str);
        }
    }

    public delegate int EnlargeMemoryDel(InstanceContext ctx);
    public static int EnlargeMemory(InstanceContext ctx)
    {
        return 0;
    }
}