﻿using FormulaEvaluator;
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
        else if (x == "x3")
        {
            return 2;
        }
        else if (x == "x4")
        {
            return 2;
        }
        else if (x == "y1")
        {
            return 4;
        }
        else if (x == "x7")
        {
            return 1;
        }
        else
            throw new ArgumentException("Unknown Variable");
    }
    static void Main(String[] args)
    {
        //Expecting 6
        Console.WriteLine(Evaluator.Evaluate("2+3*5+(3+4*8)*5+2", simpleLookUp));
    }
}
