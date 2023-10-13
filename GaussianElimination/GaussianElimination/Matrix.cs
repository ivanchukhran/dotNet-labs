using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;

namespace Solution;

public class Matrix
{
    public int Rows { get; }
    public int Columns { get; }
    private double[,] Values { get; set; }

    public Matrix(int rows, int columns, double value)
    {
        Rows = rows;
        Columns = columns;
        Values = new double[rows, columns];

        for (var i = 0; i < Rows; i++)
        {
            for (var j = 0; j < Columns; j++)
            {
                Values[i, j] = value;
            }
        }
    }

    public Matrix(double[] values)
    {
        Rows = 1;
        Columns = values.Length;
        Values = new double[Rows, Columns];
        for (var i = 0; i < Columns; i++)
        {
            Values[0, i] = values[i];
        }
    }

    public Matrix(double[,] values)
    {
        Rows = values.GetLength(0);
        Columns = values.GetLength(1);
        Values = new double[Rows, Columns];
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                Values[i, j] = values[i, j];
            }
        }
    }

    public static Matrix Random(int rows, int columns)
    {
        if (rows <= 0 || columns <= 0)
        {
            throw new ArgumentException("Rows and columns must be positive");
        }
        var matrix = new Matrix(rows, columns, 0);
        var random = new Random();
        for (var i = 0; i < rows; i++) {
            for (var j = 0; j < columns; j++)
            {
                matrix[i, j] = random.NextDouble();
            }
        }
        return matrix;
    }

    public double this[int row, int column]
    {
        get => Values[row, column];
        set => Values[row, column] = value;
    }

    public double[] this[int x]
    {
        get => GetRow(x);
        set => SetRow(x, value);
    }
    
    public Matrix Slice(int startRow, int endRow, int startColumn, int endColumn)
    {
        var slice = new Matrix(endRow - startRow, endColumn - startColumn, 0);
        for (var i = startRow; i < endRow; i++)
        {
            for (var j = startColumn; j < endColumn; j++)
            {
                slice[i - startRow, j - startColumn] = this[i, j];
            }
        }
        
        return slice;
    }

    private void SetRow(int i, double[] values)
    {
        for (var j = 0; j < Columns; j++)
        {
            this[i, j] = values[j];
        }
    }

    private void SetRow(int i, double values)
    {
        for (var j = 0; j < Columns; j++)
        {
            this[i, j] = values;
        }
    }

    private double[] GetRow(int i)
    {
        var row = new double[Columns];
        for (var j = 0; j < Columns; j++)
        {
            row[j] = this[i, j];
        }

        return row;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (var i = 0; i < Rows; i++)
        {
            sb.AppendLine(ArrayUtils.ArrayToString(GetRow(i)));
        }
        return sb.ToString();
    }

    public void SwapRows(int pivotRow, int swapRow)
    {
        (this[pivotRow], this[swapRow]) = (this[swapRow], this[pivotRow]);
    }

    public void Set(int row, int column, double i)
    {
        this[row, column] = i;
    }

    public static Matrix From(Matrix matrix)
    {
        return new Matrix(matrix.Values);
    }

    public bool IsSquare()
    {
        return Rows == Columns;
    }
}