// <copyright file="Formula_PS2.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>
// <summary>
//   <para>
//     This code is provides to start your assignment.  It was written
//     by Profs Joe, Danny, and Jim.  You should keep this attribution
//     at the top of your code where you have your header comment, along
//     with the other required information.
//   </para>
//   <para>
//     You should remove/add/adjust comments in your file as appropriate
//     to represent your work and any changes you make.
//   </para>
// </summary>
// <authors> [Leo Yu] </authors>
// <date> [Feb 7, 2025] </date>

namespace CS3500.Formula;

using System.Text.RegularExpressions;

/// <summary>
///   <para>
///     This class represents formulas written in standard infix notation using standard precedence
///     rules.  The allowed symbols are non-negative numbers written using double-precision
///     floating-point syntax; variables that consist of one or more letters followed by
///     one or more numbers; parentheses; and the four operator symbols +, -, *, and /.
///   </para>
///   <para>
///     Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
///     a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable;
///     and "x 23" consists of a variable "x" and a number "23".  Otherwise, spaces are to be removed.
///   </para>
///   <para>
///     For Assignment Two, you are to implement the following functionality:
///   </para>
///   <list type="bullet">
///     <item>
///        Formula Constructor which checks the syntax of a formula.
///     </item>
///     <item>
///        Get Variables
///     </item>
///     <item>
///        ToString
///     </item>
///   </list>
/// </summary>
public class Formula
{
    /// <summary>
    ///   All variables are letters followed by numbers.  This pattern
    ///   represents valid variable name strings.
    /// </summary>
    private const string VariableRegExPattern = @"[a-zA-Z]+\d+";
    
    /// <summary>
    /// Tracks all unique variables encountered (stored in uppercase).
    /// </summary>
    private HashSet<string> _formulaVariableSet;

    /// <summary>
    /// Stores the canonical string representation of the formula, built
    /// as tokens are processed. Used by the <see cref="ToString"/> method.
    /// </summary>
    private string _canonicalTokenExpression;
    
    /// <summary>
    ///   Initializes a new instance of the <see cref="Formula"/> class.
    ///   <para>
    ///     Creates a Formula from a string that consists of an infix expression written as
    ///     described in the class comment.  If the expression is syntactically incorrect,
    ///     throws a FormulaFormatException with an explanatory Message.  See the assignment
    ///     specifications for the syntax rules you are to implement.
    ///   </para>
    ///   <para>
    ///     NOTE: Included in-code comments to help readability of the rules and structure
    ///   </para>
    ///   <para>
    ///     Non Exhaustive Example Errors:
    ///   </para>
    ///   <list type="bullet">
    ///     <item>
    ///        Invalid variable name, e.g., x, x1x  (Note: x1 is valid, but would be normalized to X1)
    ///     </item>
    ///     <item>
    ///        Empty formula, e.g., string.Empty
    ///     </item>
    ///     <item>
    ///        Mismatched Parentheses, e.g., "(("
    ///     </item>
    ///     <item>
    ///        Invalid Following Rule, e.g., "2x+5"
    ///     </item>
    ///   </list>
    /// </summary>
    /// <param name="formula"> The string representation of the formula to be created.</param>
    public Formula( string formula )
    {
        // One Token Rule
        if (formula == string.Empty)
            throw new FormulaFormatException("No Tokens were provided.");
        
        List<string> tokens = GetTokens(formula);
        
        // White space edge case check
        if (tokens.Count == 0)
        {
            throw new FormulaFormatException("No tokens (only whitespace) were provided.");
        }
        
        if (FirstTokenRule(tokens[0]) == false)
            throw new FormulaFormatException("Invalid First Token.");
        
        _formulaVariableSet = new HashSet<string>();
        String lastToken = "";
        int leftParenCount = 0;
        int rightParenCount = 0;
        _canonicalTokenExpression = "";
        
        foreach (string current in tokens)
        {
            
            if(IsValidToken(current)==false)
                throw new FormulaFormatException("Invalid Token");
            
            // concatenates string for ToString and add variables for GetVariables
            if (IsVar(current))
            {
                if (_formulaVariableSet.Contains(current) != true)
                    _formulaVariableSet.Add(current.ToUpper());
                _canonicalTokenExpression += current.ToUpper();
            }
            else if (IsNumber(current))
            {
                double num = Double.Parse(current);
                _canonicalTokenExpression += num;
            }
            else
            {
                _canonicalTokenExpression += current;
            }
            
            if (IsParenthesis(current))
            {
                if (current == "(")
                {
                    leftParenCount++;
                }
                else
                {
                    rightParenCount++;
                }
            }
            
            // Closing Parentheses Rule
            if (rightParenCount > leftParenCount)
            {
                throw new FormulaFormatException("Invalid parentheses formatting");
            }
            
            if (ParenOperatorFollowingRule(lastToken, current) == false)
                throw new FormulaFormatException("Invalid Token Following Open Parentheses");
            
            if (ExtraFollowingRule(lastToken, current) == false)
                throw new FormulaFormatException("Invalid Syntax");
            
            lastToken = current;
            
        }
        
        // balanced paren rule check
        if (leftParenCount != rightParenCount)
            throw new FormulaFormatException("Invalid parentheses formatting");
        
        if(LastTokenRule(lastToken) == false)
            throw new FormulaFormatException("Invalid Last Token");
    }

    /// <summary>
    /// Checks Rule #8 (Extra Following Rule): If the last token was a number, variable,
    /// or a closing parenthesis, the current token must be an operator or a closing parenthesis.
    /// Used internally to maintain code readability and ensure correct formula syntax.
    /// </summary>
    /// <param name="lastToken">
    /// The previous token in the formula, potentially a number, variable, operator, or parenthesis.
    /// </param>
    /// <param name="current">
    /// The current token in the formula being evaluated.
    /// </param>
    /// <returns>
    /// True if the Extra Following Rule is satisfied; otherwise, false.
    /// </returns>
    private bool ExtraFollowingRule(string lastToken, string current)
    {
        return !((IsNumber(lastToken) || IsVar(lastToken) || lastToken == ")") &&
                 (IsOperator(current) || current == ")") == false);
    }

    /// <summary>
    /// Checks Rule #7 (Parenthesis/Operator Following Rule): If the previous token was
    /// an open parenthesis or an operator, the next token must be a number, variable,
    /// or another open parenthesis.
    /// Used internally to maintain code readability and ensure correct formula syntax.
    /// </summary>
    /// <param name="lastToken">
    /// The previous token in the formula, which might be '(' or one of the operators.
    /// </param>
    /// <param name="current">
    /// The current token in the formula being evaluated.
    /// </param>
    /// <returns>
    /// True if this rule is satisfied; false otherwise.
    /// </returns>
    private bool ParenOperatorFollowingRule(string lastToken, string current)
    {
        if ((lastToken == "(" || IsOperator(lastToken)) &&
            (IsNumber(current) || IsVar(current) || current == "(") == false)
            return false;
        return true;
    }

    /// <summary>
    /// Checks the Last Token Rule (#6): The final token in the formula must be
    /// a number, a variable, or a closing parenthesis.
    /// Used internally by the constructor to verify overall formula syntax.
    /// </summary>
    /// <param name="token">The final token of the formula string.</param>
    /// <returns>
    /// True if the token is valid as the last token; false otherwise.
    /// </returns>
    private bool LastTokenRule(string token)
    {
        if ((IsNumber(token) || IsVar(token) || token == ")") == false)
            return false;
        return true;
    }

    /// <summary>
    /// Checks the First Token Rule (#5): The initial token in the formula must be
    /// a number, a variable, or an opening parenthesis.
    /// Used internally by the constructor to verify overall formula syntax.
    /// </summary>
    /// <param name="token">The first token of the formula string.</param>
    /// <returns>
    /// True if the token is valid as the first token; false otherwise.
    /// </returns>
    private bool FirstTokenRule(string token)
    {
        if ((IsVar(token) || IsNumber(token) || token == "(") == false)
            return false;
        return true;
    }
    
    /// <summary>
    /// Rule 2 - Valid Tokens Rule: Determines if the given token is one of the valid token types:
    /// a number, variable, operator, or parenthesis. This method centralizes
    /// the basic token check and is used by the constructor for readability
    /// before applying deeper syntax checks.
    /// </summary>
    /// <param name="token">The token to validate.</param>
    /// <returns>
    /// True if <paramref name="token"/> is a valid token; otherwise, false.
    /// </returns>
    private bool IsValidToken(string token)
    { 
        if ((IsNumber(token) || IsVar(token) ||
           IsOperator(token) || IsParenthesis(token )) == false)
            return false;
        return true;
    }
    
    /// <summary>
    /// Checks if the given token can be parsed as a non-negative number
    /// (allowing decimal and scientific notation). This is a helper method
    /// used to simplify the constructor's logic and make Valid Tokens Method more
    /// readable.
    /// </summary>
    /// <param name="token">The token to check for numeric form.</param>
    /// <returns>
    /// True if <paramref name="token"/> can be parsed as a valid double; otherwise, false.
    /// </returns>
    private bool IsNumber(string token)
    {
        return Double.TryParse(token, out _);
    }
    
    /// <summary>
    /// Checks if the given token is one of the arithmetic operators: +, -, *, or /.
    /// Used in syntax checks to enforce operator-following rules.
    /// </summary>
    /// <param name="token">A token that may be an operator.</param>
    /// <returns>
    /// True if <paramref name="token"/> is recognized as an operator; otherwise, false.
    /// </returns>
    private bool IsOperator(string token)
    {
        HashSet<string> _operators = new HashSet<string> { "+", "-", "*", "/" };
        return _operators.Contains(token);
    }

    /// <summary>
    /// Checks if the given token is a parenthesis, '(' or ')'.
    /// Used in the constructor logic for counting and validating parentheses.
    /// </summary>
    /// <param name="token">A token that may be '(' or ')'.</param>
    /// <returns>
    /// True if <paramref name="token"/> is a parenthesis; otherwise, false.
    /// </returns>
    private bool IsParenthesis(String token)
    {
        return token == "(" || token == ")";
    }

    /// <summary>
    ///   <para>
    ///     Returns a set of all the variables in the formula.
    ///   </para>
    ///   <remarks>
    ///     Important: no variable may appear more than once in the returned set, even
    ///     if it is used more than once in the Formula.
	///     Variables should be returned in canonical form, having all letters converted
	///     to uppercase.
    ///   </remarks>
    ///   <list type="bullet">
    ///     <item>new("x1+y1*z1").GetVariables() should return a set containing "X1", "Y1", and "Z1".</item>
    ///     <item>new("x1+X1"   ).GetVariables() should return a set containing "X1".</item>
    ///   </list>
    /// </summary>
    /// <returns> the set of variables (string names) representing the variables referenced by the formula. </returns>
    public ISet<string> GetVariables( )
    { 
        return _formulaVariableSet;
    }

    /// <summary>
    ///   <para>
    ///     Returns a string representation of a canonical form of the formula.
    ///   </para>
    ///   <para>
    ///     The string will contain no spaces.
    ///   </para>
    ///   <para>
    ///     If the string is passed to the Formula constructor, the new Formula f 
    ///     will be such that this.ToString() == f.ToString().
    ///   </para>
    ///   <para>
    ///     All of the variable and number tokens in the string will be normalized.
    ///     For numbers, this means that the original string token is converted to
    ///     a number using double.Parse or double.TryParse, then converted back to a 
    ///     string using double.ToString.
    ///     For variables, this means all letters are uppercased.
    ///   </para>
    ///   <para>
    ///       For example:
    ///   </para>
    ///   <code>
    ///       new("x1 + Y1").ToString() should return "X1+Y1"
    ///       new("x1 + 5.0000").ToString() should return "X1+5".
    ///   </code>
    ///   <para>
    ///     This method should execute in O(1) time.
    ///   </para>
    /// </summary>
    /// <returns>
    ///   A canonical version (string) of the formula. All "equal" formulas
    ///   should have the same value here.
    /// </returns>
    public override string ToString( )
    {
        return _canonicalTokenExpression;
    }

    /// <summary>
    ///   Reports whether "token" is a variable.  It must be one or more letters
    ///   followed by one or more numbers.
    /// </summary>
    /// <param name="token"> A token that may be a variable. </param>
    /// <returns> true if the string matches the requirements, e.g., A1 or a1. </returns>
    private static bool IsVar( string token )
    {
        // notice the use of ^ and $ to denote that the entire string being matched is just the variable
        string standaloneVarPattern = $"^{VariableRegExPattern}$";
        return Regex.IsMatch( token, standaloneVarPattern );
    }

    /// <summary>
    ///   <para>
    ///     Given an expression, enumerates the tokens that compose it.
    ///   </para>
    ///   <para>
    ///     Tokens returned are:
    ///   </para>
    ///   <list type="bullet">
    ///     <item>left paren</item>
    ///     <item>right paren</item>
    ///     <item>one of the four operator symbols</item>
    ///     <item>a string consisting of one or more letters followed by one or more numbers</item>
    ///     <item>a double literal</item>
    ///     <item>and anything that doesn't match one of the above patterns</item>
    ///   </list>
    ///   <para>
    ///     There are no empty tokens; white space is ignored (except to separate other tokens).
    ///   </para>
    /// </summary>
    /// <param name="formula"> A string representing an infix formula such as 1*B1/3.0. </param>
    /// <returns> The ordered list of tokens in the formula. </returns>
    private static List<string> GetTokens( string formula )
    {
        List<string> results = [];

        string lpPattern = @"\(";
        string rpPattern = @"\)";
        string opPattern = @"[\+\-*/]";
        string doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
        string spacePattern = @"\s+";

        // Overall pattern
        string pattern = string.Format(
                                        "({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                        lpPattern,
                                        rpPattern,
                                        opPattern,
                                        VariableRegExPattern,
                                        doublePattern,
                                        spacePattern);

        // Enumerate matching tokens that don't consist solely of white space.
        foreach ( string s in Regex.Split( formula, pattern, RegexOptions.IgnorePatternWhitespace ) )
        {
            if ( !Regex.IsMatch( s, @"^\s*$", RegexOptions.Singleline ) )
            {
                results.Add(s);
            }
        }

        return results;
    }
    
    /// <summary>
    ///   <para>
    ///     Reports whether f1 == f2, using the notion of equality from the <see cref="Equals"/> method.
    ///   </para>
    /// </summary>
    /// <param name="f1"> The first of two formula objects. </param>
    /// <param name="f2"> The second of two formula objects. </param>
    /// <returns> true if the two formulas are the same.</returns>
    public static bool operator ==( Formula f1, Formula f2 )
    {
        return f1.Equals(f2);
    }

    /// <summary>
    ///   <para>
    ///     Reports whether f1 != f2, using the notion of equality from the <see cref="Equals"/> method.
    ///   </para>
    /// </summary>
    /// <param name="f1"> The first of two formula objects. </param>
    /// <param name="f2"> The second of two formula objects. </param>
    /// <returns> true if the two formulas are not equal to each other.</returns>
    public static bool operator !=( Formula f1, Formula f2 )
    {
        return !(f1 == f2);
    }

    /// <summary>
    ///   <para>
    ///     Determines if two formula objects represent the same formula.
    ///   </para>
    ///   <para>
    ///     By definition, if the parameter is null or does not reference 
    ///     a Formula Object then return false.
    ///   </para>
    ///   <para>
    ///     Two Formulas are considered equal if their canonical string representations
    ///     (as defined by ToString) are equal.  
    ///   </para>
    /// </summary>
    /// <param name="obj"> The other object.</param>
    /// <returns>
    ///   True if the two objects represent the same formula.
    /// </returns>
    public override bool Equals( object? obj )
    {
        if (obj is not Formula f){
            return false;
        }
        return this.ToString() == f.ToString();
    }

    /// <summary>
    ///   <para>
    ///     Evaluates this Formula, using the lookup delegate to determine the values of
    ///     variables.
    ///   </para>
    ///   <remarks>
    ///     When the lookup method is called, it will always be passed a normalized (capitalized)
    ///     variable name.  The lookup method will throw an ArgumentException if there is
    ///     not a definition for that variable token.
    ///   </remarks>
    ///   <para>
    ///     If no undefined variables or divisions by zero are encountered when evaluating
    ///     this Formula, the numeric value of the formula is returned.  Otherwise, a 
    ///     FormulaError is returned (with a meaningful explanation as the Reason property).
    ///   </para>
    ///   <para>
    ///     This method should never throw an exception.
    ///   </para>
    /// </summary>
    /// <param name="lookup">
    ///   <para>
    ///     Given a variable symbol as its parameter, lookup returns the variable's value
    ///     (if it has one) or throws an ArgumentException (otherwise).  This method will expect 
    ///     variable names to be normalized.
    ///   </para>
    /// </param>
    /// <returns> Either a double or a FormulaError, based on evaluating the formula.</returns>
    public object Evaluate( Lookup lookup )
    {
        Stack<double> valuesStack = new Stack<double>();
        Stack<string> operatorsStack = new Stack<string>();
        List<string> results = GetTokens(this.ToString());
        string lastToken = results.Last();

        foreach (string token in results)
        {
            if (IsNumber(token))
            {
                double currentNum = double.Parse(token);
                // If * or / is on top of operator stack
                if (operatorsStack.Count > 0 && operatorsStack.Peek() == "*" || operatorsStack.Count > 0 && operatorsStack.Peek() == "/")
                {
                    string tempOp = operatorsStack.Pop();
                    double tempInt = valuesStack.Pop(); 
                    if (tempOp == "*")
                    {
                        valuesStack.Push(tempInt * currentNum);
                    }
                    else
                    {
                        if (currentNum == 0)
                        {
                            return new FormulaError("Division by zero");
                        }
                        valuesStack.Push(tempInt / currentNum);
                    }
                }
                else
                {
                   valuesStack.Push(currentNum); 
                }
            }
            else if (IsVar(token))
            {
                // Try catch for lookup
                try
                { 
                    double currentNum  = lookup(token);
                    // If * or / is on top of operator stack
                    if (operatorsStack.Count > 0 && operatorsStack.Peek() == "*" || operatorsStack.Count > 0 && operatorsStack.Peek() == "/")
                    {
                        string tempOp = operatorsStack.Pop();
                        double tempInt = valuesStack.Pop(); 
                        if (tempOp == "*")
                        {
                            valuesStack.Push(tempInt * currentNum);
                        }
                        else
                        {
                            if (currentNum == 0)
                            {
                                return new FormulaError("Division by zero");
                            }
                            valuesStack.Push(tempInt / currentNum);
                        }
                    }
                    else
                    {
                        valuesStack.Push(currentNum); 
                    }
                }
                // Returns FormulaError if lookup failed
                catch (ArgumentException)
                {
                    return new FormulaError("Variable doesn't exist");
                }
            }
            // If token is the + or - operator
            else if (IsOperator(token) && (token == "+" || token == "-"))
            {
                // If + or - is on top of operator stack
                if (operatorsStack.Count > 0 && (operatorsStack.Peek() == "+" || operatorsStack.Peek() == "-"))
                {
                    double firstPop = valuesStack.Pop();
                    double secondPop = valuesStack.Pop();
                    string tempOperator = operatorsStack.Pop();
                    if (tempOperator == "+")
                    {
                        valuesStack.Push(secondPop + firstPop);
                    }
                    else
                    {
                        valuesStack.Push(secondPop - firstPop);
                    }
                    operatorsStack.Push(token);

                }
                else
                {
                    operatorsStack.Push(token);
                }
            }
            // If token is the * or / operator
            else if (IsOperator(token) && (token == "*" || token == "/"))
            {
                operatorsStack.Push(token);
            }
            else if (token == "(")
            {
                operatorsStack.Push(token);
            }
            // For the ")" token in the formula
            else
            {
                // If + or - is at the top of the operator stack
                if (operatorsStack.Count>0 && operatorsStack.Peek() == "+" || operatorsStack.Count>0 && operatorsStack.Peek() == "-")
                {
                    double firstPop = valuesStack.Pop();
                    double secondPop = valuesStack.Pop();
                    string tempOperator = operatorsStack.Pop();
                    if (tempOperator == "+")
                    {
                        
                        valuesStack.Push(secondPop + firstPop);
                    }
                    else
                    {
                        valuesStack.Push(secondPop - firstPop);
                    }
                }
                
                // Pops the ")" paren
                operatorsStack.Pop();

                // Finally, if * or / is at the top of the operator stack
                if (operatorsStack.Count>0 && operatorsStack.Peek() == "*" || operatorsStack.Count>0 && operatorsStack.Peek() == "/")
                {
                    double firstPop = valuesStack.Pop();
                    double secondPop = valuesStack.Pop();
                    string tempOperator = operatorsStack.Pop();
                    if (tempOperator == "*")
                    {
                        valuesStack.Push(secondPop * firstPop);
                    }
                    else
                    {
                        if (firstPop == 0)
                        {
                            return new FormulaError("Division by zero");
                        }
                        valuesStack.Push(secondPop / firstPop);
                    }
                }
            }
        }
        
        // End of loop logic
        // if operator stack is empty
        if (operatorsStack.Count == 0)
        {
            return valuesStack.Pop();
        }
        // if operator stack is not empty
        {
            double firstPop = valuesStack.Pop();
            double secondPop = valuesStack.Pop();
            string tempOperator = operatorsStack.Pop();
            if (tempOperator == "+")
            {
                return secondPop + firstPop;
            }
            else
            {
                return secondPop - firstPop;
            }
        } 
    }

    /// <summary>
    ///   <para>
    ///     Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
    ///     case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two
    ///     randomly-generated unequal Formulas have the same hash code should be extremely small.
    ///   </para>
    /// </summary>
    /// <returns> The hashcode for the object. </returns>
    public override int GetHashCode( )
    {
        return ToString().GetHashCode();
    }
}

/// <summary>
/// Used as a possible return value of the Formula.Evaluate method.
/// </summary>
public class FormulaError
{
    /// <summary>
    ///   Initializes a new instance of the <see cref="FormulaError"/> class.
    ///   <para>
    ///     Constructs a FormulaError containing the explanatory reason.
    ///   </para>
    /// </summary>
    /// <param name="message"> Contains a message for why the error occurred.</param>
    public FormulaError( string message )
    {
        Reason = message;
    }

    /// <summary>
    ///  Gets the reason why this FormulaError was created.
    /// </summary>
    public string Reason { get; private set; }
}

/// <summary>
///   Any method meeting this type signature can be used for
///   looking up the value of a variable.
/// </summary>
/// <exception cref="ArgumentException">
///   If a variable name is provided that is not recognized by the implementing method,
///   then the method should throw an ArgumentException.
/// </exception>
/// <param name="variableName">
///   The name of the variable (e.g., "A1") to lookup.
/// </param>
/// <returns> The value of the given variable (if one exists). </returns>
public delegate double Lookup( string variableName );

/// <summary>
///   Used to report syntax errors in the argument to the Formula constructor.
/// </summary>
public class FormulaFormatException : Exception
{
    /// <summary>
    ///   Initializes a new instance of the <see cref="FormulaFormatException"/> class.
    ///   <para>
    ///      Constructs a FormulaFormatException containing the explanatory message.
    ///   </para>
    /// </summary>
    /// <param name="message"> A developer defined message describing why the exception occured.</param>
    public FormulaFormatException( string message )
        : base( message )
    {
        // All this does is call the base constructor. No extra code needed.
    }
}
