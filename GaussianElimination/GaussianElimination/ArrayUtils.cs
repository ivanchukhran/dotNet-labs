namespace Solution;

public class ArrayUtils
{
    public static String ArrayToString<T>(T[] array) => $"[{string.Join(", ", array)}]";
    public static String ArrayToString<T>(T[,] array) => $"[{string.Join(", ", array)}]";
    public static void PrintArray<T>(T[] array) => Console.WriteLine(ArrayToString(array));
    public static void PrintArray<T>(T[,] array) => Console.WriteLine(ArrayToString(array));
    
}