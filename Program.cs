namespace lb11
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;

    class Program
    {
        static async Task Main(string[] args)
        {
            string url1 = "https://docs.yandex.ru/docs/view?url=ya-disk%3A%2F%2F%2Fdisk%2FМетодические%20указания%20к%20лабораторным%20работам%2FМДК.01.02%2F2курс%2FЛабораторные%20работы%2FЛабораторная%20работа%20№7-8%20-%20Системное%20тестирование.docx&name=Лабораторная%20работа%20№7-8%20-%20Системное%20тестирование.docx&uid=1611365357&nosw=1";
            string outputPath2 = "C:\\Users\\artem\\OneDrive\\Рабочий стол\\1.txt";
            string url2 = "https://get.wallhere.com/photo/landscape-mountains-lake-nature-reflection-grass-sky-river-national-park-valley-wilderness-Alps-tree-autumn-leaf-mountain-season-tarn-loch-mountainous-landforms-mountain-range-590185.jpg"; // URL изображения
            string filePath2 = "C:\\Users\\artem\\OneDrive\\Рабочий стол\\rrr.bmp";
            string audiopath1 = "C:\\Users\\artem\\OneDrive\\Рабочий стол\\хорус.wav";
            string play1 = "C:\\Program Files\\Windows Media Player\\wmplayer.exe";
            string url3 = "https://rus.hitmotop.com/song/73390019";
            string outputpath = "C::\\Users\\artem\\ATL.mp3";
            Stopwatch stopWatch1 = new Stopwatch();
            Stopwatch stopWatch2 = new Stopwatch();
            Stopwatch stopWatch3 = new Stopwatch();
            Stopwatch stopWatch4 = new Stopwatch();
            Console.WriteLine("Запуск аудиозаписи");
            stopWatch3.Start();
            await AudioStartAsync(play1, audiopath1);
            stopWatch3.Stop();
            Console.WriteLine("Скачивание файлов запущено...");
            if (GetFileType(url1, outputPath2))
            {
                stopWatch1.Start();
                await DownloadFileAsync(url1, outputPath2);
                stopWatch1.Stop();
            }
            else Console.WriteLine("Файл не удалось скачать. Несоответсвие типов");
            if (GetFileType(url2, filePath2))
            {
                stopWatch2.Start();
                await DownloadImageAsync(url2, filePath2);
                stopWatch2.Stop();
            }
            else Console.WriteLine("Файл не удалось скачать. Несоответсвие типов");
            if (GetFileType(url3, outputpath))
            {
                stopWatch4.Start();
                await DownloadAudioAsync(url3, outputpath);
                stopWatch4.Stop();
            }
            else Console.WriteLine("Файл не удалось скачать. Несоответсвие типов");
            Console.WriteLine("Скачивание завершено.");
            Console.WriteLine("Время выполнения первого скачивания и сохранения: " + stopWatch1.Elapsed.ToString());
            Console.WriteLine("Время выполнения второго скачивания и сохранения: " + stopWatch2.Elapsed.ToString());
            Console.WriteLine("Время выполнения третьего скачивания и сохранения: " + stopWatch4.Elapsed.ToString());
            Console.WriteLine("Время выполнения запуска проигрывания аудиозаписи: " + stopWatch3.Elapsed.ToString());
        }

        static bool GetFileType(string filePath, string outputpath)
        {
            try
            {
                byte[] header = new byte[4];
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    stream.Read(header, 0, header.Length);
                }
                byte[] header2 = new byte[4];
                using (var stream1 = new FileStream(outputpath, FileMode.Open, FileAccess.Read))
                {
                    stream1.Read(header2, 0, header2.Length);
                }

                if ((header[0] == 0xEF && header[1] == 0xBB && header[2] == 0xBF) == (header2[0] == 0xEF && header2[1] == 0xBB && header2[2] == 0xBF)) // UTF-8 BOM
                    return true;

                // Проверка на аудиофайлы (MP3)
                if ((header[0] == 0xFF && header[1] == 0xFB) == (header2[0] == 0xFF && header2[1] == 0xFB))
                    return true;

                // Проверка на изображения (JPEG)
                if ((header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF) == (header2[0] == 0xFF && header2[1] == 0xD8 && header2[2] == 0xFF))
                    return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }
        static async Task DownloadAudioAsync(string url, string outputPath)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    byte[] fileBytes = await httpClient.GetByteArrayAsync(url);
                    await File.WriteAllBytesAsync(outputPath, fileBytes);
                    Console.WriteLine($"Аудиофайл сохранен как {outputPath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при скачивании файла: {ex.Message}");
                }
            }
        }
        static async Task AudioStartAsync(string play, string audiopath)
        {
            try
            {
                if (File.Exists(audiopath))
                {
                    await Task.Run(() =>
                    Process.Start(play, audiopath));
                }
                else
                {
                    Console.WriteLine("Аудиофайл для запуска не найден: " + audiopath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }
        }
        static async Task DownloadFileAsync(string url, string outputPath)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    byte[] fileBytes = await httpClient.GetByteArrayAsync(url);
                    await File.WriteAllBytesAsync(outputPath, fileBytes);
                    Console.WriteLine($"Файл сохранен как {outputPath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при скачивании файла: {ex.Message}");
                }
            }
        }
        static async Task DownloadImageAsync(string url, string filePath)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    using (HttpResponseMessage response = await httpClient.GetAsync(url))
                    {
                        response.EnsureSuccessStatusCode(); // Проверка ответа на успешность

                        using (Stream stream = await response.Content.ReadAsStreamAsync())
                        using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            await stream.CopyToAsync(fileStream); // Копирование потока в файл
                        }
                        Console.WriteLine($"Картинка сохранена как {filePath}");
                    }
                }

                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при скачивании файла: {ex.Message}");
                }
            }
        }
    }
}
