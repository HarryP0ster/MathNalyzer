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
    public enum VarType
    {
        Number = 0,
        Formula = 1,
        Interval = 2
    }
    public partial class Main_Window : Form
    {
        List<Variable_window> VarHWnd = new List<Variable_window>();
        HashSet<Variable_window> list_variables = new HashSet<Variable_window>();

        internal HashSet<Variable_window> Variables
        {
            get => list_variables;
        }

        internal HashSet<string> VarNames
        {
            get
            {
                HashSet<string> names = new HashSet<string>();

                foreach (Variable_window var in list_variables)
                {
                    names.Add(var.Name);
                }
                return names;
            }
        }

        internal Dictionary<string, List<double>> VariableValues
        {
            get
            {
                Dictionary<string, List<double>> dict = new Dictionary<string, List<double>>();

                foreach (Variable_window var in list_variables)
                {
                    dict.Add(var.Name, var.Values);
                }
                return dict;
            }
        }

        internal Dictionary<string, VarType> VariableTypes
        {
            get
            {
                Dictionary<string, VarType> dict = new Dictionary<string, VarType>();

                foreach (Variable_window var in list_variables)
                {
                    dict.Add(var.Name, var.Type);
                }
                return dict;
            }
        }

        internal Variable_window RegisterVariable
        {
            set
            {
                list_variables.Add(value);
            }
        }

        public Main_Window()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            vars_table.RowStyles.Insert(0, new RowStyle(SizeType.Absolute, 65));
            VarHWnd.Add(new Variable_window());
            VarHWnd.Last().Dock = DockStyle.Fill;
            vars_table.Controls.Add(VarHWnd.Last(), 0, 0);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            for (int index = 0; index < list_variables.Count; index++)
            {
                if (list_variables.ToArray()[index].Type == VarType.Formula)
                {
                    //Variable var = list_variables.ElementAt(index);
                    //var res = ShuntingYard(var.Text);
                }
            }
        }

        internal void UpdateVar(string oldName, Variable_window newVar)
        {
            Variable_window oldVar = new Variable_window();

            foreach (Variable_window var in list_variables)
            {
                if (var.Name == oldName)
                {
                    oldVar = var;
                    break;
                }

            }
            list_variables.Remove(oldVar);
            if (newVar.Name != null && newVar.Text != "")
            {
                list_variables.Add(newVar);
            }
        }

        internal void CallUpdate(object sender, EventArgs e)
        {
            foreach (var hWnd in list_variables)
                hWnd.TryGetValues(sender, e);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Graph newGraph = new Graph();
            newGraph.Show();
        }
    }
}
