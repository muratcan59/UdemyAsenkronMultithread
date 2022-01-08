using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TaskConsoleApp
{
    public class Content
    {
        public string Site { get; set; }
        public int Len { get; set; }
    }

    public class Status
    {
        public int ThreadId { get; set; }
        public DateTime Date { get; set; }
    }

    //Console'da aktif etmek için Main1, Main2 gibi metotları Main olarak düzenlemek yeterli.
    class Program
    {
        #region ContinueWith Kullanımı
        public static void Calis(Task<string> data)
        {
            Console.WriteLine("data uzunluk: " + data.Result.Length);
            //100 satırlık bir kod varsa
        }

        async static Task Main1(string[] args)
        {
            Console.WriteLine("Hello World!");

            //ContinueWith: async olarak yapılan işlemin hemen ardından ek bir işlem yapılmak için kullanılır.

            #region ContinueWith 1.Yöntem
            //var myTask = new HttpClient().GetStringAsync("https://www.google.com").ContinueWith((data) =>
            //{
            //    Console.WriteLine("data uzunluk: " + data.Result.Length);
            //});
            //Console.WriteLine("Arada yapılacak işler");
            //await myTask;
            #endregion

            #region ContinueWith 2.Yöntem
            //var myTask = new HttpClient().GetStringAsync("https://www.google.com");
            //Console.WriteLine("Arada yapılacak işler");
            //var data = await myTask;
            //Console.WriteLine("data uzunluk: " + data.Length);
            #endregion

            #region ContinueWith 3.Yöntem
            var myTask = new HttpClient().GetStringAsync("https://www.google.com").ContinueWith(Calis);

            Console.WriteLine("Arada yapılacak işler");

            await myTask;
            #endregion
        }
        #endregion

        #region WhenAll, WhenAny, WaitAll, WaitAny, Delay Kullanımı
        async static Task Main2(string[] args)
        {
            Console.WriteLine("Main Thread: " + Thread.CurrentThread.ManagedThreadId);
            List<string> urlsList = new List<string>()
            {
                "https://www.google.com",
                "https://www.microsoft.com",
                "https://www.amazon.com",
                "https://www.n11.com",
                "https://www.haberturk.com"
            };

            List<Task<Content>> taskList = new List<Task<Content>>();

            urlsList.ToList().ForEach(x =>
            {
                taskList.Add(GetContentAsync(x));
            });

            //WhenAll: Birden fazla Task içeriğini bir liste/dizi şeklinde getirilmesini sağlar.
            #region WhenAll 1.Yöntem
            //var contents = await Task.WhenAll(taskList.ToArray());
            //
            //contents.ToList().ForEach(x =>
            //{
            //    Console.WriteLine($"{x.Site} boyut:{x.Len}");
            //});
            #endregion

            #region WhenAll 2.Yöntem
            var contents = Task.WhenAll(taskList.ToArray());

            Console.WriteLine("WhenAll methodundan sonra başka işler yaptım.");

            var data = await contents;

            data.ToList().ForEach(x =>
            {
                Console.WriteLine($"{x.Site} boyut:{x.Len}");
            });
            #endregion

            //WhenAny: Birden fazla Task içeriğinden en kısa sürede cevap alınanı getirilmesini sağlar.
            #region WhenAny
            var firstData = await Task.WhenAny(taskList);

            Console.WriteLine($"{firstData.Result.Site} - {firstData.Result.Len}");
            #endregion

            //WaitAll: Birden fazla Task içeriklerin işlemini bitirip farklı işleme geçiş yapılmasını sağlar.
            //Bu metotta belirlenen süre içerisinde işlemde bitirilebilir.
            #region WaitAll 1.Yöntem
            //Console.WriteLine("WaitAll metodundan önce");
            //Task.WaitAll(taskList.ToArray());
            //Console.WriteLine("WaitAll metodundan sonra");
            //Console.WriteLine($"{taskList.First().Result.Site} - {taskList.First().Result.Len}");
            #endregion

            #region WaitAll 2.Yöntem
            Console.WriteLine("WaitAll metodundan önce");
            bool result = Task.WaitAll(taskList.ToArray(), 3000);
            Console.WriteLine("3 saniyede geldi mi: " + result); //Sonuç True
            bool result1 = Task.WaitAll(taskList.ToArray(), 300);
            Console.WriteLine("3 saniyede geldi mi: " + result1); //Sonuç False
            Console.WriteLine("WaitAll metodundan sonra");

            Console.WriteLine($"{taskList.First().Result.Site} - {taskList.First().Result.Len}");
            #endregion

            //WaitAny: Birden fazla Task içeriklerinden ilk işlemi bitireni getirilmesini sağlar.
            #region WaitAny
            var firstTaskIndex = Task.WaitAny(taskList.ToArray());

            Console.WriteLine($"{taskList[firstTaskIndex].Result.Site} - {taskList[firstTaskIndex].Result.Len}");
            #endregion
        }

        public static async Task<Content> GetContentAsync(string url)
        {
            Content c = new Content();
            var data = await new HttpClient().GetStringAsync(url);

            //Delay: Bir task işlemini verilen süre kadar bekletilmesini sağlar.
            await Task.Delay(5000);

            c.Site = url;
            c.Len = data.Length;
            Console.WriteLine("GetContentAsync thread: " + Thread.CurrentThread.ManagedThreadId);

            return c;
        }
        #endregion

        #region StartNew Kullanımı
        async static Task Main3(string[] args)
        {
            //StartNew: Run methodu gibi çalışır.
            //Arasındaki farkı geriye nesne oluşturulabilir bir yapıya sahiptir.
            var myTask = Task.Factory.StartNew((Obj) =>
            {
                Console.WriteLine("myTask çalıştı");

                var status = Obj as Status;
                status.ThreadId = Thread.CurrentThread.ManagedThreadId;
            }, new Status() { Date = DateTime.Now });

            await myTask;

            //AsyncState: StartNew ile Task içinde oluşturulan nesne çağrılır.
            Status s = myTask.AsyncState as Status;

            Console.WriteLine(s.Date);
            Console.WriteLine(s.ThreadId);
        }
        #endregion

        #region FromResult Kullanımı
        public static string CacheData { get; set; }

        async static Task Main4(string[] args)
        {
            CacheData = await GetDataAsync();
        }

        public static Task<string> GetDataAsync()
        {
            //FromResult: Cache(Bellek)'te tutulan datayı getirmeyi sağlar.
            if (String.IsNullOrEmpty(CacheData))
                return File.ReadAllTextAsync("dosya.txt");
            else
                return Task.FromResult(CacheData);
        }
        #endregion

        #region Result Kullanımı
        private async static Task Main5(string[] args)
        {
            Console.WriteLine(GetData());
        }

        public static string GetData()
        {
            //Result: datanın anlık dönen bilgisi getirir.
            //Bloklama yapılarak istenilen data alınabilir.
            var task = new HttpClient().GetStringAsync("https://www.google.com");

            return task.Result;
        }
        #endregion

        #region ValueTask Kullanımı
        public static int cacheData { get; set; } = 150;

        private async static Task Main(string[] args)
        {
            var myTask = GetValueData();
        }

        public static ValueTask<int> GetValueData()
        {
            return new ValueTask<int>(cacheData);
        }
        #endregion
    }
}
