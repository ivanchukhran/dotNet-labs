namespace Solution;

public class LU
{
    public static Tuple<Matrix, Matrix> Decompose(Matrix a)
    {
        if (!a.IsSquare())
        {
            throw new ArgumentException("Matrix must be square");
        }
        var n = a.Rows;
        var l = new Matrix(n, n, 0);
        var u = new Matrix(n, n, 0);
        (l, u) = DecomposeSeqPartial(a, l, u, 0, n);

        return new Tuple<Matrix, Matrix>(l, u);
    }

    private static Tuple<Matrix, Matrix> DecomposeSeqPartial(Matrix a, Matrix l, Matrix u, int startRow, int endRow)
    {
        var n = a.Rows;
        for (var i = startRow; i < endRow; i++)
        {
            for (var j = i; j < n; j++)
            {
                double sum = 0;
                for (int k = 0; k < i; k++)
                {
                    sum += l[i, k] * u[k, j];
                }
                u[i, j] = a[i, j] - sum;
            }
            for (int j = i; j < n; j++)
            {
                if (i == j)
                    l[i, i] = 1;
                else
                {
                    double sum = 0;
                    for (int k = 0; k < i; k++)
                    {
                        sum += l[j, k] * u[k, i];
                    }
                    l[j, i] = (a[j, i] - sum) / u[i, i];
                }
            }
        }

        return new Tuple<Matrix, Matrix>(l, u);
    }

    public static Tuple<Matrix, Matrix> DecomposeParallel(Matrix a, int numThreads)
    {
        var n = a.Rows;
        if (!a.IsSquare())
        {
            throw new ArgumentException("Matrix must be square");
        }

        if (numThreads > n)
        {
            throw new ArgumentException("Number of threads must be less than number of rows");
        }
        
        int rowsPerThread = n / numThreads;
        if (n % numThreads != 0)
        {
            rowsPerThread++;
        }
        Thread[] threads = new Thread[numThreads];
        var l = new Matrix(n, n, 0);
        var u = new Matrix(n, n, 0);
        for (int i = 0; i < numThreads; i++)
        {
            int startRow = i * rowsPerThread;
            int endRow = (i == numThreads - 1) ? n - 1 : (i + 1) * rowsPerThread - 1;

            threads[i] = new Thread(() => DecomposeSeqPartial(a, l, u, startRow, endRow));
            threads[i].Start();
        }
        for (int i = 0; i < numThreads; i++)
        {
            threads[i].Join();
        }

        return new Tuple<Matrix, Matrix>(l, u);
    }

    public static double[] Eliminate(double[] b, Matrix l, Matrix u)
    {
        var n = l.Rows;
        double[] y = new double[n];
        double[] x = new double[n];
        
        for (int i = 0; i < n; i++)
        {
            double sum = 0;
            for (int j = 0; j < i; j++)
            {
                sum += l[i, j] * y[j];
            }
            y[i] = b[i] - sum;
        }

        for (int i = n - 1; i >= 0; i--)
        {
            double sum = 0;
            for (int j = i + 1; j < n; j++)
            {
                sum += u[i, j] * x[j];
            }
            x[i] = (y[i] - sum) / u[i, i];
        }

        return x;
    }
}