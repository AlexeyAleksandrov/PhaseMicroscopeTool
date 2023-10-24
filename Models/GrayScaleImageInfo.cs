namespace TestFFT2D.Models;

// структура для хранения информации о считанном изображении в формате градации серого
public struct GrayScaleImageInfo
{
    public double[,] grayImage { get; set; }
    public int width { get; set; }
    public int height { get; set; }
}