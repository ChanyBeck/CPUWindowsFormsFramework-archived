using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CPUWindowsFormsFramework
{
    public class WindowsFormsUtility
    {
        public static void FormatGridForSearch(DataGridView dg, string tablename)
        {
            dg.AllowUserToAddRows = false;
            dg.ReadOnly = true;
            dg.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            DoFormatGrid(dg, tablename); 
        }

        public static void FormatGridForEdit(DataGridView grid, string tablename)
        {
            grid.EditMode = DataGridViewEditMode.EditOnEnter;
            DoFormatGrid(grid, tablename);
        }

        private static void DoFormatGrid(DataGridView grid, string tablename)
        {
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            grid.RowHeadersWidth = 25;
            foreach (DataGridViewColumn col in grid.Columns)
            {
                if (col.Name.EndsWith("Id"))
                {
                    col.Visible = false;
                }
            }            
            string pkname = tablename + "Id"; 
            if (grid.Columns.Contains(pkname)){
                grid.Columns[pkname].Visible = false; 
            }
        }

        public static int GetIdFromGrid(DataGridView grid, int rowindex, string colname)
        {
            int id = 0;
            if(rowindex < grid.Rows.Count && grid.Columns.Contains(colname) && grid.Rows[rowindex].Cells[colname].Value != DBNull.Value){
                if (grid.Rows[rowindex].Cells[colname].Value is int)
                {
                    id = (int)grid.Rows[rowindex].Cells[colname].Value;
                }
            }
            return id;
        }

        public static int GetIdFromComboBox(ComboBox lst)
        {
            int value = 0;
            if(lst.SelectedValue != null && lst.SelectedValue is int)
            {
                value = (int)lst.SelectedValue;
            }
            return value;
        }

        public static void AddDeleteButtonToGrid(DataGridView grid, string colname)
        {

            grid.Columns.Add(new DataGridViewButtonColumn() { Text = "X", HeaderText = "Delete", Name = colname, UseColumnTextForButtonValue = true });
        }
        public static void AddComboBoxToGrid(DataGridView grid, DataTable dt, string displaymember, string tablename)
        {
            DataGridViewComboBoxColumn combo = new();
            combo.DataSource = dt;
            combo.DisplayMember = displaymember;
            combo.ValueMember = tablename + "id";
            combo.DataPropertyName = combo.ValueMember;
            combo.HeaderText = tablename;
            grid.Columns.Insert(0, combo);
        }

        public static void SetListBinding(ComboBox lst, DataTable sdt, string tablename, DataTable? tdt)
        {
            lst.DataSource = sdt;
            lst.ValueMember = tablename + "Id";
            lst.DisplayMember = lst.Name.Substring(3);
            if (tdt != null)
            {
                lst.DataBindings.Add("SelectedValue", tdt, lst.ValueMember, false, DataSourceUpdateMode.OnPropertyChanged);
            }
        }
        public static void SetControlBinding(Control c, BindingSource bs)
        {
            string property = "";
            string column = "";
            string controlname = c.Name.ToLower();
            if (controlname.StartsWith("txt") || controlname.StartsWith("lbl"))
            {
                column = controlname.Substring(3);
                property = "Text";
            }

            if (property != "" && column != "")
            {
                c.DataBindings.Add(property, bs, column, true, DataSourceUpdateMode.OnPropertyChanged);
            }
        }
        public static bool DoesFormExist(Type formtype, int pk = 0)
        {
            bool exists = false;
            foreach (Form frm in Application.OpenForms)
            {
                int frmpk = 0;
                if(frm.Tag != null && frm.Tag is int)
                {
                    frmpk = (int)frm.Tag;
                }
                if (frm.GetType() == formtype && frmpk == pk)
                {
                    frm.Activate();
                    exists = true;
                }
            }
            return exists;
        }
        public static void SetupNav(ToolStrip ts)
        {
            ts.Items.Clear();
            foreach (Form f in Application.OpenForms)
            {
                if (f.IsMdiContainer == false)
                {
                    ToolStripButton btn = new();
                    btn.Text = f.Text;
                    btn.Tag = f;
                    btn.Click += Btn_Click;
                    ts.Items.Add(btn);
                    ts.Items.Add(new ToolStripSeparator());
                }
            }
        }
        private static void Btn_Click(object? sender, EventArgs e)
        {
            if (sender != null && sender is ToolStripButton)
            {
                ToolStripButton btn = (ToolStripButton)sender;
                if (btn.Tag != null && btn.Tag is Form)
                {
                    ((Form)btn.Tag).Activate();
                }
            }
        }
    }
}
