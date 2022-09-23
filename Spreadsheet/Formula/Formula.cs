// Skeleton written by Profs Zachary, Kopta and Martin for CS 3500
// Read the entire skeleton carefully and completely before you
// do anything else!

// Change log:
// Last updated: 9/8, updated for non-nullable types
using SpreadsheetUtilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  The allowed symbols are non-negative numbers written using double-precision 
    /// floating-point syntax (without unary preceeding '-' or '+'); 
    /// variables that consist of a letter or underscore followed by 
    /// zero or more letters, underscores, or digits; parentheses; and the four operator 
    /// symbols +, -, *, and /.  
    /// 
    /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
    /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable; 
    /// and "x 23" consists of a variable "x" and a number "23".
    /// 
    /// Associated with every formula are two delegates:  a normalizer and a validator.  The
    /// normalizer is used to convert variables into a canonical form, and the validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard requirement 
    /// that it consist of a letter or underscore followed by zero or more letters, underscores,
    /// or digits.)  Their use is described in detail in the constructor and method comments.
    /// </summary>
    public class Formula
    {
        String formula;
        //tokens for the formula
        IEnumerator<string> tokenEnum;
        //Normalizer
        Func<string, string> normalize;
        //IsValid check
        Func<string, bool> isValid;
        //A string that holds the current normalized token
        List<String> formulaTokens;
        //A hashcode for the formula
        int hashCode;
        //A double used to perform a tryParse
        double n;

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.  
        /// </summary>
        public Formula(String formula) :
            this(formula, s => s, s => true)
        {
        }

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer and validator are the second and third parameters,
        /// respectively.  
        /// 
        /// If the formula contains a variable v such that normalize(v) is not a legal variable, 
        /// throws a FormulaFormatException with an explanatory message. 
        /// 
        /// If the formula contains a variable v such that isValid(normalize(v)) is false,
        /// throws a FormulaFormatException with an explanatory message.
        /// 
        /// Suppose that N is a method that converts all the letters in a string to upper case, and
        /// that V is a method that returns true only if a string consists of one letter followed
        /// by one digit.  Then:
        /// 
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// </summary>

        public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            this.formula = formula;
            this.normalize = normalize;
            this.isValid = isValid;
            hashCode = 0;
            formulaTokens = new List<String>();
            tokenEnum = GetTokens(formula).GetEnumerator();
            SyntaxCheck(tokenEnum);
        }
        /// <summary>
        /// This method finds any syntax errors that could be run into at the objects creation
        /// If every token passes all the test, there should be no syntax errors in the formula
        /// and the evaluator can be properly ran
        /// </summary>
        /// <param name="tokens"></param>
        /// <exception cref="FormulaFormatException"></exception>
        private void SyntaxCheck(IEnumerator<string> tokens)
        {
            //closing bracket counter
            int closeBrackets = 0;
            //opening bracket counter
            int openingBrackets = 0;
            //a check bool for when a opening bracket or variable is made
            bool followVariableCheck = false;
            //A check book for when a closing bracket, variable, or number is made
            bool followOperatorCheck = false;
            //Adds all tokens to a list of formulaTokens
            while (tokens.MoveNext())
            {
                formulaTokens.Add(normalize(tokens.Current));
            }
            //throws an exception if there are no tokens in the formula
            if (formulaTokens.Count is 0)
            {
                throw new FormulaFormatException("There is no formula put in");
            }
            //throws an exception if the formula doens't start with a number, variable, or an opening paranthesis
            if (!(Regex.IsMatch(formulaTokens[0], @"^[a-zA-Z0-9_(]")))
                throw new FormulaFormatException("Formula does not start with a number, variable, or an opening paranthesis");
            //Runs a do while loop on all of the tokens
            foreach(string currentToken in formulaTokens)
            {
                //If the current token does not contain these tokens, then throws a FormulaFormatException
                if (!(Regex.IsMatch(currentToken, @"[a-zA-Z0-9_()+/*-]")))
                {
                    throw new FormulaFormatException("An invalid token is contained within the formula");
                }
                //When a opening bracket or operator is used, throws an exception if it is not
                //Followed by a variable, opening bracket, or a number
                if (followVariableCheck is true)
                {
                    if (!(Regex.IsMatch(currentToken, @"^[a-zA-Z0-9_(.]")))
                        throw new FormulaFormatException("Opening bracket or operator is not followed by a number, variable, or opening paranthesis");
                    followVariableCheck = false;
                }
                //When a closing bracket, variable, or number is used, throws an exception if it is not
                //Followed by an operator or a closing bracket
                else if (followOperatorCheck is true)
                {
                    if (!(Regex.IsMatch(currentToken, @"^[*/+)-]")))
                        throw new FormulaFormatException("number, variable, or closing paranthesis is not followe by an operator or closing paranthesis");
                    followOperatorCheck = false;
                }
                //When a opening bracket is used, marks the variable check for the next loop and bumps the opening bracket counter
                if (currentToken is "(")
                {
                    hashCode += String.GetHashCode(currentToken);
                    followVariableCheck = true;
                    openingBrackets++;
                }
                //When a closing bracket is used, marks the operator check for the next loop and bumps the closing bracket counter
                else if (currentToken is ")")
                {
                    hashCode += String.GetHashCode(currentToken);
                    followOperatorCheck = true;
                    closeBrackets++;
                    if (openingBrackets < closeBrackets)
                    {
                        throw new FormulaFormatException("Closing brackets exceed opening brackets");
                    }
                }
                //If an operator is used marks the variable check for the next loop
                else if (Regex.IsMatch(currentToken, @"^[*/+-]"))
                {
                    hashCode += String.GetHashCode(currentToken);
                    followVariableCheck = true;
                }
                //If a variable is used, checks if the varaible is valid and marks the operator check for the next loop
                else if (Regex.IsMatch(currentToken, @"^[a-zA-Z_]"))
                {
                    if (!(isValid(currentToken)))
                        throw new FormulaFormatException("Token is not a valid variable");
                    hashCode += String.GetHashCode(currentToken);
                    followOperatorCheck = true;
                }
                //If a number is used, checks if the number is valid and marks the operator check for the next loop
                else if (Regex.IsMatch(currentToken, @"^[0-9]"))
                {
                    hashCode += String.GetHashCode(Double.Parse(currentToken).ToString());
                    followOperatorCheck = true;
                }
             //Move token every loop until it cant
            }
            //Thows an exception if the final token is not a number, variable, or closing paranthesis
            if (!(Regex.IsMatch(formulaTokens[formulaTokens.Count - 1], @"^[a-zA-Z0-9_)]")))
                throw new FormulaFormatException("Formula does not end with a number, variable, or closing paranthesis");
            //If the counter for closing brackets and opening brackets does not equal each other, throw an exception
            if (closeBrackets != openingBrackets)
                throw new FormulaFormatException("Closing brackets does not equal to the ammount of opening brackets");
        }

        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to 
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters 
        /// in a string to upper case:
        /// 
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        /// 
        /// Given a variable symbol as its parameter, lookup returns the variable's value 
        /// (if it has one) or throws an ArgumentException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        public object Evaluate(Func<string, double> lookup)
        {
            //The value at the current token through the search
            double currentValue;
            //The stack that contains values
            Stack<double> valStack = new Stack<double>();
            //The stack that contains operations
            Stack<string> opStack = new Stack<string>();

            foreach (string substring in formulaTokens)
            {
                bool numberCheck;
                //Trims the token of any whitespace
                //Checks if the token is numeric and applies the result to isNumeric
                numberCheck = double.TryParse(substring, out n);
                if (numberCheck)
                {
                    //Parses the current token into an integer
                    currentValue = double.Parse(substring);

                    //If the operator in the opStack is * or / peform an operation
                    //With that operator accordingly with the value popped from the stack and the current value
                    //If not, pushes current value onto stack
                    if (StackExtensions.IsOnTop(opStack, "*") || StackExtensions.IsOnTop(opStack, "/"))
                    {
                        //Performs the proper calculation through pushresult, and pushes the result onto the valStack
                        try
                        {
                            StackExtensions.PushResult(valStack, opStack.Pop(), currentValue);
                        }
                        catch (FormulaFormatException)
                        {
                            return new FormulaError("Divide by zero occured");
                        }
                        
                    }
                    else
                        valStack.Push(currentValue);
                }
                //Checks if the current substring is + or -
                else if (substring is "+" || substring is "-")
                {
                    //Checks if stack in operation stack is empty and checks for + or - otherwise
                    //If check fails pushes operation onto op stack
                    if (StackExtensions.IsOnTop(opStack, "+") || StackExtensions.IsOnTop(opStack, "-"))
                    {
                        //Pushes result of + or - case with PushResult
                        StackExtensions.PushResult(valStack, opStack.Pop(), valStack.Pop());
                        //Removes last operation from opStack
                        opStack.Push(substring);
                    }
                    else
                        opStack.Push(substring);
                }
                //Checks for * / and (, then pushes onto operation stack
                else if (substring is "*" || substring is "/" || substring is "(")
                {
                    opStack.Push(substring);
                }
                //Checks for ) as the substring
                else if (substring is ")")
                {
                    //Pushes result from last operation and last to values from the stack
                    if (!(opStack.Peek() is "("))
                    {
                        StackExtensions.PushResult(valStack, opStack.Pop(), valStack.Pop());
                        //Gets rid of ( from the stack
                        opStack.Pop();
                        //If there is a * or / before the (, operate that function
                        if (!(opStack.Count is 0) && (opStack.Peek() is "*" || opStack.Peek() is "/"))
                        {
                            try
                            {
                                StackExtensions.PushResult(valStack, opStack.Pop(), valStack.Pop());
                            }
                            catch(FormulaFormatException)
                            {
                                return new FormulaError("Divide by zero occured");
                            }
                            
                        }
                    }
                    //If ( is the next operator, than there is no need for it, remove
                    else
                        opStack.Pop();

                }
                //This else statement is for any other case such as variables
                else
                {
                    //Attempts to grab the value from the variable, if it fails returns a FormulaError
                    try
                    {
                        currentValue = lookup(substring);
                    }
                    catch(ArgumentException)
                    {
                        return new FormulaError("Variable Undefined");
                    }
                    

                    //Checks if the operator stack is empty, then checks which variable is on top for * or /
                    if (StackExtensions.IsOnTop(opStack, "*") || StackExtensions.IsOnTop(opStack, "/"))
                    {
                        //Performs the appropriate push calculation with either * or /
                        StackExtensions.PushResult(valStack, opStack.Pop(), currentValue);
                    }
                    //Otherwise, just push the value onto the value stack
                    else
                        valStack.Push(currentValue);
                }
            }
            //If there is an operation in opStack, runs more calculations
            if (opStack.Count > 0)
            {
                //Calculates and pushes the result of the last 2 values with the last operation
                StackExtensions.PushResult(valStack, opStack.Pop(), valStack.Pop());
            }
            //Returns the final value stored in the stack
            return valStack.Pop();
        }

        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this 
        /// formula.  No normalization may appear more than once in the enumeration, even 
        /// if it appears more than once in this Formula.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        public IEnumerable<String> GetVariables()
        {
            List<String> variables = new List<String>();
            foreach(String currentToken in formulaTokens)
            {
                if(Regex.IsMatch(currentToken, @"^[a-zA-Z_]") && !(variables.Contains(currentToken)))
                {
                    variables.Add(currentToken);
                    yield return currentToken;
                }
                    
            }
        }

        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        public override string ToString()
        {
            String formulaBuild = "";
            foreach (string currentToken in formulaTokens)
            {
                formulaBuild += currentToken;
            }
            return formulaBuild;
        }

        /// <summary>
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        /// 
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  To determine token equality, all tokens are compared as strings 
        /// except for numeric tokens and variable tokens.
        /// Numeric tokens are considered equal if they are equal after being "normalized" 
        /// by C#'s standard conversion from string to double, then back to string. This 
        /// eliminates any inconsistencies due to limited floating point precision.
        /// Variable tokens are considered equal if their normalized forms are equal, as 
        /// defined by the provided normalizer.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///  
        /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
        /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
        /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        public override bool Equals(object? obj)
        {
            if (obj is null || !(obj is Formula))
                return false;
            Formula compare = (Formula)obj;
            for(int i = 0; i < formulaTokens.Count; i++)
            {
                if (Double.TryParse(formulaTokens[i], out n) && Double.TryParse(formulaTokens[i], out n))
                {
                    if (Double.Parse(formulaTokens[i]) != Double.Parse(compare.formulaTokens[i]))
                        return false;
                }
                else if(formulaTokens[i] != compare.formulaTokens[i])
                {
                    return false;
                }
            }
            return true;

        }

        /// <summary>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// Note that f1 and f2 cannot be null, because their types are non-nullable
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {
            if (f1.Equals(f2))
                return true;
            return false;
        }

        /// <summary>
        /// Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// Note that f1 and f2 cannot be null, because their types are non-nullable
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            if (f1.Equals(f2))
                return false;
            return true;
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {
            return hashCode;
        }

        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }

        }


    }

    /// <summary>
    /// Used to report syntactic errors in the argument to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Used as a possible return value of the Formula.Evaluate method.
    /// </summary>
    public struct FormulaError
    {
        /// <summary>
        /// Constructs a FormulaError containing the explanatory reason.
        /// </summary>
        /// <param name="reason"></param>
        public FormulaError(String reason)
            : this()
        {
            Reason = reason;
        }

        /// <summary>
        ///  The reason why this FormulaError was created.
        /// </summary>
        public string Reason { get; private set; }
    }
}
/**
 * A static class for stack extensions
 */
public static class StackExtensions
{
    /**
     * This extended method checks if the operation stack is empty and checks if the operation stack contains the right operation
     */
    public static bool IsOnTop(this Stack<String> stack, String c)
    {
        return stack.Count > 0 && stack.Peek() == c;
    }
    /**
     * This extended method does the proper calculations for all cases of + - / * and the closing )
     * This extended method uses the valStack class to perform such operations, then pushes the result onto the stack
     */
    public static void PushResult(this Stack<double> stack, String c, double val)
    {
        //The final value from the calculations
        double finalVal;
        if (c is "*" && stack.Count > 0)
        {
            finalVal = stack.Pop() * val;
            stack.Push(finalVal);
        }
        else if (c is "/" && stack.Count > 0)
        {
            //Checks if the value is 0, if it is throws an argument exception
            if (val is 0)
               throw new FormulaFormatException("Cannot divide by zero");
            finalVal = stack.Pop() / val;
            stack.Push(finalVal);
        }
        else if (c is "+" && stack.Count > 0)
        {
            finalVal = stack.Pop() + val;
            stack.Push(finalVal);
        }
        else if (c is "-" && stack.Count > 0)
        {
            finalVal = stack.Pop() - val;
            stack.Push(finalVal);
        }
    }
}
