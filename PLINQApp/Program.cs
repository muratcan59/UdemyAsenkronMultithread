using PLINQApp.Models;
using System;
using System.Linq;
using System.Threading;

namespace PLINQApp
{
    internal class Program
    {
        private static bool Islem(int x)
        {
            return x % 2 == 0;
        }

        private static void WriteLog(Product p)
        {
            Console.WriteLine(p.Name + "log'a kaydedildi");
        }

        static void Main(string[] args)
        {
            var array = Enumerable.Range(1, 100).ToList();

            var newArray = array.AsParallel().Where(Islem);
            //var newArray = array.AsParallel().Where(x => x % 2 == 0);

            array.Where(x => x % 2 == 0).ToList().ForEach(x =>
            {
                Console.WriteLine(x);
            });

            newArray.ToList().ForEach(x =>
            {
                Console.WriteLine(x);
            });

            newArray.ForAll(x =>
            {
                Thread.Sleep(500);
                Console.WriteLine(x);
            });

            AdventureWorks2017Context context = new AdventureWorks2017Context();

            context.Product.Take(10).ToList().ForEach(x =>
            {
                Console.WriteLine(x.Name);
            });

            var product = (from p in context.Product.AsParallel()
                           where p.ListPrice > 10M
                           select p).Take(10);

            product.ForAll(x =>
            {
                Console.WriteLine(x.Name);
            });

            product.AsParallel().Where(x => x.Name.StartsWith('a'));

            //var product2 = context.Product.AsParallel().Where(p => p.ListPrice > 10M).Take(10);
            context.Product.AsParallel().ForAll(p =>
            {
                WriteLog(p);
            });

            //Senkron
            //var product = (from p in context.Product
            //               where p.ListPrice > 10M
            //               select p).Take(10);

            //product.ToList().ForEach(x =>
            //{
            //    Console.WriteLine(x);
            //});

            //WithDegreeOfParallelism: Kaç işlemcide çalışacağı belirtilir.
            context.Product.AsParallel().WithDegreeOfParallelism(1).ForAll(p =>
            {
                WriteLog(p);
            });

            //WithExecutionMode: Yazılan sorgularda paralel yapılıp yapılmayacağını verilen duruma göre belirtir.
            context.Product.AsParallel().WithExecutionMode(ParallelExecutionMode.ForceParallelism).ForAll(p =>
            {
                WriteLog(p);
            });

            //AsOrdered: Veritabanındaki sıralamaya göre getirir.
            context.Product.AsParallel().AsOrdered().Where(p => p.ListPrice > 10M).ToList().ForEach(x =>
            {
                Console.WriteLine($"{x.Name} - {x.ListPrice}");
            });
        }
    }
}
