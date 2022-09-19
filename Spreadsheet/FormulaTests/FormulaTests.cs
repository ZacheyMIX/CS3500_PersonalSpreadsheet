using SpreadsheetUtilities;
using System;

namespace FormulaTests

{
    [TestClass]
    public class FormulaTests
    {
        //Hello
        [TestMethod]
        public void testEquals()
        {
            Formula t = new Formula("32 + A32_C3_F * _A983BC_", s => s, s => true);
            Formula t2 = new Formula("32 + A32_C3_F * _A983BC_", s => s, s => true);
            Assert.IsTrue(t.Equals(t2));
            Assert.IsTrue(t2.Equals(t));
            Assert.IsTrue(t == t2);
            Assert.IsTrue(t2 == t);
            Assert.IsFalse(t != t2);
            Assert.IsFalse(t2 != t);
        }

        [TestMethod]
        public void testEqualsWithDouble()
        {
            Formula t = new Formula("32 + A32_C3_F * _A983BC_", s => s, s => true);
            Formula t2 = new Formula("32.00 + A32_C3_F * _A983BC_", s => s, s => true);
            Assert.IsTrue(t.Equals(t2));
            Assert.IsTrue(t2.Equals(t));
            Assert.IsTrue(t == t2);
            Assert.IsTrue(t2 == t);
            Assert.IsFalse(t != t2);
            Assert.IsFalse(t2 != t);
        }

        [TestMethod]
        public void testUnEquals()
        {
            Formula t = new Formula("32 + A32_C3_F * _A983BC_", s => s, s => true);
            Formula t2 = new Formula("32.001 + A32_C3_F * _A983BC_", s => s, s => true);
            Assert.IsFalse(t.Equals(t2));
            Assert.IsFalse(t2.Equals(t));
            Assert.IsFalse(t == t2);
            Assert.IsFalse(t2 == t);
            Assert.IsTrue(t != t2);
            Assert.IsTrue(t2 != t);
        }

        [TestMethod]
        public void testUnEqualHashCode()
        {
            Formula t = new Formula("32 + A32_C3_F * _A983BC_", s => s, s => true);
            Formula t2 = new Formula("32.00 + A32_C3_F * _A983BC", s => s, s => true);
            Assert.IsTrue(t.GetHashCode() != t2.GetHashCode());
            Assert.IsTrue(t2.GetHashCode() != t.GetHashCode());
            Assert.IsFalse(t.GetHashCode() == t2.GetHashCode());
            Assert.IsFalse(t2.GetHashCode() == t.GetHashCode());
        }

        [TestMethod]
        public void testEqualHashCode()
        {
            Formula t = new Formula("32 + A32_C3_F * _A983BC_", s => s, s => true);
            Formula t2 = new Formula("32.00 + A32_C3_F * _A983BC_", s => s, s => true);
            Assert.IsTrue(t.GetHashCode() == t2.GetHashCode());
            Assert.IsTrue(t2.GetHashCode() == t.GetHashCode());
            Assert.IsFalse(t.GetHashCode() != t2.GetHashCode());
            Assert.IsFalse(t2.GetHashCode() != t.GetHashCode());
        }

        //[TestMethod]
        //public void getVariablesTest()
        //{
        //    Formula t = new Formula("32 + A32_C3_F * _A983BC_", s => s, s => true);
        //    List<String> test = new List<String>();
        //    IEnumerable<String> variables = t.GetVariables();
        //    Assert.AreEqual(test, variables);
        //}

        [TestMethod]
        public void validFormulaTest2()
        {
            Formula t = new Formula("(32 + A32_C3_F) * _A983BC_", s => s, s => true);
        }

        [TestMethod]
        public void validFormulaTest3()
        {
            Formula t = new Formula("A32_C3_F + 32 * _A983BC_", s => s, s => true);
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void badParanthesisTest()
        {
            Formula t = new Formula("((32 + 3)", s => s, s => true);
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void badClosingToken()
        {
            Formula t = new Formula("((32 + 3)*", s => s, s => true);
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void badParanthesisFormulaTest()
        {
            Formula t = new Formula("(5 + (AB / 7)", s => s, s => true);
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void badClosingParenthesisTest()
        {
            Formula t = new Formula("(3 + A3) + C2) + 3", s => s, s => true);
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void emptyTest()
        {
            Formula t = new Formula("", s => s, s => true);
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void startingTokenTest()
        {
            Formula t = new Formula("*3(2", s => s, s => true);  
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void closingTokenTest()
        {
            Formula t = new Formula("3 + ( + C2 + (", s => s, s => true);
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void invalidTokenTest()
        {
            Formula t = new Formula("3 + ($ + C2) + 3", s => s, s => true);
        }

        [TestMethod()]
        public void TestSingleNumber()
        {
            Formula t = new Formula("5", s => s, s => true);
            Assert.AreEqual(5.0, t.Evaluate(s => 0));
        }

        [TestMethod()]
        public void TestSingleVariable()
        {
            Formula t = new Formula("X5", s => s, s => true);
            Assert.AreEqual(13.0, t.Evaluate(s => 13));
        }

        [TestMethod()]
        public void TestAddition()
        {
            Formula t = new Formula("5+3", s => s, s => true);
            Assert.AreEqual(8.0, t.Evaluate(s => 0));
        }

        [TestMethod()]
        public void TestSubtraction()
        {
            Formula t = new Formula("18-10", s => s, s => true);
            Assert.AreEqual(8.0, t.Evaluate( s => 0));
        }

        [TestMethod()]
        public void TestMultiplication()
        {
            Formula t = new Formula("2*4", s => s, s => true);
            Assert.AreEqual(8.0, t.Evaluate(s => 0));
        }

        [TestMethod()]
        public void TestDivision()
        {
            Formula t = new Formula("16/2", s => s, s => true);
            Assert.AreEqual(8.0, t.Evaluate(s => 0));
        }

        [TestMethod()]
        public void TestArithmeticWithVariable()
        {
            Formula t = new Formula("2+X1", s => s, s => true);
            Assert.AreEqual(6.0, t.Evaluate(s => 4));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestUnknownVariable()
        {
            Formula t = new Formula("2+X1", s => s, s => true);
            t.Evaluate(s => { throw new ArgumentException("Unknown variable"); });
        }

        [TestMethod()]
        public void TestLeftToRight()
        {
            Formula t = new Formula("2*6+3", s => s, s => true);
            Assert.AreEqual(15.0, t.Evaluate(s => 0));
        }

        [TestMethod()]
        public void TestOrderOperations()
        {
            Formula t = new Formula("2+6*3", s => s, s => true);
            Assert.AreEqual(20.0, t.Evaluate(s => 0));
        }

        [TestMethod()]
        public void TestParenthesesTimes()
        {
            Formula t = new Formula("(2+6)*3", s => s, s => true);
            Assert.AreEqual(24.0, t.Evaluate(s => 0));
        }

        [TestMethod()]
        public void TestTimesParentheses()
        {
            Formula t = new Formula("2*(3+5)", s => s, s => true);
            Assert.AreEqual(16.0, t.Evaluate(s => 0));
        }

        [TestMethod()]
        public void TestPlusParentheses()
        {
            Formula t = new Formula("2+(3+5)", s => s, s => true);
            Assert.AreEqual(10.0, t.Evaluate(s => 0));
        }

        [TestMethod()]
        public void TestPlusComplex()
        {
            Formula t = new Formula("2+(3+5*9)", s => s, s => true);
            Assert.AreEqual(50.0, t.Evaluate(s => 0));
        }

        [TestMethod()]
        public void TestOperatorAfterParens()
        {
            Formula t = new Formula("(1*1)-2/2", s => s, s => true);
            Assert.AreEqual(0.0, t.Evaluate(s => 0));
        }

        [TestMethod()]
        public void TestComplexTimesParentheses()
        {
            Formula t = new Formula("2+3*(3+5)", s => s, s => true);
            Assert.AreEqual(26.0, t.Evaluate(s => 0));
        }

        [TestMethod()]
        public void TestComplexAndParentheses()
        {
            Formula t = new Formula("2+3*5+(3+4*8)*5+2", s => s, s => true);
            Assert.AreEqual(194.0, t.Evaluate(s => 0));
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestDivideByZero()
        {
            Formula t = new Formula("5/0", s => s, s => true);
            t.Evaluate(s => 0);
        }

        [TestMethod()]
        public void TestToString()
        {
            Formula t = new Formula("2+3*5+(3+4*8)*5+2", s => s, s => true);
            Assert.AreEqual("2+3*5+(3+4*8)*5+2", t.ToString());
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestSingleOperator()
        {
            Formula t = new Formula("+", s => s, s => true);
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestExtraOperator()
        {
            Formula t = new Formula("2+5+", s => s, s => true);
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestExtraParentheses()
        {
            Formula t = new Formula("2+5*7)", s => s, s => true);
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestParensNoOperator()
        {
            Formula t = new Formula("5+7+(5)8", s => s, s => true);
        }

        [TestMethod()]
        public void TestComplexMultiVar()
        {
            Formula t = new Formula("y1*3-8/2+4*(8-9*2)/80*x7", s => s, s => true);
            Assert.AreEqual(7.5, t.Evaluate(s => (s == "x7") ? 1 : 4));
        }

        [TestMethod()]
        public void TestComplexNestedParensRight()
        {
            Formula t = new Formula("x1+(x2+(x3+(x4+(x5+x6))))", s => s, s => true);
            Assert.AreEqual(6.0, t.Evaluate(s => 1));
        }

        [TestMethod()]
        public void TestComplexNestedParensLeft()
        {
            Formula t = new Formula("((((x1+x2)+x3)+x4)+x5)+x6", s => s, s => true);
            Assert.AreEqual(12.0, t.Evaluate(s => 2));
        }

        [TestMethod()]
        public void TestRepeatedVar()
        {
            Formula t = new Formula("a4-a4*a4/a4", s => s, s => true);
            Assert.AreEqual(0.0, t.Evaluate(s => 3));
        }
    }
}