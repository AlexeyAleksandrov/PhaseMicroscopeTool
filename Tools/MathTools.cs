namespace TestFFT2D.Tools;

public static class MathTools
{
    public static void Normalize(double[,] massive)
        {
            int n = massive.GetLength(0);
            Normalize(massive, 0, 0, n, n);
        }

        public static void Normalize(double[,] massive, int start_i, int start_j, int end_i, int end_j)
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
        
        public static void Denormalize(double[,] massive, double min, double max)
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
        
        public static void UnwrapMassive(double[,] massive, int n)
        {
            // развёртка по Y
            for (int y = 0; y < n; y++)
            {
                double[] row = new double[n];    // создаём стобец

                for (int x = 0; x < n; x++)     // заполняем столбец
                {
                    row[x] = massive[x,y];
                }

                row = Unwrap2(row);   // выполняем развёртку столбца

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

                column = Unwrap2(column);

                for (int y = 0; y < n; y++)
                {
                    massive[x,y] = column[y];
                }
            }
        }
        
        public static double[] Unwrap2(double[] massive)
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