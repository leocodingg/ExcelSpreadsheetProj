// These tests are for private use only
// Redistributing this file is strictly against SoC policy.


using System.Text.Json;
using CS3500.Formula;
using CS3500.Spreadsheet;
using Newtonsoft.Json.Linq;

namespace SpreadsheetTester
{
    /// <summary>
    ///This is a test class for SpreadsheetTest and is intended
    ///to contain all SpreadsheetTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PS6GradingTests
    {

        /// <summary>
        /// Helper method to verify an arbitrary spreadsheet's values
        /// Cell names and eexpeced values are given in an array in alternating pairs
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="constraints"></param>
        public void VerifyValues( Spreadsheet sheet, params object[] constraints )
        {
            for ( int i = 0; i < constraints.Length; i += 2 )
            {
                if ( constraints[i + 1] is double )
                {
                    Assert.AreEqual( (double) constraints[i + 1], (double) sheet.GetCellValue( (string) constraints[i] ), 1e-9 );
                }
                else
                {
                    Assert.AreEqual( constraints[i + 1], sheet.GetCellValue( (string) constraints[i] ) );
                }
            }
        }


        /// <summary>
        /// Helper method to set the contents of a given cell for a given spreadsheet
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="name"></param>
        /// <param name="contents"></param>
        /// <returns></returns>
        public IEnumerable<string> Set( Spreadsheet sheet, string name, string contents )
        {
            List<string> result = new List<string>(sheet.SetContentsOfCell(name, contents));
            return result;
        }

        // Tests IsValid
        [TestMethod, Timeout( 2000 )]
        [TestCategory( "1" )]
        public void SetContentsOfCell_SetString_IsValid()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell( "A1", "x" );
        }

        [TestMethod, Timeout( 2000 )]
        [TestCategory( "2" )]
        [ExpectedException( typeof( InvalidNameException ) )]
        public void SetContentsOfCell_InvalidName_Throws()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell( "1a", "x" );
        }

        [TestMethod, Timeout( 2000 )]
        [TestCategory( "3" )]
        public void SetContentsOfCell_SetFormula_IsValid()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell( "B1", "= A1 + C1" );
        }

        [TestMethod, Timeout( 2000 )]
        [TestCategory( "4" )]
        [ExpectedException( typeof( FormulaFormatException ) )]
        public void SetContentsOfCell_SetInvalidFormula_Throws() // try construct an invalid formula
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell( "B1", "= A1 + 1C" );
        }

        // Tests Normalize
        [TestMethod, Timeout( 2000 )]
        [TestCategory( "5" )]
        public void GetCellContents_LowerCaseName_IsValid()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell( "B1", "hello" );
            Assert.AreEqual( "hello", s.GetCellContents( "b1" ) );
        }

        /// <summary>
        /// Increase the weight by repeating the previous test
        /// </summary>
        [TestMethod, Timeout( 2000 )]
        [TestCategory( "6" )]
        public void GetCellContents_LowerCaseName_IsValid2()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell( "B1", "hello" );
            Assert.AreEqual( "hello", ss.GetCellContents( "b1" ) );
        }

        [TestMethod, Timeout( 2000 )]
        [TestCategory( "7" )]
        public void GetCellValue_CaseSensitivity_IsValid()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell( "a1", "5" );
            s.SetContentsOfCell( "B1", "= A1" );
            Assert.AreEqual( 5.0, (double) s.GetCellValue( "B1" ), 1e-9 );
        }

        [TestMethod, Timeout( 2000 )]
        [TestCategory( "8" )]
        public void GetCellValue_CaseSensitivity_IsValid2()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell( "A1", "5" );
            ss.SetContentsOfCell( "B1", "= a1" );
            Assert.AreEqual( 5.0, (double) ss.GetCellValue( "B1" ), 1e-9 );
        }

        // Simple tests
        [TestMethod, Timeout( 2000 )]
        [TestCategory( "9" )]
        public void Constructor_Empty_CorrectValue()
        {
            Spreadsheet ss = new Spreadsheet();
            VerifyValues( ss, "A1", "" );
        }


        [TestMethod, Timeout( 2000 )]
        [TestCategory( "10" )]
        public void GetCellValue_GetString_IsValid()
        {
            Spreadsheet ss = new Spreadsheet();
            OneString( ss );
        }
        
        /// <summary>
        /// Helper method that sets one string in one cell and verifies the value
        /// </summary>
        /// <param name="ss"></param>
        public void OneString( Spreadsheet ss )
        {
            Set( ss, "B1", "hello" );
            VerifyValues( ss, "B1", "hello" );
        }


        [TestMethod, Timeout( 2000 )]
        [TestCategory( "11" )]
        public void GetCellValue_GetNumber_IsValid()
        {
            Spreadsheet ss = new Spreadsheet();
            OneNumber( ss );
        }

        /// <summary>
        /// Helper method that sets one number in one cell and verifies the value
        /// </summary>
        /// <param name="ss"></param>
        public void OneNumber( Spreadsheet ss )
        {
            Set( ss, "C1", "17.5" );
            VerifyValues( ss, "C1", 17.5 );
        }


        [TestMethod, Timeout( 2000 )]
        [TestCategory( "12" )]
        public void GetCellValue_GetFormula_IsValid()
        {
            Spreadsheet ss = new Spreadsheet();
            OneFormula( ss );
        }

        /// <summary>
        /// Helper method that sets one formula in one cell and verifies the value
        /// </summary>
        /// <param name="ss"></param>
        public void OneFormula( Spreadsheet ss )
        {
            Set( ss, "A1", "4.1" );
            Set( ss, "B1", "5.2" );
            Set( ss, "C1", "= A1+B1" );
            VerifyValues( ss, "A1", 4.1, "B1", 5.2, "C1", 9.3 );
        }


        [TestMethod, Timeout( 2000 )]
        [TestCategory( "13" )]
        public void Changed_AfterModify_IsTrue()
        {
            Spreadsheet ss = new Spreadsheet();
            Assert.IsFalse( ss.Changed );
            Set( ss, "C1", "17.5" );
            Assert.IsTrue( ss.Changed );
        }

        [TestMethod, Timeout( 2000 )]
        [TestCategory( "13b" )]
        public void Changed_AfterSave_IsFalse()
        {
            Spreadsheet ss = new Spreadsheet();
            Set( ss, "C1", "17.5" );
            ss.Save( "changed.txt" );
            Assert.IsFalse( ss.Changed );
        }


        [TestMethod, Timeout( 2000 )]
        [TestCategory( "14" )]
        public void GetCellValue_DivideByZero_ReturnsError()
        {
            Spreadsheet ss = new Spreadsheet();
            DivisionByZero1( ss );
        }

        /// <summary>
        /// Helper method to test a formula that indirectly divides by zero
        /// </summary>
        /// <param name="ss"></param>
        public void DivisionByZero1( Spreadsheet ss )
        {
            Set( ss, "A1", "4.1" );
            Set( ss, "B1", "0.0" );
            Set( ss, "C1", "= A1 / B1" );
            Assert.IsInstanceOfType( ss.GetCellValue( "C1" ), typeof( FormulaError ) );
        }

        [TestMethod, Timeout( 2000 )]
        [TestCategory( "15" )]
        public void GetCellValue_DivideByZero_ReturnsError2()
        {
            Spreadsheet ss = new Spreadsheet();
            DivisionByZero2( ss );
        }

        /// <summary>
        /// Helper method that directly divides by zero
        /// </summary>
        /// <param name="ss"></param>
        public void DivisionByZero2( Spreadsheet ss )
        {
            Set( ss, "A1", "5.0" );
            Set( ss, "A3", "= A1 / 0.0" );
            Assert.IsInstanceOfType( ss.GetCellValue( "A3" ), typeof( FormulaError ) );
        }



        [TestMethod, Timeout( 2000 )]
        [TestCategory( "16" )]
        public void GetCellValue_FormulaBadVariable_ReturnsError()
        {
            Spreadsheet ss = new Spreadsheet();
            EmptyArgument( ss );
        }

        /// <summary>
        /// Helper method that tests a formula that references an empty cell
        /// </summary>
        /// <param name="ss"></param>
        public void EmptyArgument( Spreadsheet ss )
        {
            Set( ss, "A1", "4.1" );
            Set( ss, "C1", "= A1 + B1" );
            Assert.IsInstanceOfType( ss.GetCellValue( "C1" ), typeof( FormulaError ) );
        }


        [TestMethod, Timeout( 2000 )]
        [TestCategory( "17" )]
        public void GetCellValue_FormulaBadVariable_ReturnsError2()
        {
            Spreadsheet ss = new Spreadsheet();
            StringArgument( ss );
        }

        /// <summary>
        /// Helper method that tests a formula that references a non-empty string cell
        /// </summary>
        /// <param name="ss"></param>
        public void StringArgument( Spreadsheet ss )
        {
            Set( ss, "A1", "4.1" );
            Set( ss, "B1", "hello" );
            Set( ss, "C1", "= A1 + B1" );
            Assert.IsInstanceOfType( ss.GetCellValue( "C1" ), typeof( FormulaError ) );
        }


        [TestMethod, Timeout( 2000 )]
        [TestCategory( "18" )]
        public void GetCellValue_FormulaIndirectBadVariable_ReturnsError()
        {
            Spreadsheet ss = new Spreadsheet();
            ErrorArgument( ss );
        }

        /// <summary>
        /// Helper method that creates a formula that indirectly references an empty cell
        /// </summary>
        /// <param name="ss"></param>
        public void ErrorArgument( Spreadsheet ss )
        {
            Set( ss, "A1", "4.1" );
            Set( ss, "B1", "" );
            Set( ss, "C1", "= A1 + B1" );
            Set( ss, "D1", "= C1" );
            Assert.IsInstanceOfType( ss.GetCellValue( "D1" ), typeof( FormulaError ) );
        }


        [TestMethod, Timeout( 2000 )]
        [TestCategory( "19" )]
        public void GetCellValue_FormulaWithVariable_IsValid()
        {
            Spreadsheet ss = new Spreadsheet();
            NumberFormula1( ss );
        }

        /// <summary>
        /// Helper method that creates a simple formula with a variable reference
        /// </summary>
        /// <param name="ss"></param>
        public void NumberFormula1( Spreadsheet ss )
        {
            Set( ss, "A1", "4.1" );
            Set( ss, "C1", "= A1 + 4.2" );
            VerifyValues( ss, "C1", 8.3 );
        }


        [TestMethod, Timeout( 2000 )]
        [TestCategory( "20" )]
        public void GetCellValue_FormulaWithNumber_IsValid()
        {
            Spreadsheet ss = new Spreadsheet();
            NumberFormula2( ss );
        }

        /// <summary>
        /// Helper method that creates a simple formula that's just a number
        /// </summary>
        /// <param name="ss"></param>
        public void NumberFormula2( Spreadsheet ss )
        {
            Set( ss, "A1", "= 4.6" );
            VerifyValues( ss, "A1", 4.6 );
        }


        // Repeats the simple tests all together
        [TestMethod, Timeout( 2000 )]
        [TestCategory( "21" )]
        public void StressTestVariety()
        {
            Spreadsheet ss = new Spreadsheet();
            Set( ss, "A1", "17.32" );
            Set( ss, "B1", "This is a test" );
            Set( ss, "C1", "= A1+B1" );
            OneString( ss );
            OneNumber( ss );
            OneFormula( ss );
            DivisionByZero1( ss );
            DivisionByZero2( ss );
            StringArgument( ss );
            ErrorArgument( ss );
            NumberFormula1( ss );
            NumberFormula2( ss );
        }

        // Four kinds of formulas
        [TestMethod, Timeout( 2000 )]
        [TestCategory( "22" )]
        public void StressTestFormulas()
        {
            Spreadsheet ss = new Spreadsheet();
            Formulas( ss );
        }

        public void Formulas( Spreadsheet ss )
        {
            Set( ss, "A1", "4.4" );
            Set( ss, "B1", "2.2" );
            Set( ss, "C1", "= A1 + B1" );
            Set( ss, "D1", "= A1 - B1" );
            Set( ss, "E1", "= A1 * B1" );
            Set( ss, "F1", "= A1 / B1" );
            VerifyValues( ss, "C1", 6.6, "D1", 2.2, "E1", 4.4 * 2.2, "F1", 2.0 );
        }

        /// <summary>
        /// Repeated for increased weight
        /// </summary>
        [TestMethod, Timeout( 2000 )]
        [TestCategory( "23" )]
        public void StressTestFormulas2()
        {
            StressTestFormulas();
        }

        /// <summary>
        /// Repeated for increased weight
        /// </summary>
        [TestMethod, Timeout( 2000 )]
        [TestCategory( "24" )]
        public void StressTestFormulas3()
        {
            StressTestFormulas();
        }


        [TestMethod, Timeout( 2000 )]
        [TestCategory( "25" )]
        public void Constructor_MultipleSpreadsheets_DontIntefere()
        {
            Spreadsheet s1 = new Spreadsheet();
            Spreadsheet s2 = new Spreadsheet();
            Set( s1, "X1", "hello" );
            Set( s2, "X1", "goodbye" );
            VerifyValues( s1, "X1", "hello" );
            VerifyValues( s2, "X1", "goodbye" );
        }

        /// <summary>
        /// Repeated for increased weight
        /// </summary>
        [TestMethod, Timeout( 2000 )]
        [TestCategory( "26" )]
        public void Constructor_MultipleSpreadsheets_DontIntefere2()
        {
            Constructor_MultipleSpreadsheets_DontIntefere();
        }

        /// <summary>
        /// Repeated for increased weight
        /// </summary>
        [TestMethod, Timeout( 2000 )]
        [TestCategory( "27" )]
        public void Constructor_MultipleSpreadsheets_DontIntefere3()
        {
            Constructor_MultipleSpreadsheets_DontIntefere();
        }

        /// <summary>
        /// Repeated for increased weight
        /// </summary>
        [TestMethod, Timeout( 2000 )]
        [TestCategory( "28" )]
        public void Constructor_MultipleSpreadsheets_DontIntefere4()
        {
            Constructor_MultipleSpreadsheets_DontIntefere();
        }

        // Reading/writing spreadsheets
        [TestMethod, Timeout( 2000 )]
        [TestCategory( "29" )]
        [ExpectedException( typeof( SpreadsheetReadWriteException ) )]
        public void Save_InvalidPath_Throws()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.Save( Path.GetFullPath( "/missing/save.txt" ) );
        }

        [TestMethod, Timeout( 2000 )]
        [TestCategory( "30" )]
        [ExpectedException( typeof( SpreadsheetReadWriteException ) )]
        public void Load_InvalidPath_Throws()
        {
            Spreadsheet ss = new Spreadsheet(Path.GetFullPath("/missing/save.txt"));
        }

        [TestMethod, Timeout( 2000 )]
        [TestCategory( "31" )]
        public void SaveLoad_SimpleSheet_IsValid()
        {
            Spreadsheet s1 = new Spreadsheet();
            Set( s1, "A1", "hello" );
            s1.Save( "save1.txt" );
            s1 = new Spreadsheet( "save1.txt" );
            Assert.AreEqual( "hello", s1.GetCellContents( "A1" ) );
        }

        [TestMethod, Timeout( 2000 )]
        [TestCategory( "32" )]
        [ExpectedException( typeof( SpreadsheetReadWriteException ) )]
        public void Load_InvalidJson_Throws()
        {
            using ( StreamWriter writer = new StreamWriter( "save2.txt" ) )
            {
                writer.WriteLine( "This" );
                writer.WriteLine( "is" );
                writer.WriteLine( "a" );
                writer.WriteLine( "test!" );
            }
            Spreadsheet ss = new Spreadsheet("save2.txt");
        }



        [TestMethod, Timeout( 2000 )]
        [TestCategory( "35" )]
        public void Load_FromManualJson_IsValid()
        {
            var sheet = new
            {
                Cells = new
                {
                    A1 = new {StringForm = "hello"},
                    A2 = new {StringForm = "5.0"},
                    A3 = new {StringForm = "4.0"},
                    A4 = new {StringForm = "= A2 + A3"}
                },
            };

            File.WriteAllText( "save5.txt", JsonSerializer.Serialize( sheet ) );


            Spreadsheet ss = new Spreadsheet("save5.txt");
            VerifyValues( ss, "A1", "hello", "A2", 5.0, "A3", 4.0, "A4", 9.0 );
        }

        /// <summary>
        /// This test saves your spreadsheet and then loads it into 
        /// a general (dynamic) object, not using your spreadsheet's load constructor
        /// </summary>
        [TestMethod, Timeout( 2000 )]
        [TestCategory( "36" )]
        public void Save_ToGeneralObject_IsValid()
        {
            Spreadsheet ss = new Spreadsheet();
            Set( ss, "A1", "hello" );
            Set( ss, "A2", "5.0" );
            Set( ss, "A3", "4.0" );
            Set( ss, "A4", "= A2 + A3" );
            ss.Save( "save6.txt" );

            string fileContents = File.ReadAllText("save6.txt");

            dynamic? o = JObject.Parse(fileContents);

            Assert.IsNotNull( o );

            Assert.AreEqual( "hello", o?.Cells.A1.StringForm.ToString() );
            Assert.AreEqual( 5.0, double.Parse( o?.Cells.A2.StringForm.ToString() ), 1e-9 );
            Assert.AreEqual( 4.0, double.Parse( o?.Cells.A3.StringForm.ToString() ), 1e-9 );
            Assert.AreEqual( "=A2+A3", o?.Cells.A4.StringForm.ToString().Replace( " ", "" ) );
        }


        // Fun with formulas
        [TestMethod, Timeout( 2000 )]
        [TestCategory( "37" )]
        public void FormulaStress1()
        {
            Formula1( new Spreadsheet() );
        }

        /// <summary>
        /// Helper method for formula stress tests
        /// </summary>
        /// <param name="ss"></param>
        public void Formula1( Spreadsheet ss )
        {
            Set( ss, "a1", "= a2 + a3" );
            Set( ss, "a2", "= b1 + b2" );
            Assert.IsInstanceOfType( ss.GetCellValue( "a1" ), typeof( FormulaError ) );
            Assert.IsInstanceOfType( ss.GetCellValue( "a2" ), typeof( FormulaError ) );
            Set( ss, "a3", "5.0" );
            Set( ss, "b1", "2.0" );
            Set( ss, "b2", "3.0" );
            VerifyValues( ss, "a1", 10.0, "a2", 5.0 );
            Set( ss, "b2", "4.0" );
            VerifyValues( ss, "a1", 11.0, "a2", 6.0 );
        }

        [TestMethod, Timeout( 2000 )]
        [TestCategory( "38" )]
        public void FormulaStress2()
        {
            Formula2( new Spreadsheet() );
        }

        /// <summary>
        /// Helper method for formula stress tests
        /// </summary>
        /// <param name="ss"></param>
        public void Formula2( Spreadsheet ss )
        {
            Set( ss, "a1", "= a2 + a3" );
            Set( ss, "a2", "= a3" );
            Set( ss, "a3", "6.0" );
            VerifyValues( ss, "a1", 12.0, "a2", 6.0, "a3", 6.0 );
            Set( ss, "a3", "5.0" );
            VerifyValues( ss, "a1", 10.0, "a2", 5.0, "a3", 5.0 );
        }

        [TestMethod, Timeout( 2000 )]
        [TestCategory( "39" )]
        public void FormulaStress3()
        {
            Formula3( new Spreadsheet() );
        }

        /// <summary>
        /// Helper method for formula stress tests
        /// </summary>
        /// <param name="ss"></param>
        public void Formula3( Spreadsheet ss )
        {
            Set( ss, "a1", "= a3 + a5" );
            Set( ss, "a2", "= a5 + a4" );
            Set( ss, "a3", "= a5" );
            Set( ss, "a4", "= a5" );
            Set( ss, "a5", "9.0" );
            VerifyValues( ss, "a1", 18.0 );
            VerifyValues( ss, "a2", 18.0 );
            Set( ss, "a5", "8.0" );
            VerifyValues( ss, "a1", 16.0 );
            VerifyValues( ss, "a2", 16.0 );
        }

        [TestMethod, Timeout( 2000 )]
        [TestCategory( "40" )]
        public void FormulaStress4()
        {
            Spreadsheet ss = new Spreadsheet();
            Formula1( ss );
            Formula2( ss );
            Formula3( ss );
        }

        [TestMethod, Timeout( 2000 )]
        [TestCategory( "41" )]
        public void FormulaStress5()
        {
            FormulaStress4();
        }


        [TestMethod, Timeout( 2000 )]
        [TestCategory( "42" )]
        public void MediumStress()
        {
            Spreadsheet ss = new Spreadsheet();
            MediumSheet( ss );
        }

        /// <summary>
        /// Helper method for formula stress tests
        /// </summary>
        /// <param name="ss"></param>
        public void MediumSheet( Spreadsheet ss )
        {
            Set( ss, "A1", "1.0" );
            Set( ss, "A2", "2.0" );
            Set( ss, "A3", "3.0" );
            Set( ss, "A4", "4.0" );
            Set( ss, "B1", "= A1 + A2" );
            Set( ss, "B2", "= A3 * A4" );
            Set( ss, "C1", "= B1 + B2" );
            VerifyValues( ss, "A1", 1.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 3.0, "B2", 12.0, "C1", 15.0 );
            Set( ss, "A1", "2.0" );
            VerifyValues( ss, "A1", 2.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 4.0, "B2", 12.0, "C1", 16.0 );
            Set( ss, "B1", "= A1 / A2" );
            VerifyValues( ss, "A1", 2.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 1.0, "B2", 12.0, "C1", 13.0 );
        }

        [TestMethod, Timeout( 2000 )]
        [TestCategory( "43" )]
        public void MediumStress2()
        {
            MediumStress();
        }


        [TestMethod, Timeout( 2000 )]
        [TestCategory( "44" )]
        public void MediumStressSave()
        {
            Spreadsheet ss = new Spreadsheet();
            MediumSheet( ss );
            ss.Save( "save7.txt" );
            ss = new Spreadsheet( "save7.txt" );
            VerifyValues( ss, "A1", 2.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 1.0, "B2", 12.0, "C1", 13.0 );
        }

        [TestMethod, Timeout( 2000 )]
        [TestCategory( "45" )]
        public void MediumStressSave2()
        {
            MediumStressSave();
        }


        // A long chained formula. Solutions that re-evaluate 
        // cells on every request, rather than after a cell changes,
        // will timeout on this test.
        // This test is repeated to increase its scoring weight
        [TestMethod, Timeout( 6000 )]
        [TestCategory( "46" )]
        public void StressLongFormulaChain()
        {
            object result = "";
            LongFormulaHelper( out result );
            Assert.AreEqual( "ok", result );
        }

        [TestMethod, Timeout( 6000 )]
        [TestCategory( "47" )]
        public void StressLongFormulaChain2()
        {
            object result = "";
            LongFormulaHelper( out result );
            Assert.AreEqual( "ok", result );
        }

        [TestMethod, Timeout( 6000 )]
        [TestCategory( "48" )]
        public void StressLongFormulaChain3()
        {
            object result = "";
            LongFormulaHelper( out result );
            Assert.AreEqual( "ok", result );
        }

        [TestMethod, Timeout( 6000 )]
        [TestCategory( "49" )]
        public void StressLongFormulaChain4()
        {
            object result = "";
            LongFormulaHelper( out result );
            Assert.AreEqual( "ok", result );
        }

        [TestMethod, Timeout( 6000 )]
        [TestCategory( "50" )]
        public void StressLongFormulaChain5()
        {
            object result = "";
            LongFormulaHelper( out result );
            Assert.AreEqual( "ok", result );
        }

        /// <summary>
        /// Helper method for long formula stress tests
        /// </summary>
        /// <param name="result"></param>
        public void LongFormulaHelper( out object result )
        {
            try
            {
                Spreadsheet s = new Spreadsheet();
                s.SetContentsOfCell( "sum1", "= a1 + a2" );
                int i;
                int depth = 100;
                for ( i = 1; i <= depth * 2; i += 2 )
                {
                    s.SetContentsOfCell( "a" + i, "= a" + ( i + 2 ) + " + a" + ( i + 3 ) );
                    s.SetContentsOfCell( "a" + ( i + 1 ), "= a" + ( i + 2 ) + "+ a" + ( i + 3 ) );
                }
                s.SetContentsOfCell( "a" + i, "1" );
                s.SetContentsOfCell( "a" + ( i + 1 ), "1" );
                Assert.AreEqual( Math.Pow( 2, depth + 1 ), (double) s.GetCellValue( "sum1" ), 1.0 );
                s.SetContentsOfCell( "a" + i, "0" );
                Assert.AreEqual( Math.Pow( 2, depth ), (double) s.GetCellValue( "sum1" ), 1.0 );
                s.SetContentsOfCell( "a" + ( i + 1 ), "0" );
                Assert.AreEqual( 0.0, (double) s.GetCellValue( "sum1" ), 0.1 );
                result = "ok";
            }
            catch ( Exception e )
            {
                result = e;
            }
        }

    }
}
