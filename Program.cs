using TestFFT2D.Models;
using TestFFT2D.Tools;

namespace TestFFT2D
{
    internal class Program
    { 
        static void Main(string[] args)
        {
            // пути к изображению
            string imagePath = "C:\\Users\\ASUS\\source\\repos\\TestFFT2D\\Interferogramma.jpg";
            string imagePathOut = "C:\\Users\\ASUS\\Pictures\\Interferogramma_out.jpg";

            // преобразование
            GrayScaleImageInfo imageInfo = ImagesTools.LoadGrayScaleImage(imagePath);   // загружаем изображение из файла
            HilbertTransform.Transform(imageInfo);      // преобразование Гильберта
            ImagesTools.ConvertAngstromToImage(imageInfo);  // конвертация данных для вывода на экран
            
            // сохранение изображения
            ImagesTools.SaveGrayScaleImage(imagePathOut, imageInfo);     // сохраняем изображение
            Console.WriteLine("Изображение сохранено в файл: " + imagePathOut);
        }
    }
}