using FormulaEvaluator;
class test
{

    /**
     * This method will lookup variable names and check its value. If no value exists, throw an IllegalArgument
     */
    public int simpleLookUp(String x)
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
        List<String> list = new List<String>();
        list.Add("b1");
        list.Add("c1");
        list.Insert(0, "a1");
        Console.WriteLine(list.Count);
    }

}
