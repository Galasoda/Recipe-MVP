using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SBC_2D.Infrastructures
{
    public static class Msgbox
    {
        public static void MyError(string strMessage)
        {
            MessageBox.Show(strMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void MyError(string ProgramName, string strMessage)
        {
            MessageBox.Show(strMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static bool MyConfirmation(string strMessage)
        {
            bool IsConfirm = true;
            if (MessageBox.Show(strMessage, "Confirmation", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
            {
                IsConfirm = false;
            }
            return IsConfirm;
        }

        public static void MyInformation(string strMessage)
        {
            MessageBox.Show(strMessage, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void MyWarning(string strMessage)
        {
            MessageBox.Show(strMessage, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
