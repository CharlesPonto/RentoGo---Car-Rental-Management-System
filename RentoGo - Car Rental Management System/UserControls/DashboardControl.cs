using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace RentoGo___Car_Rental_Management_System.UserControls
{
    public partial class DashboardControl : UserControl
    {
        //private string connectionString = @"Server=LAPTOP-8NVU0AIF\SQLEXPRESS;Database=RentoGoDB;Trusted_Connection=True;";
        private readonly string connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=RentoGoDB;Trusted_Connection=True;";

        public DashboardControl()
        {
            InitializeComponent();
        }

        private void DashboardControl_Load(object sender, EventArgs e)
        {
            LoadDashboardStats();
            LoadVehicleStatusChart();
            LoadRecentRentals();
        }

        private void LoadDashboardStats()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Vehicles", con))
                    {
                        int totalVehicles = Convert.ToInt32(cmd.ExecuteScalar());
                        lblTotalVehicles.Text = totalVehicles.ToString();
                    }

                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Customers", con))
                    {
                        int totalCustomers = Convert.ToInt32(cmd.ExecuteScalar());
                        lblTotalCustomers.Text = totalCustomers.ToString();
                    }

                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Rentals WHERE Status = 'Active'", con))
                    {
                        int activeRentals = Convert.ToInt32(cmd.ExecuteScalar());
                        lblActiveRentals.Text = activeRentals.ToString();
                    }

                    using (SqlCommand cmd = new SqlCommand(@"
                        SELECT ISNULL(SUM(Amount), 0)
                        FROM Payments
                        WHERE MONTH(PaymentDate) = MONTH(GETDATE())
                          AND YEAR(PaymentDate) = YEAR(GETDATE());", con))
                    {
                        decimal monthlyRevenue = Convert.ToDecimal(cmd.ExecuteScalar());
                        lblMonthlyRevenue.Text = $"₱{monthlyRevenue:N2}";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("⚠ Error loading dashboard stats:\n" + ex.Message,
                    "Dashboard Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //chart
        private void LoadVehicleStatusChart()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "SELECT Status, COUNT(*) AS Count FROM Vehicles GROUP BY Status";
                    SqlCommand cmd = new SqlCommand(query, con);
                    SqlDataReader reader = cmd.ExecuteReader();

                    chartVehicleStatus.Series["VehicleStatus"].Points.Clear();

                    while (reader.Read())
                    {
                        string status = reader["Status"].ToString();
                        int count = Convert.ToInt32(reader["Count"]);
                        chartVehicleStatus.Series["VehicleStatus"].Points.AddXY(status, count);
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("⚠ Error loading vehicle status chart:\n" + ex.Message,
                    "Chart Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //rental log table
        private void LoadRecentRentals()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = @"
                 SELECT TOP 8 
                     r.RentalID,
                     c.FullName AS Customer,
                     v.Model AS Vehicle,
                     r.StartDate,
                     r.EndDate,
                     r.Status
                 FROM Rentals r
                 JOIN Customers c ON r.CustomerID = c.CustomerID
                 JOIN Vehicles v ON r.VehicleID = v.VehicleID
                 ORDER BY r.StartDate DESC";

                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvRecentRentals.DataSource = dt;
                }

                dgvRecentRentals.ClearSelection();
                dgvRecentRentals.CurrentCell = null;

                // remove selection
                dgvRecentRentals.SelectionChanged += (s, e) =>
                {
                    dgvRecentRentals.ClearSelection();
                    dgvRecentRentals.CurrentCell = null;
                };

                dgvRecentRentals.GotFocus += (s, e) =>
                {
                    this.ActiveControl = null;
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show("⚠ Error loading recent rentals:\n" + ex.Message,
                    "Table Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



    }
}