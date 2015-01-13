using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ClearProcess
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            txtTerm.Text = DbStuff.LoadThisTerm();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (txtId.Text.Length == 0) { DbStuff.ErrMsg("Missing ID."); return; }
            if (txtTerm.Text.Length == 0) { DbStuff.ErrMsg("Missing Term Code."); return; }

            // Gets the PIDM
            string pidm = DbStuff.LoadStudentPidm(txtId.Text);
            if (pidm.Length == 0) { DbStuff.ErrMsg("No PIDM found."); return; }

            button1.Enabled = false;

            // Check for an error code
            string error_code = DbStuff.CheckForError(pidm, txtTerm.Text);

            if (error_code.Length == 0)
            {
                // The deleting
                bool res = DbStuff.DeleteSfrracl(pidm, txtTerm.Text);
                if (res == true) { res = DbStuff.DeleteSftregs(pidm, txtTerm.Text); }
                if (res == true) { res = DbStuff.DeleteSftarea(pidm, txtTerm.Text); }
                if (res == false) { DbStuff.ErrMsg(String.Format("There was a problem in deleting records. Please check {0}.", Program.c.Database)); }
                else { MessageBox.Show("Complete."); }
            }
            else
            {
                DbStuff.ErrMsg(String.Format("Error code: {0}", error_code));
            }

            button1.Enabled = true;

            txtId.Text = "";
        }
    }
}
