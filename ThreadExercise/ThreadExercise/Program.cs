using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;

internal class Program
{
    static ConcurrentQueue<string> kelimeListesi = new ConcurrentQueue<string>(); //ConcurrentQueue Thread-safe çalıştığı için listemi ConcurrentQueue ile yaptım.

    private static void Main(string[] args)
    {
        Console.Write("Thread sayısını girin: ");
        int threadSayisi = Convert.ToInt32((Console.ReadLine())); //Kaç thread kullanacağımızı belirtmek için Readline kullanıyoruz gelen değerin her halükarda int olması içinde dönüştürme işlemi yapıyorum.

        Thread okumaThread = new Thread(OkuVeEkle); // Okuma yapacak ayrı thread oluşturma.
        okumaThread.Start();

        Thread[] yazdiranThread = new Thread[threadSayisi]; //Sayını belirttiğimiz threadleri oluşturup her threade Yazdırma görevini atayıp çalıştırıyorum.
        for (int i = 0; i < threadSayisi; i++)
        {
            yazdiranThread[i] = new Thread(Yazdir);
            yazdiranThread[i].Start();
        }

    }

    public static void OkuVeEkle() // Okuma işlemlerini yapacağım metot.
    {
        string[] dosyaOkuma = File.ReadAllLines("dosya.txt"); //Dışarıdan alınacak dosyanın içerisindekileri verileri okuyabilmek için ReadAllLines metodunu kullanıyorum.

        foreach (string d in dosyaOkuma)
        {
            string[] kelimeler = d.Split(' '); // Dosya içerisinde foreach ile gezerken kelimeleri ayırt edebilmek için split metodunu kullanıyoruz her boşluktan sonra yeni bir kelime, kelimeler dizisine atılıyor.
            foreach (string kelime in kelimeler)
            {
                kelimeListesi.Enqueue(kelime); //Dosyadan aldığımız kelimeleri ConcurrentQueue'ya ekliyorum.
            }
            Thread.Sleep(3000); //Yavaş çalışıyor süsü vermek için sleep kullanıyorum.
        }
    }

    public static void Yazdir() // Yazdırma işlemlerini yapacağım metot
    {
        while (true)
        {
            string kelime;
            if (kelimeListesi.TryDequeue(out kelime)) //Burda TryDequeue sayesinde kelimeListesinden alınan eleman kelime değişkenine aktarılıp kelimeListesinden çıkarılır. Eğer bunu yapmazsak görüntülerken önceden eklenmiş olan elemanları yine gösteriyor.
            {
                Console.WriteLine($"{kelime} : {kelime.Length}"); //Kelime ismini ve boyutunu yazdırıyorum.

            }

            Thread.Sleep(3000);//Yavaş çalışıyor süsü vermek için sleep kullanıyorum.
        }
    }
}
