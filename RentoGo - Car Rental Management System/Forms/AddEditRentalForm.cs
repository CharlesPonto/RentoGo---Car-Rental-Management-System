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
    public partial class AddEditRentalForm : Form
    {
        public AddEditRentalForm()
        {
            InitializeComponent();
        }

        public AddEditRentalForm(int id)
        {
            id = 0;
            InitializeComponent();
        }
    }
}
