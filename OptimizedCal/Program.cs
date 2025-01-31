using System;
using System.Linq;
using System.Collections.Generic;

namespace Cin7.Rookies.Calculator.AdvancedCal
{
    #region Interfaces

    public interface IOperation
    {
        char Symbol { get; }
        decimal Execute(decimal Part1, decimal Part2);
    }

    #endregion

    #region Structs

    public struct Operators
    {
        public const char Plus = '+';
        public const char Minus = '-';
        public const char Asterisk = '*';
        public const char ForwardSlash = '/';
        public const char Equal = '=';
        public const char Backspace = '\b';
        public const char Dot = '.';
    }

    public struct DisplayMessages
    {
        public const string WelcomeMessage = "\t\tWelcome to Cin7 Rookie Cal\n\t\t=========================\n";
        public const string ResetMessage = "Please press 'Ctrl + R' to reset or 'Ctrl + Q' to close the calculator";
        public const string DivisionByZeroError = "Cannot perform division by zero.";
        public const string FinalMessage = "\t---------------\n\t=     {Result}\n\t===============";
        public const string ProgressMessage = "\t\t{_Display}\n\t{Operator}     {Temp}";
        public const string ErrorMessage = "An error occurred";
        public const string CalculationResultMessage = "\t---------------\n\t=      {Result}\n\t===============";
        public const string InputDisplayMessage = "\t\t{Part1}\n\t\t     {Temp}";  
        public const string InputDisplayWithStepsMessage = "\t\t{_Display}\n\t{Operator}       {Temp}";
        public const string InputFormatError = "Input format error: ";
        public const string UnexpectedError = "An unexpected error occurred: ";
    }



    #endregion

    #region Classes: Supportive

    public class Addition : IOperation
    {
        public char Symbol => Operators.Plus;
        public decimal Execute(decimal Part1, decimal Part2) => Part1 + Part2;
    }

    public class Subtraction : IOperation
    {
        public char Symbol => Operators.Minus;
        public decimal Execute(decimal Part1, decimal Part2) => Part1 - Part2;
    }

    public class Multiplication : IOperation
    {
        public char Symbol => Operators.Asterisk;
        public decimal Execute(decimal Part1, decimal Part2) => Part1 * Part2;
    }

    public class Division : IOperation
    {
        public char Symbol => Operators.ForwardSlash;

        public decimal Execute(decimal Part1, decimal Part2)
        {
            if (Part2 == 0)
            {
                Console.WriteLine(DisplayMessages.DivisionByZeroError);
            }

            try
            {
                return Part1 / Part2;
            }
            catch (Exception)
            {
                Console.WriteLine(DisplayMessages.UnexpectedError);
                throw;
            }
        }
    }

    #endregion

    #region Classes: Operational

    public sealed class Calculator : IDisposable
    {
        private static readonly Lazy<Calculator> _Instance = new Lazy<Calculator>(() => new Calculator());
        private Addition _Addition;
        private Division _Division;
        private Multiplication _Multiplication;
        private Subtraction _Subtraction;

        // Public static property to access the single instance (singleton)
        public static Calculator Instance => _Instance.Value;

        // Private constructor to prevent external instantiation (singleton)
        private Calculator()
        {
            _Addition = new Addition();
            _Division = new Division();
            _Multiplication = new Multiplication();
            _Subtraction = new Subtraction();
        }

        public decimal Calculate(char OperatorChar, decimal Part1, decimal Part2)
        {
            switch (OperatorChar)
            {
                case Operators.Plus:
                    return _Addition.Execute(Part1, Part2);
                case Operators.Minus:
                    return _Subtraction.Execute(Part1, Part2);
                case Operators.Asterisk:
                    return _Multiplication.Execute(Part1, Part2);
                case Operators.ForwardSlash:
                    return _Division.Execute(Part1, Part2);
                default:
                    throw new InvalidOperationException("Invalid Operation");
            }
        }

        public void Dispose()
        {
            _Addition = null;
            _Division = null;
            _Multiplication = null;
            _Subtraction = null;
        }
    }

    public sealed class Display : IDisposable
    {
        private static readonly Lazy<Display> _Instance = new Lazy<Display>(() => new Display());
        private List<KeyValuePair<string, char>> _Steps;

        public static Display Instance => _Instance.Value;

        private Display()
        {
            _Steps = new List<KeyValuePair<string, char>>();
        }

        public void Add(string Value, char OperatorChar)
        {
            if (!string.IsNullOrWhiteSpace(Value))
            {
                _Steps.Add(new KeyValuePair<string, char>(Value, OperatorChar));
            }
        }

        public bool IsEmpty() => _Steps.Count == 0;

        public void Clear() => _Steps.Clear();

        public override string ToString() => string.Join("\n\t", _Steps.Select(Step => $"{Step.Value}\t{Step.Key}")).Substring(2);

        public void Dispose()
        {
            _Steps.Clear();
            _Steps = null;
        }
    }

    #endregion

    internal class Program
    {
        private static bool _Exit = false;
        private static bool _Calculate = false;
        private static string _Part1 = string.Empty;
        private static string _Temp = string.Empty;
        private static char _Operator = Operators.Plus;
        private static Calculator _Calculator;
        private static Display _Display;

        static Program()
        {
            _Part1 = string.Empty;
            _Temp = string.Empty;
            _Operator = Operators.Plus;
            _Calculator = Calculator.Instance;
            _Display = Display.Instance;
        }

        private static void ResetCalculator()
        {
            _Exit = false;
            _Calculate = false;
            _Part1 = string.Empty;
            _Temp = string.Empty;
            _Operator = Operators.Plus;
            _Display.Clear();
            ClearScreen();
        }

        private static void ClearScreen()
        {
            Console.Clear();
            Console.WriteLine(DisplayMessages.WelcomeMessage);
        }

        static void Process(bool Calculate, char OperatorChar, ref string Part1, ref string Part2)
        {
            try
            {
                _Display.Add(Part2, OperatorChar);

                Part1 = Calculate
                    ? _Calculator.Calculate(OperatorChar, Convert.ToDecimal(Part1), Convert.ToDecimal(Part2)).ToString()
                    : Part1.Length > 0 ? Part1 : Part2;

                Part2 = string.Empty;
            }
            catch (FormatException)
            {
                Console.WriteLine(DisplayMessages.InputFormatError);
                throw;
            }
            catch (DivideByZeroException)
            {
                Console.WriteLine(DisplayMessages.DivisionByZeroError);
                throw;
            }
            catch (Exception)
            {
                Console.WriteLine(DisplayMessages.UnexpectedError);
                throw;  
            }

        }

        static void Main(string[] args)
        {
            string DisplayTemp = null;
            ConsoleKeyInfo KeyInfo;

            ClearScreen();

            try
            {
                do
                {
                    KeyInfo = Console.ReadKey(true);

                    if (KeyInfo.Key == ConsoleKey.R && KeyInfo.Modifiers.Equals(ConsoleModifiers.Control))
                    {
                        ResetCalculator();
                        continue;
                    }

                    _Exit = KeyInfo.Key == ConsoleKey.Q && KeyInfo.Modifiers.Equals(ConsoleModifiers.Control);

                    if (_Exit)
                        break;

                    if (_Operator == Operators.Equal)
                        continue;

                    _Calculate = _Part1.Length > 0 && _Temp.Length > 0;

                    switch (KeyInfo.KeyChar)
                    {
                        case Operators.Plus:
                            Process(_Calculate, _Operator, ref _Part1, ref _Temp);
                            _Operator = Operators.Plus;
                            DisplayTemp = "0";
                            break;

                        case Operators.Minus:
                            Process(_Calculate, _Operator, ref _Part1, ref _Temp);
                            _Operator = Operators.Minus;
                            DisplayTemp = "0";
                            break;

                        case Operators.Asterisk:
                            Process(_Calculate, _Operator, ref _Part1, ref _Temp);
                            _Operator = Operators.Asterisk;
                            DisplayTemp = "1";
                            break;

                        case Operators.ForwardSlash:
                            Process(_Calculate, _Operator, ref _Part1, ref _Temp);
                            _Operator = Operators.ForwardSlash;
                            DisplayTemp = "1";
                            break;

                        case Operators.Equal:
                            Process(_Calculate, _Operator, ref _Part1, ref _Temp);
                            _Operator = Operators.Equal;
                            Console.WriteLine(DisplayMessages.ResetMessage);
                            continue;

                        case Operators.Dot:
                            if (!_Temp.Contains(Operators.Dot))
                            {
                                _Temp += Operators.Dot;
                            }
                            break;

                        case Operators.Backspace:
                            if (_Temp.Length > 0)
                            {
                                _Temp = _Temp.Substring(0, _Temp.Length - 1);
                            }
                            break;

                        default:
                            if (char.IsNumber(KeyInfo.KeyChar))
                            {
                                _Temp += KeyInfo.KeyChar;
                            }
                            break;
                    }

                    ClearScreen();

                    Console.WriteLine(_Display.IsEmpty() ?
                        DisplayMessages.InputDisplayMessage.Replace("{Part1}", _Part1).Replace("{Temp}", _Temp) :
                        DisplayMessages.InputDisplayWithStepsMessage.Replace("{_Display}", _Display.ToString()).Replace("{Operator}", _Operator.ToString()).Replace("{Temp}", _Temp));

                    if (!_Display.IsEmpty())
                    {
                        var Result = _Calculator.Calculate(_Operator, Convert.ToDecimal(_Part1), Convert.ToDecimal(string.IsNullOrEmpty(_Temp) ? DisplayTemp : _Temp));
                        Console.WriteLine(DisplayMessages.CalculationResultMessage.Replace("{Operators.Equal}", Operators.Equal.ToString()).Replace("{Result}", Result.ToString()));
                    }
                } while (!_Exit);
            }
            catch (Exception)
            {
                Console.WriteLine(DisplayMessages.ErrorMessage);
            }
            finally
            {
                _Calculator.Dispose();
                _Display.Dispose();
            }
        }
    }
}
