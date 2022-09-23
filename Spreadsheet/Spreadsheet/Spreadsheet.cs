using SpreadsheetUtilities;
using System.Text.RegularExpressions;

namespace SS
{
    public class Spreadsheet : AbstractSpreadsheet
    {
        DependencyGraph depList;
        Cell cells;

        /// <summary>
        /// Generates a spreadsheet with a dependency list and a dictionary of strings called list
        /// </summary>
        public Spreadsheet()
        {
            depList = new DependencyGraph();
            cells = new Cell();
        }

        /// <summary>
        /// The private helper method for checking if a cell given has a valid name
        /// A valid name must start with a letter or an underscore and must only
        /// consist of leters, numbers, and underscores.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool isValid(string name)
        {
            if (Regex.IsMatch(name, @"^[a-zA-Z_]") && Regex.IsMatch(name, @"[a-zA-Z0-9._]"))
                return true;
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
            //Checks if name is valid
            if (!(isValid(name)))
                throw new InvalidNameException();

            //Returns the named cells content in its original form
            //string, double, or Formula
            return cells.GetCellContent(name);
        }

        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>

        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            foreach(string name in cells.Name.Keys)
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
            //Checks if name is valid
            if (!(isValid(name)))
                throw new InvalidNameException();

            //Either adds a new entry to cell or replaces an existing ones content
            cells.SetCellContent(name, number);

            //Generates a list of variables of name and its dependees 
            IEnumerator<string> variableEnum = GetDirectDependents(name).GetEnumerator();
            IList<string> variableList = new List<string>();
            variableList.Add(name);
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
            //Checks if name is valid
            if (!(isValid(name)))
                throw new InvalidNameException();

            //Either puts in a new name and content into cells or replaces an existing names content with new content
            cells.SetCellContent(name, text);

            //Generates a list of variables of name and its dependees 
            IEnumerator<string> variableEnum = GetDirectDependents(name).GetEnumerator();
            IList<string> variableList = new List<string>();
            variableList.Add(name);
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
            //Checks if the name is valid
            if (!(isValid(name)))
                throw new InvalidNameException();

            //Checks if formula contains name
            if (formula.GetVariables().Contains(name))
                throw new CircularException();

            //Checks if variables in formula are dependent of name
            foreach (string variable in formula.GetVariables())
            {
                if (depList.GetDependents(variable).Contains(name))
                    throw new CircularException();
            }

            //Either adds a new name and its content to cells or replaces an already existing cells content with new content
            //Then either adds dependencies or replaces them
            cells.SetCellContent(name, formula);
            if (cells.Name.ContainsKey(name))
            {
                depList.ReplaceDependents(name, formula.GetVariables());
            }
            else {
                foreach (string variable in formula.GetVariables())
                {
                    depList.AddDependency(name, variable);
                }
            }

            //Generates a list of variables of name and its dependees 
            IEnumerator<string> variableEnum = GetDirectDependents(name).GetEnumerator();
            IList<string> variableList = new List<string>();
            variableList.Add(name);
            while (variableEnum.MoveNext())
            {
                variableList.Add(variableEnum.Current);
            }

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
            IEnumerator<string> depEnum = depList.GetDependees(name).GetEnumerator();

            while (depEnum.MoveNext())
                yield return depEnum.Current;
        }
    }

    /// <summary>
    /// A Cell class that represents a cell within a spreadsheet.
    /// A Cell contains a name, its content, and its value.
    /// Content consists of a string, a double, or a Formula
    /// Value consists of a string, a double, or a FormulaError
    /// </summary>
    public class Cell
    {
        Dictionary<string, string> cell;

        /// <summary>
        /// The cell constructor that creates a new dictionary of strings
        /// </summary>
        public Cell()
        {
            cell = new Dictionary<string, string>();
        }

        public Dictionary<string, string> Name
        {
            get { return cell; }
        }

        /// <summary>
        /// Sets the named cells content if a string is passed in
        /// </summary>
        /// <param name="name"></param>
        /// <param name="text"></param>
        public void SetCellContent(string name, string text)
        {
            if (cell.ContainsKey(name))
                cell[name] = text;
            else
                cell.Add(name, text);
        }

        /// <summary>
        /// Sets the named cells content if a double is passed in 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="num"></param>
        public void SetCellContent(string name, double num)
        {
            if (cell.ContainsKey(name))
                cell[name] = num.ToString();
            else
                cell.Add(name, num.ToString());
        }

        /// <summary>
        /// Sets the named cells content if a Formula is passed in
        /// </summary>
        /// <param name="name"></param>
        /// <param name="formula"></param>
        public void SetCellContent(string name, Formula formula)
        {
            if (cell.ContainsKey(name))
                cell[name] = formula.ToString();
            else
                cell.Add(name, formula.ToString());
        }

        /// <summary>
        /// This method grabs the content of named cell and returns it in its original form
        /// string, double, or Formula
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object GetCellContent(string name)
        {
            //Double integer for tryParse
            double n;

            //Checks if name is contained in cells, returns empty string otherwise
            if (!cell.ContainsKey(name))
                return "";

            //Checks type of content and returns accordingly
            if (double.TryParse(cell[name], out n))
                return n;
            else if (isFormula(cell[name]))
                return new Formula(cell[name]);
            else
                return cell[name];
        }

        /// <summary>
        /// This method returns the value of the named cells content
        /// either a string, a double, or a FormulaError
        /// </summary>
        /// <param name="name"></param>
        /// <param name="lookup"></param>
        /// <returns></returns>
        public object GetCellValue(string name, Func<string, double> lookup)
        {
            //Double integer for tryParse
            double n;

            //Checks if name is contained in cells, returns empty string otherwise
            if (!cell.ContainsKey(name))
                return "";
            //Checks type of content and returns accordingly
            if (double.TryParse(cell[name], out n))
                return n;
            else if (isFormula(cell[name]))
                return new Formula(cell[name]).Evaluate(lookup);
            else
                return cell[name];
        }

        /// <summary>
        /// Private helper method to check if a string is a formula
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool isFormula(string value)
        {
            if (!Regex.IsMatch(value, @"[+/*-]"))
                return false;
            return true;
        }
    }
}
