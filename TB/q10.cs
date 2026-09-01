namespace CodingProblems;

public class Question10_LargestInteger
{
    public static int FindLargest(int a, int b, int c)
    {
        int largest;
        
        if (a >= b && a >= c)
        {
            largest = a;
        }
        else if (b >= a && b >= c)
        {
            largest = b;
        }
        else
        {
            largest = c;
        }
        
        return largest;
    }
}
