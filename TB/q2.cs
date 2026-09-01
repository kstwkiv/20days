namespace CodingProblems;

public class Question2_MultiplicationTable
{
    public static int[] GetMultiplicationRow(int n, int upto)
    {
        int[] result = new int[upto];
        
        for (int i = 0; i < upto; i++)
        {
            result[i] = n * (i + 1);
        }
        
        return result;
    }
}
