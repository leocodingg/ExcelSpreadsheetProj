// <copyright file="GradingTestsPS4.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>

namespace CS3500.FormulaEvaluationGradingTests;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using CS3500.Formula;
using System.Text;

/// <summary>
/// Authors:   Joe Zachary
///            Daniel Kopta
///            Jim de St. Germain
/// Date:      Updated Fall 2024
/// Course:    CS 3500, University of Utah, School of Computing
/// Copyright: CS 3500 - This work may not be copied for use
///                      in Academic Coursework.  See below.
///
/// File Contents
///
///   This file contains proprietary grading tests for CS 3500.  These tests cases
///   are for individual student use only and MAY NOT BE SHARED.  Do not back them up
///   nor place them in any public online repository.  Improper use of these test cases
///   can result in removal from the course and an academic misconduct sanction.
///
///   These tests are for your private use only to improve the quality of the
///   rest of your assignments.
/// </summary>
/// <date> Updated Fall 2024. </date>
[TestClass]
public class GradingTestsPS4
{
    /// <summary>
    ///   Test that a single number equals itself.
    /// </summary>
    [TestMethod]
    [Timeout( 5000 )]
    [TestCategory( "1" )]
    public void Evaluate_SingleNumber_Equals5( )
    {
        Formula formula = new("5");
        Assert.AreEqual( 5.0, formula.Evaluate( s => 0 ) );
    }

    /// <summary>
    ///   Test that a single variable evaluates to the expected value.
    /// </summary>
    [TestMethod]
    [Timeout( 5000 )]
    [TestCategory( "2" )]
    public void Evaluate_SingleVariable_EqualsLookup( )
    {
        Formula formula = new("X5");
        Assert.AreEqual( 13.0, formula.Evaluate( s => 13 ) );
        Assert.AreEqual( 20.0, formula.Evaluate( s => 20 ) );
        Assert.AreEqual( -1.0, formula.Evaluate( s => -1 ) );
    }

    /// <summary>
    ///   Test that simple addition works.
    /// </summary>
    [TestMethod]
    [Timeout( 5000 )]
    [TestCategory( "3" )]
    public void Evaluate_AdditionOperator_Equals8( )
    {
        Formula formula = new("5+3");
        Assert.AreEqual( 8.0, formula.Evaluate( s => 0 ) );
    }

    /// <summary>
    ///   Test that simple subtraction works.
    /// </summary>
    [TestMethod]
    [Timeout( 5000 )]
    [TestCategory( "4" )]
    public void Evaluate_SubtractionOperator_Equals8( )
    {
        Formula formula = new("18-10");
        Assert.AreEqual( 8.0, formula.Evaluate( s => 0 ) );
    }

    /// <summary>
    ///   Test that simple multiplication works.
    /// </summary>
    [TestMethod]
    [Timeout( 5000 )]
    [TestCategory( "5" )]
    public void Evaluate_MultiplicationOperator_Equals8( )
    {
        Formula formula = new("2*4");
        Assert.AreEqual( 8.0, formula.Evaluate( s => 0 ) );
    }

    /// <summary>
    ///   Test that simple division works.
    /// </summary>
    [TestMethod]
    [Timeout( 5000 )]
    [TestCategory( "6" )]
    public void Evaluate_DivisionOperator_Equals8( )
    {
        Formula formula = new("16/2");
        Assert.AreEqual( 8.0, formula.Evaluate( s => 0 ) );
    }

    /// <summary>
    ///   Test that Variables work with Arithmetic.
    /// </summary>
    [TestMethod]
    [Timeout( 5000 )]
    [TestCategory( "7" )]
    public void Evaluate_VariablePlusValue_Equals8( )
    {
        Formula formula = new("2+X1");
        Assert.AreEqual( 8.0, formula.Evaluate( s => 6 ) );
    }

    /// <summary>
    ///   Test multiple variables.
    /// </summary>
    [TestMethod]
    [Timeout( 5000 )]
    [TestCategory( "8" )]
    public void Evaluate_MultipleVariables_Equals100( )
    {
        Formula formula = new( "X1+X2+X3+X4" );
        Assert.AreEqual( 100.0, formula.Evaluate( s => ( s == "X1" ) ? 55 : 15 ) );
    }

    /// <summary>
    ///   Test variables normalization.
    /// </summary>
    [TestMethod]
    [Timeout( 5000 )]
    [TestCategory( "9" )]
    public void Evaluate_LowerCaseVariable_Equals100( )
    {
        Formula formula = new( "x1+X1" );
        Assert.AreEqual( 100.0, formula.Evaluate( s => 50 ) );
    }

    /// <summary>
    ///   Test that an unknown variable returns a formula error object.
    /// </summary>
    [TestMethod]
    [Timeout( 5000 )]
    [TestCategory( "10" )]
    public void Evaluate_TestUnknownVariable( )
    {
        Formula formula = new("2+X1");
        var result = formula.Evaluate( s => { throw new ArgumentException( "Unknown variable" ); } );
        Assert.IsInstanceOfType( result, typeof( FormulaError ) );
    }

    /// <summary>
    ///   Test order of operation precedence * before +.
    /// </summary>
    [TestMethod]
    [Timeout( 5000 )]
    [TestCategory( "11" )]
    public void Evaluate_OperatorPrecedence_MultiplicationThenAdd( )
    {
        Formula formula = new("2*3+2");
        Assert.AreEqual( 8.0, formula.Evaluate( s => 0 ) );
    }

    /// <summary>
    ///   Test order of operation precedence * before +.
    /// </summary>
    [TestMethod]
    [Timeout( 5000 )]
    [TestCategory( "12" )]
    public void Evaluate_OperatorPrecedence_SubtractThenMultiplication( )
    {
        Formula formula = new("26-6*3");
        Assert.AreEqual( 8.0, formula.Evaluate( s => 0 ) );
    }

    /// <summary>
    ///   Test that parentheses override precedence rules (or that they have
    ///   the highest precedence).
    /// </summary>
    [TestMethod]
    [Timeout( 5000 )]
    [TestCategory( "13" )]
    public void Evaluate_ParenthesesBeforeTimes_Equals8( )
    {
        Formula formula = new("(2+2)*2");
        Assert.AreEqual( 8.0, formula.Evaluate( s => 0 ) );
    }

    /// <summary>
    ///   Test that parentheses have higher precedence even when
    ///   they come last.
    /// </summary>
    [TestMethod]
    [Timeout( 5000 )]
    [TestCategory( "14" )]
    public void Evaluate_ParenthesesAfterTimes_Equals100( )
    {
        Formula formula = new("20*(6-1)");
        Assert.AreEqual( 100.0, formula.Evaluate( s => 0 ) );
    }

    /// <summary>
    ///   Evaluate that parentheses don't make a difference when
    ///   they shouldn't make a difference.
    /// </summary>
    [TestMethod]
    [Timeout( 5000 )]
    [TestCategory( "15" )]
    public void Evaluate_PlusInParentheses_Equals100( )
    {
        Formula formula = new("25+(25+25)+25");
        Assert.AreEqual( 100.0, formula.Evaluate( s => 0 ) );
    }

    /// <summary>
    ///   Evaluate a slightly more involved expression with
    ///   parentheses and order of precedence.
    /// </summary>
    [TestMethod]
    [Timeout( 5000 )]
    [TestCategory( "16" )]
    public void Evaluate_PlusTimesAndParentheses_Equals100( )
    {
        Formula formula = new("2+(3+5*9)-(50-100)");
        Assert.AreEqual( 100.0, formula.Evaluate( s => 0 ) );
    }

    /// <summary>
    ///   Test operators directly after parentheses.
    /// </summary>
    [TestMethod]
    [Timeout( 5000 )]
    [TestCategory( "17" )]
    public void Evaluate_OperatorAfterParens_Equals100( )
    {
        Formula formula = new("(10*11)-10/1");
        Assert.AreEqual( 100.0, formula.Evaluate( s => 0 ) );
    }

    /// <summary>
    ///   Test another more complex set of parentheses with all operators.
    /// </summary>
    [TestMethod]
    [Timeout( 5000 )]
    [TestCategory( "18" )]
    public void Evaluate_TestComplexAllOperatorsAndParentheses_Equals100( )
    {
        Formula formula = new("200-3*(3+5)*3/2-(8*8)");
        Assert.AreEqual( 100.0, formula.Evaluate( s => 0 ) );
    }

    /// <summary>
    ///   Test another complex equation.
    /// </summary>
    [TestMethod]
    [Timeout( 5000 )]
    [TestCategory( "19" )]
    public void Evaluate_ComplexAndParentheses_Equals100( )
    {
        Formula formula = new("(2+3*5+(3+4*8)*5+2)-94");
        Assert.AreEqual( 100.0, formula.Evaluate( s => 0 ) );
    }

    /// <summary>
    ///   Test division by zero.
    /// </summary>
    [TestMethod]
    [Timeout( 5000 )]
    [TestCategory( "20" )]
    public void Evaluate_DirectDivideByZero_FormulaError( )
    {
        Formula formula = new("5/0");
        var result = formula.Evaluate( s => 0 );
        Assert.IsInstanceOfType( result, typeof( FormulaError ) );
    }

    /// <summary>
    ///   Divide by zero as computed by variables.
    /// </summary>
    [TestMethod]
    [Timeout( 2000 )]
    [TestCategory( "21" )]
    public void TestDivideByZeroVars( )
    {
        Formula f = new("(5 + X1) / (X1 - 3)");
        var result = f.Evaluate( s => 3 );
        Assert.IsInstanceOfType( result, typeof( FormulaError ) );
    }

    /// <summary>
    ///   Test complex formula with multiple variables.
    /// </summary>
    [TestMethod]
    [Timeout( 5000 )]
    [TestCategory( "22" )]
    public void Evaluate_ComplexMultiVar_EqualsNegative18( )
    {
        Formula formula = new( "(Y1*3-8/2+4*(8-9*2)/2*X7)-6" );
        Assert.AreEqual( -18.0, formula.Evaluate( s => ( s == "X7" ) ? 1 : 4 ) );
    }

    /// <summary>
    ///   Lots of nested parens, following on the right.
    /// </summary>
    [TestMethod]
    [Timeout( 5000 )]
    [TestCategory( "23" )]
    public void Evaluate_TestComplexNestedParensRight_Equals6( )
    {
        Formula formula = new( "x1+(x2+(x3+(x4+(x5+x6))))" );
        Assert.AreEqual( 6.0, formula.Evaluate( s => 1 ) );
    }

    /// <summary>
    ///  Lots of nested parens, starting on the left side.
    /// </summary>
    [TestMethod]
    [Timeout( 5000 )]
    [TestCategory( "24" )]
    public void Evaluate_TestComplexNestedParensLeft_Equals12( )
    {
        Formula formula = new( "((((x1+x2)+x3)+x4)+x5)+x6" );
        Assert.AreEqual( 12.0, formula.Evaluate( s => 2 ) );
    }

    /// <summary>
    ///   Simple Repeated Variable.
    /// </summary>
    [TestMethod]
    [Timeout( 5000 )]
    [TestCategory( "25" )]
    public void Evaluate_RepeatedVarWithVariousOperators_Equals3( )
    {
        Formula formula = new( "a4-a4*a4/a4+a4" );
        Assert.AreEqual( 3.0, formula.Evaluate( s => 3 ) );
    }

    /// <summary>
    ///   Test that the formula is not using a shared stack between
    ///   calls.
    /// </summary>
    [TestMethod]
    [Timeout( 5000 )]
    [TestCategory( "26" )]
    public void Evaluate_SeparateStacks_StacksClearedEachTimeAndEquals15( )
    {
        Formula formula = new( "2*6+3" );
        Assert.AreEqual( 15.0, formula.Evaluate( s => 0 ) );
        Assert.AreEqual( 15.0, formula.Evaluate( s => 0 ) );
    }

    /// <summary>
    ///   Test that the formula is not using a shared stack between
    ///   multiple formulas.
    /// </summary>
    [TestMethod]
    [Timeout( 5000 )]
    [TestCategory( "27" )]
    public void Evaluate_FormulasAreIndependent_Equal15_14_11( )
    {
        Formula formula1 = new( "2*6+3" );
        Formula formula2 = new( "2*6+2" );
        Formula formula3 = new( "2*6-1" );
        Assert.AreEqual( 15.0, formula1.Evaluate( s => 0 ) );
        Assert.AreEqual( 14.0, formula2.Evaluate( s => 0 ) );
        Assert.AreEqual( 11.0, formula3.Evaluate( s => 0 ) );
    }

    /// <summary>
    ///   Test that variable values don't matter if there are no variables.
    /// </summary>
    [TestMethod]
    [Timeout( 5000 )]
    [TestCategory( "28" )]
    public void Evaluate_VariablesHaveValueButFormulaHasNoVariables_Equals10( )
    {
        Formula formula = new( "2*6+3" );
        Assert.AreEqual( 15.0, formula.Evaluate( s => 100 ) );
    }

    /// <summary>
    ///   Check a formula that computes a lot of decimal places.
    /// </summary>
    [TestMethod]
    [Timeout( 2000 )]
    [TestCategory( "29" )]
    public void Evaluate_ComplexLotsOfDecimalPlaces_Equals514285714285714( )
    {
        Formula f = new( "y1*3-8/2+4*(8-9*2)/14*x7" );
        double result = (double) f.Evaluate( s => ( s == "X7" ) ? 1 : 4 );
        Assert.AreEqual( 5.14285714285714, result, 1e-9 );
    }

    /// <summary>
    ///   Check a formula that computes pi to 10 decimal places using
    ///   10000 adds and subtracts.
    /// </summary>
    [TestMethod]
    [Timeout( 2000 )]
    [TestCategory( "30" )]
    public void Evaluate_ComputePiStress_PiTo4DecimalPlaces( )
    {
        StringBuilder formulaString = new("4 * ( 1");
        bool negative = true;
        for ( int i = 3; i < 10000; i += 2 )
        {
            formulaString.Append( ( negative ? "-" : "+" ) + $"1/{i}" );
            negative = !negative;
        }

        formulaString.Append(')');
        Formula f = new( formulaString.ToString() );
        double result = (double) f.Evaluate( s => 0 );
        Assert.AreEqual( 3.1415926535, result, 1e-3 );
    }

    // Equality and Hash tests

    /// <summary>
    ///   Test basic equality of two identical formulas.
    /// </summary>
    [TestMethod]
    [Timeout( 2000 )]
    [TestCategory( "31" )]
    public void Equals_TwoSameFormula_AreEqual( )
    {
        Formula f1 = new("X1+X2");
        Formula f2 = new("X1+X2");
        Assert.IsTrue( f1.Equals( f2 ) );
    }

    /// <summary>
    ///   Test that whitespace doesn't matter to equals.
    /// </summary>
    [TestMethod]
    [Timeout( 2000 )]
    [TestCategory( "32" )]
    public void Equals_CheckWhitespace_SameEquation( )
    {
        Formula f1 = new("X1+X2");
        Formula f2 = new(" X1  +  X2   ");
        Assert.IsTrue( f1.Equals( f2 ) );
    }

    /// <summary>
    ///   Test that different number notations don't matter to equals.
    /// </summary>
    [TestMethod]
    [Timeout( 2000 )]
    [TestCategory( "33" )]
    public void Equals_DifferentNumberSyntaxes_SameFormula( )
    {
        Formula f1 = new("2+X1*3.00");
        Formula f2 = new("2.00+X1*3.0");
        Assert.IsTrue( f1.Equals( f2 ) );
    }

    /// <summary>
    ///   Test a little more complex string equality (canonical form).
    /// </summary>
    [TestMethod]
    [Timeout( 2000 )]
    [TestCategory( "34" )]
    public void Equals_MoreComplexEquality_SameFormula( )
    {
        Formula f1 = new("1e-2 + X5 + 17.00 * 19 ");
        Formula f2 = new("   0.0100  +     X5+ 17 * 19.00000 ");
        Assert.IsTrue( f1.Equals( f2 ) );
    }

    /// <summary>
    ///   Test on null and empty string.
    /// </summary>
    [TestMethod]
    [Timeout( 2000 )]
    [TestCategory( "35" )]
    public void Equals_NullAndEmptyString_NotEqual( )
    {
        Formula f = new("2");

        Assert.IsFalse( f.Equals( null ) );
        Assert.IsFalse( f.Equals( string.Empty ) );
    }

    /// <summary>
    ///   Test on a different object type.
    /// </summary>
    [TestMethod]
    [Timeout( 2000 )]
    [TestCategory( "36" )]
    public void Equals_NonFormulaObject_NotEqual( )
    {
        Formula f = new("2");

        Assert.IsFalse( f.Equals( new List<string>() ) );
    }

    /// <summary>
    ///   Test the == operator.
    /// </summary>
    [TestMethod]
    [Timeout( 2000 )]
    [TestCategory( "37" )]
    public void OperatorDoubleEquals_TwoDifferentObjects_AreEqual( )
    {
        Formula f1 = new("2");
        Formula f2 = new("2");

        Assert.IsTrue( f1 == f2 );
    }

    /// <summary>
    ///   Test that == shows that two different formula are different.
    /// </summary>
    [TestMethod]
    [Timeout( 2000 )]
    [TestCategory( "38" )]
    public void OperatorDoubleEquals_TwoDifferentFormula_NotEqual( )
    {
        Formula f1 = new("2");
        Formula f2 = new("5");
        Assert.IsFalse( f1 == f2 );
    }

    /// <summary>
    ///   Test the not equals operator.
    /// </summary>
    [TestMethod]
    [Timeout( 2000 )]
    [TestCategory( "39" )]
    public void OperatorNotEqual_TwoEqualFormula_NotEqual( )
    {
        Formula f1 = new("2");
        Formula f2 = new("2");
        Assert.IsFalse( f1 != f2 );
    }

    /// <summary>
    ///   Test thenot equals operator on two different objects with different values.
    /// </summary>
    [TestMethod]
    [Timeout( 2000 )]
    [TestCategory( "40" )]
    public void OperatorNotEqual_TwoDifferentFormulas_NotEqual( )
    {
        Formula f1 = new("2");
        Formula f2 = new("5");
        Assert.IsTrue( f1 != f2 );
    }

    /// <summary>
    ///   Test that the hashcode of two alike formulas are the same.
    /// </summary>
    [TestMethod]
    [Timeout( 2000 )]
    [TestCategory( "41" )]
    public void GetHashCode_EqualFormulas_SameHashCodes( )
    {
        Formula f1 = new("2*5");
        Formula f2 = new("2*5");
        Assert.IsTrue( f1.GetHashCode() == f2.GetHashCode() );
    }

    /// <summary>
    ///   Technically the hashcodes could not be equal and still be valid,
    ///   extremely unlikely though. Check their implementation if this fails.
    ///   <para>
    ///     While it is very unlikely that two different formula will have
    ///     the same hashcode, it becomes ridiculously unlikely if we do
    ///     three formulas.
    ///   </para>
    /// </summary>
    [TestMethod]
    [Timeout( 2000 )]
    [TestCategory( "42" )]
    public void GetHashCode_DifferentFormulas_DifferentCodes( )
    {
        Formula f1 = new("2*5");
        Formula f2 = new("3/8*2+(7)");
        Formula f3 = new("1");
        Assert.IsTrue( f1.GetHashCode() != f2.GetHashCode() ||
                       f2.GetHashCode() != f3.GetHashCode() );
    }

    /// <summary>
    ///   Check to make sure that the hash code is computed on the
    ///   normalized form.
    /// </summary>
    [TestMethod]
    [Timeout( 2000 )]
    [TestCategory( "43" )]
    public void GetHashCode_CheckCanonicalForms_AreSame( )
    {
        Formula f1 = new("2 * 5 + 4.00 - x1");
        Formula f2 = new("2*5+4-X1");
        Assert.IsTrue( f1.GetHashCode() == f2.GetHashCode() );
    }

    // A set of tests just to verify no regression has taken place!

    /// <summary>
    ///  Test invalid formula (really a previous assignment verification).
    /// </summary>
    [TestMethod]
    [Timeout( 5000 )]
    [TestCategory( "44" )]
    [ExpectedException( typeof( FormulaFormatException ) )]
    public void FormulaConstructor_InvalidFormula_Throws( )
    {
        _ = new Formula( "+" );
    }

    /// <summary>
    ///   Test extra operator at end.
    /// </summary>
    [TestMethod]
    [Timeout( 5000 )]
    [TestCategory( "45" )]
    [ExpectedException( typeof( FormulaFormatException ) )]
    public void FormulaConstructor_ExtraOperator_Throws( )
    {
        _ = new Formula( "2+5+" );
    }

    /// <summary>
    ///   Test extra parentheses.
    /// </summary>
    [TestMethod]
    [Timeout( 5000 )]
    [TestCategory( "46" )]
    [ExpectedException( typeof( FormulaFormatException ) )]
    public void FormulaConstructor_ExtraParentheses_Throws( )
    {
        _ = new Formula( "2+5*7)" );
    }

    /// <summary>
    ///   Test Invalid Variable Name.
    /// </summary>
    [TestMethod]
    [Timeout( 5000 )]
    [TestCategory( "47" )]
    [ExpectedException( typeof( FormulaFormatException ) )]
    public void FormulaConstructor_InvalidVariable_Throws( )
    {
        _ = new Formula( "xx" );
    }

    /// <summary>
    ///   Test no implicit multiplication (5)(5) does not equal 25.
    /// </summary>
    [TestMethod]
    [Timeout( 5000 )]
    [TestCategory( "48" )]
    [ExpectedException( typeof( FormulaFormatException ) )]
    public void FormulaConstructor_NoImplicitMultiplication_Throws( )
    {
        _ = new Formula( "5+7+(5)8" );
    }

    /// <summary>
    ///   Test that Empty Formula throws.
    /// </summary>
    [TestMethod]
    [Timeout( 5000 )]
    [TestCategory( "49" )]
    [ExpectedException( typeof( FormulaFormatException ) )]
    public void FormulaConstructor_Empty_Throws( )
    {
        _ = new Formula( string.Empty );
    }

    /// <summary>
    ///   Test that ToString continues to work.
    /// </summary>
    [TestMethod]
    [Timeout( 5000 )]
    [TestCategory( "50" )]
    public void FormulaToString_CreatesEqualFormula_EqualEachOther( )
    {
        Formula f1 = new( "(1+2*(3/4))" );
        Formula f2 = new( f1.ToString( ) );

        Assert.AreEqual( f1.Evaluate( s => 0 ), f2.Evaluate( s => 0 ) );
        Assert.AreEqual( f1.ToString(), f2.ToString( ) );
    }
}
