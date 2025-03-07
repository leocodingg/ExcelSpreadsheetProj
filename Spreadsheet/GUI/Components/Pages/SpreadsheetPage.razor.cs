// <summary>
//   <para>
//     Page component for the spreadsheet application that handles user interactions and spreadsheet operations
//   </para>
//   <para>
//     It provides an interface for selecting cells, editing their contents, and displaying
//     evaluated values (or errors, if formulas are invalid or circular). Additionally, the class
//     handles saving/loading spreadsheet data to/from JSON, undo operations, and shows a modal
//     for error messages (e.g., circular references or syntax issues).
//   </para>
// </summary>
// <authors> [Leo Yu], [Ethan Edwards] </authors>
// <date> [March 6, 2025] </date>

using CS3500.Spreadsheet;
using CS3500.Formula;


namespace GUI.Client.Pages;

using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Diagnostics;

/// <summary>
/// Page component for the spreadsheet application that handles user interactions and spreadsheet operations
/// </summary>
/// <summary>
/// Represents the Blazor page component for the spreadsheet application,
/// responsible for handling all UI interactions (clicks, typing, file IO, etc.)
/// and coordinating with the underlying <see cref="Spreadsheet"/> logic.
/// </summary>
public partial class SpreadsheetPage
{
    /// <summary>
    /// Number of rows to display in the spreadsheet UI.
    /// Adjust this for performance or usability if desired.
    /// </summary>
    private const int ROWS = 50;

    /// <summary>
    /// Number of columns to display in the spreadsheet UI.
    /// They are labeled from 'A' to 'Z'.
    /// </summary>
    private const int COLS = 26;

    /// <summary>
    /// Provides a way to map column indices (0,1,2,...) to letters (A,B,C,...).
    /// </summary>
    private char[] Alphabet { get; } = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

    /// <summary>
    /// The filename to use when saving the spreadsheet to disk.
    /// This can be changed by the user or left as a default.
    /// </summary>
    private string FileSaveName { get; set; } = "Spreadsheet.sprd";

    /// <summary>
    /// A 2D array of strings that backs the HTML display for the grid cells.
    /// This is updated whenever a cell changes, to show the cell's evaluated value or an error message.
    /// </summary>
    private string[,] CellsBackingStore { get; set; } = new string[ROWS, COLS];
    
    /// <summary>
    /// The normalized name of the currently selected cell in the spreadsheet.
    /// By default, this is "A1".
    /// </summary>
    private string currentSelectedCell = "A1";

    /// <summary>
    /// A reference to the HTML input area (the text box) for the cell's formula/content,
    /// allowing us to programmatically focus it after updates.
    /// </summary>
    private ElementReference textArea;

    /// <summary>
    /// Tracks the current row of the selected cell for convenience, so we don't have to parse from the cell name.
    /// </summary>
    private int currentRow = 0;

    /// <summary>
    /// Tracks the current column of the selected cell for convenience, so we don't have to parse from the cell name.
    /// </summary>
    private int currentColumn = 0;

    /// <summary>
    /// Stores the user-editable text of the currently selected cell (including '=' if it's a formula).
    /// This is bound to the UI text box.
    /// </summary>
    private string currentContents = "";

    /// <summary>
    /// A stack used to implement "undo" functionality. Each entry represents
    /// a (cellName, oldContents) pair that allows reverting the last edit.
    /// </summary>
    private Stack<Tuple<string, string>> versionTracker = new();

    /// <summary>
    /// The underlying spreadsheet model from PS6/PS7 code. We delegate all
    /// cell content/values to this Spreadsheet object.
    /// </summary>
    private Spreadsheet spreadsheet = new Spreadsheet();

    /// <summary>
    /// Displays the evaluated value of the currently selected cell.
    /// This is read-only in the UI, showing the result of the cell's formula or direct content.
    /// </summary>
    private string currentValue = "";

    /// <summary>
    /// Indicates whether an error modal dialog should be displayed in the UI.
    /// </summary>
    private bool ShowError { get; set; } = false;

    /// <summary>
    /// Stores the error message text displayed in the error modal.
    /// </summary>
    private string ErrorMessage { get; set; } = "";

    /// <summary>
    /// Initializes the spreadsheet when the component is first loaded
    /// </summary>
    protected override async void OnInitialized()
    {
        await base.OnInitializedAsync();
        
        // Initialize with A1 selected as default
        currentSelectedCell = "A1";
        currentRow = 0;
        currentColumn = 0;
        currentContents = "";
        currentValue = "";
    }

    /// <summary>
    /// Invoked when a user clicks on a cell in the spreadsheet UI.
    /// Updates <see cref="currentSelectedCell"/>, <see cref="currentRow"/>, <see cref="currentColumn"/>,
    /// and displays the selected cell's contents and value in the respective UI elements.
    /// </summary>
    /// <param name="row">The row index (0-based) of the clicked cell.</param>
    /// <param name="col">The column index (0-based) of the clicked cell.</param>
    private void CellClicked(int row, int col)
    {
        // Displays the cell in letter followed by number fashion, plus 1 for 0 row offset
        currentSelectedCell = $"{Alphabet[col]}{row+1}";
        currentRow = row;
        currentColumn = col;
        
        // When clicking on a cell, we want to show its contents in the editable text area
        string cellName = currentSelectedCell;
        object contents = spreadsheet.GetCellContents(cellName);
        
        // Format contents based on type (Formula objects need to be prefixed with =)
        if (contents.ToString()!.StartsWith("="))
        {
            currentContents = "=" + contents.ToString();
        }
        else
        {
            currentContents = contents.ToString() ?? "";
        }
        
        // Set the value display (shown in read-only field)
        currentValue = spreadsheet.GetCellValue(cellName).ToString() ?? "";
        
        // Update the cell display to show values
        UpdateCellsDisplay();
        
        // Focus the text area for editing
        textArea.FocusAsync();
    }
    
    /// <summary>
    /// Iterates over all rows and columns and updates the <see cref="CellsBackingStore"/>
    /// with the evaluated values or error messages from the underlying <see cref="spreadsheet"/>.
    /// This method is typically called after any cell content change.
    /// </summary>
    private void UpdateCellsDisplay()
    {
        // Update all cells in the backing store to show values
        for (int r = 0; r < ROWS; r++)
        {
            for (int c = 0; c < COLS; c++)
            {
                string cellName = $"{Alphabet[c]}{r+1}";
                
                
                object evaluatedValue = spreadsheet.GetCellValue(cellName);

                if (evaluatedValue is FormulaError fe)
                {
                    // If the cell has a FormulaError:
                    //  - We set a placeholder like "ERROR" or just blank
                    CellsBackingStore[r, c] = "Error"; 

                    // If this cell is currently selected, we can show the error modal
                    if (cellName == currentSelectedCell)
                    {
                        ShowError = true;
                        ErrorMessage = $"Cell {cellName} Error: {fe.Reason}";
                        // Also update currentValue to reflect the error visually
                        currentValue = "Error";
                    }
                }
                else
                {
                    // Normal display: just show the string value
                    CellsBackingStore[r, c] = evaluatedValue?.ToString() ?? "";
                }
            }
        }
    }

    
    /// <summary>
    /// Called when the user edits the text box of the currently selected cell.
    /// Tries to update the <see cref="spreadsheet"/> with the new text as cell contents,
    /// handling circular references and invalid formulas gracefully.
    /// </summary>
    /// <param name="e">A <see cref="ChangeEventArgs"/> that contains the changed text.</param>
    private void CellContentChanged(ChangeEventArgs e)
    {
        string data = e.Value?.ToString() ?? "";
        
        try
        {
            // Track the previous state for undo functionality
            versionTracker.Push(Tuple.Create(currentSelectedCell, currentContents));
            
            // Update the spreadsheet with the new cell contents
            spreadsheet.SetContentsOfCell(currentSelectedCell, data);
            
            // Update the current contents and value
            currentContents = data;
            currentValue = spreadsheet.GetCellValue(currentSelectedCell).ToString() ?? "";
            
           
            
            // Update the display of all cells
            UpdateCellsDisplay();
        }
        catch (CircularException)
        {
            // Show error message for circular dependency
            ShowError = true;
            ErrorMessage = "Error: Circular dependency detected.";
            
            // Revert the contents to the last valid state
            if (versionTracker.Count > 0)
            {
                var lastState = versionTracker.Pop();
                currentContents = lastState.Item2;
            }
            
        }
        catch (FormulaFormatException ex)
        {
            // Show error message for invalid formula format
            ShowError = true;
            ErrorMessage = $"Error: Invalid formula. {ex.Message}";
            
            // Revert the contents to the last valid state
            if (versionTracker.Count > 0)
            {
                var lastState = versionTracker.Pop();
                currentContents = lastState.Item2;
            }
        }
        
        // Refocus the text area
        textArea.FocusAsync();
    }

    /// <summary>
    /// Dismisses the error message
    /// </summary>
    private void DismissError()
    {
        ShowError = false;
        ErrorMessage = "";
    }

    /// <summary>
    /// Undoes the last operation
    /// </summary>
    private void UndoOperation()
    {
        if (versionTracker.Count > 0)
        {
            Tuple<string, string> lastVersion = versionTracker.Pop();
            string cellName = lastVersion.Item1;
            string contents = lastVersion.Item2;
            
            try
            {
                // Update the spreadsheet with the previous contents
                spreadsheet.SetContentsOfCell(cellName, contents);
                
                // If the undone operation was on the current cell, update the display
                if (cellName == currentSelectedCell)
                {
                    currentContents = contents;
                    currentValue = spreadsheet.GetCellValue(cellName).ToString() ?? "";
                }
                
                // Update the display of all cells
                UpdateCellsDisplay();
            }
            catch (Exception)
            {
                // If the undo fails, push the version back onto the stack
                versionTracker.Push(lastVersion);
            }
        }
    }

    /// <summary>
    /// Saves the current spreadsheet, by providing a download of a file
    /// containing the json representation of the spreadsheet.
    /// </summary>
    private async void SaveFile()
    {
        string jsonRepresentation = spreadsheet.GetSpreadsheetJson();
        await JSRuntime.InvokeVoidAsync("downloadFile", FileSaveName, 
            jsonRepresentation);
    }

    /// <summary>
    /// This method will run when the file chooser is used, for loading a file.
    /// Uploads a file containing a json representation of a spreadsheet, and 
    /// replaces the current sheet with the loaded one.
    /// </summary>
    /// <param name="args">The event arguments, which contains the selected file name</param>
    private async void HandleFileChooser(InputFileChangeEventArgs args)
    {
        try
        {
            string fileContent = string.Empty;

            if (args.FileCount == 1)
            {
                var file = args.File;
                if (file is null)
                {
                    return;
                }

                using var stream = file.OpenReadStream();
                using var reader = new StreamReader(stream);

                // fileContent will contain the contents of the loaded file
                fileContent = await reader.ReadToEndAsync();

                try
                {
                    // Clear the version tracker when loading a new file
                    versionTracker.Clear();
                    
                    // Load the spreadsheet from the file
                    spreadsheet.SetSpreadsheetJson(fileContent);
                    
                    // Update the display to show the loaded spreadsheet
                    UpdateCellsDisplay();
                    
                    // Reset the current cell information
                    currentSelectedCell = "A1";
                    currentRow = 0;
                    currentColumn = 0;
                    currentContents = spreadsheet.GetCellContents("A1").ToString() ?? "";
                    currentValue = spreadsheet.GetCellValue("A1").ToString() ?? "";
                }
                catch (Exception ex)
                {
                    ShowError = true;
                    ErrorMessage = $"Error loading spreadsheet: {ex.Message}";
                }

                StateHasChanged();
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine("an error occurred while loading the file..." + e);
            ShowError = true;
            ErrorMessage = $"Error loading file: {e.Message}";
        }
    }
}