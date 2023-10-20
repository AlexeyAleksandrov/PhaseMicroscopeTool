using System.Drawing;

namespace TestFFT2D;

public class ImagesTools
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
}

// структура для хранения информации о считанном изображении в формате градации серого
public struct GrayScaleImageInfo
{
    public double[,] grayImage { get; set; }
    public int width { get; set; }
    public int height { get; set; }
}