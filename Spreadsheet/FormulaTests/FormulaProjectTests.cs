// <copyright file="FormulaSyntaxTests.cs" company="UofU-CS3500">
//   Copyright 2025 UofU-CS3500. All rights reserved.
// </copyright>
// <authors> [Leo Yu] </authors>
// <date> [Feb 7, 2025] </date>

namespace CS3500.FormulaTests;

using CS3500.Formula;

/// <summary>
///   <para>
///     This class demonstrates usage of the MSTest framework to validate
///     the syntax rules of the <see cref="Formula"/> class. Each test
///     method either ensures the constructor accepts valid expressions or
///     throws a <see cref="FormulaFormatException"/> for invalid expressions.
///   </para>
///   <para>
///     The tests here are organized by major syntax rules, such as:
///     <list type="bullet">
///       <item>One Token Rule</item>
///       <item>Valid Token Rule</item>
///       <item>Closing Parenthesis Rule</item>
///       <item>Balanced Parentheses Rule</item>
///       <item>First Token Rule</item>
///       <item>Last Token Rule</item>
///       <item>Parenthesis/Operator Following Rule</item>
///       <item>Extra Following Rule</item>
///     </list>
///     In addition, this file includes tests for the <see cref="Formula.ToString"/> and
///     <see cref="Formula.GetVariables"/> methods.
///   </para>
/// </summary>
[TestClass]
public class FormulaSyntaxTests
{
    // --- Tests for One Token Rule ---
    
    /// <summary>
    ///   <para>
    ///     This test makes sure the right kind of exception is thrown
    ///     when trying to create a formula with no tokens.
    ///   </para>
    /// </summary>
    [TestMethod]
    [ExpectedException( typeof( FormulaFormatException ) )]
    public void FormulaConstructor_TestNoTokens_Invalid( )
    {
        _ = new Formula( string.Empty );
    }
    
    [TestMethod]
    [ExpectedException( typeof( FormulaFormatException ) )]
    public void FormulaConstructor_TestNoTokensButSpaces_Invalid( )
    {
        _ = new Formula("  ");
    }
     
    [TestMethod]
    public void FormulaConstructor_TestNumberOneToken_Valid( )
    {
        _ = new Formula( "1" );
    }
    
    /// <summary>
    ///  <para>
    ///     This test makes sure the right kind of exception is thrown
    ///     when trying to create a formula with incorrect singular "a" variable token.
    ///   </para>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestIncorrectSingleVariableToken_Invalid()
    {
        _ = new Formula( "a" );
    }

    // --- Tests for Valid Token Rule ---
    // The following operator tests also cover rule 8
   
    [TestMethod] 
    public void FormulaConstructor_TestAdditionOperator_Valid()
    {
        _ = new Formula("1+1");
    }

    [TestMethod]
    public void FormulaConstructor_TestDivisionOperator_Valid()
    {
        _ = new Formula("1/2");
    }

    [TestMethod]
    public void FormulaConstructor_TestMutiplicationOperator_Valid()
    {
        _ = new Formula("1*2");
    }

    [TestMethod]
    public void FormulaConstructor_TestSubtractionOperator_Valid()
    {
        _ = new Formula("1-1");
    }
    
    [TestMethod]
    public void FormulaConstructor_TestNumberInParenthesisTokens_Valid()
    {
        _ = new Formula("(1975)");
    }
    
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestOnlyParentheses_Invalid()
    {
        _ = new Formula("()");
    }
    
    [TestMethod]
    public void FormulaConstructor_TestNormalDecimalToken_Valid()
    {
        _ = new Formula( "3.14" );
    }

    [TestMethod]
    public void FormulaConstructor_TestDecialNumberWithoutZero_Valid()
    {
        _ = new Formula( ".231" );
    }
    
    [TestMethod]
    public void FormulaConstructor_TestNumberTokenWithLowercaseE_Valid()
    {
        _ = new Formula( "2e5" );
    }
    
    [TestMethod]
    public void FormulaConstructor_TestNumberTokenWithUppercaseE_Valid()
    {
        _ = new Formula( "2E5" );
    }
    
    [TestMethod]
    public void FormulaConstructor_TestNumberTokenWithDecimalsAndE_Valid()
    {
        _ = new Formula( "3.5E-6" );
    }
    
    /// <summary>
    /// No number after E making it an incomplete number token
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestNumberMissingAfterEToken_Invalid()
    {
        _ = new Formula( "2.5E" );
    }
    
    /// <summary>
    /// Tests no Number After the + making it an incomplete number token
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestMissingTokenAfterNumberToken_Invalid()
    {
        _ = new Formula( "2e+" );
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestNegativeNumber_Invalid()
    {
        _ = new Formula( "-2" );
    }

    [TestMethod]
    public void FormulaConstructor_TestVariablesWithCaps_Valid()
    {
        _ = new Formula( "A1" );
    }

    [TestMethod]
    public void FormulaConstructor_TestVariablesWithoutCaps_Valid()
    {
        _ = new Formula( "a1" );
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestIncorrectVariablesNumberBeforeLetter_Invalid()
    {
        _ = new Formula( "1A" );
    }

    /// <summary>
    /// This serves as a comprehensive test for the numbers with E and operators together in one test
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestComplexNumberArithmeticWithEWithAllOperators_Valid()
    {
        _ = new Formula( "3.5E-6+3.5E-6*3.5E-6/3.5E-6-3.5E-6" );
    }
    
    [TestMethod] 
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestLetterAfterVariableToken_Invalid()
    {
        _ = new Formula("a1b");
    }
    
    [TestMethod] 
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestDollarSignToken_Invalid()
    {
        _ = new Formula("12+$");
    }
    
    [TestMethod] 
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestHashtagSignToken_Invalid()
    {
        _ = new Formula("#12");
    }
    
    [TestMethod] 
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestPercentSignToken_Invalid()
    {
        _ = new Formula("12%");
    }
    
    [TestMethod] 
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestIncorrectParenthesisTypeToken_Invalid()
    {
        _ = new Formula("{1+1}");
    }
    
    // --- Tests for Closing Parenthesis Rulee

    [TestMethod]
    public void FormulaConstructor_TestSameAmountOfOpenAndClosedParenthesisLeftToRight_Valid()
    {
        _ = new Formula("(5)+(5)+(5)+(5)");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestMoreClosingThanOpenParenthesis_Invalid()
    {
        _ = new Formula("(5))");
    }
    
    /// <summary>
    /// This tests to see even though it is balanced, it is breaking the left to right rule 3
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestReversedOrderParenthesis_Invalid()
    {
        _ = new Formula(")5(");
    }
    
    // --- Tests for Balanced Parentheses Rule
    
    /// <summary>
    /// This overlaps with FormulaConstructor_TestClosingParenthesisAfterClosingParenthesis_Valid
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestNestedParenthesisFor1Formula_Valid()
    {
        _ = new Formula("((5+5))");
    }
    
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestUnbalancedParenthesisTokenNoClosing_Invalid()
    {
        _ = new Formula( "(" );
    }
    
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestSingleClosingParenthesisWithoutOpenParenthesis_Invalid()
    {
        _ = new Formula(")");
    }
    
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestMultipleOpenThanClosedParenthesis_Invalid()
    {
        _ = new Formula("((5)+((5)+(5)+(5)");
    }
    
    /// <summary>
    /// It is balanced but the ordering of the parenthesis are wrong
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestBalancedParenthesisAtTheEndButMoreClosingParenthesisMidway_Invalid()
    {
        _ = new Formula("(5))(");
    }

    // --- Tests for First Token Rule
    
    public void FormulaConstructor_TestFirstTokenIsNumber_Valid( )
    {
        _ = new Formula( "1" );
    }
    
    /// <summary>
    /// Even though I know spaces are ignored, I still want a check here to make sure it is
    /// not registered as a first token.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestSpaceBeforeTheFirstToken_Valid()
    {
        _ = new Formula( "      1" );
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestOperatorIsFirstToken_Invalid()
    {
        _ = new Formula( "+5-1" );
    }

    // --- Tests for  Last Token Rule ---

    [TestMethod]
    public void FormulaConstructor_TestLastTokenVariable_Valid()
    {
        _ = new Formula( "abcde112 + x1" );
    }

    [TestMethod]
    public void FormulaConstructor_TestLastTokenParenthesis_Valid()
    {
        _ = new Formula( "34 + (5+1)");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestLastTokenIsOperator_Invalid()
    {
        _ = new Formula( "1+1+" );
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestLastTokenIsOpenParenthesis_Invalid()
    {
        _ = new Formula( "(34+1)(" );
    }

    // --- Tests for Parentheses/Operator Following Rule ---
    [TestMethod]
    public void FormulaConstructor_TestVariableAfterOpenParenthesis_Valid()
    {
        _ = new Formula( "(abcde12345+1)" );
    }

    [TestMethod]
    public void FormulaConstructor_TestParenthesisAfterOpenParenthesis_Valid()
    {
        _ = new Formula( "((1+1) + 1)" );
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestOperatorFollowOpenParenthesis_Invalid()
    {
        _ = new Formula( "(+5)" );
    }

    // --- Tests for Extra Following Rule ---
    
    [TestMethod]
    public void FormulaConstructor_TestClosingParenthesisAfterNumber_Valid()
    {
        _ = new Formula("(x1+54)");
    }
    
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestOpeningParenthesisAfterNumber_Invalid()
    {
        _ = new Formula("5(1+1)");
    }
    
    [TestMethod]
    public void FormulaConstructor_TestMutiplyOperatorAfterVariable_Valid()
    {
        _ = new Formula("A5*(1+1)");
    }

    [TestMethod]
    public void FormulaConstructor_TestClosingParenthesisAfterVariable_Valid()
    {
        _ = new Formula("(The1975)");
    }

    /// <summary>
    /// This basically tests if there is a number after a variable
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestSpaceBetweenVariableAndNumberWithNoOperatorOrParenthesis_Invalid()
    {
        _ = new Formula("ab12 333333");
    }
    
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestOpeningParenthesisAfterVariable_Invalid()
    {
        _ = new Formula("Frank1(Ocean1)");
    }

    [TestMethod]
    public void FormulaConstructor_TestOperatorAfterClosingParenthesis_Valid()
    {
        _ = new Formula("(abcd1)+1");
    }
    
    /// <summary>
    /// This overlaps with FormulaConstructor_TestNestedParenthesisFor1Formula_Valid
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestClosingParenthesisAfterClosingParenthesis_Valid()
    {
        _ = new Formula("(1+(abcd1))");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestNumberAfterClosingParenthesis_Invalid()
    {
        _ = new Formula("(1)1");
    }
    
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestVariableAfterClosingParenthesis_Invalid()
    {
        _ = new Formula("(1)The1975");
    }
    
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestOpeningParenthesisAfterClosingParenthesis_Invalid()
    {
        _ = new Formula("(1)(1)");
    }
    
    // --- ToString Tests ---
    [TestMethod]
    public void ToString_TestNormalVariablePlusNum_Valid()
    {
        Formula expression = new Formula("x1 + 45");
        string result = expression.ToString();
        Assert.AreEqual("X1+45", result);
    }
    
    [TestMethod]
    public void ToString_TestNormalVariablePlusDecimal_Valid()
    {
        Formula expression = new Formula("x1 + 45.000");
        string result = expression.ToString();
        Assert.AreEqual("X1+45", result);
    }
    
    [TestMethod]
    public void ToString_TestNormalVariablePlusE_Valid()
    {
        Formula expression = new Formula("x1 + 2E2");
        string result = expression.ToString();
        Assert.AreEqual("X1+200", result);
    }

    [TestMethod]
    public void ToString_TestNormalVariablePlusNumAndParen_Valid()
    {
        Formula expression = new Formula("x1 + 45 + (45)");
        string result = expression.ToString();
        Assert.AreEqual("X1+45+(45)", result);
    }
    
    // --- GetVariables() tests ---
    
    [TestMethod]
    public void GetVariables_TestTwoVariables_Valid()
    {
        Formula expression = new Formula("X3+b1");
        ISet<string> result = expression.GetVariables();
        var expected = new HashSet<string>
        {
            "X3",
            "B1" 
        };
            
        CollectionAssert.AreEquivalent(expected.ToList(), result.ToList());
    }
    
    [TestMethod]
    public void GetVariables_TestDuplicateVariables_Valid()
    {
        Formula expression = new Formula("X3+b1+b1+b1");
        ISet<string> result = expression.GetVariables();
        var expected = new HashSet<string>
        {
            "X3",
            "B1" 
        };
            
        CollectionAssert.AreEquivalent(expected.ToList(), result.ToList());
    }
    
    // ------ Evaluate() ------
    
    [TestMethod]
    public void Evaluate_DoubleArithmetic_Valid()
    {
        Formula expression = new Formula("2.3+1.2");
        Assert.AreEqual(3.5, (double)expression.Evaluate(x => 0), 1e-9);
    }

    [TestMethod]
    public void Evaluate_WholeNumberArithmetic_Valid()
    {
        Formula expression = new Formula("2+1");
        Assert.AreEqual(3, (double)expression.Evaluate(x=>0), 1e-9);
    }

    [TestMethod]
    public void Evaluate_VariableArithmetic_Valid()
    {
        Formula expression = new Formula("A1*2");
        Assert.AreEqual(0, (double)expression.Evaluate(x=>0), 1e-9);
    }

    [TestMethod]
    public void Evaluate_ParenthesisArithmetic_Valid()
    {
        Formula expression = new Formula("(3+3)+3");
        Assert.AreEqual(9, (double)expression.Evaluate(x=>0), 1e-9);
    }

    [TestMethod]
    public void Evaluate_ExponentsArithmetic_Valid()
    {
        Formula expression = new Formula("2e+2 + 3");
        Assert.AreEqual(203, (double)expression.Evaluate(x=>0), 1e-9);
    }

    [TestMethod]
    public void Evaluate_ArithmeticWithOrderofOperationsWholeNums_Valid()
    {
        Formula expression = new Formula("2 + 4 * 5");
        Assert.AreEqual(22, (double)expression.Evaluate(x=>0), 1e-9);
    }

    [TestMethod]
    public void Evaluate_ArithmeticWithOrderofOperationsParenthesis_Valid()
    {
        Formula expression = new Formula("(2 + 4) * 5 * 2");
        Assert.AreEqual(60, (double)expression.Evaluate(x=>0), 1e-9);
    }

    [TestMethod]
    public void Evaluate_ChainedAdditionArithmetic_Valid()
    {
        Formula expression = new Formula("2+2+2");
        Assert.AreEqual(6, (double)expression.Evaluate(x=>0), 1e-9);

    }
    
    [TestMethod]
    public void Evaluate_ChainedSubtractionArithmetic_Valid()
    {
        Formula expression = new Formula("8-2-2");
        Assert.AreEqual(4, (double)expression.Evaluate(x=>0), 1e-9);

    }

    [TestMethod]
    public void Evaluate_LongStressTestWholeNumberArithmetic_Valid()
    {
        Formula expression = new Formula("(2e+2 + 4) * 5 * 2 / 10 * 2 + (10 / 2 + 5)");
        Assert.AreEqual(418, (double)expression.Evaluate(x=>0), 1e-9);
    }
    
    [TestMethod]
    public void Evaluate_LongStressTestWholeNumberArithmeticVersion2_Valid()
    {
        
        Formula expression = new Formula("(2e+2 + 4) * 5 + 2 / 10 * 2 - (10 / 2 * 5)");
        Assert.AreEqual(995.4, (double)expression.Evaluate(x=>0), 1e-9);
    }
    
    [TestMethod]
    public void Evaluate_LongStressTestWholeNumberArithmeticVersion3_Valid()
    {
        
        Formula expression = new Formula("(2e+2 + 4) * 5 + 2 / 10 * 2 * (10 / 2 * 5)");
        Assert.AreEqual(1030, (double)expression.Evaluate(x=>0), 1e-9);
    }
    
    [TestMethod]
    public void Evaluate_ComplexNestedParenthesesArithmetic_Valid()
    {
        Formula expression = new Formula("((2+3) * 4) / 2 + 5");
        Assert.AreEqual(15, (double)expression.Evaluate(x => 0), 1e-9);
    }
    
    [TestMethod]
    public void Evaluate_SubtractionBeforeOpenParenArithmetic_Valid()
    {
        Formula expression = new Formula("((2+3) * 4) / 2 + 5");
        Assert.AreEqual(15, (double)expression.Evaluate(x => 0), 1e-9);
    }

    [TestMethod]
    public void Evaluate_DivsionAfterOpenParenArithmetic_Valid()
    {
        Formula expression = new Formula("2 / (1+1)");
        Assert.AreEqual(1, (double)expression.Evaluate(x => 0), 1e-9);
    }

    
    // ------ Division by zero test ------

    [TestMethod]
    public void Evaluate_DivisionByZero_Invalid()
    {
        Formula expression = new Formula("1/0");
        object result = expression.Evaluate(x => 0);

        Assert.IsInstanceOfType(result, typeof(FormulaError));

        FormulaError error = (FormulaError)result; 
        Assert.AreEqual("Division by zero", error.Reason); 
    }
    
    [TestMethod]
    public void Evaluate_DivisionByZeroInLastParenOfExpression_Invalid()
    {
        Formula expression = new Formula("5+5 / (3-3)");
        object result = expression.Evaluate(x => 0);

        Assert.IsInstanceOfType(result, typeof(FormulaError));

        FormulaError error = (FormulaError)result; 
        Assert.AreEqual("Division by zero", error.Reason); 
    }

    // ----------------------------------------------------------------
    // ------ Lookup doesn't exist and exists tests ------ START ------
    // ----------------------------------------------------------------
    
    /// <summary>
    /// A delegate to be used in testing the lookop behavior of Evaluate()
    /// Takes a string and returns a double
    /// </summary>
    public delegate double Lookup(string variableName);
    /// <summary>
    /// A private lookup method used to simulate successful lookup
    /// </summary>
    /// <param name="variableName">
    /// takes in a variable/cell name to retrieve double
    /// </param>
    /// <returns>
    /// returns a double of 1 for easy arithmetic if lookup is A1
    /// </returns>
    /// <exception cref="ArgumentException">
    /// If the variable looked up doesn't exist it throws a new exception which is
    /// caught by the evaluate try catch to return formula error
    /// </exception>
    private double LookupTestMethod(string variableName)
    {
        if (variableName == "A1")
        {
            return 1;
        }
        else
        {
            throw new ArgumentException("Variable doesn't exist");
        }
    }
    
    /// <summary>
    /// A private lookup method used to simulate successful lookup, however
    /// the zero value leads to an exception, thus this is used for testing
    /// the edge case of division by a lookup resulting in zero
    /// </summary>
    /// <param name="variableName">
    /// takes in a variable/cell name to retrieve double
    /// </param>
    /// <returns>
    /// returns a double of 0 for easy arithmetic if lookup is A1
    /// </returns>
    /// <exception cref="ArgumentException">
    /// If the variable looked up doesn't exist it throws a new exception which is
    /// caught by the evaluate try catch to return formula error
    /// </exception>
    private double LookupIsZeroTestMethod(string variableName)
    {
        if (variableName == "A1")
        {
            return 0;
        }
        else
        {
            throw new ArgumentException("Variable doesn't exist");
        }
    }
    
    [TestMethod]
    public void Evaluate_SingleVariable_Valid()
    {
        Formula expression = new Formula("A1");
        Assert.AreEqual(42, (double)expression.Evaluate(x => 42), 1e-9);
    }

    [TestMethod]
    public void Evaluate_DoubleVariable_Valid()
    {
        Formula expression = new Formula("1/A1");
        Assert.AreEqual(1, (double)expression.Evaluate(LookupTestMethod), 1e-9);

    }
    
    [TestMethod] 
    public void Evaluate_LookupVariableExistButReturnZero_Invalid()
    {
        Formula expression = new Formula("1/A1");
        object result = expression.Evaluate(LookupIsZeroTestMethod);

        Assert.IsInstanceOfType(result, typeof(FormulaError));

        FormulaError error = (FormulaError)result; 
        Assert.AreEqual("Division by zero", error.Reason); 
    }
    
    [TestMethod] 
    public void Evaluate_LookupVariableDoesntExist_Invalid()
    {
        Formula expression = new Formula("A2+2");
        object result = expression.Evaluate(LookupTestMethod);

        Assert.IsInstanceOfType(result, typeof(FormulaError));

        FormulaError error = (FormulaError)result; 
        Assert.AreEqual("Variable doesn't exist", error.Reason); 
    }
    
    [TestMethod]
    public void Evaluate_LookupVariableDoesExistAddition_Valid()
    {
        Formula expression = new Formula("A1+2");
        Assert.AreEqual(3, (double)expression.Evaluate(LookupTestMethod), 1e-9); 
    }

    [TestMethod]
    public void Evaluate_LookupVariableDoesExistVariableBeforeMultiplication_Valid()
    {
        Formula expression = new Formula("A1*2");
        Assert.AreEqual(2, (double)expression.Evaluate(LookupTestMethod), 1e-9);
    }
    
    [TestMethod]
    public void Evaluate_LookupVariableDoesExistVariableAfterMultiplication_Valid()
    {
        Formula expression = new Formula("2*A1");
        Assert.AreEqual(2, (double)expression.Evaluate(LookupTestMethod), 1e-9);
    }

    
    [TestMethod]
    public void Evaluate_LookupVariableDoesExistDivision_Valid()
    {
        Formula expression = new Formula("A1/2");
        Assert.AreEqual(.5, (double)expression.Evaluate(LookupTestMethod), 1e-9);
    }
    // ----------------------------------------------------------------
    // ------ Lookup doesn't exist and exists tests ------- END -------
    // ----------------------------------------------------------------
    
    // ------ Equals() & operator overloads ------
    
    [TestMethod]
    public void Equals_EquivalentFormulas_Valid()
    {
        Formula f1 = new Formula("2+3");
        Formula f2 = new Formula("2+3");

        Assert.IsTrue(f1.Equals(f2));
    }
    
    [TestMethod]
    public void Equals_NotFormulaObject_Invalid()
    {
        Formula expression = new Formula("2+3");
        List<int> expression2 = new List<int>();
        
        Assert.IsFalse(expression.Equals(expression2));
    }


    [TestMethod]
    public void OperatorOverloadEquals_LookupVariableExists_Valid()
    {
        Formula expression = new Formula("2");
        Formula expression2 = new Formula("2");
        Assert.IsTrue(expression == expression2);

    }
    [TestMethod]
    public void OperatorOverloadNotEquals_DifferentFormulas_Valid()
    {
        Formula f1 = new Formula("2+3");
        Formula f2 = new Formula("3+2"); 
        Assert.IsTrue(f1 != f2);
    }
    
    [TestMethod]
    public void OperatorOverloadEquals_NullFormula_Invalid()
    {
        Formula f1 = new Formula("2+3");
        Formula? f2 = null;
        Assert.IsFalse(f1 == f2);
    }
    
    // ------ GetHashCode() ------
    
    [TestMethod]
    public void GetHashCode_EqualFormulas_SameHash()
    {
        Formula f1 = new Formula("A1+2");
        Formula f2 = new Formula("A1+2");

        Assert.AreEqual(f1.GetHashCode(), f2.GetHashCode());
    }
    
    [TestMethod]
    public void GetHashCode_DifferentFormulas_DifferentHash()
    {
        Formula f1 = new Formula("A1+2");
        Formula f2 = new Formula("A1+3");

        Assert.AreNotEqual(f1.GetHashCode(), f2.GetHashCode());
    }
}