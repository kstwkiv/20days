namespace CodingProblems;

public class Question9_ArithmeticExpressions
{
    public static string EvaluateExpression(string expression)
    {
        string[] parts = expression.Split(' ');
        
        if (parts.Length != 3)
        {
            return "Error:InvalidExpression";
        }
        
        string aStr = parts[0];
        string op = parts[1];
        string bStr = parts[2];
        
        if (!int.TryParse(aStr, out int a))
        {
            return "Error:InvalidNumber";
        }
        
        if (!int.TryParse(bStr, out int b))
        {
            return "Error:InvalidNumber";
        }
        
        int result;
        
        if (op == "+")
        {
            result = a + b;
        }
        else if (op == "-")
        {
            result = a - b;
        }
        else if (op == "*")
        {
            result = a * b;
        }
        else if (op == "/")
        {
            if (b == 0)
            {
                return "Error:DivideByZero";
            }
            result = a / b;
        }
        else
        {
            return "Error:UnknownOperator";
        }
        
        return result.ToString();
    }
}
