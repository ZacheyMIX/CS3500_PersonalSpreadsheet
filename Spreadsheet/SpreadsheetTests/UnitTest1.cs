using SS;
using SpreadsheetUtilities;
using System;

namespace SpreadsheetTests
{
    [TestClass]
    public class UnitTest1
    {
        /// <summary>
        /// Tests the invalid name exception in GetCell
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetCellInvalidName()
        {
            Spreadsheet test = new Spreadsheet();
            test.GetCellContents("7");
        }

        /// <summary>
        /// Tests if a cell is an empty
        /// </summary>
        [TestMethod]
        public void TestGetCellEmpty()
        {
            Spreadsheet test = new Spreadsheet();
            Assert.AreEqual("", test.GetCellContents("A2"));
        }

        /// <summary>
        /// This tests the private method isFormula by adding a formula to spreadsheet
        /// then grabbing the value at cell that was put in
        /// If it passes isFormula a formula will be returned
        /// </summary>
        [TestMethod]
        public void TestGetCellContentsIsFormula()
        {
            Spreadsheet test = new Spreadsheet();
            Formula formulaTest = new Formula("a2 + b2");
            test.SetCellContents("b1", new Formula("a2 + b2"));
            Assert.AreEqual(formulaTest, test.GetCellContents("b1"));
        }

        /// <summary>
        /// This method tests the InvalidNameException within SetCell of Formula value
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellFormulaInvalidName()
        {
            Spreadsheet test = new Spreadsheet();
            test.SetCellContents("7", new Formula("a2 + b2"));
        }

        /// <summary>
        /// This method tests the first circular error within SetCell of Formula Value
        /// by creating a formula that has the name of the cell being added
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void TestSetCellFormulaCircularErrorWithinFormula()
        {
            Spreadsheet test = new Spreadsheet();
            test.SetCellContents("b1", new Formula("b1 + b2"));
        }

        /// <summary>
        /// This method tests the second circular error within SetCell of Formula Value
        /// by creating 2 formulas, 1 reliant on 1 while the other is reliant other
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void TestSetCellFormulaCircularErrorWithinDependencies()
        {
            Spreadsheet test = new Spreadsheet();
            test.SetCellContents("b1", new Formula("a1"));
            test.SetCellContents("a1", new Formula("b1 + b2"));
        }

        /// <summary>
        /// This method tests the ILists produced from SetCell of Formula Value by creating 1 cell, and comparing
        /// with an empty IList and creating another cell that is reliant on the first cell and matching the
        /// IList of a1List
        /// </summary>
        [TestMethod]
        public void TestSetCellFormulaList()
        {
            Spreadsheet test = new Spreadsheet();
            IList<String> b1List = new List<String>();
            IList<String> a1List = new List<String>();
            a1List.Add("a1");
            a1List.Add("b1");
            Assert.AreEqual(b1List.ToString(), test.SetCellContents("b1", new Formula("a2 + b2")).ToString());
            Assert.AreEqual(a1List.ToString(), test.SetCellContents("a1", new Formula("b1 + b2")).ToString());
        }

        /// <summary>
        /// This method tests the replace function of SetCell of formula value by creating two cells of the same name
        /// and checking its contents each time
        /// </summary>
        [TestMethod]
        public void TestSetCellReplaceFormula()
        {
            Spreadsheet test = new Spreadsheet();
            IList<String> a1List = new List<String>();
            a1List.Add("a1");
            test.SetCellContents("a1", new Formula("a2 + b2"));
            Assert.AreNotEqual(new Formula("b1+b2"), test.GetCellContents("a1"));
            test.SetCellContents("a1", new Formula("b1 + b2"));
            Assert.AreEqual(new Formula("b1+b2"), test.GetCellContents("a1"));
        }

        /// <summary>
        /// This method tests the InvalidNameException within SetCell of Double value
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellDoubleInvalidName()
        {
            Spreadsheet test = new Spreadsheet();
            test.SetCellContents("7", 32.0);
        }

        /// <summary>
        /// This method tests the IList formed from SetCell of double value
        /// </summary>
        [TestMethod]
        public void TestSetCellDouble()
        {
            Spreadsheet test = new Spreadsheet();
            IList<String> a1List = new List<String>();
            IList<String> b1List = new List<String>();
            b1List.Add("b1");
            b1List.Add("a1");
            Assert.AreNotEqual(a1List.ToString(), test.SetCellContents("a1", 32.0).ToString());
            Assert.AreEqual(b1List.ToString(), test.SetCellContents("b1", 32.0).ToString());
        }

        //This method tests a replace value on the same cell if the same cell is set multiple times
        [TestMethod]
        public void TestSetCellReplaceDouble()
        {
            Spreadsheet test = new Spreadsheet();
            test.SetCellContents("a1", new Formula("a2 + b2"));
            Assert.AreNotEqual(32.0, test.GetCellContents("a1"));
            test.SetCellContents("a1", 32.0);
            Assert.AreEqual(32.0, test.GetCellContents("a1"));
        }

        /// <summary>
        /// This method tests the invalid name exception for Set Cell of value string
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellStringInvalidName()
        {
            Spreadsheet test = new Spreadsheet();
            test.SetCellContents("7", new Formula("a2 + b2"));
        }

        /// <summary>
        /// This method tests the ILists returned from SetCell of value string
        /// </summary>
        [TestMethod]
        public void TestSetCellStringList()
        {
            Spreadsheet test = new Spreadsheet();
            IList<String> a1List = new List<String>();
            IList<String> b1List = new List<String>();
            b1List.Add("b1");
            b1List.Add("a1");
            test.SetCellContents("a1", new Formula("b1 + b2"));
            Assert.AreNotEqual(a1List.ToString(), test.SetCellContents("a1", "Test").ToString());
            test.SetCellContents("b1", "Test");
            Assert.AreEqual(b1List.ToString(), test.SetCellContents("a1", "Test").ToString());
        }

        /// <summary>
        /// This method tests a replace function if two variables of the same name but different content are 
        /// called in setCell of value string
        /// </summary>
        [TestMethod]
        public void TestSetCellReplaceString()
        {
            Spreadsheet test = new Spreadsheet();
            test.SetCellContents("a1", new Formula("a2 + b2"));
            Assert.AreNotEqual("Test", test.GetCellContents("a1"));
            test.SetCellContents("a1", "Test");
            Assert.AreEqual("Test", test.GetCellContents("a1"));
        }
    }
}