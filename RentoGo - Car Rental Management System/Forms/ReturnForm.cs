using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RentoGo___Car_Rental_Management_System.Forms
{
    public partial class ReturnForm : Form
    {
        public ReturnForm()
        {
            InitializeComponent();
        }

        public ReturnForm(int id)
        {
            InitializeComponent();
            id = 0;
        }
    }
}
