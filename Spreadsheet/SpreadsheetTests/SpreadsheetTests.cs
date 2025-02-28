// Testing Class for DependencyGraph.cs
// <authors> [Leo Yu] </authors>
// <date> [Feb 21, 2025] </date>

using System.Formats.Asn1;
using CS3500.Formula;

namespace SpreadsheetTests;

using CS3500.Spreadsheet;

/// <summary>
///   This is a test class for Spreadsheet and is intended
///   to contain all Spreadsheet Unit Tests
/// </summary>
[TestClass]
public class SpreadsheetTests
{
    [TestMethod]
    public void GetNamesOfAllNonEmptyCells_GetsAllThreeCellTypes_Valid()
    {
        Spreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetContentsOfCell("A1", "75.0");
        spreadsheet.SetContentsOfCell("A2", "The 1975 is best band");
        spreadsheet.SetContentsOfCell("A3", "=75");

        ISet<string> result = spreadsheet.GetNamesOfAllNonemptyCells();
        var expected = new HashSet<string>
        {
            "A1",
            "A2",
            "A3"
        };
        CollectionAssert.AreEquivalent(expected.ToList(), result.ToList());
    }

    [TestMethod]
    public void GetCellContents_GetsAllThreeCellContentTypes_Valid()
    {
        Spreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetContentsOfCell("A1", "75.0");
        spreadsheet.SetContentsOfCell("A2", "The 1975 is best band");
        spreadsheet.SetContentsOfCell("A3", "=75");
        
        Assert.AreEqual(75.0, spreadsheet.GetCellContents("A1"));
        Assert.AreEqual("The 1975 is best band", spreadsheet.GetCellContents("A2"));
        
        Object result = spreadsheet.GetCellContents("A3");
        Assert.IsInstanceOfType(result, typeof(Formula));        
    }

    [TestMethod]
    public void SetCellContents_SetsAllThreeCellContentTypes_Valid()
    {
        Spreadsheet spreadsheet = new Spreadsheet();
        spreadsheet.SetContentsOfCell("A1", "75.0");
        spreadsheet.SetContentsOfCell("A2", "The 1975 is best band");
        spreadsheet.SetContentsOfCell("A3", "=75");
        
        Assert.AreEqual(75.0, spreadsheet.GetCellContents("A1"));
        Assert.AreEqual("The 1975 is best band", spreadsheet.GetCellContents("A2"));
        
        Object result = spreadsheet.GetCellContents("A3");
        Assert.IsInstanceOfType(result, typeof(Formula));  
        
        spreadsheet.SetContentsOfCell("A1", "19.0");
        spreadsheet.SetContentsOfCell("A2", "Being funny in a foreign language");
        spreadsheet.SetContentsOfCell("A3", "=19");
        
        Assert.AreEqual(19.0, spreadsheet.GetCellContents("A1"));
        Assert.AreEqual("Being funny in a foreign language", spreadsheet.GetCellContents("A2"));
        
        result = spreadsheet.GetCellContents("A3");
        Assert.IsInstanceOfType(result, typeof(Formula));  
    }

    [TestMethod]
    public void GetCellsToRecalculate_ReplacesAContent_Valid()
    {
        Spreadsheet spreadsheet = new Spreadsheet();

        spreadsheet.SetContentsOfCell("A1", "75.0");
        spreadsheet.SetContentsOfCell("A2", "=A1 + 2");
        spreadsheet.SetContentsOfCell("A3", "=A2 + 3");
        
        IList<string> recalc = spreadsheet.SetContentsOfCell("A2", "some text");

        // Because A2 changed, and A3 depends on A2, we expect A2 and A3
        // in the returned list, in a valid topological order (A2 before A3).
        Assert.AreEqual(2, recalc.Count, "Expected exactly two cells to recalc: A2, A3.");
        Assert.AreEqual("A2", recalc[0], "A2 should be first in the order.");
        Assert.AreEqual("A3", recalc[1], "A3 should be after A2.");
    }
    
    [TestMethod]
    public void SetCellContents_EmptyStringRemovesFromNonemptyCells_Valid()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("A1", "Hello");
        Assert.IsTrue(ss.GetNamesOfAllNonemptyCells().Contains("A1"));

        ss.SetContentsOfCell("A1", "");
    
        Assert.IsFalse(ss.GetNamesOfAllNonemptyCells().Contains("A1"));
        Assert.AreEqual("", ss.GetCellContents("A1"));
    }
    
    [TestMethod]
    public void SetCellContents_SimpleFormulaDependsOnAnotherCell_Valid()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("X1", "10.0");

        ss.SetContentsOfCell("A1", "=X1 + 5");

        object contents = ss.GetCellContents("A1");
        Assert.IsInstanceOfType(contents, typeof(Formula));
        Formula f = (Formula)contents;
        Assert.AreEqual("X1+5", f.ToString());  // canonical string form

        Assert.AreEqual(10.0, ss.GetCellContents("X1"));
    }
    
    [TestMethod]
    public void SetCellContents_ChainedDependencyReturnsCorrectOrder_Valid()
    {
        Spreadsheet ss = new Spreadsheet();
        // B1 depends on A1
        ss.SetContentsOfCell("B1", "=A1 + 2");
        // C1 depends on B1
        ss.SetContentsOfCell("C1", "=B1 * 3");

        // Now set A1
        IList<string> order = ss.SetContentsOfCell("A1", "10.0");

        // The list must include A1, B1, C1 in an order that respects dependencies.
        // One valid order: [A1, B1, C1]
        // Another valid topological order could be [A1, C1, B1] if the code visits differently,
        // but typically you see A1->B1->C1.
        // We'll just check that A1 is first, and B1 is somewhere before C1.
        Assert.IsTrue(order.Contains("A1"));
        Assert.IsTrue(order.Contains("B1"));
        Assert.IsTrue(order.Contains("C1"));

        // Basic check of ordering
        int idxA1 = order.IndexOf("A1");
        int idxB1 = order.IndexOf("B1");
        int idxC1 = order.IndexOf("C1");
        Assert.IsTrue(idxA1 < idxB1, "A1 should come before B1");
        Assert.IsTrue(idxB1 < idxC1, "B1 should come before C1");
    }

    [TestMethod]
    [ExpectedException(typeof(CircularException))]
    public void SetCellContents_CircularDependencyThrowsException_Valid()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("A1", "=B1 + 2");
        ss.SetContentsOfCell("B1", "=A1 + 3");
        // The second call should throw CircularException 
    }
    
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void SetCellContents_OnlyLetterCellName_Invalid()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("A", "12.0");
        // Should throw InvalidNameException
    }
    
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void SetCellContents_OnlyNumberCellName_Invalid()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("123", "12.0");
        // Should throw InvalidNameException
    }
    
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void SetCellContents_EmptyCellName_Invalid()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("", "12.0");
        // Should throw InvalidNameException
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void GetCellContents_LetterOnlyCellName_Invalid()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.GetCellContents("xyz");  
        // Should throw InvalidNameException
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void GetCellContents_NumberOnlyCellName_Invalid()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.GetCellContents("123");  
        // Should throw InvalidNameException
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void GetCellContents_EmptyCellName_Invalid()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.GetCellContents("");
        // Should throw InvalidNameException
    }
    
    [TestMethod]
    public void GetNamesOfAllNonemptyCells_AddAndRemoveCells_Valid()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("A1", "1.0");
        ss.SetContentsOfCell("B1", "Hello");
        ss.SetContentsOfCell("C1", "=A1 + B1");  

        var names = ss.GetNamesOfAllNonemptyCells();
        Assert.AreEqual(3, names.Count);
        Assert.IsTrue(names.Contains("A1"));
        Assert.IsTrue(names.Contains("B1"));
        Assert.IsTrue(names.Contains("C1"));

        ss.SetContentsOfCell("B1", "");
        names = ss.GetNamesOfAllNonemptyCells();
        Assert.AreEqual(2, names.Count);
        Assert.IsFalse(names.Contains("B1"));
    }
    
    [TestMethod]
    public void GetCellContents_OverwriteCellContents_Valid()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("X1", "5.0");
        Assert.AreEqual(5.0, ss.GetCellContents("X1"));

        // Overwrite with new string
        ss.SetContentsOfCell("X1", "New text");
        Assert.AreEqual("New text", ss.GetCellContents("X1"));

        // Overwrite again with a formula
        ss.SetContentsOfCell("X1", "=10 + 10");
        object contents = ss.GetCellContents("X1");
        Assert.IsInstanceOfType(contents, typeof(Formula));
        Assert.AreEqual("10+10", contents.ToString());
    }
    
    // ----- New Spreadsheet() Tests -----
    // ----- New Constructor ------
    
    [TestMethod]
    public void Spreadsheet_DefaultConstructorChangedIsFals_Valid()
    {
        Spreadsheet ss = new Spreadsheet();
        Assert.IsFalse(ss.Changed);
    }

    [TestMethod]
    public void Spreadsheet_LoadConstructorChangedIsFalse_Valid()
    {
        // create a JSON file with a single cell
        string json = "{\"Cells\": { \"A1\": { \"StringForm\": \"5\" } } }";
        File.WriteAllText("save.txt", json);

        Spreadsheet ss = new Spreadsheet("save.txt");
        Assert.IsFalse(ss.Changed);
    }

    [TestMethod]
    public void Spreadsheet_StringConstructorLoadFromFile_Valid()
    {
        string sheet = "{\"Cells\": { \"A1\": { \"StringForm\": \"5\" } } }";

        File.WriteAllText( "save.txt", sheet );

        Spreadsheet ss = new Spreadsheet("save.txt");

        object contents = ss.GetCellContents("A1");
        Assert.IsInstanceOfType(contents, typeof(double));
        Assert.AreEqual(5.0, (double)contents);

        // Optionally, verify that "A1" is among the non-empty cells.
        var nonEmptyCells = ss.GetNamesOfAllNonemptyCells();
        Assert.IsTrue(nonEmptyCells.Contains("A1"));
    }

    [TestMethod] [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void Spreadsheet_StringConstructorLoadFromFileIncorrectCellName_Invalid()
    {
        string sheet = "{\"Cells\": { \"1A1\": { \"StringForm\": \"5\" } } }";

        File.WriteAllText( "save.txt", sheet );
        Spreadsheet ss = new Spreadsheet("save.txt");
    }
    
    [TestMethod] [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void Spreadsheet_StringConstructorLoadFromFileCircularException_Invalid()
    {
        string sheet = "{\"Cells\": { \"A1\": { \"StringForm\": \"=B1+1\" }, \"B1\": { \"StringForm\": \"=A1+1\" } } }";

        
        File.WriteAllText( "save.txt", sheet );
        Spreadsheet ss = new Spreadsheet("save.txt");
    }
    
    [TestMethod] [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void Spreadsheet_StringConstructorLoadFromFilePathDoesntExist_Invalid()
    {
        string sheet = "{\"Cells\": { \"1A1\": { \"StringForm\": \"5\" } } }";

       // File.WriteAllText( "!!??C://\\sakfaoskgw.", sheet );
        Spreadsheet ss = new Spreadsheet("utryt//save.txt");
    }
    
    [TestMethod] [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void Spreadsheet_StringConstructorLoadFromFileWeirdIncorrectStringValue_Invalid()
    {
        string sheet = "{\"Cells\": { \"1A1\": { \"StringForm\": \"!!!!!\" } } }";

        File.WriteAllText( "save.txt", sheet );
        Spreadsheet ss = new Spreadsheet("save.txt");
    }
    
    [TestMethod]
    public void Spreadsheet_LoadEmptyJsonValidOrEmpty_Valid()
    {
        File.WriteAllText("save.txt", "{}");

        Spreadsheet ss = new Spreadsheet("save.txt");
        // Expect no cells
        Assert.AreEqual(0, ss.GetNamesOfAllNonemptyCells().Count);
    }

    
    // ------ GetCellValue() ------
    
    [TestMethod]
    public void Spreadsheet_GetCellValueDouble_Valid()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("A1", "1.0");

        object cellValue = ss.GetCellValue("A1");
        Assert.IsInstanceOfType(cellValue, typeof(double));
        double result = (double)cellValue;
        Assert.AreEqual(1.0, result, 1e-9);
    }
    
    [TestMethod]
    public void Spreadsheet_GetCellValueString_Valid()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("A1", "hello");

        object cellValue = ss.GetCellValue("A1");
        Assert.IsInstanceOfType(cellValue, typeof(string));
        string result = (string)cellValue;
        Assert.AreEqual("hello", result);
    }
    
    [TestMethod]
    public void Spreadsheet_GetCellValueFormula_Valid()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("A1", "=5+1");

        object cellValue = ss.GetCellValue("A1");
        Assert.IsInstanceOfType(cellValue, typeof(double));
        double result = (double)cellValue;
        Assert.AreEqual(6.0, result, 1e-9);
    }
    
    [TestMethod]
    public void Spreadsheet_GetCellValueAfterChange_Valid()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("A1", "=5+1");

        object cellValue = ss.GetCellValue("A1");
        Assert.IsInstanceOfType(cellValue, typeof(double));
        double result = (double)cellValue;
        Assert.AreEqual(6.0, result, 1e-9);
        
        ss.SetContentsOfCell("A1", "string");
        object cellValue2 = ss.GetCellValue("A1");
        Assert.IsInstanceOfType(cellValue2, typeof(string));
        string result2 = (string)cellValue2;
        Assert.AreEqual(result2, "string");
    }
    
    [TestMethod]
    public void GetCellValue_FormulaDependsOnEmptyCellFormulaError_Valid()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("A1", "=B1 + 5");
    
        // A1 depends on B1, which is empty => not a double => formula yields a FormulaError
        object val = ss.GetCellValue("A1");
        Assert.IsInstanceOfType(val, typeof(FormulaError));
    }
    
    [TestMethod]
    public void GetCellValue_FormulaDivisionByZeroFormulaError_Valid()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("A1", "=10/0");

        object val = ss.GetCellValue("A1");
        Assert.IsInstanceOfType(val, typeof(FormulaError));
        FormulaError err = (FormulaError)val;
        Assert.IsTrue(err.Reason.Contains("zero", StringComparison.OrdinalIgnoreCase));
    }
    
    [TestMethod]
    public void GetCellValue_FormulaReferencesCellWithFormulaErrorFormulaError_Valid()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("A1", "=1/0");     // leads to a division-by-zero error
        ss.SetContentsOfCell("B1", "=A1 + 5");  // references A1

        object b1Value = ss.GetCellValue("B1");
        Assert.IsInstanceOfType(b1Value, typeof(FormulaError));
    }
    
    [TestMethod]
    public void Spreadsheet_GetCellValueEmptyCell_Invalid()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("A1", "=5+1");
        
        object cellValue = ss.GetCellValue("A2");
        Assert.IsInstanceOfType(cellValue, typeof(string));
        string result = (string)cellValue;
        Assert.AreEqual("", result);
    }
    
    // ------ Changed() ------
    [TestMethod]
    public void Spreadsheet_ChangedTrueAfterSetCellContents_Valid()
    {
        Spreadsheet ss = new Spreadsheet();
        Assert.AreEqual(false, ss.Changed);
        
        ss.SetContentsOfCell("A1", "1");
        Assert.AreEqual(true, ss.Changed);
    }
    
    [TestMethod]
    public void Spreadsheet_ChangedFalseAfterSave_Valid()
    {
        Spreadsheet ss = new Spreadsheet();
        Assert.AreEqual(false, ss.Changed);
        
        ss.SetContentsOfCell("A1", "1");
        Assert.AreEqual(true, ss.Changed);
        
        ss.Save("save.txt");
        Assert.AreEqual(false, ss.Changed);
    }
    
    // ------ Save() ------
    
    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void Spreadsheet_Save_ReadOnlyFileThrowsException_Valid()
    {
        // 1. Create a file so it exists.
        File.WriteAllText("readOnly.txt", "Initial content.");
    
        // 2. Mark the file as read-only so writing fails.
        FileInfo fi = new FileInfo("readOnly.txt");
        fi.IsReadOnly = true;

        try
        {
            // 3. Create a spreadsheet and attempt to save to the read-only file.
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "Hello");
            ss.Save("readOnly.txt");

            // Expect an exception before reaching this line.
            Assert.Fail("Expected a SpreadsheetReadWriteException to be thrown due to write failure.");
        }
        finally
        {
            // 4. Clean up: remove the read-only attribute so your test environment stays consistent.
            fi.IsReadOnly = false;
        }
    }
    
    [TestMethod]
    public void Spreadsheet_SaveEmpty_Valid()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.Save("saveEmpty.txt");

        // Reload
        Spreadsheet ss2 = new Spreadsheet("saveEmpty.txt");
        Assert.AreEqual(0, ss2.GetNamesOfAllNonemptyCells().Count);
    }

    [TestMethod]
    public void Spreadsheet_Save_Valid()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("A1", "5");
        ss.Save("save.txt");
        
        Spreadsheet ss2 = new Spreadsheet("save.txt");
        double result = (double)ss2.GetCellContents("A1");
        double expected = (double)ss.GetCellContents("A1");
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void Spreadsheet_SaveToPath_Invalid()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("A1", "5");
        ss.Save("C:./a.df,.save.txt");
        
        Spreadsheet ss2 = new Spreadsheet("C:./a.df,.save.txt");
        double result = (double)ss2.GetCellContents("A1");
        double expected = (double)ss.GetCellContents("A1");
        Assert.AreEqual(expected, result);
    }
    
    // ----- this[string name] ------
    [TestMethod]
    public void Spreadsheet_thisIndexer_Valid()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("A1", "5");
        object result = ss["A1"];
        Assert.IsInstanceOfType(result, typeof(double));
        Assert.AreEqual(5.0, (double)result, 1e-9);
    }
    
    // ----- SetContentsOfCell() ------ indirectly tested a lot in other tests ------
    
    [TestMethod]
    public void SetContentsOfCell_VariousContentTypes_Valid()
    {
        Spreadsheet ss = new Spreadsheet();

        // 1) Set A1 to a double
        IList<string> changed = ss.SetContentsOfCell("A1", "12.5");
        Assert.AreEqual(1, changed.Count);
        Assert.AreEqual("A1", changed[0]);
        Assert.AreEqual(12.5, ss.GetCellContents("A1"));

        // 2) Set B1 to a string
        changed = ss.SetContentsOfCell("B1", "hello");
        Assert.AreEqual(1, changed.Count);
        Assert.AreEqual("B1", changed[0]);
        Assert.AreEqual("hello", ss.GetCellContents("B1"));

        // 3) Set C1 to a formula referencing A1
        changed = ss.SetContentsOfCell("C1", "=A1+5");
        // Without any dependent, changed likely returns just [C1].
        // So add a dependent cell D1 that references C1.
        ss.SetContentsOfCell("D1", "=C1+10");

        // Now change C1's content again; this should cause both C1 and D1 to be recalculated.
        changed = ss.SetContentsOfCell("C1", "=A1+5");
    
        // Expect two cells in the recalculation list: C1 (changed) and D1 (dependent)
        Assert.AreEqual(2, changed.Count, "Expected exactly two cells to recalc: C1 and D1.");
        Assert.IsTrue(changed.Contains("C1"));
        Assert.IsTrue(changed.Contains("D1"));
    
        // Confirm C1's contents is indeed a Formula and in canonical form.
        var c1Contents = ss.GetCellContents("C1");
        Assert.IsInstanceOfType(c1Contents, typeof(Formula));
        Assert.AreEqual("A1+5", ((Formula)c1Contents).ToString());
    
        // (Optional) Verify that getCellValue("C1") is 17.5 => 12.5 + 5
        object c1Value = ss.GetCellValue("C1");
        Assert.IsInstanceOfType(c1Value, typeof(double));
        Assert.AreEqual(17.5, (double)c1Value, 1e-9);
    }

    
    [TestMethod]
    public void SetContentsOfCell_OverwriteExistingContents_Valid()
    {
        Spreadsheet ss = new Spreadsheet();
    
        // Initially set to a double
        ss.SetContentsOfCell("A1", "10.0");
        Assert.AreEqual(10.0, ss.GetCellContents("A1"));

        // Overwrite with a string
        IList<string> changed = ss.SetContentsOfCell("A1", "New text");
        // Typically changed might return [A1] 
        Assert.AreEqual(1, changed.Count);
        Assert.AreEqual("A1", changed[0]);
        Assert.AreEqual("New text", ss.GetCellContents("A1"));

        // Overwrite again with a formula
        changed = ss.SetContentsOfCell("A1", "=2+2");
        // Now changed might again be [A1], or [A1 plus dependents]
        Assert.AreEqual(1, changed.Count);
        Assert.AreEqual("A1", changed[0]);

        object currentContents = ss.GetCellContents("A1");
        Assert.IsInstanceOfType(currentContents, typeof(Formula));
        Assert.AreEqual("2+2", ((Formula)currentContents).ToString());
    }
    
    [TestMethod]
    public void SetContentsOfCell_RemoveContentsByEmptyString_Valid()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("B2", "some data");
        Assert.IsTrue(ss.GetNamesOfAllNonemptyCells().Contains("B2"));
        Assert.AreEqual("some data", ss.GetCellContents("B2"));

        // Now remove it
        IList<string> changed = ss.SetContentsOfCell("B2", "");
        // typically [B2] in the returned list
        Assert.AreEqual(1, changed.Count);
        Assert.AreEqual("B2", changed[0]);
    
        // The cell no longer exists in the dictionary => considered empty
        Assert.IsFalse(ss.GetNamesOfAllNonemptyCells().Contains("B2"));
        Assert.AreEqual("", ss.GetCellContents("B2"));
        Assert.AreEqual("", ss.GetCellValue("B2")); // typically empty string for value as well
    }
    
    [TestMethod]
    public void SetContentsOfCell_FormulaWithMultipleReferences_Valid()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("A1", "10");
        ss.SetContentsOfCell("B1", "2");

        IList<string> changed = ss.SetContentsOfCell("C1", "=A1 + B1");
        // The returned list might have 1 or 3 cells, depending on your topological ordering logic.
        Assert.IsTrue(changed.Contains("C1"));

        // Check the contents
        object c1Contents = ss.GetCellContents("C1");
        Assert.IsInstanceOfType(c1Contents, typeof(Formula));
        Assert.AreEqual("A1+B1", c1Contents.ToString());

        // The value of C1 should now be 12.0
        object c1Value = ss.GetCellValue("C1");
        Assert.IsInstanceOfType(c1Value, typeof(double));
        Assert.AreEqual(12.0, (double)c1Value, 1e-9);
    }
    
    [TestMethod]
    public void SetContentsOfCell_ChainDependencyOrder_Valid()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("B1", "=A1 + 1");
        ss.SetContentsOfCell("C1", "=B1 + 2");

        // Now set A1 => expect [A1, B1, C1] or a topological sequence
        IList<string> changed = ss.SetContentsOfCell("A1", "10");
    
        Assert.AreEqual(3, changed.Count);
        Assert.IsTrue(changed.Contains("A1"));
        Assert.IsTrue(changed.Contains("B1"));
        Assert.IsTrue(changed.Contains("C1"));

        int idxA1 = changed.IndexOf("A1");
        int idxB1 = changed.IndexOf("B1");
        int idxC1 = changed.IndexOf("C1");
        // A1 should appear before B1, B1 before C1
        Assert.IsTrue(idxA1 < idxB1, "Expected A1 before B1");
        Assert.IsTrue(idxB1 < idxC1, "Expected B1 before C1");
    }

    [TestMethod] [ExpectedException(typeof(CircularException))]
    public void SetContentsOfCell_FormulaCircularDependencyThrowsCircularException_Invalid()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("A1", "5");
        // Set B1 to a formula that depends on A1.
        ss.SetContentsOfCell("B1", "=A1+1");
    
        // attempt to change A1's contents to a formula that depends on B1.
        // This creates a cycle: A1 -> B1 -> A1, which should trigger a CircularException.
        ss.SetContentsOfCell("A1", "=B1+1");
    }
    
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void SetContentsOfCell_InvalidNameExceptionThrowsCircularException_Invalid()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("1A1", "5");
        // Set B1 to a formula that depends on A1.
        ss.SetContentsOfCell("1B1", "=A1+1");
    
        // attempt to change A1's contents to a formula that depends on B1.
        // This creates a cycle: A1 -> B1 -> A1, which should trigger a CircularException.
        ss.SetContentsOfCell("1A1", "=B1+1");
    }
    
    // ------- Stress Testing ---------
    
    [TestMethod]
    [Timeout(2000)]
    public void Spreadsheet_StressTest_Valid()
    {
        Spreadsheet ss = new Spreadsheet();

        // Fill 1000 cells with numeric values
        for (int i = 1; i <= 1000; i++)
        {
            ss.SetContentsOfCell("A" + i, i.ToString());
        }

        // Now create a chain in column B referencing previous cells
        // B2 = B1 + 1, B3 = B2 + 1, ...
        ss.SetContentsOfCell("B1", "10");
        for (int i = 2; i <= 100; i++)
        {
            ss.SetContentsOfCell("B" + i, "=B" + (i - 1) + " + 1");
        }
        
        object val = ss.GetCellValue("B100");
        Assert.IsInstanceOfType(val, typeof(double));
        Assert.AreEqual(109.0, (double)val, 1e-9);
    }

}