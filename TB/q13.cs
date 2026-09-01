namespace CodingProblems;

public class Question13_DisplayHeight
{
    public static string GetHeightCategory(int heightCm)
    {
        if (heightCm < 150)
            return "Short";
        else if (heightCm < 180)
            return "Average";
        else
            return "Tall";
    }
}
