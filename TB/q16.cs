namespace CodingProblems;

public class Question16_LuckyNumbers
{
    private static int SumOfDigits(long n)
    {
        int sum = 0;
        while (n > 0)
        {
            sum += (int)(n % 10);
            n /= 10;
        }
        return sum;
    }

    private static bool IsPrime(int n)
    {
        if (n < 2) return false;
        for (int i = 2; i * i <= n; i++)
        {
            if (n % i == 0) return false;
        }
        return true;
    }

    private static bool IsLuckyNumber(int x)
    {
        if (IsPrime(x)) return false;

        int sx = SumOfDigits(x);
        int sxSquared = SumOfDigits((long)x * x);

        return sxSquared == sx * sx;
    }

    public static int CountLuckyNumbers(int m, int n)
    {
        int count = 0;
        for (int i = m; i <= n; i++)
        {
            if (IsLuckyNumber(i))
                count++;
        }
        return count;
    }
}
