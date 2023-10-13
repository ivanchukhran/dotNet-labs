using System.Runtime.InteropServices;
using System.Text;

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

    private static Tuple<Matrix, Matrix> DecomposeSeqPartial(Matrix a, Matrix l, Matrix u, int start, int end)
    {
        var n = a.Rows;
        for (var i = start; i < end; i++)
        {
            FillU(a, l, u, i, i, n);
            FillL(a, l, u, i, i, n);
        }

        return new Tuple<Matrix, Matrix>(l, u);
    }

    private static void FillU(Matrix a, Matrix l, Matrix u, int i, int start, int end)
    {
        for (var j = start; j < end; j++)
        {
            double sum = 0;
            for (int k = 0; k < i; k++)
            {
                sum += l[i, k] * u[k, j];
            }

            u[i, j] = a[i, j] - sum;
        }
    }

    private static void FillL(Matrix a, Matrix l, Matrix u, int i, int start, int end)
    {
        for (var j = start; j < end; j++)
        {
            if (i == j)
            {
                l[i, j] = 1;
            }
            else
            {
                double sum = 0;
                for (var k = 0; k < i; k++)
                {
                    sum += l[j, k] * u[k, i];
                }

                l[j, i] = (a[j, i] - sum) / u[i, i];
            }
            
        }
    }

    public static Tuple<Matrix, Matrix> DecomposeParallel(Matrix a, int maxThreads)
    {
        var n = a.Columns;
        if (!a.IsSquare())
        {
            throw new ArgumentException("Matrix must be square");
        }

        if (maxThreads > n)
        {
            throw new ArgumentException("Number of threads must be less than number of rows");
        }

        var threads = new Thread[maxThreads];
        var l = new Matrix(n, n, 0);
        var u = new Matrix(n, n, 0);

        for (int i = 0; i < n; i++)
        {
            var columnsPerThread = (n - i) / maxThreads;
            if (n % maxThreads != 0 || columnsPerThread == 0)
            {
                columnsPerThread++;
            }


            for (int j = 0; j < maxThreads; j++)
            {
                int startColumn = i + j * columnsPerThread;
                int endColumn = i + (j + 1) * columnsPerThread;
                if (endColumn > n)
                {
                    endColumn = n ;
                }

                threads[j] = new Thread(() => FillU(a, l, u, i, startColumn, endColumn));
                threads[j].Start();
            }

            for (var j = 0; j < maxThreads; j++)
            {
                threads[j].Join();
            }

            for (var j = 0; j < maxThreads; j++)
            {
                int startColumn = i + j * columnsPerThread;
                int endColumn = i + (j + 1) * columnsPerThread;
                if (endColumn > n)
                {
                    endColumn = n;
                }
                threads[j] = new Thread(() => FillL(a, l, u, i, startColumn, endColumn));
                threads[j].Start();
            }
            for (var j = 0; j < maxThreads; j++)
            {
                threads[j].Join();
            }
        }

        return new Tuple<Matrix, Matrix>(l, u);
    }

    public static Matrix Eliminate(double[] b, Matrix l, Matrix u)
    {
        var n = l.Rows;
        double[] y = new double[n];
        double[] x = new double[n];
        ComposeY(l, b, y, 0, n);
        ComposeX(u, x, y, 0, n);

        return new Matrix(x);
    }
    
    public static Matrix EliminateParallel(double[] b, Matrix l, Matrix u, int maxThreads)
    {
        var n = l.Rows;
        double[] y = new double[n];
        double[] x = new double[n];
        
        var rowsPerThread = n / maxThreads;
        if (n % maxThreads != 0 || rowsPerThread == 0)
        {
            rowsPerThread++;
        }
        var threads = new Thread[maxThreads];
        for (var i = 0; i < maxThreads; i++)
        {
            int startRow = i * rowsPerThread;
            int endRow = (i + 1) * rowsPerThread;
            if (endRow > n)
            {
                endRow = n;
            }
            threads[i] = new Thread(() => ComposeY(l, b, y, startRow, endRow));
            threads[i].Start();
            
        }

        for (var i = 0; i < maxThreads; i++)
        {
            threads[i].Join();
        }
        
        for (var i = 0; i < maxThreads; i++)
        {
            int startRow = i * rowsPerThread;
            int endRow = (i + 1) * rowsPerThread;
            if (endRow > n)
            {
                endRow = n;
            }
            threads[i] = new Thread(() => ComposeX(u, x, y, startRow, endRow));
            threads[i].Start();
            
        }
        
        for (var i = 0; i < maxThreads; i++)
        {
            threads[i].Join();
        }

        return new Matrix(x);
    }
    
    private static void ComposeY(Matrix l, double[] b, double[] y, int startRow, int endRow)
    {
        for (var i = startRow; i < endRow; i++)
        {
            double sum = 0;
            for (var j = 0; j < i; j++)
            {
                sum += l[i, j] * y[j];
            }

            y[i] = b[i] - sum;
        }
    }

    private static void ComposeX( Matrix u, double[] x, double[] y, int startRow, int endRow)
    {
        for (int i = endRow - 1; i >= startRow; i--)
        {
            double sum = 0;
            for (int j = i + 1; j < endRow; j++)
            {
                sum += u[i, j] * x[j];
            }

            x[i] = (y[i] - sum) / u[i, i];
        }
    }
}