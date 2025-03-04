// <copyright file="Spreadsheet.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>

// Written by Joe Zachary for CS 3500, September 2013
// Update by Profs Kopta and de St. Germain, Fall 2021, Fall 2024
//     - Updated return types
//     - Updated documentation
// <authors> [Leo Yu] </authors>
// <date> [Feb 21, 2025] </date>

using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace CS3500.Spreadsheet;

using CS3500.Formula;
using CS3500.DependencyGraph;

/// <summary>
///   <para>
///     Thrown to indicate that a change to a cell will cause a circular dependency.
///   </para>
/// </summary>
public class CircularException : Exception
{
}

/// <summary>
///   <para>
///     Thrown to indicate that a name parameter was invalid.
///   </para>
/// </summary>
public class InvalidNameException : Exception
{
}

/// <summary>
/// <para>
///   Thrown to indicate that a read or write attempt has failed with
///   an expected error message informing the user of what went wrong.
/// </para>
/// </summary>
public class SpreadsheetReadWriteException : Exception
{
  /// <summary>
  ///   <para>
  ///     Creates the exception with a message defining what went wrong.
  ///   </para>
  /// </summary>
  /// <param name="msg"> An informative message to the user. </param>
  public SpreadsheetReadWriteException( string msg )
    : base( msg )
  {
  }
}

/// <summary>
///   <para>
///     An Spreadsheet object represents the state of a simple spreadsheet.  A
///     spreadsheet represents an infinite number of named cells.
///   </para>
/// <para>
///     Valid Cell Names: A string is a valid cell name if and only if it is one or
///     more letters followed by one or more numbers, e.g., A5, BC27.
/// </para>
/// <para>
///    Cell names are case insensitive, so "x1" and "X1" are the same cell name.
///    Your code should normalize (uppercased) any stored name but accept either.
/// </para>
/// <para>
///     A spreadsheet represents a cell corresponding to every possible cell name.  (This
///     means that a spreadsheet contains an infinite number of cells.)  In addition to
///     a name, each cell has a contents and a value.  The distinction is important.
/// </para>
/// <para>
///     The <b>contents</b> of a cell can be (1) a string, (2) a double, or (3) a Formula.
///     If the contents of a cell is set to the empty string, the cell is considered empty.
/// </para>
/// <para>
///     By analogy, the contents of a cell in Excel is what is displayed on
///     the editing line when the cell is selected.
/// </para>
/// <para>
///     In a new spreadsheet, the contents of every cell is the empty string. Note:
///     this is by definition (it is IMPLIED, not stored).
/// </para>
/// <para>
///     The <b>value</b> of a cell can be (1) a string, (2) a double, or (3) a FormulaError.
///     (By analogy, the value of an Excel cell is what is displayed in that cell's position
///     in the grid.) We are not concerned with cell values yet, only with their contents,
///     but for context:
/// </para>
/// <list type="number">
///   <item>If a cell's contents is a string, its value is that string.</item>
///   <item>If a cell's contents is a double, its value is that double.</item>
///   <item>
///     <para>
///       If a cell's contents is a Formula, its value is either a double or a FormulaError,
///       as reported by the Evaluate method of the Formula class.  For this assignment,
///       you are not dealing with values yet.
///     </para>
///   </item>
/// </list>
/// <para>
///     Spreadsheets are never allowed to contain a combination of Formulas that establish
///     a circular dependency.  A circular dependency exists when a cell depends on itself,
///     either directly or indirectly.
///     For example, suppose that A1 contains B1*2, B1 contains C1*2, and C1 contains A1*2.
///     A1 depends on B1, which depends on C1, which depends on A1.  That's a circular
///     dependency.
/// </para>
/// </summary>
public class Spreadsheet
{
  /// <summary>
  ///   <para>
  ///     A dictionary that stores all non-empty cells in the spreadsheet.
  ///     The key is the normalized cell name (uppercase), and the value is the <see cref="Cell"/> object
  ///     representing the cell's contents.
  ///   </para>
  ///   <para>
  ///     This dictionary ensures efficient lookup and management of non-empty cells.
  ///   </para>
  /// </summary>
  [JsonPropertyName("Cells")][JsonInclude] private Dictionary<string, Cell> _cells;

  /// <summary>
  ///   <para>
  ///     A dependency graph that tracks the relationships between cells.
  ///     It is used to manage dependencies between cells, ensuring that changes to one cell
  ///     propagate correctly to all dependent cells.
  ///   </para>
  ///   <para>
  ///     For example, if cell B1 depends on cell A1, the dependency graph ensures that
  ///     changes to A1 trigger recalculation of B1.
  ///   </para>
  /// </summary>
  [JsonIgnore]
  private DependencyGraph _dependencyGraph;

  /// <summary>
  /// A nested class representing an individual cell within the spreadsheet.
  /// It holds the <see cref="Contents"/> (string, double, or Formula).
  /// Only hold content logic, and each cell will serve as a value for Dictionary
  /// keeping track on non-empty cells.
  /// </summary>
  private class Cell
  {
    public Cell()
    {
      StringForm = "";
      Contents = "";
    }

  public Cell(object initialVar)
    {
      Contents = initialVar;
      if (initialVar is string || initialVar is double)
      {
        StringForm = initialVar.ToString() ?? "";
        Value = initialVar;
      }
      else
      {
        this.Value = 0.0;
        this.StringForm = "=" + initialVar.ToString();
      }
      
    }
    public string StringForm { get; set; }

    [JsonIgnore]
    public object Contents { get; }
    [JsonIgnore]
    public object? Value { get; set; }
  }

  /// <summary>
  /// Creates a new, empty <see cref="Spreadsheet"/>.
  /// Initializes the internal dictionary and dependency graph.
  /// </summary>
  public Spreadsheet()
  {
    _cells = new Dictionary<string, Cell>();
    _dependencyGraph = new DependencyGraph();
  }

  /// <summary>
  ///   Provides a copy of the normalized names of all of the cells in the spreadsheet
  ///   that contain information (i.e., non-empty cells).
  /// </summary>
  /// <returns>
  ///   A set of the names of all the non-empty cells in the spreadsheet.
  /// </returns>
  public ISet<string> GetNamesOfAllNonemptyCells()
  {
    return new HashSet<string>(_cells.Keys);
  }

  /// <summary>
  ///   Returns the contents (as opposed to the value) of the named cell.
  /// </summary>
  ///
  /// <exception cref="InvalidNameException">
  ///   Thrown if the name is invalid.
  /// </exception>
  ///
  /// <param name="name">The name of the spreadsheet cell to query. </param>
  /// <returns>
  ///   The contents as either a string, a double, or a Formula.
  ///   See the class header summary.
  /// </returns>
  public object GetCellContents(string name)
  {
    string normalizedName = ValidateNameAndNormalize(name);

    // Check if the cell is in our dictionary
    if (_cells.TryGetValue(normalizedName, out Cell? cell))
    {
      return cell.Contents;
    }
    else
    {
      // Empty cells have empty-string contents by definition
      return "";
    }
  }

  /// <summary>
  ///  Set the contents of the named cell to the given number.
  /// </summary>
  ///
  /// <exception cref="InvalidNameException">
  ///   If the name is invalid, throw an InvalidNameException.
  /// </exception>
  ///
  /// <param name="name"> The name of the cell. </param>
  /// <param name="number"> The new contents of the cell. </param>
  /// <returns>
  ///   <para>
  ///     This method returns an ordered list consisting of the passed in name
  ///     followed by the names of all other cells whose value depends, directly
  ///     or indirectly, on the named cell.
  ///   </para>
  ///   <para>
  ///     The order must correspond to a valid dependency ordering for recomputing
  ///     all of the cells, i.e., if you re-evaluate each cells in the order of the list,
  ///     the overall spreadsheet will be correctly updated.
  ///   </para>
  ///   <para>
  ///     For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
  ///     list [A1, B1, C1] is returned, i.e., A1 was changed, so then A1 must be
  ///     evaluated, followed by B1, followed by C1.
  ///   </para>
  /// </returns>
  private IList<string> SetCellContents(string name, double number)
  {
    string normalizedName = ValidateNameAndNormalize(name);

    _cells[normalizedName] = new Cell(number);

    _dependencyGraph.ReplaceDependees(normalizedName, new HashSet<string>());

    IEnumerable<string> toRecalc = GetCellsToRecalculate(normalizedName);
    return new List<string>(toRecalc);
  }

  /// <summary>
  ///   The contents of the named cell becomes the given text.
  /// </summary>
  ///
  /// <exception cref="InvalidNameException">
  ///   If the name is invalid, throw an InvalidNameException.
  /// </exception>
  /// <param name="name"> The name of the cell. </param>
  /// <param name="text"> The new contents of the cell. </param>
  /// <returns>
  ///   The same list as defined in <see cref="SetCellContents(string, double)"/>.
  /// </returns>
  private IList<string> SetCellContents(string name, string text)
  {
    string normalizedName = ValidateNameAndNormalize(name);

    if (string.IsNullOrEmpty(text))
    {
      // If empty, remove from dictionary (it's no longer "non-empty")
      _cells.Remove(normalizedName);
      // Clear dependencies
      _dependencyGraph.ReplaceDependees(normalizedName, new HashSet<string>());
    }
    else
    {
      _cells[normalizedName] = new Cell(text);

      // A plain string has no dependencies
      _dependencyGraph.ReplaceDependees(normalizedName, new HashSet<string>());
    }

    IEnumerable<string> toRecalc = GetCellsToRecalculate(normalizedName);
    return new List<string>(toRecalc);
  }

  /// <summary>
  ///   Set the contents of the named cell to the given formula.
  /// </summary>
  /// <exception cref="InvalidNameException">
  ///   If the name is invalid, throw an InvalidNameException.
  /// </exception>
  /// <exception cref="CircularException">
  ///   <para>
  ///     If changing the contents of the named cell to be the formula would
  ///     cause a circular dependency, throw a CircularException, and no
  ///     change is made to the spreadsheet.
  ///   </para>
  /// </exception>
  /// <param name="name"> The name of the cell. </param>
  /// <param name="formula"> The new contents of the cell. </param>
  /// <returns>
  ///   The same list as defined in <see cref="SetCellContents(string, double)"/>.
  /// </returns>
  private IList<string> SetCellContents(string name, Formula formula)
  {
    string normalizedName = ValidateNameAndNormalize(name);

    IEnumerable<string> oldDependees = _dependencyGraph.GetDependees(normalizedName).ToList();

    // Save the old cell contents, if any.
    bool hadOldCell = _cells.TryGetValue(normalizedName, out Cell? oldCell);

    _cells[normalizedName] = new Cell(formula);

    ISet<string> vars = formula.GetVariables();
    _dependencyGraph.ReplaceDependees(normalizedName, vars);

    try
    {
      // Attempt to get all cells that need to be recalculated.
      IEnumerable<string> toRecalc = GetCellsToRecalculate(normalizedName);
      return new List<string>(toRecalc);
    }
    catch (CircularException)
    {
      // Revert dependency graph changes.
      _dependencyGraph.ReplaceDependees(normalizedName, oldDependees);

      // Revert cell contents: restore the old cell, or remove it if there was none.
      if (hadOldCell)
      {
        _cells[normalizedName] = oldCell!;
      }
      else
      {
        _cells.Remove(normalizedName);
      }

      // Rethrow the exception to signal the circular dependency.
      throw;
    }
  }


  /// <summary>
  ///   Returns an enumeration, without duplicates, of the names of all cells whose
  ///   values depend directly on the value of the named cell.
  /// </summary>
  /// <param name="name"> This <b>MUST</b> be a valid name.  </param>
  /// <returns>
  ///   <para>
  ///     Returns an enumeration, without duplicates, of the names of all cells
  ///     that contain formulas containing name.
  ///   </para>
  ///   <para>For example, suppose that: </para>
  ///   <list type="bullet">
  ///      <item>A1 contains 3</item>
  ///      <item>B1 contains the formula A1 * A1</item>
  ///      <item>C1 contains the formula B1 + A1</item>
  ///      <item>D1 contains the formula B1 - C1</item>
  ///   </list>
  ///   <para> The direct dependents of A1 are B1 and C1. </para>
  /// </returns>
  private IEnumerable<string> GetDirectDependents(string name)
  {
    return _dependencyGraph.GetDependents(name);
  }

  /// <summary>
  ///   <para>
  ///     This method is implemented for you, but makes use of your GetDirectDependents.
  ///   </para>
  ///   <para>
  ///     Returns an enumeration of the names of all cells whose values must
  ///     be recalculated, assuming that the contents of the cell referred
  ///     to by name has changed.  The cell names are enumerated in an order
  ///     in which the calculations should be done.
  ///   </para>
  ///   <exception cref="CircularException">
  ///     If the cell referred to by name is involved in a circular dependency,
  ///     throws a CircularException.
  ///   </exception>
  ///   <para>
  ///     For example, suppose that:
  ///   </para>
  ///   <list type="number">
  ///     <item>
  ///       A1 contains 5
  ///     </item>
  ///     <item>
  ///       B1 contains the formula A1 + 2.
  ///     </item>
  ///     <item>
  ///       C1 contains the formula A1 + B1.
  ///     </item>
  ///     <item>
  ///       D1 contains the formula A1 * 7.
  ///     </item>
  ///     <item>
  ///       E1 contains 15
  ///     </item>
  ///   </list>
  ///   <para>
  ///     If A1 has changed, then A1, B1, C1, and D1 must be recalculated,
  ///     and they must be recalculated in an order which has A1 first, and B1 before C1
  ///     (there are multiple such valid orders).
  ///     The method will produce one of those enumerations.
  ///   </para>
  ///   <para>
  ///      PLEASE NOTE THAT THIS METHOD DEPENDS ON THE METHOD GetDirectDependents.
  ///      IT WON'T WORK UNTIL GetDirectDependents IS IMPLEMENTED CORRECTLY.
  ///   </para>
  /// </summary>
  /// <param name="name"> The name of the cell.  Requires that name be a valid cell name.</param>
  /// <returns>
  ///    Returns an enumeration of the names of all cells whose values must
  ///    be recalculated.
  /// </returns>
  private IEnumerable<string> GetCellsToRecalculate(string name)
  {
    LinkedList<string> changed = new();
    HashSet<string> visited = [];
    Visit(name, name, visited, changed);
    return changed;
  }

  /// <summary>
  ///   A helper for the GetCellsToRecalculate method.
  ///   performs a depth-first search starting from <paramref name="name"/>.
  ///   It visits all direct and indirect dependents, 
  ///   adding them to <paramref name="changed"/> in an order that respects dependencies.
  ///   as well as inline comments in the code.
  /// </summary>
  /// <param name="start">
  /// The cell name from which the original DFS call began. Used to detect cycles 
  /// (if we ever visit it again).
  /// </param>
  /// <param name="name">
  /// The cell name currently being visited.
  /// </param>
  /// <param name="visited">
  /// A set of cells that have already been visited in this DFS, to prevent 
  /// repeated visits and detect cycles.
  /// </param>
  /// <param name="changed">
  /// A linked list to which we will add cells in the order they should be 
  /// recalculated (reverse topological order). Each time we finish visiting
  /// a node's dependents, we add that node to the front of the list.
  /// </param>
  /// <exception cref="CircularException">
  /// Thrown if we detect a cycle (i.e., revisit the <paramref name="start"/> cell).
  /// </exception>
  /// 
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
  /// Validates that <paramref name="name"/> matches the pattern [A-Za-z]+[0-9]+ and is not null/whitespace.
  /// Returns the uppercase version if valid; otherwise, throws <see cref="InvalidNameException"/>.
  /// </summary>
  /// <param name="name">The cell name to validate and normalize.</param>
  /// <returns>The uppercase version of <paramref name="name"/> if valid.</returns>
  /// <exception cref="InvalidNameException">Thrown if <paramref name="name"/> is invalid.</exception>
  private static string ValidateNameAndNormalize(string name)
  {
    // pattern for valid Cell name 
    string VariableRegExPattern = @"^[a-zA-Z]+\d+";

    if (string.IsNullOrWhiteSpace(name))
    {
      throw new InvalidNameException();
    }

    string upper = name.ToUpperInvariant();

    // Must match letters+digits
    if (!Regex.IsMatch(upper, VariableRegExPattern))
    {
      throw new InvalidNameException();
    }

    return upper;
  }

  //---- New PS6 Methods ----

  /// <summary>
  /// <para>
  /// Return the value of the named cell, as defined by
  /// <see cref="GetCellValue(string)"/>.
  /// </para>
  /// </summary>
  /// <param name="name"> The cell in question. </param>
  /// <returns>
  /// <see cref="GetCellValue(string)"/>
  /// </returns>
  /// <exception cref="InvalidNameException">
  /// If the provided name is invalid, throws an InvalidNameException.
  /// </exception>
  /// </exception>
  public object this[string name]
  {
    get
    {
      return GetCellValue(name);
    }
  }
  
  /// <summary>
  /// True if this spreadsheet has been changed since it was 
  /// created or saved (whichever happened most recently),
  /// False otherwise.
  /// </summary>
  [JsonIgnore]
  public bool Changed { get; private set; }


  /// <summary>
  /// Constructs a spreadsheet using the saved data in the file refered to by
  /// the given filename. 
  /// <see cref="Save(string)"/>
  /// </summary>
  /// <exception cref="SpreadsheetReadWriteException">
  ///   Thrown if the file can not be loaded into a spreadsheet for any reason
  /// </exception>
  /// <param name="filename">The path to the file containing the spreadsheet to load</param>
  public Spreadsheet(string filename)
  {
    Changed = false;
    _cells = new Dictionary<string, Cell>();
    _dependencyGraph = new DependencyGraph();
    
    // If file doesn't exist
    if (!File.Exists(filename))
    {
      throw new SpreadsheetReadWriteException("File not found: " + filename);
    }

    // Catches all possible errors and throws SpreadSheetReadWriteException()
    try
    {
      string jsonText = File.ReadAllText(filename);
      // Spreadsheet ss = JsonSerializer.Deserialize<Spreadsheet>(jsonText)
      //                  ?? throw new SpreadsheetReadWriteException("Null Spreadsheet object");
      //
      // foreach (var kvp in ss._cells)
      // {
      //   string cellName = kvp.Key;
      //   // Assumes that the saved contents are stored as a string
      //   string savedStringForm = kvp.Value.StringForm ?? "";
      //
      //   // Rebuild the spreadsheet via your standard logic
      //   // This checks for CircularException
      //   SetContentsOfCell(cellName, savedStringForm);
      // }
      SetSpreadsheetJson(jsonText);
    }
    catch (Exception e)
    {
      throw new SpreadsheetReadWriteException($"Failed to load: {e.Message}");
    }
    Changed = false;
  }


  /// <summary>
  /// Saves this spreadsheet to a file
  /// </summary>
  /// <param name="filename"> The name (with path) of the file to save to.</param>
  /// <exception cref="SpreadsheetReadWriteException">
  ///   If there are any problems opening, writing, or closing the file, 
  ///   the method should throw a SpreadsheetReadWriteException with an
  ///   explanatory message.
  /// </exception>
  public void Save(string filename)
  {
    try
    {
      // string jsonSerialize = JsonSerializer.Serialize(this);
      string jsonSerialize = GetSpreadsheetJson();
      File.WriteAllText(filename, jsonSerialize);
    
      Changed = false;
    }
    catch (Exception)
    {
      throw new SpreadsheetReadWriteException("Error writing to:" + filename);
    }
  }

  /// <summary>
  ///   <para>
  ///     Return the value of the named cell.
  ///   </para>
  /// </summary>
  /// <param name="name"> The cell in question. </param>
  /// <returns>
  ///   Returns the value (as opposed to the contents) of the named cell.  The return
  ///   value should be either a string, a double, or a CS3500.Formula.FormulaError.
  /// </returns>
  /// <exception cref="InvalidNameException">
  ///   If the provided name is invalid, throws an InvalidNameException.
  /// </exception>
  public object GetCellValue(string name)
  {
    name = ValidateNameAndNormalize(name);
    if (!_cells.TryGetValue(name, out Cell? cell))
    {
      return "";
    }

    return cell.Value ?? "";
  }

  /// <summary>
  ///   <para>
  ///     Set the contents of the named cell to be the provided string
  ///     which will either represent (1) a string, (2) a number, or 
  ///     (3) a formula (based on the prepended '=' character).
  ///   </para>
  ///   <para>
  ///     Rules of parsing the input string:
  ///   </para>
  ///   <list type="bullet">
  ///     <item>
  ///       <para>
  ///         If 'content' parses as a double, the contents of the named
  ///         cell becomes that double.
  ///       </para>
  ///     </item>
  ///     <item>
  ///         If the string does not begin with an '=', the contents of the 
  ///         named cell becomes 'content'.
  ///     </item>
  ///     <item>
  ///       <para>
  ///         If 'content' begins with the character '=', an attempt is made
  ///         to parse the remainder of content into a Formula f using the Formula
  ///         constructor.  There are then three possibilities:
  ///       </para>
  ///       <list type="number">
  ///         <item>
  ///           If the remainder of content cannot be parsed into a Formula, a 
  ///           CS3500.Formula.FormulaFormatException is thrown.
  ///         </item>
  ///         <item>
  ///           Otherwise, if changing the contents of the named cell to be f
  ///           would cause a circular dependency, a CircularException is thrown,
  ///           and no change is made to the spreadsheet.
  ///         </item>
  ///         <item>
  ///           Otherwise, the contents of the named cell becomes f.
  ///         </item>
  ///       </list>
  ///     </item>
  ///   </list>
  /// </summary>
  /// <returns>
  ///   <para>
  ///     The method returns a list consisting of the name plus the names 
  ///     of all other cells whose value depends, directly or indirectly, 
  ///     on the named cell. The order of the list should be any order 
  ///     such that if cells are re-evaluated in that order, their dependencies 
  ///     are satisfied by the time they are evaluated.
  ///   </para>
  ///   <example>
  ///     For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
  ///     list {A1, B1, C1} is returned.
  ///   </example>
  /// </returns>
  /// <exception cref="InvalidNameException">
  ///     If name is invalid, throws an InvalidNameException.
  /// </exception>
  /// <exception cref="CircularException">
  ///     If a formula would result in a circular dependency, throws CircularException.
  /// </exception>
  public IList<string> SetContentsOfCell(string name, string content)
  {
    name = ValidateNameAndNormalize(name);
    IList<string> recalcList = new List<string>();
    if (Double.TryParse(content, out double result))
    {
      recalcList = SetCellContents(name, result);
    }
    else if (content.StartsWith("="))
    {
      string cleanedString = content.Substring(1);
      Formula formula;
      
      formula = new Formula(cleanedString);
      
      recalcList = SetCellContents(name, formula);
    }
    // If the content is string
    else
    {
      recalcList = SetCellContents(name, content);

    }
    RecalculateCellsInOrder(recalcList);

    Changed = true;
  
    return recalcList;
  }

  /// <summary>
  ///   <para>
  ///     A private method to recalculate affected cells during <see cref="SetContentsOfCell"/>
  ///     Iterates through list and updates the values.
  ///   </para>
  ///   <para>
  ///     The formula constructor utilizes the private <see cref="lookupValue"/> method to get value of cell
  ///   </para>
  /// </summary>
  /// <param name="recalcList">
  /// An ordered collection of cell names that require recalculation. This list must include the name
  /// of the changed cell and any cells that depend on it (directly or indirectly).
  /// </param>
  /// <param name="recalcList"></param>
  private void RecalculateCellsInOrder(IEnumerable<string> recalcList)
  {
    foreach (string cellName in recalcList)
    {
      //if the cell has been removed due to being empty, continue
      if (!_cells.ContainsKey(cellName))
        continue;
      
      Cell c = _cells[cellName];

      // If we ever removed the cell because it’s empty, skip
      // (or check for ContainsKey first)
      if (c.Contents is double d)
      {
        // If the contents is a double, the value is the same double
        c.Value = d;
      }
      else if (c.Contents is string s)
      {
        // If the contents is a string, the value is that string
        c.Value = s;
      }
      else if (c.Contents is Formula f)
      {
        // Evaluate the formula
        object result = f.Evaluate(LookupValue);
        c.Value = result; 
      }
    }
  }

  public string GetSpreadsheetJson()
  {
    //TODO is this it? DO i need any exception checking
    return JsonSerializer.Serialize(this);
  }

  //TODO write javadoc
  public void SetSpreadsheetJson(string jsonString)
  {
    //TODO is it okay to make this unnullable 
    Spreadsheet ss = JsonSerializer.Deserialize<Spreadsheet>(jsonString)
                     ?? throw new SpreadsheetReadWriteException("Null Spreadsheet object");;
    foreach (var kvp in ss._cells)
    {
      string cellName = kvp.Key;
      string savedStringForm = kvp.Value.StringForm;
      SetContentsOfCell(cellName, savedStringForm);
    }
  }
  /// <summary>
  ///   Looks up the value of the cell for formula evaluation
  /// </summary>
  /// <param name="varName"> The cell that needs to be looked up. </param>
  /// <returns> The numeric value stored in the cell. </returns>
  /// <exception cref="ArgumentException">
  ///   Thrown if the specified cell does not exist or is not numeric value.
  ///   The <exception cref="ArgumentException"> is used to throw the <see cref="FormulaError"/> Object</exception>
  /// </exception>
  private double LookupValue(string varName)
  {
    varName = ValidateNameAndNormalize(varName);
    // If that variable isn’t in _cells or if its Value is not a double,
    // throw ArgumentException for the formula evaluation to yield a FormulaError.
    if (!_cells.TryGetValue(varName, out Cell? c))
    {
      throw new ArgumentException($"No such cell {varName}");
    }

    // If that cell’s Value isn’t a double, e.g. it’s a string or a FormulaError,
    // the formula returns a FormulaError. d, 
    // that happens automatically if we throw
    if (c.Value is double d) return d;
    throw new ArgumentException($"Cell {varName} does not have a numeric value");
  }
}
