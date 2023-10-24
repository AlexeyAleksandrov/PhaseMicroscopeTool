using System.Drawing;
using TestFFT2D.Models;
using static TestFFT2D.Tools.MathTools;

namespace TestFFT2D.Tools;

public static class ImagesTools
{
    public static GrayScaleImageInfo LoadGrayScaleImage(string imagePath)
    {
        // Загрузка изображения
        Bitmap originalImage = new Bitmap(imagePath);

        // Получение ширины и высоты изображения
        int width = originalImage.Width;
        int height = originalImage.Height;

        // Создание двумерного массива для градаций серого
        double[,] grayImage = new double[width, height];

        // Преобразование изображения в градации серого
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Получение пикселя из оригинального изображения
                Color pixel = originalImage.GetPixel(x, y);

                // Вычисление значения яркости пикселя по формуле
                // Яркость = 0.299R + 0.587G + 0.114B
                double grayValue = 0.299 * pixel.R + 0.587 * pixel.G + 0.114 * pixel.B;

                // Присвоение значения яркости пикселя в двумерный массив
                grayImage[x, y] = grayValue;
            }
        }

        // структура для изображения
        GrayScaleImageInfo imageInfo = new GrayScaleImageInfo();
        imageInfo.grayImage = grayImage;
        imageInfo.width = width;
        imageInfo.height = height;
        
        return imageInfo;
    }

    public static void SaveGrayScaleImage(string imagePathOut, GrayScaleImageInfo imageInfo)
    {
        double[,] grayImage = imageInfo.grayImage;
        int width = imageInfo.width;
        int height = imageInfo.height;
        
        // Создание изображения
        Bitmap grayImageOut = new Bitmap(width, height);

        // Заполнение изображения значениями пикселей
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Получение значения пикселя из массива
                double pixelValue = grayImage[x, y];

                // Преобразование значения пикселя к значению grayValue типа byte
                byte grayValue = (byte)Math.Round(pixelValue);

                // Создание цвета градации серого
                Color grayColor = Color.FromArgb(grayValue, grayValue, grayValue);

                // Установка значения пикселя на изображении
                grayImageOut.SetPixel(x, y, grayColor);
            }
        }

        // Сохранение изображения в файл
        grayImageOut.Save(imagePathOut, System.Drawing.Imaging.ImageFormat.Jpeg);
    }

    public static void ConvertAngstromToImage(GrayScaleImageInfo imageInfo)
    {
        double[,] grayImage = imageInfo.grayImage;
        int width = imageInfo.width;
        int height = imageInfo.height;
        
        // подготовка к выводу изображения
        // нормализуем данные
        Normalize(grayImage, 1, 1, width-1, height-1);
        
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

        Normalize(grayImage, 1, 1, width-1, height-1);
        
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
    }
}