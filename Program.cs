using System.Drawing;
using System.Drawing.Imaging;
using Accord.Math;
using System.Numerics;
using static System.Runtime.InteropServices.JavaScript.JSType;
using String = System.String;
using static TestFFT2D.MathTools;
using static TestFFT2D.FourierTransform;

namespace TestFFT2D
{
    internal partial class Program
    { 
        static void Main(string[] args)
        {
            // Путь к изображению
            string imagePath = "C:\\Users\\ASUS\\source\\repos\\TestFFT2D\\Interferogramma.jpg";
            string imagePathOut = "C:\\Users\\ASUS\\Pictures\\Interferogramma_out.jpg";

            // загружаем изображение из файла
            GrayScaleImageInfo imageInfo = ImagesTools.LoadGrayScaleImage(imagePath);

            double[,] grayImage = imageInfo.grayImage;
            int width = imageInfo.width;
            int height = imageInfo.height;
            
            // создаем массив комплексных чисел
            Complex[][] complexes = new Complex[height][];
            for (int y = 0; y < height; y++)
            {
                complexes[y] = new Complex[width];
            }
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    complexes[x][y] = new Complex(grayImage[x, y], 0);
                }
            }
            
            // прямое фурье-преобразование
            FFT2(complexes, FourierTransform.Direction.Forward);
            
            // заполняем левую половину нулями
            const double grayscale = 0;
            for (int x = 0; x < width/2; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    complexes[x][y] = new Complex(grayscale, grayscale);
                }
            }
            
            // обратное преобразование фурье
            FourierTransform.FFT2(complexes, FourierTransform.Direction.Backward);
            
            // делим мнимую чатсь на действительную
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    grayImage[x, y] = complexes[x][y].Imaginary / complexes[x][y].Real;
                }
            }
            
            // считаем арктангенс
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    grayImage[x, y] = Math.Atan(grayImage[x, y]);
                }
            }
            
            // нормализуем значения от -PI до +PI
            normalize(grayImage);
            denormalize(grayImage, -1 * Math.PI, Math.PI);
            
            // выполняем развёртку
            unwrapMassive(grayImage, width);
            
            // переводим в ангстремы
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    const double waveLength = 6328;
                    grayImage[x, y] = (grayImage[x, y] * waveLength) / (4 * Math.PI);
                }
            }
            
            // подготовка к выводу изображения
            // нормализуем данные
            normalize(grayImage, 1, 1, width-1, height-1);
            
            // конвертируем нормализованное значение в 0 - 255
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    grayImage[x, y] *= 255;
                }
            }

            // считаем каким должен быть градиент
            double[] trendMassive = new double[width];

            for (int i = 0; i < width; i++)
            {
                double k = (((double)(width - 1 - i) / (double) (width - 1)));
                trendMassive[i] = 255 * k;
            }

            // вычитаем из градиента получившееся значение
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    grayImage[x, y] = trendMassive[x] - grayImage[x, y];
                }
            }

            normalize(grayImage, 1, 1, width-1, height-1);
            
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    grayImage[x, y] = Math.Abs(255 - (grayImage[x, y] * 255));
                }
            }

            // зануляем граничные значения
            for (int i = 0; i < width; i++)
            {
                grayImage[0, i] = 0;      // верхняя горизонтальная линия
                grayImage[width-1, i] = 0;    // нижняя горизонтальная линия
                grayImage[i, 0] = 0;      // левая вертикальная линия
                grayImage[i, height-1] = 0;    // правая вертикальная линия
            }
            
            // сохраняем изображение
            GrayScaleImageInfo imageInfoOut = new GrayScaleImageInfo();
            imageInfoOut.grayImage = grayImage;
            imageInfoOut.width = width;
            imageInfoOut.height = height;

            ImagesTools.SaveGrayScaleImage(imagePathOut, imageInfoOut);

            Console.WriteLine("Изображение сохранено в файл: " + imagePathOut);
        }
    }
}