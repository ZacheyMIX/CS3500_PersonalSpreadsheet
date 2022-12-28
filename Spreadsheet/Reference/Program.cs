using FormulaEvaluator;
class test
{
    /**
     * This method will lookup variable names and check its value. If no value exists, throw an IllegalArgument
     */
    public static int headsOrTails()
    {
        Random random = new Random();
        return random.Next(2);
    }

    public static void result(int times)
    {
        int heads = 0;
        for(int i = 0; i < times; i++)
        {
            for (int j = 0; j < 20; j++)
            {
                if (headsOrTails() == 0)
                    heads++;
            }
            Console.WriteLine(heads);
            heads = 0;
        }
        

    }

    public static void Main(string[] args)
    {
        result(20);
    }

}
