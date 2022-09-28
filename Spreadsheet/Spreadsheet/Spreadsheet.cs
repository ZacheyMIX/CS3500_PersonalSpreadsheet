using SpreadsheetUtilities;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace SS
{
    public class Spreadsheet : AbstractSpreadsheet
    {
        DependencyGraph depList;
        Cell cells;
        Func<string, string> normalize;
        Func<string, bool> isValid;
        string version;


        public override bool Changed { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }



        /// <summary>
        /// Generates a spreadsheet with a dependency list and a dictionary of strings called list
        /// </summary>
        public Spreadsheet()
        {
            depList = new DependencyGraph();
            cells = new Cell();
        }

        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version)
        {
            this.isValid = isValid;
            this.version = version;
            this.normalize = normalize;
        }

        /// <summary>
        /// The private helper method for checking if a cell given has a valid name
        /// A valid name must start with a letter or an underscore and must only
        /// consist of leters, numbers, and underscores.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool isValidName(string name)
        {
            if (Regex.IsMatch(name, @"^[a-zA-Z]") && Regex.IsMatch(name, @"[a-zA-Z0-9]"))
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
            if (!(isValidName(name)))
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
                if (!(cells.GetCellContent(name) is ""))
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
        protected override IList<string> SetCellContents(string name, double number)
        {
            //Checks if name is valid
            if (!(isValidName(name)))
                throw new InvalidNameException();

            IEnumerator<string> dependents = depList.GetDependents(name).GetEnumerator();
            while (dependents.MoveNext())
            {
                depList.RemoveDependency(name, dependents.Current);
            }

            //Either adds a new entry to cell or replaces an existing ones content
            cells.SetCellContent(name, number);
            
            

            //Generates a list of the name and all the variables it is dependent on
            IList<string> variableList = new List<string>();
            IEnumerator<string> variableEnum = GetCellsToRecalculate(name).GetEnumerator();
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
        protected override IList<string> SetCellContents(string name, string text)
        {
            //Checks if name is valid
            if (!(isValidName(name)))
                throw new InvalidNameException();

            IEnumerator<string> dependents = depList.GetDependents(name).GetEnumerator();
            while (dependents.MoveNext())
            {
                depList.RemoveDependency(dependents.Current, name);
            }

            //Either puts in a new name and content into cells or replaces an existing names content with new content
            cells.SetCellContent(name, text);
            

            //Generates a list of variables of name and its dependees          
            IList<string> variableList = new List<string>();
            IEnumerator<string> variableEnum = GetCellsToRecalculate(name).GetEnumerator();
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
        protected override IList<string> SetCellContents(string name, Formula formula)
        {
            //Checks if the name is valid
            if (!(isValidName(name)))
                throw new InvalidNameException();

            //Checks if formula contains name
            if (formula.GetVariables().Contains(name))
                throw new CircularException();

            //Stores the current content in the cell before the change
            Object storedContent = cells.GetCellContent(name);

            //Either adds a new name and its content to cells or replaces an already existing cells content with new content
            //Then either replaces all variables in deplist with name
            cells.SetCellContent(name, formula);
            foreach (string variable in formula.GetVariables())
            {
                depList.AddDependency(name, variable);
            }

            //Generates a list of the name and its dependees
            IEnumerator<string> variableEnum;
            IList<String> variableList = new List<String>();
            try
            {
                variableEnum = GetCellsToRecalculate(name).GetEnumerator();
            }
            catch (CircularException e)
            {
                if (storedContent is Double)
                    cells.SetCellContent(name, (double)storedContent);
                else if (storedContent is Formula)
                    cells.SetCellContent(name, (Formula)storedContent);
                else {
                    cells.SetCellContent(name, (string)storedContent);
                }

                foreach (string variable in formula.GetVariables())
                {
                    depList.RemoveDependency(name, variable);
                }
                throw e;
            }   
            while (variableEnum.MoveNext())
            {
                variableList.Add(variableEnum.Current);
            }

            return variableList;

        }

        /// <summary>
        /// 
        /// Requires that name be a valid cell name.
        /// 
        /// If the cell referred to by name is involved in a circular dependency,
        /// throws a CircularException.
        /// 
        /// Otherwise, returns an enumeration of the names of all cells whose values must
        /// be recalculated, assuming that the contents of the cell referred to by name has changed.
        /// The cell names are enumerated in an order in which the calculations should be done.  
        /// 
        /// For example, suppose that 
        /// A1 contains 5
        /// B1 contains the formula A1 + 2
        /// C1 contains the formula A1 + B1
        /// D1 contains the formula A1 * 7
        /// E1 contains 15
        /// 
        /// If A1 has changed, then A1, B1, C1, and D1 must be recalculated,
        /// and they must be recalculated in an order which has A1 first, and B1 before C1
        /// (there are multiple such valid orders).
        /// The method will produce one of those enumerations.
        /// 
        /// PLEASE NOTE THAT THIS METHOD DEPENDS ON THE ABSTRACT METHOD GetDirectDependents.
        /// IT WON'T WORK UNTIL GetDirectDependents IS IMPLEMENTED CORRECTLY.
        /// </summary>
        protected IEnumerable<string> GetCellsToRecalculate(string name)
        {
            LinkedList<string> changed = new LinkedList<string>();
            HashSet<string> visited = new HashSet<string>();
            Visit(name, name, visited, changed);
            return changed;
        }
        /// <summary>
        /// This method is a helper method for GetCellsToRecalulate
        /// It visits all dependents of a variable and checks for any CircularDependency
        /// This method is recursive as it will return back with the next dependent from the initial name
        /// </summary>
        /// <param name="start"></param>
        /// <param name="name"></param>
        /// <param name="visited"></param>
        /// <param name="changed"></param>
        /// <exception cref="CircularException"></exception>
        private void Visit(string start, string name, ISet<string> visited, LinkedList<string> changed)
        {
            visited.Add(name);
            foreach (string n in GetDirectDependents(name))
            {
                if (n.Equals(start))
                {
                    throw new CircularException();
                }
                else if (!visited.Contains(n))
                {
                    Visit(start, n, visited, changed);
                }
            }
            changed.AddFirst(name);
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

        /// <summary>
        /// Writes the contents of this spreadsheet to the named file using a JSON format.
        /// The JSON object should have the following fields:
        /// "Version" - the version of the spreadsheet software (a string)
        /// "cells" - an object containing 0 or more cell objects
        ///           Each cell object has a field named after the cell itself 
        ///           The value of that field is another object representing the cell's contents
        ///               The contents object has a single field called "stringForm",
        ///               representing the string form of the cell's contents
        ///               - If the contents is a string, the value of stringForm is that string
        ///               - If the contents is a double d, the value of stringForm is d.ToString()
        ///               - If the contents is a Formula f, the value of stringForm is "=" + f.ToString()
        /// 
        /// For example, if this spreadsheet has a version of "default" 
        /// and contains a cell "A1" with contents being the double 5.0 
        /// and a cell "B3" with contents being the Formula("A1+2"), 
        /// a JSON string produced by this method would be:
        /// 
        /// {
        ///   "cells": {
        ///     "A1": {
        ///       "stringForm": "5"
        ///     },
        ///     "B3": {
        ///       "stringForm": "=A1+2"
        ///     }
        ///   },
        ///   "Version": "default"
        /// }
        /// 
        /// If there are any problems opening, writing, or closing the file, the method should throw a
        /// SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public override void Save(string filename)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// If name is invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the value (as opposed to the contents) of the named cell.  The return
        /// value should be either a string, a double, or a SpreadsheetUtilities.FormulaError.
        /// </summary>
        public override object GetCellValue(string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// If name is invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if content parses as a double, the contents of the named
        /// cell becomes that double.
        /// 
        /// Otherwise, if content begins with the character '=', an attempt is made
        /// to parse the remainder of content into a Formula f using the Formula
        /// constructor.  There are then three possibilities:
        /// 
        ///   (1) If the remainder of content cannot be parsed into a Formula, a 
        ///       SpreadsheetUtilities.FormulaFormatException is thrown.
        ///       
        ///   (2) Otherwise, if changing the contents of the named cell to be f
        ///       would cause a circular dependency, a CircularException is thrown,
        ///       and no change is made to the spreadsheet.
        ///       
        ///   (3) Otherwise, the contents of the named cell becomes f.
        /// 
        /// Otherwise, the contents of the named cell becomes content.
        /// 
        /// If an exception is not thrown, the method returns a list consisting of
        /// name plus the names of all other cells whose value depends, directly
        /// or indirectly, on the named cell. The order of the list should be any
        /// order such that if cells are re-evaluated in that order, their dependencies 
        /// are satisfied by the time they are evaluated.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        public override IList<string> SetContentsOfCell(string name, string content)
        {
            throw new NotImplementedException();
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
        Dictionary<string, object> cell;

        /// <summary>
        /// The cell constructor that creates a new dictionary of strings
        /// </summary>
        public Cell()
        {
            cell = new Dictionary<string, object>();
        }

        public Dictionary<string, object> Name
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
                cell[name] = num;
            else
                cell.Add(name, num);
        }

        /// <summary>
        /// Sets the named cells content if a Formula is passed in
        /// </summary>
        /// <param name="name"></param>
        /// <param name="formula"></param>
        public void SetCellContent(string name, Formula formula)
        {
            if (cell.ContainsKey(name))
                cell[name] = formula;
            else
                cell.Add(name, formula);
        }

        /// <summary>
        /// This method grabs the content of named cell and returns it in its original form
        /// string, double, or Formula
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object GetCellContent(string name)
        {
            //Checks if name is contained in cells, returns empty string otherwise
            if (!cell.ContainsKey(name))
                return "";

            object test = cell[name];
            //Checks type of content and returns accordingly
            if (cell[name] is Double)
                return (Double) cell[name];
            else if (cell[name] is Formula)
                return (Formula) cell[name];
            else
                return (String) cell[name];
        }

        /// <summary>
        /// This method returns the value of the named cells content
        /// either a string, a double, or a FormulaError
        /// </summary>
        /// <param name="name"></param>
        /// <param name="lookup"></param>
        /// <returns></returns>
        //public object GetCellValue(string name, Func<string, double> lookup)
        //{
        //    //Double integer for tryParse
        //    double n;

        //    //Checks if name is contained in cells, returns empty string otherwise
        //    if (!cell.ContainsKey(name))
        //        return "";
        //    //Checks type of content and returns accordingly
        //    if (cell[name] is double)
        //        return (double)cell[name];
        //    else if (cell[name] is Formula)
        //        Formula content = new Formula(cell[name].ToString());
        //        return cell[name].Evaluate(lookup);
        //    else
        //        return cell[name];
        //}
    }
}
