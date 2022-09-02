using FormulaEvaluator;
class test
{

    /**
     * This method will lookup variable names and check its value. If no value exists, throw an IllegalArgument
     */
    public static int simpleLookUp(String x)
    {
        if (x == "a4")
        {
            return 1;
        }
        else if (x == "a1")
        {
            return 3;
        }
        else
            throw new ArgumentException();
    }
    static void Main(String[] args)
    {
        Console.WriteLine(Evaluator.Evaluate("9 + 9 / a1", simpleLookUp));
    }
}
