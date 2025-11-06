using RentoGo___Car_Rental_Management_System.UserControls;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RentoGo___Car_Rental_Management_System
{
    public partial class MainForm : Form
    {
        private Guna.UI2.WinForms.Guna2Button currentButton;

        // Cached UserControls (created once, reused later)
        private DashboardControl dashboardControl1;
        private VehiclesControl vehiclesControl1;
        private CustomerControl customerControl1;
        private RentalsControl rentalsControl1;
        private PaymentsControl paymentsControl1;
        private ReturnsControl returnsControl1;

        public MainForm()
        {
            InitializeComponent();

            // Preload Dashboard (default page)
            dashboardControl1 = new DashboardControl();
            ShowControl(dashboardControl1);
            ActivateButton(btnDashboard);
        }

        private void ShowControl(UserControl controlToShow)
        {
            panelPages.Controls.Clear();
            controlToShow.Dock = DockStyle.Fill;
            panelPages.Controls.Add(controlToShow);
            controlToShow.BringToFront();
        }

        private void ActivateButton(Guna.UI2.WinForms.Guna2Button button)
        {
            if (currentButton != null)
            {
                currentButton.FillColor = Color.White;
                currentButton.ForeColor = Color.FromArgb(64, 64, 64);
            }

            currentButton = button;
            currentButton.FillColor = Color.FromArgb(211, 194, 255); 
        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            if (dashboardControl1 == null)
                dashboardControl1 = new DashboardControl();

            ShowControl(dashboardControl1);
            ActivateButton(btnDashboard);
        }

        private void btnVehicles_Click(object sender, EventArgs e)
        {
            if (vehiclesControl1 == null)
                vehiclesControl1 = new VehiclesControl();

            ShowControl(vehiclesControl1);
            ActivateButton(btnVehicles);
        }

        private void btnCustomers_Click(object sender, EventArgs e)
        {
            if (customerControl1 == null)
                customerControl1 = new CustomerControl();

            ShowControl(customerControl1);
            ActivateButton(btnCustomers);
        }

        private void btnRentals_Click(object sender, EventArgs e)
        {
            if (rentalsControl1 == null)
                rentalsControl1 = new RentalsControl();

            ShowControl(rentalsControl1);
            ActivateButton(btnRentals);
        }

        private void btnPayments_Click(object sender, EventArgs e)
        {
            if (paymentsControl1 == null)
                paymentsControl1 = new PaymentsControl();

            ShowControl(paymentsControl1);
            ActivateButton(btnPayments);
        }

        private void btnReturns_Click(object sender, EventArgs e)
        {
            if (returnsControl1 == null)
                returnsControl1 = new ReturnsControl();

            ShowControl(returnsControl1);
            ActivateButton(btnReturns);
        }
    }
}
