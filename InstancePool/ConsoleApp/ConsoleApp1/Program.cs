using BatchImport.v1;
using BatchImport.v2;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Diagnostics;
using System.IO;

namespace BatchImport
{
    //2 verificar em debug se os numeros postar em 1000 , 5000 e 10000 objetos
    //3 focar no post na diferença entre ter intancia e n ter instancia 
    //4 ver se faz sentido remover o statico, medir o tempo com e sem p ver se posto os 2
    //5 postar a imagem da topologia tb
    [GcServer]
    [GcConcurrent]
    [MemoryDiagnoser]
    public class Comparer
    {
        readonly RefactoredService _refactoredService = new RefactoredService();
        readonly OriginalService _originalService = new OriginalService();
        readonly ContainerDto _container;

        public Comparer()
        {
            _container = ContainerDto.ContainerFactory(File.ReadAllLines("live-courses - 3.csv"));           
        }

        [Benchmark]
        public void ObjectCreation() => _originalService.Validate(_container);

        [Benchmark]
        public void ObjectPooling() => _refactoredService.Validate(_container);

    }

    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<Comparer>();
        }
    }

    //class Program
    //{
    //    static void Main(string[] args)
    //    {
    //        var sw = new Stopwatch();
    //        var before2 = GC.CollectionCount(2);
    //        var before1 = GC.CollectionCount(1);
    //        var before0 = GC.CollectionCount(0);
    //        var comparer = new Comparer();

    //        sw.Start();
    //        for (int i = 0; i < 10_000; i++)
    //        {
    //            //comparer.ObjectCreation();
    //            comparer.ObjectPooling();
    //        }

    //        sw.Stop();

    //        Console.WriteLine($"Tempo total: {sw.ElapsedMilliseconds}ms");
    //        Console.WriteLine($"GC Gen #2 : {GC.CollectionCount(2) - before2}");
    //        Console.WriteLine($"GC Gen #1 : {GC.CollectionCount(1) - before1}");
    //        Console.WriteLine($"GC Gen #0 : {GC.CollectionCount(0) - before0}");
    //        Console.WriteLine("Done!");
    //    }
    //}
}