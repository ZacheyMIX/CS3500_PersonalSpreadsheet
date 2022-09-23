using SpreadsheetUtilities;
using System.Text.RegularExpressions;

namespace SS
{
    public class Spreadsheet : AbstractSpreadsheet
    {
        DependencyGraph depList;
        Dictionary<string, string> cells;

        /// <summary>
        /// Generates a spreadsheet with a dependency list and a dictionary of strings called list
        /// </summary>
        public Spreadsheet()
        {
            depList = new DependencyGraph();
            cells = new Dictionary<string, string>();
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
            //Double integer for tryParse
            double n;

            //Checks if name is valid
            if (!isValid(name))
                throw new InvalidNameException();

            //Checks if name is contained in cells, returns empty string otherwise
            if (!cells.ContainsKey(name))
                return "";

            //Checks type of content and returns accordingly
            if (double.TryParse(cells[name], out n))
                return n;
            else if (isFormula(cells[name]))
                return new Formula(cells[name]);
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
            if (!Regex.IsMatch(value, @"[+/*-]"))
                return false;
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
            //Checks if name is valid
            if (!(isValid(name)))
                throw new InvalidNameException();

            //Either adds a new entry to cell or replaces an existing ones content
            if (cells.ContainsKey(name))
                cells[name] = number.ToString();
            else
                cells.Add(name, number.ToString());

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
            if (cells.ContainsKey(name))
                cells[name] = text;
            else
                cells.Add(name, text);

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
            if (cells.ContainsKey(name))
            {
                cells[name] = formula.ToString();
                depList.ReplaceDependents(name, formula.GetVariables());
            }
            else {
                cells.Add(name, formula.ToString());
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
}
