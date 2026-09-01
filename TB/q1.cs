namespace CodingProblems;

public class Question1_Swapping
{
    public static void SwapWithRef(ref int a, ref int b)
    {
        int temp = a;
        a = b;
        b = temp;
    }

    public static void SwapWithOut(int a, int b, out int first, out int second)
    {
        first = b;
        second = a;
    }
}
