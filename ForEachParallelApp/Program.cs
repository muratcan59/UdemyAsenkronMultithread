using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ForEachParallelApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();
            string picturesPath = @"C:\Users\uzantı";

            var files = Directory.GetFiles(picturesPath);

            //Burada aldığı dizi yapısını(multithread) belirli parçalara ayırıp farklı threadlerde yazılmasını sağlar.
            #region ParallelForEach Asenkron
            sw.Start();

            Parallel.ForEach(files, (item) =>
            {
                Console.WriteLine("thread no:" + Thread.CurrentThread.ManagedThreadId);
                Image img = new Bitmap(item);

                var thumbnail = img.GetThumbnailImage(50, 50, () => false, IntPtr.Zero);
                thumbnail.Save(Path.Combine(picturesPath, "thumbnail", Path.GetFileName(item)));
            });

            sw.Stop();
            Console.WriteLine("İşlem bitti. Harcanan süre:" + sw.ElapsedMilliseconds);
            #endregion

            sw.Reset();

            //Burada aldığı dizi tek bir threadler ile işlem görür.
            #region ParallelForEach Senkron
            sw.Start();

            files.ToList().ForEach(x =>
            {
                Console.WriteLine("thread no:" + Thread.CurrentThread.ManagedThreadId);
                Image img = new Bitmap(x);

                var thumbnail = img.GetThumbnailImage(50, 50, () => false, IntPtr.Zero);
                thumbnail.Save(Path.Combine(picturesPath, "thumbnail", Path.GetFileName(x)));
            });

            sw.Stop();
            Console.WriteLine("İşlem bitti. Harcanan süre:" + sw.ElapsedMilliseconds);
            #endregion

            long FilesByte = 0;
            Parallel.ForEach(files, (item) =>
            {
                Console.WriteLine("thread no:" + Thread.CurrentThread.ManagedThreadId);

                FileInfo f = new FileInfo(item);

                //Interlocked: Sistem ve donanımın özelliklerini kullanarak async / sync nin daha üst performans ile kullanımını sağlar.
                //Interlocked.Increment ile verilen value karşılığının artması sağlar.
                //Interlocked.Decrement ile verilen value karşılığının azalmasını sağlar.
                Interlocked.Add(ref FilesByte, 100);
            });

            Console.WriteLine("Toplam Boyut:" + FilesByte.ToString());

            #region ParallelFor Kullanımı
            long totalByte = 0;

            var filess = Directory.GetFiles(@"C:\Users\uzantı");

            Parallel.For(0, files.Length, (index) =>
            {
                var file = new FileInfo(filess[index]);

                Interlocked.Add(ref totalByte, file.Length);
            });

            Console.WriteLine("Total Byte:" + totalByte.ToString());
            #endregion

            int total = 0;

            Parallel.ForEach(Enumerable.Range(1, 100).ToList(), () => 0, (x, loop, subtotal) =>
            {
                subtotal += x;
                return subtotal;
            }, (y) => Interlocked.Add(ref total, y));

            Console.WriteLine(total);

            Parallel.For(0, 100, () => 0, (x, loop, subtotal) =>
            {
                subtotal += x;
                return subtotal;
            }, (y) => Interlocked.Add(ref total, y));
        }
    }
}
