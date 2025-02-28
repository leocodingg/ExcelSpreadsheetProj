// <copyright file="GradingTests.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>

/// <summary>
/// Authors:   Joe Zachary
///            Daniel Kopta
///            Jim de St. Germain
/// Date:      Updated Spring 2022
/// Course:    CS 3500, University of Utah, School of Computing
/// Copyright: CS 3500 - This work may not be copied for use
///                      in Academic Coursework.  See below.
///
/// File Contents
///
///   This file contains proprietary grading tests for CS 3500.  These tests cases
///   are for individual student use only and MAY NOT BE SHARED.  Do not back them up
///   nor place them in any online repository.  Improper use of these test cases
///   can result in removal from the course and an academic misconduct sanction.
///
///   These tests are for your private use only to improve the quality of the
///   rest of your assignments
/// </summary>
/// <date> Updated Fall 2024 </date>

namespace GradingTests;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using CS3500.Formula;

#pragma warning disable

[TestClass]
public class GradingTests
{
    // --- Tests One Token Rule ---

    /// <summary>
    ///   Test that an empty formula throws the formula format exception.
    /// </summary>
    [TestMethod]
    [TestCategory( "1" )]
    [ExpectedException( typeof( FormulaFormatException ) )]
    public void FormulaConstructor_TestOneToken_Fails( )
    {
        _ = new Formula( string.Empty );
    }

    /// <summary>
    ///   Test that an empty formula, but with spaces, also fails.
    /// </summary>
    [TestMethod]
    [TestCategory( "2" )]
    [ExpectedException( typeof( FormulaFormatException ) )]
    public void FormulaConstructor_TestOneTokenSpaces_Fails( )
    {
        _ = new Formula( "  " );
    }

    // --- Test Valid Token Rules ---

    /// <summary>
    ///   Test that invalid tokens throw the appropriate exception.
    /// </summary>
    [TestMethod]
    [TestCategory( "3" )]
    [ExpectedException( typeof( FormulaFormatException ) )]
    public void FormulaConstructor_TestInvalidTokensOnly_Fails( )
    {
        _ = new Formula( "$" );
    }

    /// <summary>
    ///   Test for another invalid token in the formula.
    /// </summary>
    [TestMethod]
    [TestCategory( "4" )]
    [ExpectedException( typeof( FormulaFormatException ) )]
    public void FormulaConstructor_TestInvalidTokenInFormula_Fails( )
    {
        _ = new Formula( "5 + 5 ," );
    }

    /// <summary>
    ///   Test that _all_ the valid tokens can be parsed,
    ///   e.g., math operators, numbers, variables, parens.
    /// </summary>
    [TestMethod]
    [TestCategory( "5" )]
    public void FormulaConstructor_TestValidTokenTypes_Succeeds( )
    {
        _ = new Formula( "5 + (1-2) * 3.14 / 1e6 + 0.2E-9 - A1 + bb22" );
    }

    // --- Test Closing Parenthesis Rule ---

    /// <summary>
    ///   Test that a closing paren cannot occur without
    ///   an opening paren first.
    /// </summary>
    [TestMethod]
    [TestCategory( "6" )]
    [ExpectedException( typeof( FormulaFormatException ) )]
    public void FormulaConstructor_TestClosingWithoutOpening_Fails( )
    {
        _ = new Formula( "5 )" );
    }

    /// <summary>
    ///   Test that the number of closing parens cannot be larger than
    ///   the number of opening parens already seen.
    /// </summary>
    [TestMethod]
    [TestCategory( "7" )]
    [ExpectedException( typeof( FormulaFormatException ) )]
    public void FormulaConstructor_TestClosingAfterBalanced_Fails( )
    {
        _ = new Formula( "(5 + 5))" );
    }

    /// <summary>
    ///   Test that even when "balanced", the order of parens must be correct.
    /// </summary>
    [TestMethod]
    [TestCategory( "8" )]
    [ExpectedException( typeof( FormulaFormatException ) )]
    public void FormulaConstructor_TestClosingBeforeOpening_Fails( )
    {
        _ = new Formula( "5)(" );
    }

    /// <summary>
    ///   Make sure multiple/nested parens that are correct, are accepted.
    /// </summary>
    [TestMethod]
    [TestCategory( "9" )]
    public void FormulaConstructor_TestValidComplexParens_Succeeds( )
    {
        _ = new Formula( "(5 + ((3+2) - 5 / 2))" );
    }

    // --- Test Balanced Parentheses Rule ---

    /// <summary>
    ///   Make sure that an unbalanced parentheses set throws an exception.
    /// </summary>
    [TestMethod]
    [TestCategory( "10" )]
    [ExpectedException( typeof( FormulaFormatException ) )]
    public void FormulaConstructor_TestUnclosedParens_Fails( )
    {
        _ = new Formula( "(5 + 2" );
    }

    /// <summary>
    ///   Test that multiple sets of balanced parens work properly.
    /// </summary>
    [TestMethod]
    [TestCategory( "11" )]
    public void FormulaConstructor_TestManyParens_Succeeds( )
    {
        _ = new Formula( "(1 + 2) - (1 + 2) - (1 + 2)" );
    }

    /// <summary>
    ///   Test that lots of balanced nested parentheses are accepted.
    /// </summary>
    [TestMethod]
    [TestCategory( "12" )]
    public void FormulaConstructor_TestDeeplyNestedParens_Succeeds( )
    {
        _ = new Formula( "(((5)))" );
    }

    // --- Test First Token Rule ---

    /// <summary>
    ///   The first token cannot be a closing paren.
    /// </summary>
    [TestMethod]
    [TestCategory( "13" )]
    [ExpectedException( typeof( FormulaFormatException ) )]
    public void FormulaConstructor_TestInvalidFirstTokenClosingParen_Fails( )
    {
        _ = new Formula( ")" );
    }

    /// <summary>
    ///   Test that the first token cannot be a math operator (+).
    /// </summary>
    [TestMethod]
    [TestCategory( "14" )]
    [ExpectedException( typeof( FormulaFormatException ) )]
    public void FormulaConstructor_TestInvalidFirstTokenPlus_Fails( )
    {
        _ = new Formula( "+" );
    }

    /// <summary>
    ///   Test that the first token cannot be a math operator (*).
    /// </summary>
    [TestMethod]
    [TestCategory( "15" )]
    [ExpectedException( typeof( FormulaFormatException ) )]
    public void FormulaConstructor_TestInvalidFirstTokenMultiply_Fails( )
    {
        _ = new Formula( "*" );
    }

    /// <summary>
    ///   Test that an integer number can be a valid first token.
    /// </summary>
    [TestMethod]
    [TestCategory( "16" )]
    public void FormulaConstructor_TestValidFirstTokenInteger_Succeeds( )
    {
        _ = new Formula( "1" );
    }

    /// <summary>
    ///   Test that a floating point number can be a valid first token.
    /// </summary>
    [TestMethod]
    [TestCategory( "17" )]
    public void FormulaConstructor_TestValidFirstTokenFloat_Succeeds( )
    {
        _ = new Formula( "1.0" );
    }

    // --- Test Last Token Rule ---

    /// <summary>
    ///   Make sure the last token is valid, in this case, not an operator (plus).
    /// </summary>
    [TestMethod]
    [TestCategory( "18" )]
    [ExpectedException( typeof( FormulaFormatException ) )]
    public void FormulaConstructor_TestInvalidLastTokenPlus_Fails( )
    {
        _ = new Formula( "5 +" );
    }

    /// <summary>
    ///   Make sure the last token is valid, in this case, not a closing paren.
    /// </summary>
    [TestMethod]
    [TestCategory( "19" )]
    [ExpectedException( typeof( FormulaFormatException ) )]
    public void FormulaConstructor_TestInvalidLastTokenClosingParen_Fails( )
    {
        _ = new Formula( "5 (" );
    }

    // --- Test Parentheses/Operator Following Rule ---

    /// <summary>
    ///   Test that after an opening paren, there cannot be an invalid token, in this
    ///   case a math operator (+).
    /// </summary>
    [TestMethod]
    [TestCategory( "20" )]
    [ExpectedException( typeof( FormulaFormatException ) )]
    public void FormulaConstructor_TestOpAfterOpenParen_Fails( )
    {
        _ = new Formula( "( + 2)" );
    }

    /// <summary>
    ///   Test that a closing paren cannot come after an opening paren.
    /// </summary>
    [TestMethod]
    [TestCategory( "21" )]
    [ExpectedException( typeof( FormulaFormatException ) )]
    public void FormulaConstructor_TestEmptyParens_Fails( )
    {
        _ = new Formula( "()" );
    }

    // --- Test Extra Following Rule ---

    /// <summary>
    ///   Make sure that two consecutive numbers are invalid.
    /// </summary>
    [TestMethod]
    [TestCategory( "22" )]
    [ExpectedException( typeof( FormulaFormatException ) )]
    public void FormulaConstructor_TestConsecutiveNumbers_Fails( )
    {
        _ = new Formula( "5 5" );
    }

    /// <summary>
    ///   Test that two consecutive operators is invalid.
    /// </summary>
    [TestMethod]
    [TestCategory( "23" )]
    [ExpectedException( typeof( FormulaFormatException ) )]
    public void FormulaConstructor_TestConsecutiveOps_Fails( )
    {
        _ = new Formula( "5+-2" );
    }

    /// <summary>
    ///   Test that a closing paren cannot come after an operator (plus).
    /// </summary>
    [TestMethod]
    [TestCategory( "24" )]
    [ExpectedException( typeof( FormulaFormatException ) )]
    public void TestCloseParenAfterOp( )
    {
        _ = new Formula( "(5+)2" );
    }

    /// <summary>
    ///   Test bad variable name.
    /// </summary>
    [TestMethod]
    [TestCategory( "25" )]
    [ExpectedException( typeof( FormulaFormatException ) )]
    public void FormulaConstructor_TestInvalidVariableName_Throws( )
    {
        _ = new Formula( "a" );
    }

    // Get Vars Tests
    [TestMethod]
    [Timeout( 2000 )]
    [TestCategory( "26" )]
    public void GetVars_BasicVariable_ReturnsVariable( )
    {
        Formula f = new("2+X1");
        ISet<string> vars = f.GetVariables();

        Assert.IsTrue( vars.SetEquals( ["X1"] ) );
    }

    [TestMethod]
    [Timeout( 2000 )]
    [TestCategory( "27" )]
    public void GetVariables_ManyVariables_ReturnsThemAll( )
    {
        Formula f = new("X1+X2+X3+X4+A1+B1+C5");
        ISet<string> vars = f.GetVariables();

        Assert.IsTrue( vars.SetEquals( ["X1", "X2", "X3", "X4", "A1", "B1", "C5"] ) );
    }

    [TestMethod]
    [Timeout( 2000 )]
    [TestCategory( "28" )]
    public void TestGetVars_ManySameVariable_ReturnsUniqueVariable( )
    {
        Formula f = new("X1+X1+X1+X1+X1+X1+X1+X1+X1+X1+X1");
        ISet<string> vars = f.GetVariables();

        Assert.IsTrue( vars.SetEquals( ["X1"] ) );
    }

    // To String Tests
    [TestMethod]
    [Timeout( 2000 )]
    [TestCategory( "29" )]
    public void ToString_BasicFormula_ReturnsSameFormula( )
    {
        Formula f1 = new("2+A1");

        Assert.IsTrue( f1.ToString().Equals( "2+A1" ) );
    }

    [TestMethod]
    [Timeout( 2000 )]
    [TestCategory( "30" )]
    public void ToString_Numbers_UsesCanonicalForm( )
    {
        Formula f1 = new("2.0000+A1");
        Assert.IsTrue( f1.ToString().Equals( "2+A1" ) );
        f1 = new( "2.0000-3" );
        Assert.IsTrue( f1.ToString().Equals( "2-3" ) );
        f1 = new( "2.0000-3e2" );
        Assert.IsTrue( f1.ToString().Equals( "2-300" ) );
        f1 = new( "1e20" );
        Assert.IsTrue( f1.ToString().Equals( "1E+20" ) );
    }

    [TestMethod]
    [Timeout( 2000 )]
    [TestCategory( "31" )]
    public void ToString_SpacesInFormula_SpacesRemoved( )
    {
        Formula f1 = new("        2             +                    A1          ");
        Assert.IsTrue( f1.ToString().Equals( "2+A1" ) );
    }

    [TestMethod]
    [Timeout( 2000 )]
    [TestCategory( "32" )]
    public void NormalizerAndToString_LowerCaseAndUpperCase_ResultInSameString( )
    {
        Formula f1 = new("2+x1");
        Formula f2 = new("2+X1");

        Assert.IsTrue( f1.ToString().Equals( "2+X1" ) );
        Assert.IsTrue( f1.ToString().Equals( f2.ToString() ) );
    }

    // Normalizer tests
    [TestMethod]
    [Timeout( 2000 )]
    [TestCategory( "32" )]
    public void NormalizerAndGetVars_LowerCaseVariable_UpCasesVariable( )
    {
        Formula f = new("2+x1");
        ISet<string> vars = f.GetVariables();

        Assert.IsTrue( vars.SetEquals( ["X1"] ) );
    }

    [TestMethod]
    [Timeout( 2000 )]
    [TestCategory( "33" )]
    public void GetVars_ManyCaseSwappingVariables_UpCasesAll( )
    {
        Formula f = new("x1+X2+x3+X4+a1+B1+c5");
        ISet<string> vars = f.GetVariables();

        Assert.IsTrue( vars.SetEquals( ["X1", "X2", "X3", "X4", "A1", "B1", "C5"] ) );
    }

    // Some more general syntax errors detected by the constructor
    [TestMethod]
    [Timeout( 2000 )]
    [TestCategory( "34" )]
    [ExpectedException( typeof( FormulaFormatException ) )]
    public void TestSingleOperator( )
    {
        new Formula( "+" );
    }

    [TestMethod]
    [Timeout( 2000 )]
    [TestCategory( "35" )]
    [ExpectedException( typeof( FormulaFormatException ) )]
    public void TestExtraOperator( )
    {
        new Formula( "2+5+" );
    }

    [TestMethod]
    [Timeout( 2000 )]
    [TestCategory( "36" )]
    [ExpectedException( typeof( FormulaFormatException ) )]
    public void TestExtraCloseParen( )
    {
        new Formula( "2+5*7)" );
    }

    [TestMethod]
    [Timeout( 2000 )]
    [TestCategory( "37" )]
    [ExpectedException( typeof( FormulaFormatException ) )]
    public void TestExtraOpenParen( )
    {
        new Formula( "((3+5*7)" );
    }

    [TestMethod]
    [Timeout( 2000 )]
    [TestCategory( "38" )]
    [ExpectedException( typeof( FormulaFormatException ) )]
    public void TestXasMultiply( )
    {
        new Formula( "5x5" );
    }

    [TestMethod]
    [Timeout( 2000 )]
    [TestCategory( "39" )]
    [ExpectedException( typeof( FormulaFormatException ) )]
    public void TestNoOperator2( )
    {
        new Formula( "5+5x" );
    }

    [TestMethod]
    [Timeout( 2000 )]
    [TestCategory( "40" )]
    [ExpectedException( typeof( FormulaFormatException ) )]
    public void TestNoOperator3( )
    {
        new Formula( "5+7+(5)8" );
    }

    [TestMethod]
    [Timeout( 2000 )]
    [TestCategory( "41" )]
    [ExpectedException( typeof( FormulaFormatException ) )]
    public void TestNoOperator4( )
    {
        new Formula( "5 5" );
    }

    [TestMethod]
    [Timeout( 2000 )]
    [TestCategory( "42" )]
    [ExpectedException( typeof( FormulaFormatException ) )]
    public void TestDoubleOperator( )
    {
        new Formula( "5 + + 3" );
    }

    // Some more complicated formula evaluations
    [TestMethod]
    [Timeout( 2000 )]
    [TestCategory( "43" )]
    public void FormulaConstructor_TestComplex_IsValid( )
    {
        Formula f = new("y1*3-8/2+4*(8-9*2)/14*x7");
    }

    [TestMethod]
    [Timeout( 2000 )]
    [TestCategory( "44" )]
    public void FormulaConstructor_MatchingParens_EachLeftHasARight( )
    {
        Formula f = new("x1+(x2+(x3+(x4+(x5+x6))))");
    }

    [TestMethod]
    [Timeout( 2000 )]
    [TestCategory( "45" )]
    public void FormulaConstructor_TestLotsOfLeftParens_IsValidAndMatching( )
    {
        Formula f = new("((((x1+x2)+x3)+x4)+x5)+x6");
    }

    [TestMethod]
    [Timeout( 2000 )]
    [TestCategory( "46" )]
    public void ToString_Whitespace_RemovedInCannonicalForm( )
    {
        Formula f1 = new("X1+X2");
        Formula f2 = new(" X1  +  X2   ");
        Assert.IsTrue( f1.ToString().Equals( f2.ToString() ) );
    }

    [TestMethod]
    [Timeout( 2000 )]
    [TestCategory( "47" )]
    public void ToString_DifferentNumberRepresentations_EquateToSameCanonicalForm( )
    {
        Formula f1 = new("2+X1*3.00");
        Formula f2 = new("2.00+X1*3.0");
        Assert.IsTrue( f1.ToString().Equals( f2.ToString() ) );
    }

    [TestMethod]
    [Timeout( 2000 )]
    [TestCategory( "48" )]
    public void ToString_DifferentNumberRepresentations_EquateToSameCanonicalForm2( )
    {
        Formula f1 = new("1e-2 + X5 + 17.00 * 19 ");
        Formula f2 = new("   0.0100  +     X5+ 17 * 19.00000 ");
        Assert.IsTrue( f1.ToString().Equals( f2.ToString() ) );
    }

    [TestMethod]
    [Timeout( 2000 )]
    [TestCategory( "49" )]
    public void ToString_DifferentFormulas_HaveDifferentStrings( )
    {
        Formula f1 = new("2");
        Formula f2 = new("5");
        Assert.IsTrue( f1.ToString() != f2.ToString() );
    }

    // Tests of GetVariables method
    [TestMethod]
    [Timeout( 2000 )]
    [TestCategory( "50" )]
    public void GetVariables_NoVariables_ReturnsEmptySet( )
    {
        Formula f = new("2*5");
        Assert.IsFalse( f.GetVariables().Any() );
    }

    [TestMethod]
    [Timeout( 2000 )]
    [TestCategory( "51" )]
    public void GetVariables_OneVariable_ReturnsTheOne( )
    {
        Formula f = new("2*X2");
        List<string> actual = new(f.GetVariables());
        HashSet<string> expected = ["X2"];
        Assert.AreEqual( actual.Count, 1 );
        Assert.IsTrue( expected.SetEquals( actual ) );
    }

    [TestMethod]
    [Timeout( 2000 )]
    [TestCategory( "52" )]
    public void GetVariables_TwoVariables_ReturnsBoth( )
    {
        Formula f = new("2*X2+Y3");
        List<string> actual = new(f.GetVariables());
        HashSet<string> expected = ["Y3", "X2"];
        Assert.AreEqual( actual.Count, 2 );
        Assert.IsTrue( expected.SetEquals( actual ) );
    }

    [TestMethod]
    [Timeout( 2000 )]
    [TestCategory( "53" )]
    public void GetVariables_Duplicated_ReturnsOnlyOneValue( )
    {
        Formula f = new("2*X2+X2");
        List<string> actual = new(f.GetVariables());
        HashSet<string> expected = ["X2"];
        Assert.AreEqual( actual.Count, 1 );
        Assert.IsTrue( expected.SetEquals( actual ) );
    }

    [TestMethod]
    [Timeout( 2000 )]
    [TestCategory( "54" )]
    public void GetVariables_LotsOfVariablesWithOperatorsAndRepeats_ReturnsCompleteList( )
    {
        Formula f = new("X1+Y2*X3*Y2+Z7+X1/Z8");
        List<string> actual = new(f.GetVariables());
        HashSet<string> expected = ["X1", "Y2", "X3", "Z7", "Z8"];
        Assert.AreEqual( actual.Count, 5 );
        Assert.IsTrue( expected.SetEquals( actual ) );
    }

    // Test some longish valid formulas.
    [TestMethod]
    [Timeout( 2000 )]
    [TestCategory( "55" )]
    public void FormulaConstructor_LongComplexFormula_IsAValidFormula( )
    {
        _ = new Formula( "(((((2+3*X1)/(7e-5+X2-X4))*X5+.0005e+92)-8.2)*3.14159) * ((x2+3.1)-.00000000008)" );
    }

    [TestMethod]
    [Timeout( 2000 )]
    [TestCategory( "56" )]
    public void FormulaConstructor_LongComplexFormula2_IsAValidFormula( )
    {
        _ = new Formula( "5 + (1-2) * 3.14 / 1e6 + 0.2E-9 - A1 + bb22" );
    }

}

