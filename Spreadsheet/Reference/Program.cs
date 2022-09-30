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
            return 3;
        }
        else if (x == "x2")
        {
            return 2;
        }
        else
            throw new ArgumentException("Unknown Variable");
    }


    public static void Main(string[] args)
    {
        Evaluator.Evaluate("32+3", simpleLookUp);

    }

}
