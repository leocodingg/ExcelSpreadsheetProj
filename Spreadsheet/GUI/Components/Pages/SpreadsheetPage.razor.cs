// <summary>
//   <para>
//     TODO This is the logic of a partial class for the frontend of the GUI to interact with.
//   </para>
//   <para>
//    TODO
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
public partial class SpreadsheetPage
{
    /// <summary>
    /// Based on your computer, you could shrink/grow this value based on performance.
    /// </summary>
    private const int ROWS = 50;

    /// <summary>
    /// Number of columns, which will be labeled A-Z.
    /// </summary>
    private const int COLS = 26;

    /// <summary>
    /// Provides an easy way to convert from an index to a letter (0 -> A)
    /// </summary>
    private char[] Alphabet { get; } = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();


    /// <summary>
    ///   Gets or sets the name of the file to be saved
    /// </summary>
    private string FileSaveName { get; set; } = "Spreadsheet.sprd";


    /// <summary>
    ///   <para> Gets or sets the data for all of the cells in the spreadsheet GUI. </para>
    ///   <remarks>Backing Store for HTML</remarks>
    /// </summary>
    private string[,] CellsBackingStore { get; set; } = new string[ROWS, COLS];
    
    //TODO write javadoc for member variables
    private string currentSelectedCell = "A1";
    private ElementReference textArea;
    
    // ------- Added Spreadsheet Logic -------

    private int currentRow = 0;
    private int currentColumn = 0;
    private string currentContents = "";
    private Stack<Tuple<string, string>> versionTracker = new();
    private Spreadsheet spreadsheet = new Spreadsheet();
    private string currentValue = "";

    // ------- Error handling properties -------
    private bool ShowError { get; set; } = false;
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
    /// Handler for when a cell is clicked
    /// </summary>
    /// <param name="row">The row component of the cell's coordinates</param>
    /// <param name="col">The column component of the cell's coordinates</param>
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
    /// Updates the display of all cells in the spreadsheet
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
    /// Handles changes to the content of the currently selected cell
    /// </summary>
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
        //TODO should this be formula format
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