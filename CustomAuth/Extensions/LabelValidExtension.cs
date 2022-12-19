using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PawnShop.Forms.Extensions
{
    public static class LabelValidExtension
    {
        public static void SetValid(this Label label)
        {
            label.Text = "Valid";
            label.ForeColor = Color.Green;
        }

        public static void SetInvalid(this Label label)
        {
            label.Text = "Invalid";
            label.ForeColor = Color.Red;
        }
    }
}
