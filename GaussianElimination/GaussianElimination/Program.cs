// See https://aka.ms/new-console-template for more information

using System.Numerics;
using System.Text;
using Solution;

// var matrix = new double[3, 4];
// for (int i = 0; i < 3; i++)
// {
//     for (int j = 0; j < 4; j++)
//     {
//         matrix[i, j] = i + j;
//     }
// }
//
// var sb = new StringBuilder();
// for (int i = 0; i < 3; i++)
// {
//     sb.Append("[");
//     for (int j = 0; j < 4; j++)
//     {
//         sb.Append($"{matrix[i, j]} ");
//     }
//     sb.Append("]\n");
// }
// Console.WriteLine(sb.ToString());

// var matrix = new Matrix(3, 4, 3.14);
// var solution = GaussianElimination.Solve(matrix);
// Console.WriteLine($"Source matrix: \n{matrix.ToString()}");
// Console.WriteLine($"Solution: \n{solution.ToString()}");

double[,] matrix = { {1, 2, 3 }, {4, 5, 6}, {7, 8, 9} };
var a = new Matrix(matrix);
var (l, u) = LU.Decompose(a);
double[] b = {1, 2, 3};
LU.Eliminate(b, l, u);
Console.WriteLine($"Source matrix: \n{a.ToString()}");
Console.WriteLine($"L: \n{l.ToString()}");
Console.WriteLine($"U: \n{u.ToString()}");
Console.WriteLine($"Solution: \n{b.ToString()}");