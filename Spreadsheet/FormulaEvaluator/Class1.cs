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
            //An int used to perform a tryParse
            int n;
            //A boolean that checks if a token is an integer or not
            bool isNumeric;
            //A string used as the current token in the search but without white space
            string trim;
            //The value at the current token through the search
            int currentValue;
            //Results of operations done
            int val;
            //The stack that contains values
            Stack<int> valStack = new Stack<int>();
            //The stack that contains operations
            Stack<string> opStack = new Stack<string>();
            //Checks each token in the substring and evaluates the final result
            foreach (string substring in substrings)
            {
                //Trims the token of any whitespace
                trim = String.Concat(substring.Where(c => !char.IsWhiteSpace(c)));
                //Checks if the token is numeric and applies the result to isNumeric
                isNumeric = int.TryParse(trim, out n);
                if (isNumeric)
                {
                    //Parses the current token into an integer
                    currentValue = int.Parse(trim);

                    //If the operator in the opStack is * or / peform an operation
                    //With that operator accordingly with the value popped from the stack and the current value
                    //If not, pushes current value onto stack
                    if (StackExtensions.IsOnTop(opStack, "*") || StackExtensions.IsOnTop(opStack, "/"))
                    {
                        StackExtensions.PushResult(valStack, opStack.Pop(), currentValue);
                    }
                    else
                        valStack.Push(currentValue);
                }
                else if (trim is "+" || trim is "-")
                {
                    //Checks if the operator stack is empty
                    if (opStack.Count == 0)
                    {
                        opStack.Push(trim);
                    }
                    else if (opStack.Peek() is "+" || opStack.Peek() is "-")
                    {
                        if (opStack.Pop() is "+")
                            val = valStack.Pop() + valStack.Pop();
                        else
                            val = valStack.Pop() - valStack.Pop();
                        valStack.Push(val);
                        opStack.Push(trim);
                    }
                    else
                        opStack.Push(trim);
                }
                else if (trim is "*" || trim is "/" || trim is "(")
                {
                    opStack.Push(trim);
                }
                else if (trim is ")")
                {
                    if (opStack.Count is 0 || valStack.Count >= 2)
                    {

                    }
                    else if (opStack.Peek() is "+" || opStack.Peek() is "-")
                    {
                        if (opStack.Pop() is "+")

                            val = valStack.Pop() + valStack.Pop();
                        else
                            val = valStack.Pop() - valStack.Pop();

                        valStack.Push(val);
                    }
                }
                else
                {
                    currentValue = variableEvaluator(trim);
                    if (opStack.Count is 0)
                    {
                        valStack.Push(currentValue);
                    }
                    //If the operator in the opStack is * or / peform an operation
                    //With that operator accordingly with the value popped from the stack and the current value
                    //If not, pushes current value onto stack
                    else if (opStack.Peek() is "*" || opStack.Peek() is "/")
                    {
                        if (opStack.Pop() is "*")
                            val = valStack.Pop() * currentValue;
                        else
                            val = valStack.Pop() / currentValue;
                        //Pushes the result onto the value stack
                        valStack.Push(val);
                    }
                    else
                        valStack.Push(currentValue);
                }
            }
            if (opStack.Count > 0)
            {
                val = valStack.Pop();
                if (opStack.Pop() is "+")
                    val = valStack.Pop() + val;
                else
                    val = valStack.Pop() - val;

                valStack.Push(val);
            }
            return valStack.Pop();
        }
    }
    public static class StackExtensions
    {
        public static bool IsOnTop(this Stack<String> stack, String c)
        {
            return stack.Count > 0 && stack.Peek() == c;
        }

        public static void PushResult(this Stack<int> stack, String c, int val)
        {
            int finalVal;
            if(c is "*")
            {
                finalVal = stack.Pop() * val;
                stack.Push(finalVal);
            }
            else if(c is "/")
            {
                finalVal = stack.Pop() / val;
                stack.Push(finalVal);
            }

        }

        }
    }