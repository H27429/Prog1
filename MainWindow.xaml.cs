using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace Calculator
{

    public partial class MainWindow : Window
    {

        const int maxMemoryLabelLength = 6;
        const int defaultFontSize = 48;

        bool operationCheck;
        bool functionCheck;
        bool clearNext;
        bool isResult;
        bool isOldText;
        double memory = 0;
        string previousText;
        enum trigModes
        {
            STANDARD,
            HYPERBOLIC,
            ARC
        }
        trigModes currentTrigMode;
        Dictionary<trigModes, string> trigModeSymbols = new Dictionary<trigModes, string>()
        {
            { trigModes.STANDARD, "STD" },
            { trigModes.ARC, "ARC" },
        };
        Angles.units angleUnit;
        Dictionary<Angles.units, string> angleUnitSymbols = new Dictionary<Angles.units, string>()
            {
                { Angles.units.RADIANS, "RAD" },
                { Angles.units.GRADIANS, "GRAD" }
            };
        static string OVERFLOW = "Overflow";
        static string INVALID_INPUT = "Invalid input";
        static string NOT_A_NUMBER = "NaN";
        string[] errors = { OVERFLOW, INVALID_INPUT, NOT_A_NUMBER };
        operations currentOperation = operations.NULL;
        enum operations
        {
            ADDITION,
            SUBTRACTION,
            DIVISION,
            MULTIPLICATION,
            POWER,
            NULL
        }

        public MainWindow()
        {
            InitializeComponent();
            angle_unit_button.Content = angleUnitSymbols[angleUnit];
            trig_mode_button.Content = trigModeSymbols[currentTrigMode];
        }

        private void showText(string text, bool clear = true)
        {
            try
            {
                if (double.Parse(text) == 0)
                    text = "0";
            }
            catch (Exception)
            {
                showError(INVALID_INPUT);
                return;
            }

            if (text.Length > 30)
                return;
            if (text.Length > 12)
                resultBox.FontSize = 25;
            if (text.Length > 24)
                resultBox.FontSize = 20;

            clearNext = clear;
            resultBox.Text = text;
        }

        private void showError(string text)
        {
            resultBox.Text = text;
            previousText = null;
            operationCheck = false;
            clearNext = true;
            updateEquationBox("");
            currentOperation = operations.NULL;
            resetFontSize();
        }


        private void updateEquationBox(string equation, bool append = false)
        {

            equation = Regex.Replace(equation, @"(\d+)\.\s", "$1 ");

            if (equation.Length > 10)
                equationBox.FontSize = 18;

            if (!append)
                equationBox.Text = equation;
            else
                equationBox.Text += equation;
        }


        private void updateMemoryLabel()
        {
            memoryLabel.Content = memory.ToString();
            if (memoryLabel.Content.ToString().Length > maxMemoryLabelLength)
                memoryLabel.Content = memoryLabel.Content.ToString().Substring(0, 5) + "...";
        }

        private double getNumber()
        {
            double number = double.Parse(resultBox.Text);
            return number;
        }

        private void resetFontSize()
        {
            resultBox.FontSize = defaultFontSize;
        }

        private void calculateResult()
        {
            if (currentOperation == operations.NULL)
                return;

            double a = double.Parse(previousText);
            double b = double.Parse(resultBox.Text);
            double result;

            switch (currentOperation)
            {
                case operations.DIVISION:
                    result = a / b;
                    break;
                case operations.MULTIPLICATION:
                    result = a * b;
                    break;
                case operations.ADDITION:
                    result = a + b;
                    break;
                case operations.SUBTRACTION:
                    result = a - b;
                    break;
                case operations.POWER:
                    result = Math.Pow(a, b);
                    break;
                default:
                    return;
            }

            if (errors.Contains(resultBox.Text))
                return;

            operationCheck = false;
            previousText = null;
            string equation;

            if (!functionCheck)
                equation = equationBox.Text + b.ToString();
            else
            {
                equation = equationBox.Text;
                functionCheck = false;
            }
            updateEquationBox(equation);
            showText(result.ToString());
            currentOperation = operations.NULL;
            isResult = true;
        }


        private void numberClick(object sender, RoutedEventArgs e)
        {
            isResult = false;
            Button button = (Button)sender;

            if (resultBox.Text == "0" || errors.Contains(resultBox.Text))
                resultBox.Clear();

            string text;

            if (clearNext)
            {
                resetFontSize();
                text = button.Content.ToString();
                isOldText = false;
            }
            else
                text = resultBox.Text + button.Content.ToString();

            if (!operationCheck && equationBox.Text != "")
                updateEquationBox("");
            showText(text, false);
        }


        private void angle_unit_button_Click(object sender, RoutedEventArgs e)
        {
            List<Angles.units> units = new List<Angles.units>()
            {
                Angles.units.RADIANS,
                Angles.units.GRADIANS
            };

            Button button = (Button)sender;
            angleUnit = units.ElementAtOrDefault(units.IndexOf(angleUnit) + 1);
            button.Content = angleUnitSymbols[angleUnit];
        }


        private void trig_mode_button_Click(object sender, RoutedEventArgs e)
        {
            List<trigModes> modes = new List<trigModes>()
            {
                trigModes.STANDARD,
                trigModes.ARC,
            };

            Button button = (Button)sender;
            currentTrigMode = modes.ElementAtOrDefault(modes.IndexOf(currentTrigMode) + 1);
            button.Content = trigModeSymbols[currentTrigMode];

            if (currentTrigMode == trigModes.STANDARD)
            {
                sin_button.Content = "sin";
                cos_button.Content = "cos";
                tan_button.Content = "tan";
            }

            if (currentTrigMode == trigModes.ARC)
            {
                sin_button.Content = "asin";
                cos_button.Content = "acos";
                tan_button.Content = "atan";
            }
        }


        private void function(object sender, RoutedEventArgs e)
        {
            if (errors.Contains(resultBox.Text))
                return;

            Button button = (Button)sender;
            string buttonText = button.Content.ToString();
            double number = getNumber();
            string equation = "";
            string result = "";

            switch (buttonText)
            {

                case "!":
                    if (number < 0 || number.ToString().Contains("."))
                    {
                        showError(INVALID_INPUT);
                        return;
                    }

                    if (number > 3248)
                    {
                        showError(OVERFLOW);
                        return;
                    }
                    double res = 1;
                    if (number == 1 || number == 0)
                        result = res.ToString();
                    else
                    {
                        for (int i = 2; i <= number; i++)
                        {
                            res *= i;
                        }
                    }
                    equation = "fact(" + number.ToString() + ")";
                    result = res.ToString();
                    break;

                case "ln":
                    equation = "ln(" + number + ")";
                    result = Math.Log(number).ToString();
                    break;

                case "log":
                    equation = "log(" + number + ")";
                    result = Math.Log10(number).ToString();
                    break;

                case "√":
                    equation = "√(" + number + ")";
                    result = Math.Sqrt(number).ToString();
                    break;

                case "-n":
                    equation = "negate(" + number + ")";
                    result = decimal.Negate((decimal)number).ToString();
                    break;
            }

            if (operationCheck)
            {
                equation = equationBox.Text + equation;
                functionCheck = true;
            }

            updateEquationBox(equation);
            showText(result);
        }


        private void trigFunction(object sender, RoutedEventArgs e)
        {
            if (errors.Contains(resultBox.Text))
                return;

            Button button = (Button)sender;
            string buttonText = button.Content.ToString();
            string equation = "";
            string result = "";
            double number = getNumber();

            switch (currentTrigMode)
            {

                case trigModes.STANDARD:
                    double radianAngle = Angles.Converter.radians(number, angleUnit);
                    switch (buttonText)
                    {
                        case "sin":
                            equation = "sin(" + number.ToString() + ")";
                            result = Math.Sin(radianAngle).ToString();
                            break;

                        case "cos":
                            equation = "cos(" + number.ToString() + ")";
                            result = Math.Cos(radianAngle).ToString();
                            break;

                        case "tan":
                            equation = "tan(" + number.ToString() + ")";
                            result = Math.Tan(radianAngle).ToString();
                            break;
                    }
                    break;

                case trigModes.ARC:
                    switch (buttonText)
                    {
                        case "asin":
                            equation = "asin(" + number + ")";
                            result = Math.Asin(number).ToString();
                            break;

                        case "acos":
                            equation = "acos(" + number + ")";
                            result = Math.Acos(number).ToString();
                            break;

                        case "atan":
                            equation = "atan(" + number + ")";
                            result = Math.Atan(number).ToString();
                            break;
                    }
                    break;
            }


            if (currentTrigMode == trigModes.ARC)
            {
                switch (angleUnit)
                {
                    case Angles.units.GRADIANS:
                        result = Angles.Converter.gradians(double.Parse(result), Angles.units.RADIANS).ToString();
                        break;
                    default:
                        break;
                }
            }

            if (operationCheck)
            {
                equation = equationBox.Text + equation;
                functionCheck = true;
            }

            updateEquationBox(equation);
            showText(result);
        }


        private void doubleOperandFunction(object sender, RoutedEventArgs e)
        {
            if (errors.Contains(resultBox.Text))
                return;

            if (operationCheck && !isOldText)
                calculateResult();

            Button button = (Button)sender;

            operationCheck = true;
            previousText = resultBox.Text;
            string buttonText = button.Content.ToString();
            string equation = previousText + " " + buttonText + " ";
            switch (buttonText)
            {
                case "/":
                    currentOperation = operations.DIVISION;
                    break;
                case "x":
                    currentOperation = operations.MULTIPLICATION;
                    break;
                case "-":
                    currentOperation = operations.SUBTRACTION;
                    break;
                case "+":
                    currentOperation = operations.ADDITION;
                    break;
                case "^":
                    currentOperation = operations.POWER;
                    break;
            }
            updateEquationBox(equation);
            resetFontSize();
            showText(resultBox.Text);
            isOldText = true;
        }

        private void decimal_button_Click(object sender, RoutedEventArgs e)
        {
            if (!resultBox.Text.Contains("."))
            {
                string text = resultBox.Text += ".";
                showText(text, false);
            }
        }

        private void pi_button_Click(object sender, RoutedEventArgs e)
        {
            if (!operationCheck)
                updateEquationBox("");
            showText(Math.PI.ToString());
            isResult = true;
        }

        private void e_button_Click(object sender, RoutedEventArgs e)
        {
            if (!operationCheck)
                updateEquationBox("");
            showText(Math.E.ToString());
            isResult = true;
        }

        private void madd_button_Click(object sender, RoutedEventArgs e)
        {
            if (errors.Contains(resultBox.Text))
                return;
            memory += getNumber();
            updateMemoryLabel();
        }

        private void msub_button_Click(object sender, RoutedEventArgs e)
        {
            if (errors.Contains(resultBox.Text))
                return;
            memory -= getNumber();
            updateMemoryLabel();
        }

        private void clear_button_Click(object sender, RoutedEventArgs e)
        {
            resultBox.Text = "0";
            operationCheck = false;
            previousText = null;
            updateEquationBox("");
            resetFontSize();
        }

        private void clr_entry_button_Click(object sender, RoutedEventArgs e)
        {
            resultBox.Text = "0";
            resetFontSize();
        }

        private void equals_button_Click(object sender, RoutedEventArgs e)
        {
            calculateResult();
        }

        private void copy_button_Click(object sender, RoutedEventArgs e)
        {
            if (errors.Contains(resultBox.Text))
                return;

            Clipboard.SetData(DataFormats.UnicodeText, resultBox.Text);
        }

        private void paste_button_Click(object sender, RoutedEventArgs e)
        {
            object clipboardData = Clipboard.GetData(DataFormats.UnicodeText);
            if (clipboardData != null)
            {
                string data = clipboardData.ToString();
                showText(data.ToString());
            }
            else
                return;
        }

        private void back_button_Click(object sender, RoutedEventArgs e)
        {
            if (isResult)
                return;

            string text;

            if (resultBox.Text.Length == 1)
                text = "0";
            else
                text = resultBox.Text.Substring(0, resultBox.Text.Length - 1);

            showText(text, false);

        }

        private void mr_button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
