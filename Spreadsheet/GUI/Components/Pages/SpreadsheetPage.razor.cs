// <copyright file="SpreadsheetPage.razor.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>

using CS3500.Spreadsheet;

namespace GUI.Client.Pages;

using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Diagnostics;

/// <summary>
/// TODO: Fill in
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
    
    private string currentSelectedCell = "";
    private ElementReference textArea;
    private int currentRow = 0;
    private int currentColumn = 0;
    private Stack<Tuple<string, string>> versionTracker = new();
    private Spreadsheet spreadsheet = new Spreadsheet();
    private string currentValue = "";
    private string currentContents = "";

    /// <summary>
    /// Handler for when a cell is clicked
    /// </summary>
    /// <param name="row">The row component of the cell's coordinates</param>
    /// <param name="col">The column component of the cell's coordinates</param>
    private void CellClicked( int row, int col )
    {
        // Displays the cell in letter followed by number fashion, plus 1 for 0 row offset
        currentSelectedCell = $"{Alphabet[col]}{row+1}";
        currentRow = row;
        currentColumn = col;
        
        // Allows us to get the value of a cell by clicking on the cell
        currentValue = spreadsheet.GetCellValue(currentSelectedCell).ToString()??"";
        
        textArea.FocusAsync();
    }
    
    
    // TODO if you go to a new cell is it
    //supposed to change the input box to what ever is the content or just delete it yourself

    // TODO if clicked on cell it should be contents in cell and text box and value in readonly
    // if not clicked on cell it should be the value of the contents
    private void CellContentChanged(ChangeEventArgs e)
    {
        //TODO should match the contents
        string data = e.Value!.ToString() ?? "";

        CellsBackingStore[currentRow, currentColumn] = data;
        
        spreadsheet.SetContentsOfCell(currentSelectedCell, data);
        currentValue = spreadsheet.GetCellValue(currentSelectedCell).ToString()??"";
        currentContents = spreadsheet.GetCellContents(currentSelectedCell).ToString() ?? "";
        
        //TODO is this implemented correctly? checks if the current state chaged is the same and does push to avoid redundant work.
        // if(!versionTracker.Pop().Equals(Tuple.Create(currentSelectedCell, currentContents)) || versionTracker.Count == 0)
        //     versionTracker.Push(Tuple.Create(currentSelectedCell, currentContents));
        //
        textArea.FocusAsync();
        //TODO you might very unlikely need to call StateHasChanged() for when things don't refresh
    }

    //TODO should there be a change events args?
    // private void UndoOperation()
    // {
    //     Tuple<string, string> lastVersion = versionTracker.Pop();
    //     spreadsheet.SetContentsOfCell(lastVersion.Item1, lastVersion.Item2);
    // }

    /// <summary>
    /// Saves the current spreadsheet, by providing a download of a file
    /// containing the json representation of the spreadsheet.
    /// </summary>
    private async void SaveFile()
    {
        //TODO double check
        string jsonRepresentation = spreadsheet.GetSpreadsheetJson();
        await JSRuntime.InvokeVoidAsync( "downloadFile", FileSaveName, 
            jsonRepresentation );
    }

    /// <summary>
    /// This method will run when the file chooser is used, for loading a file.
    /// Uploads a file containing a json representation of a spreadsheet, and 
    /// replaces the current sheet with the loaded one.
    /// </summary>
    /// <param name="args">The event arguments, which contains the selected file name</param>
    private async void HandleFileChooser( EventArgs args )
    {
        try
        {
            string fileContent = string.Empty;

            InputFileChangeEventArgs eventArgs = args as InputFileChangeEventArgs ?? throw new Exception("unable to get file name");
            if ( eventArgs.FileCount == 1 )
            {
                var file = eventArgs.File;
                if ( file is null )
                {
                    return;
                }

                using var stream = file.OpenReadStream();
                using var reader = new StreamReader(stream);

                // fileContent will contain the contents of the loaded file
                fileContent = await reader.ReadToEndAsync();

                // TODO: Use the loaded fileContent to replace the current spreadsheet
                spreadsheet.SetSpreadsheetJson(fileContent);

                StateHasChanged();
            }
        }
        catch ( Exception e )
        {
            Debug.WriteLine( "an error occurred while loading the file..." + e );
        }
    }

}
