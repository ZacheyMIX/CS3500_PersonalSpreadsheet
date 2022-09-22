using SpreadsheetUtilities;
using System.Text.RegularExpressions;

namespace SS
{
    public class Spreadsheet : AbstractSpreadsheet
    {
        DependencyGraph depList;
        Dictionary<string, string> cells;
        //A zero argument consructor that generates a spreadsheet
        public Spreadsheet()
        {
            depList = new DependencyGraph();
            cells = new Dictionary<string, string>();
        }

        //The private helper method for checking if a cell given has a valid name
        private bool isValid(string name)
        {
            //If the string starts with a letter or a underscore and then checks if the string only contains
            //letters, numbers, or underscores.  If it passes, returns true
            if (Regex.IsMatch(name, @"^[a-zA-Z_]") && Regex.IsMatch(name, @"[a-zA-Z0-9._]"))
                return true;
            //Otherwise returns false
            return false;
        }

        /// <summary>
        /// If name is invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        /// </summary>
        public override object GetCellContents(string name)
        {
            //Double integer for tryParse statements
            double n;

            //Checks if name is valid, if not throws InvalidNameException
            if (!(isValid(name)))
                throw new InvalidNameException();
            //If name has not been intialized, return an empty string
            if (!(cells.ContainsKey(name)))
                return "";
            //TryParses the value of the cell, if successful, returns the double value
            if (double.TryParse(cells[name], out n))
                return n;
            //Checks if the value in name is a formula, if it is returns it as a new formula
            else if (isFormula(cells[name]))
                return new Formula(cells[name]);
            //Otherwise returns the value of name as a string
            else
                return cells[name];
        }

        /// <summary>
        /// Helper method for GetCellContents that checks if the cells value is a formula
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool isFormula(string value)
        {
            //Checks if the value contains any operators, if it doesnt returns false
            if (!(Regex.IsMatch(value, @"[+/*-]")))
                return false;
            //Otherwise returns true
            return true;

        }

        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>

        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            foreach(string name in cells.Keys)
            {
                yield return name;
            }
        }

        /// <summary>
        /// If name is invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes number.  The method returns a
        /// list consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        public override IList<string> SetCellContents(string name, double number)
        {
            //Checks if name is valid, if not throw InvalidNameException
            if (!(isValid(name)))
                throw new InvalidNameException();

            //If cells contains the name, change the value
            if (cells.ContainsKey(name))
                cells[name] = number.ToString();
            //If cells does not contain name, adds the name and value to cells
            else
                cells.Add(name, number.ToString());

            //Creates an enumerator of the variables that name is a dependent of
            IEnumerator<string> variableEnum = depList.GetDependees(name).GetEnumerator();
            //Creates a list of variables to build
            IList<string> variableList = new List<string>();
            variableList.Add(name);
            //Adds the variables contained in variableEnum to variableList
            while (variableEnum.MoveNext())
            {
                variableList.Add(variableEnum.Current);
            }
            return variableList;
        }

        /// <summary>
        /// If name is invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes text.  The method returns a
        /// list consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        public override IList<string> SetCellContents(string name, string text)
        {
            //Checks if name is valid, if not throw InvalidNameException
            if (!(isValid(name)))
                throw new InvalidNameException();

            //If cells contains the name, change the value
            if (cells.ContainsKey(name))
                cells[name] = text;
            //If cells does not contain name, adds the name and value to cells
            else
                cells.Add(name, text);

            //Creates an enumerator of the variables that name is a dependent of
            IEnumerator<string> variableEnum = depList.GetDependees(name).GetEnumerator();
            //Creates a list of variables to build
            IList<string> variableList = new List<string>();
            variableList.Add(name);
            //Adds the variables contained in variableEnum to variableList
            while (variableEnum.MoveNext())
            {
                variableList.Add(variableEnum.Current);
            }
            return variableList;
        }

        /// <summary>
        /// If name is invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if changing the contents of the named cell to be the formula would cause a 
        /// circular dependency, throws a CircularException, and no change is made to the spreadsheet.
        /// 
        /// Otherwise, the contents of the named cell becomes formula.  The method returns a
        /// list consisting of name plus the names of all other cells whose value depends,
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        public override IList<string> SetCellContents(string name, Formula formula)
        {
            //Checks if name is valid, if not throw InvalidNameException
            if (!(isValid(name)))
                throw new InvalidNameException();
            //Creates an enumerator of the variables that name is a dependent of
            IEnumerator<string> variableEnum = depList.GetDependees(name).GetEnumerator();
            //Creates a list of variables to build
            IList<string> variableList = new List<string>();
            //Adds the variables contained in variableEnum to variableList
            while (variableEnum.MoveNext())
            {
                variableList.Add(variableEnum.Current);
            }
            //If variableList contains name, throw CircularExpression
            if (formula.GetVariables().Contains(name))
                throw new CircularException();
            //If variables within the formula have dependency to the name, throw CircularExpression
            foreach (string variable in variableList)
            {
                if (depList.GetDependents(variable).Contains(name))
                    throw new CircularException();
            }
            //Add variables in formula to dependencyList
            foreach (string variable in formula.GetVariables())
            {
                depList.AddDependency(name, variable);
            }
            //inserts name into variable list at the begining
            variableList.Insert(0, name);
            //If cells contains the name, change the value
            if (cells.ContainsKey(name))
                cells[name] = formula.ToString();
            //If cells does not contain name, adds the name and value to cells
            else
                cells.Add(name, formula.ToString());
            //Returns final variableList
            return variableList;
        }

        /// <summary>
        /// Returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell.  In other words, returns
        /// an enumeration, without duplicates, of the names of all cells that contain
        /// formulas containing name.
        /// 
        /// For example, suppose that
        /// A1 contains 3
        /// B1 contains the formula A1 * A1
        /// C1 contains the formula B1 + A1
        /// D1 contains the formula B1 - C1
        /// The direct dependents of A1 are B1 and C1
        /// </summary>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            //Checks if name is valid, otherwise throws a InvalidNameException
            if(!isValid(name))
                throw new InvalidNameException();
            //Starts a build with name
            yield return name;
            //Creates an enumerator of names dependents
            IEnumerator<string> depEnum = depList.GetDependents(name).GetEnumerator();
            //Builds the IEnumerable with all of names dependents
            while (depEnum.MoveNext())
                yield return depEnum.Current;
        }
    }
}
