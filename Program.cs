using System.Drawing;
using System.Drawing.Imaging;
using Accord.Math;
using System.Numerics;
using static System.Runtime.InteropServices.JavaScript.JSType;
using String = System.String;

namespace TestFFT2D
{
    internal partial class Program
    { 
        static void Main(string[] args)
        {
            // Путь к изображению
            string imagePath = "C:\\Users\\ASUS\\source\\repos\\TestFFT2D\\Interferogramma.jpg";
            string imagePathOut = "C:\\Users\\ASUS\\Pictures\\Interferogramma_out.jpg";

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
            FourierTransform.FFT2(complexes, FourierTransform.Direction.Forward);
            
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

            Console.WriteLine("Изображение сохранено в файл: " + imagePathOut);
        }
        
        public static void normalize(double[,] massive)
        {
            int n = massive.GetLength(0);
            normalize(massive, 0, 0, n, n);
        }

        public static void normalize(double[,] massive, int start_i, int start_j, int end_i, int end_j)
        {
            // нормализация
            double min = massive[start_i,start_j];
            double max = massive[start_i,start_j];
            for (int i = start_i; i < end_i; i++)
            {
                for (int j = start_j; j < end_j; j++)
                {
                    if(min < massive[i,j])
                    {
                        min = massive[i,j];
                    }
                    if(max > massive[i,j])
                    {
                        max = massive[i,j];
                    }
                }
            }

            // производим нормализацию
            for (int i = start_i; i < end_i; i++)
            {
                for (int j = start_j; j < end_j; j++)
                {
                    massive[i,j] = (massive[i,j] - min) / (max - min);
                }
            }
        }
        
        public static void denormalize(double[,] massive, double min, double max)
        {
            int n = massive.GetLength(0);

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    massive[i,j] = massive[i,j] * (max - min) + min;
                }
            }
        }
        
        public static void unwrapMassive(double[,] massive, int n)
        {
            // развёртка по Y
            for (int y = 0; y < n; y++)
            {
                double[] row = new double[n];    // создаём стобец

                for (int x = 0; x < n; x++)     // заполняем столбец
                {
                    row[x] = massive[x,y];
                }

                row = unwrap2(row);   // выполняем развёртку столбца

                for (int x = 0; x < n; x++)     // заполняем матрицу
                {
                    massive[x,y] = row[x];
                }
            }

            // развёртка по X
            for (int x = 0; x < n; x++)
            {
                double[] column = new double[n];    // создаём стобец

                for (int y = 0; y < n; y++)
                {
                    column[y] = massive[x,y];
                }

                column = unwrap2(column);

                for (int y = 0; y < n; y++)
                {
                    massive[x,y] = column[y];
                }
            }
        }
        
        public static double[] unwrap2(double[] massive)
        {
            int k = 0;
            int n = massive.Length;
            double c = 0.7;

            double[] unwrapped = new double[n];

            for (int i = 0; i < n; i++)
            {
                if(i==n-1)
                {
                    unwrapped[n-1] = massive[n-1] + (2 * Math.PI * k);
                    return unwrapped;
                }
                unwrapped[i] = massive[i] + (2 * Math.PI * k);
                if(Math.Abs(massive[i+1] - massive[i]) > (Math.PI * c))
                {
                    if(massive[i+1] < massive[i])
                    {
                        k++;
                    }
                    else
                    {
                        k--;
                    }
                }
            }

            unwrapped[n-1] = massive[n-1] + (2 * Math.PI * k);

            return unwrapped;
        }
    }
}