using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace FormulaEvaluator
{
    public static class Evaluator
    {
        public delegate int Lookup(String v);

        public static int Evaluate(String exp, Lookup variableEvaluator)
        {
            string[] substrings = Regex.Split(exp, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");
            int n;
            bool isNumeric;
            string trim;
            string operatorValue;
            int num;
            int currentValue;
            int val;
            Stack<int> valStack = new Stack<int>();
            Stack<string> opStack = new Stack<string>();
            foreach(string substring in substrings)
            {
                trim = String.Concat(substring.Where(c => !char.IsWhiteSpace(c)));
                isNumeric = int.TryParse(trim, out n);
                if (isNumeric)
                {
                    currentValue = int.Parse(trim);
                    if(opStack.Peek() is "*" || opStack.Peek() is "/")
                    {
                        num = valStack.Pop();
                        if(opStack.Pop() is "*")
                            val = num * currentValue;
                        else
                            val = num / currentValue;
                        Console.WriteLine(val);
                        valStack.Push(val);
                    }
                    valStack.Push(int.Parse(trim));
                }
                else if (substring is "*" || substring is "/" || substring is "+" || substring is "-")
                {
                    opStack.Push(trim);
                }
                else
                {
                    int varVal = variableEvaluator(substring);
                    valStack.Push(varVal);
                }              
            }
            return 0;
        }
    }
}