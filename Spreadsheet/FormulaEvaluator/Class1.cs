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
            Stack<int> intStack = new Stack<int>();
            Stack<string> operatorStack = new Stack<string>();

            foreach(string substring in substrings)
            {
                trim = String.Concat(substring.Where(c => !char.IsWhiteSpace(c)));
                isNumeric = int.TryParse(trim, out n);
                if (isNumeric)
                {
                    intStack.Push(int.Parse(trim));
                }
                else if (substring is "*" || substring is "/" || substring is "+" || substring is "-")
                {
                    operatorStack.Push(trim);
                }
                else
                {
                    int varVal = variableEvaluator(substring);
                    intStack.Push(varVal);
                }              
            }
            return 0;
        }
    }
}