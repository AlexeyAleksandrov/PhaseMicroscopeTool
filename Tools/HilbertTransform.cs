using System.Numerics;
using TestFFT2D.Models;
using static TestFFT2D.Tools.MathTools;
using static TestFFT2D.Tools.FourierTransform;

namespace TestFFT2D.Tools;

public class HilbertTransform
{
    public static void Transform(GrayScaleImageInfo imageInfo)
    {
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
        Normalize(grayImage);
        Denormalize(grayImage, -1 * Math.PI, Math.PI);
        
        // выполняем развёртку
        UnwrapMassive(grayImage, width);
        
        // переводим в ангстремы
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                const double waveLength = 6328;
                grayImage[x, y] = (grayImage[x, y] * waveLength) / (4 * Math.PI);
            }
        }
    }
}