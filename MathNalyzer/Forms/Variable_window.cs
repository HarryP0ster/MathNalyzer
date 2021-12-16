using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MathNalyzer
{
    public partial class Variable_window : TableLayoutPanel
    {
        TextBox VarName = new TextBox();
        TextBox VarValue = new TextBox();
        ToolTip ErrorsHandler = new ToolTip();
        bool IsRegistered = false;
        public VarType Type = VarType.Number;
        public new string Name = "";
        public string Value = "";
        public List<double> Values = new List<double>();
        string left_interval = "";
        string right_interval = "";
        string step;
        

        delegate double Step(double targetValue, double value);
        delegate double StepSingle(double targetValue);
        Dictionary<char, int> Precedence = new Dictionary<char, int>() { { '+', 1 }, { '-', 1 }, { '*', 2 }, { '/', 2 }, { '%', 2 }, { '^', 3 }, { ':', 3 }, { '[', 3 }, { ']', 3 }, { '$', 3 }, { '(', 0 }, { ')', 0 } };
        Dictionary<char, bool> Associativity = new Dictionary<char, bool>() { { '+', false }, { '-', false }, { '*', false }, { '/', false }, { '%', false }, { '^', true }, { '[', false }, { ']', false }, { '$', false }, { ':', true }, { '(', true }, { ')', true } };

        public Variable_window()
        {
            InitializeComponent();
            BackColor = Color.Transparent;
            Margin = new Padding(10, 3, 25, 3);
            VarName.Dock = DockStyle.Fill;
            VarValue.Dock = DockStyle.Fill;
            VarName.Name = "имя переменной";
            VarValue.Name = "значение переменной";
            ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 5));
            ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 5));
            ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));
            ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));
            ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));
            Controls.Add(VarName, 0, 0);
            Controls.Add(new Label(), 1, 0);
            Controls.Add(VarValue, 2, 0);
            Controls.Add(new ComboBox(), 3, 0);
            Controls.Add(new Label(), 4, 0);
            Controls.Add(new Button(), 5, 0);
            (Controls[1] as Label).Text = "=";
            (Controls[1] as Label).TextAlign = ContentAlignment.MiddleCenter;
            (Controls[4] as Label).Text = "Ок";
            (Controls[4] as Label).TextAlign = ContentAlignment.MiddleCenter;
            (Controls[5] as Button).Text = "Results";
            (Controls[5] as Button).Click += ShowResults;
            (Controls[5] as Button).BackColor = Color.White;
            //ErrorsHandler.SetToolTip(Controls[4], "Имя переменной не указано");
            VarValue.TextChanged += DataCheck;
            VarName.TextChanged += DataCheck;
            (Controls[3] as ComboBox).Items.Add(new Label().Text = "Число");
            (Controls[3] as ComboBox).Items.Add(new Label().Text = "Формула");
            (Controls[3] as ComboBox).Items.Add(new Label().Text = "Интервал");
            (Controls[3] as ComboBox).DropDownStyle = ComboBoxStyle.DropDownList;
            (Controls[3] as ComboBox).SelectedIndex = 0;
            (Controls[3] as ComboBox).SelectedIndexChanged += Variable_window_SelectedIndexChanged;
            VarName.TextChanged += Variable_window_SelectedIndexChanged;
            VarValue.TextChanged += Variable_window_SelectedIndexChanged;
            VarValue.TextChanged += Program.MainHwnd.CallUpdate;
            (Controls[3] as ComboBox).SelectedIndexChanged += Program.MainHwnd.CallUpdate;
        }

        private void Variable_window_SelectedIndexChanged(object sender, EventArgs e) //Calls an update of variable in the main list of variables for this variable
        {
            Type = (VarType)(Controls[3] as ComboBox).SelectedIndex;
            Value = VarValue.Text;
            Program.MainHwnd.UpdateVar(Name, this);
            Name = VarName.Text;
            if (Type == VarType.Interval)
            {
                VarValue.TextChanged += TryGetInterval;
                TryGetInterval(this, new EventArgs());
            }
            else
                VarValue.TextChanged -= TryGetInterval;
        }

        private void ShowResults(object sender, EventArgs e) //show values on button press
        {
            string end_results = "{ ";
            foreach (var value in Values)
                end_results += value + " ";
            end_results += "}";
            MessageBox.Show(end_results);
        }
        private void TryGetInterval(object sender, EventArgs e) //Real time interval edges calculation
        {
            left_interval = "";
            right_interval = "";
            step = "";
            bool isOrder = false;
            bool readStep = false;
            if (Type == VarType.Interval && VarValue.Text.Contains("(") && VarValue.Text.Contains(")"))
            {
                string val;
                FormatToken(VarValue.Text, out val);

                for (int i = 0; i < val.Length; i++)
                {
                    if ("0123456789-,".Contains(val[i]) && !readStep)
                    {
                        if (!isOrder)
                            left_interval += val[i];
                        else
                            right_interval += val[i];
                    }
                    else if ("0123456789-,".Contains(val[i]) && readStep)
                    {
                        step += val[i];
                    }
                    else
                    {
                        if (left_interval != "" && !isOrder)
                            isOrder = true;
                        else if (isOrder)
                            readStep = true;
                        continue;
                    }
                }
            }
        }

        internal void TryGetValues(object sender, EventArgs e) //Real time value evaluation
        {
            Values.Clear();
            try
            {
                string val;
                FormatToken(VarValue.Text, out val);

                if (Type == VarType.Number) //If value is a number
                {
                    Values.Add(Convert.ToDouble(val));
                }
                else if (Type == VarType.Interval) //If valus is an interval
                {
                    if (left_interval != "" && right_interval != "" && step != "" && Convert.ToDouble(step) != 0 && 
                        ((Convert.ToDouble(left_interval) < Convert.ToDouble(right_interval) && Convert.ToDouble(step) > 0) || (Convert.ToDouble(left_interval) > Convert.ToDouble(right_interval) && Convert.ToDouble(step) < 0)))
                    {
                        for (double i = Convert.ToDouble(left_interval); i <= Convert.ToDouble(right_interval); i += Convert.ToDouble(step))
                            Values.Add(i);
                    }
                    else
                    {
                        string new_num = "";

                        for (int i = 0; i < val.Length; i++)
                        {
                            if ("0123456789-,".Contains(val[i]))
                            {
                                new_num += val[i];
                            }
                            else if (new_num != "")
                            {
                                Values.Add(Convert.ToDouble(new_num));
                                new_num = "";
                            }
                        }
                    }
                }
                else //if value is a formula
                {
                    double min_interval = -1;
                    foreach (var hWnd in Program.MainHwnd.Variables) //getting min count of values between all used intervals/formulas
                    {
                        if (Value.Contains(hWnd.Name) && (hWnd.Type == VarType.Interval || hWnd.Type == VarType.Formula) && (hWnd.Values.Count < min_interval || min_interval == -1))
                            min_interval = hWnd.Values.Count;
                    }
                    ShuntingYard(val, (int)min_interval, (int)min_interval);
                }
                (Controls[4] as Label).Text = "Ok"; //No errors here, program can proceed as intended
                (Controls[5] as Button).Enabled = true;
            }
            catch (Exception errorMsg) //Could not get values
            {
                Console.WriteLine(errorMsg.Message);
                (Controls[4] as Label).Text = "Error";
                (Controls[5] as Button).Enabled = false;
            }
        }

        private void DataCheck(object sender, EventArgs e)
        {
            if (!IsRegistered)
            {
                Program.MainHwnd.RegisterVariable = this;
                IsRegistered = true;
            }
        }

        double Add(double targetValue, double value) //delegate for +
        {
            return value + targetValue;
        }

        double Sub(double targetValue, double value) //delegate for -
        {
            return value - targetValue;
        }

        double Multiply(double targetValue, double value) //delegate for  *
        {
            return value * targetValue;
        }

        double Divide(double targetValue, double value) //delegate for /
        {
            return value / targetValue;
        }

        double Mod(double targetValue, double value) //delegate for %
        {
            return value % targetValue;
        }

        double Power(double targetValue, double value) //delegate for ^
        {
            return Math.Pow(value, targetValue);
        }
        double Root(double targetValue, double value) //delegate for Root
        {
            if (value < 0) throw new Exception();
            return Math.Pow(value, 1/targetValue);
        }
        double Sinus(double targetValue) //delegate for Sin
        {
            return Math.Sin(targetValue);
        }
        double Cosinus(double targetValue) //delegate for Cos
        {
            return Math.Cos(targetValue);
        }
        double Exponent(double targetValue) //delegate for Exp
        {
            return Math.Exp(targetValue);
        }
        private bool ShuntingYard(string Token, int itterations, int pointer) //getting reverse polish notation
        {
            Queue<string> Output = new Queue<string>();
            Stack<char> OperStack = new Stack<char>();

            for (int index = 0; index < Token.Count(); index++)
            {
                if ("0123456789,".Contains(Token[index]))
                {
                    int flag = index;
                    string next = "";
                    while (flag < Token.Length && "0123456789,".Contains(Token[flag]))
                    {
                        next += Token[flag];
                        flag++;
                    }
                    index = --flag;
                    Output.Enqueue(next);
                }
                else if (Program.MainHwnd.VarNames.Contains(Token[index].ToString()))
                {
                    Output.Enqueue(Program.MainHwnd.VariableValues[Token[index].ToString()].ElementAt(itterations - pointer).ToString());
                }
                else if ("+=-*/%^:[]$".Contains(Token[index]))
                {
                    while (OperStack.Count > 0 && ((Precedence[OperStack.First()] >= Precedence[Token[index]] && !Associativity[Token[index]]) || (Precedence[OperStack.First()] > Precedence[Token[index]] && Associativity[Token[index]])))
                        Output.Enqueue(OperStack.Pop().ToString());
                    OperStack.Push(Token[index]);
                }
                else if (Token[index] == '(')
                {
                    OperStack.Push(Token[index]);
                }
                else if (Token[index] == ')')
                {
                    while (OperStack.Count > 0 && OperStack.First() != '(')
                        Output.Enqueue(OperStack.Pop().ToString());
                    if (OperStack.Count > 0)
                        OperStack.Pop();
                }
                else
                {
                    Console.WriteLine("Something is wrong here");
                    throw new Exception();
                }
            }

            while (OperStack.Count > 0)
                Output.Enqueue(OperStack.Pop().ToString());

            Values.Add(Convert.ToDouble(Evaluate(Output)));
            if (pointer-1 > 0)
                ShuntingYard(Token, itterations, --pointer);

            return true;
        }

        private string Evaluate(Queue<string> Output) //evaluating reverse polish notation
        {
            Stack<string> stack = new Stack<string>();

            for (int index = 0; index < Output.Count(); index++)
            {
                var token = Output.ElementAt(index);

                if ("+=-*/%^:".Contains(token))
                {
                    var number1 = stack.Pop();
                    var number2 = stack.Pop();
                    stack.Push(Operate(token, number1, number2).ToString());
                }
                else if ("[]$".Contains(token))
                {
                    var number1 = stack.Pop();
                    stack.Push(Operate(token, number1).ToString());
                }
                else
                {
                    stack.Push(token);
                }
            }
            return stack.Pop();
        }

        private void FormatToken(string oldValue, out string token)
        {
            string newValue;
            newValue = oldValue.Replace("sin", "[");
            newValue = newValue.Replace("cos", "]");
            newValue = newValue.Replace("exp", "$");
            for (int i = 1; i < 25; i++)
                newValue = newValue.Replace(i+"pi", (i * Math.PI).ToString());
            newValue = newValue.Replace("pi", Math.PI.ToString());
            token = newValue;
        }

        private double Operate(string operation, string nFirst, string nSecond) //Selects delegate to invoke
        {
            Step myStep;

            switch (operation)
            {
                case "+":
                    myStep = new Step(Add);
                    break;
                case "-":
                    myStep = new Step(Sub);
                    break;
                case "*":
                    myStep = new Step(Multiply);
                    break;
                case "/":
                    myStep = new Step(Divide);
                    break;
                case "%":
                    myStep = new Step(Mod);
                    break;
                case "^":
                    myStep = new Step(Power);
                    break;
                case ":":
                    myStep = new Step(Root);
                    break;
                default:
                    throw new Exception();
            }
            return myStep.Invoke(Convert.ToDouble(nFirst), Convert.ToDouble(nSecond)); //Perform evaluation
        }

        private double Operate(string operation, string nFirst) //Selects delegate to invoke
        {
            StepSingle StepSingle;

            switch (operation)
            {
                case "[":
                    StepSingle = new StepSingle(Sinus);
                    break;
                case "]":
                    StepSingle = new StepSingle(Cosinus);
                    break;
                case "$":
                    StepSingle = new StepSingle(Exponent);
                    break;
                default:
                    throw new Exception();
            }
            return StepSingle.Invoke(Convert.ToDouble(nFirst)); //Perform evaluation
        }
    }
}
