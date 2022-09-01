using FormulaEvaluator;
class test
{

    /**
     * This method will lookup variable names and check its value. If no value exists, throw an IllegalArgument
     */
    public static int simpleLookUp(String x)
    {
        return 0;
    }
    static void Main(String[] args)
    {
        Console.WriteLine(Evaluator.Evaluate("abc + 9 + 9 / 3", simpleLookUp));
    }
}
