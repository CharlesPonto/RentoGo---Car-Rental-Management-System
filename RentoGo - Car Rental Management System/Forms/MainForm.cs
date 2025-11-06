using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RentoGo___Car_Rental_Management_System
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        //public void loadForm(object Form)
        //{
        //    //if (this.panelMain.Controls.Count > 0)
        //    //    this.panelMain.Controls.RemoveAt(0);
        //    //Form f = Form as Form;

        //    //f.TopLevel = false;
        //    //f.Dock = DockStyle.Fill;
        //    //this.panelMain.Controls.Add(f);
        //    //this.panelMain.Tag = f;
        //    //f.Show();
        //}

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            dashboardControl.Visible = true;
            vehiclesControl.Visible = false;
            customerControl.Visible = false;
            rentalsControl.Visible = false;
            paymentsControl.Visible = false;   
            returnsControl.Visible = false;
        }

        private void btnVehicles_Click(object sender, EventArgs e)
        {
            dashboardControl.Visible = false;
            vehiclesControl.Visible = true;
            customerControl.Visible = false;
            rentalsControl.Visible = false;
            paymentsControl.Visible = false;
            returnsControl.Visible = false;
        }

        private void btnCustomers_Click(object sender, EventArgs e)
        {
            dashboardControl.Visible = false;
            vehiclesControl.Visible = false;
            customerControl.Visible = true;
            rentalsControl.Visible = false;
            paymentsControl.Visible = false;
            returnsControl.Visible = false;
        }

        private void btnRentals_Click(object sender, EventArgs e)
        {
            dashboardControl.Visible = false;
            vehiclesControl.Visible = false;
            customerControl.Visible = false;
            rentalsControl.Visible = true;
            paymentsControl.Visible = false;
            returnsControl.Visible = false;
        }

        private void btnPayments_Click(object sender, EventArgs e)
        {
            dashboardControl.Visible = false;
            vehiclesControl.Visible = false;
            customerControl.Visible = false;
            rentalsControl.Visible = false;
            paymentsControl.Visible = true;
            returnsControl.Visible = false;
        }

        private void btnReturns_Click(object sender, EventArgs e)
        {
            dashboardControl.Visible = false;
            vehiclesControl.Visible = false;
            customerControl.Visible = false;
            rentalsControl.Visible = false;
            paymentsControl.Visible = false;
            returnsControl.Visible = true;
        }

    }
}
