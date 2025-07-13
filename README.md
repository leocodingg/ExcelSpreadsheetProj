# Excel Spreadsheet Web Application

A fully-featured spreadsheet application built with .NET 8.0 and Blazor, providing Excel-like functionality in your web browser.

## 🚀 Live Demo

**Try it now: https://excelspreadsheetproj.onrender.com**

No installation required - just open the link and start using the spreadsheet!

## 📋 Features

- **Cell Management**: Create and edit cells with formulas and values
- **Formula Support**: Full formula evaluation with standard mathematical operations
- **Real-time Updates**: Instant cell recalculation when dependencies change
- **Dependency Tracking**: Automatic tracking of cell dependencies using a directed graph
- **Web-based Interface**: Clean, responsive UI built with Blazor
- **File Operations**: Save and load spreadsheet files
- **Error Handling**: Comprehensive error detection for circular dependencies and invalid formulas

## 📖 How to Use

1. **Visit**: Go to https://excelspreadsheetproj.onrender.com
2. **Enter Data**: Click any cell and type a value or formula
3. **Formulas**: Start with `=` (e.g., `=A1+B2*C3`)
4. **Navigation**: Use arrow keys or mouse to navigate between cells
5. **Save**: Use the save button to download your spreadsheet to your computer
6. **Load**: Upload a previously saved spreadsheet file to continue working

## 🛠️ Technology Stack

- **Backend**: C# / .NET 8.0
- **Frontend**: Blazor WebAssembly
- **Architecture**: MVC(Model,View,Controller) with clear separation of concerns
- **Hosting**: Deployed on Render with Docker

## Further Details

### Project Structure

```
Spreadsheet/
├── GUI/                    # Blazor web application
├── Spreadsheet/           # Core spreadsheet logic
├── Formula/               # Formula parsing and evaluation
├── DependencyGraph/       # Cell dependency management
├── *Tests/                # Unit test projects
└── Dockerfile            # Container configuration
```

### Running Locally

If you want to run or modify the code:

1. Clone the repository:
```bash
git clone https://github.com/leocodingg/ExcelSpreadsheetProj.git
cd ExcelSpreadsheetProj
```

2. Build and run:
```bash
cd GUI
dotnet run
```

3. Open `http://localhost:5000` in your browser

### Running Tests

```bash
dotnet test
```

## 🤝 Contributors

- Leo Yu - CS3500, University of Utah

## 📄 License

This project was created as part of CS3500 coursework at the University of Utah.

## 🙏 Acknowledgments

- University of Utah CS3500 course staff
- Original project specifications by UofU CS department
