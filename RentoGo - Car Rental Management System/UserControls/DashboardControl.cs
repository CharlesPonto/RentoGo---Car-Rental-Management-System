using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing; // For printing
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using RentoGo___Car_Rental_Management_System.Forms;

namespace RentoGo___Car_Rental_Management_System.UserControls
{
    public partial class DashboardControl : UserControl
    {
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

            // auto updates
            AppEvents.VehiclesUpdated += () =>
            {
                LoadDashboardStats();
                LoadVehicleStatusChart();
            };
            AppEvents.RentalsUpdated += () =>
            {
                LoadDashboardStats();
                LoadRecentRentals();
            };
            AppEvents.PaymentsUpdated += () => LoadDashboardStats();
            AppEvents.CustomersUpdated += () => LoadDashboardStats();
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

        // 📊 Pie Chart (Show numbers inside slices)
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

                // Chart Formatting: Show numbers only inside pie slices
                var series = chartVehicleStatus.Series["VehicleStatus"];
                series.ChartType = SeriesChartType.Pie;
                series.IsValueShownAsLabel = true;   // Show values
                series.Label = "#VAL";               // Display only numbers (count)
                series.LegendText = "#VALX";         // Keep status text in legend
                series.Font = new Font("Segoe UI", 10, FontStyle.Bold);  // Better visibility
                series["PieLabelStyle"] = "Inside"; // Place labels inside slices

                // Optional: Show numbers + percentage:
                // series.Label = "#VAL (#PERCENT{P0})";
            }
            catch (Exception ex)
            {
                MessageBox.Show("⚠ Error loading vehicle status chart:\n" + ex.Message,
                    "Chart Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 🧾 Recent Rentals Table
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

        // Generate Report Button Click
        private void btnGenerateReport_Click(object sender, EventArgs e)
        {
            using (ReportOptionsForm optionsForm = new ReportOptionsForm())
            {
                if (optionsForm.ShowDialog() == DialogResult.OK)
                {
                    string reportType = optionsForm.ReportType;
                    DateTime startDate = optionsForm.StartDate;
                    DateTime endDate = optionsForm.EndDate;

                    GenerateReport(reportType, startDate, endDate);
                }
            }
        }

        // Generate and Print Report
        private void GenerateReport(string reportType, DateTime startDate, DateTime endDate)
        {
            try
            {
                // Fetch data for the report
                DataTable rentals = GetRentalsData(startDate, endDate);
                DataTable payments = GetPaymentsData(startDate, endDate);
                decimal totalRevenue = GetTotalRevenue(startDate, endDate);

                // Create PrintDocument
                PrintDocument printDoc = new PrintDocument();
                printDoc.PrintPage += (s, e) => PrintReportPage(e, reportType, startDate, endDate, rentals, payments, totalRevenue);

                // Show Print Preview
                PrintPreviewDialog previewDialog = new PrintPreviewDialog
                {
                    Document = printDoc,
                    Width = 800,
                    Height = 600
                };
                previewDialog.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating report:\n" + ex.Message, "Report Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Fetch Rentals Data
        private DataTable GetRentalsData(DateTime startDate, DateTime endDate)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = @"
                    SELECT r.RentalID, c.FullName AS Customer, v.Model AS Vehicle, r.StartDate, r.EndDate, r.TotalCharge, r.Status
                    FROM Rentals r
                    JOIN Customers c ON r.CustomerID = c.CustomerID
                    JOIN Vehicles v ON r.VehicleID = v.VehicleID
                    WHERE r.StartDate >= @start AND r.EndDate <= @end
                    ORDER BY r.StartDate";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.SelectCommand.Parameters.AddWithValue("@start", startDate);
                da.SelectCommand.Parameters.AddWithValue("@end", endDate);
                da.Fill(dt);
            }
            return dt;
        }

        // Fetch Payments Data
        private DataTable GetPaymentsData(DateTime startDate, DateTime endDate)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = @"
                    SELECT p.RentalID, p.Amount, p.Type, p.PaymentDate
                    FROM Payments p
                    WHERE p.PaymentDate >= @start AND p.PaymentDate <= @end
                    ORDER BY p.PaymentDate";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.SelectCommand.Parameters.AddWithValue("@start", startDate);
                da.SelectCommand.Parameters.AddWithValue("@end", endDate);
                da.Fill(dt);
            }
            return dt;
        }

        // Get Total Revenue
        private decimal GetTotalRevenue(DateTime startDate, DateTime endDate)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT ISNULL(SUM(Amount), 0) FROM Payments WHERE PaymentDate >= @start AND PaymentDate <= @end", con);
                cmd.Parameters.AddWithValue("@start", startDate);
                cmd.Parameters.AddWithValue("@end", endDate);
                return Convert.ToDecimal(cmd.ExecuteScalar());
            }
        }

        // Print Report Page (Improved Formatting)
        private void PrintReportPage(PrintPageEventArgs e, string reportType, DateTime startDate, DateTime endDate, DataTable rentals, DataTable payments, decimal totalRevenue)
        {
            Graphics g = e.Graphics;
            Font titleFont = new Font("Arial", 18, FontStyle.Bold);
            Font headerFont = new Font("Arial", 14, FontStyle.Bold);
            Font subHeaderFont = new Font("Arial", 12, FontStyle.Bold);
            Font bodyFont = new Font("Arial", 10);
            Brush brush = Brushes.Black;
            Pen pen = new Pen(Brushes.Black, 1);

            float yPos = 50;
            float leftMargin = 50;
            float pageWidth = e.PageBounds.Width - 100;

            // Title
            g.DrawString("RentoGo Rental Report", titleFont, brush, leftMargin, yPos);
            yPos += 40;
            g.DrawString($"Report Type: {reportType}", subHeaderFont, brush, leftMargin, yPos);
            yPos += 20;
            g.DrawString($"Period: {startDate.ToShortDateString()} to {endDate.ToShortDateString()}", bodyFont, brush, leftMargin, yPos);
            yPos += 30;

            // Summary Section
            g.DrawString("Summary", headerFont, brush, leftMargin, yPos);
            yPos += 25;
            g.DrawLine(pen, leftMargin, yPos, pageWidth, yPos); // Line
            yPos += 10;
            g.DrawString($"Total Vehicles: {lblTotalVehicles.Text}", bodyFont, brush, leftMargin, yPos);
            yPos += 15;
            g.DrawString($"Total Customers: {lblTotalCustomers.Text}", bodyFont, brush, leftMargin, yPos);
            yPos += 15;
            g.DrawString($"Active Rentals: {lblActiveRentals.Text}", bodyFont, brush, leftMargin, yPos);
            yPos += 15;
            g.DrawString($"Total Revenue for Period: ₱{totalRevenue:N2}", bodyFont, brush, leftMargin, yPos);
            yPos += 30;

            // Rentals Section
            if (rentals.Rows.Count > 0)
            {
                g.DrawString("Rentals", headerFont, brush, leftMargin, yPos);
                yPos += 25;
                g.DrawLine(pen, leftMargin, yPos, pageWidth, yPos);
                yPos += 10;

                // Table Header
                g.DrawString("ID | Customer | Vehicle | Start Date | End Date | Charge | Status", subHeaderFont, brush, leftMargin, yPos);
                yPos += 20;
                g.DrawLine(pen, leftMargin, yPos, pageWidth, yPos);
                yPos += 10;

                foreach (DataRow row in rentals.Rows)
                {
                    string line = $"{row["RentalID"]} | {row["Customer"]} | {row["Vehicle"]} | {Convert.ToDateTime(row["StartDate"]).ToShortDateString()} | {Convert.ToDateTime(row["EndDate"]).ToShortDateString()} | ₱{row["TotalCharge"]} | {row["Status"]}";
                    g.DrawString(line, bodyFont, brush, leftMargin, yPos);
                    yPos += 15;
                    if (yPos > e.PageBounds.Height - 50) { e.HasMorePages = true; return; }
                }
                yPos += 20;
            }

            // Payments Section
            if (payments.Rows.Count > 0)
            {
                g.DrawString("Payments", headerFont, brush, leftMargin, yPos);
                yPos += 25;
                g.DrawLine(pen, leftMargin, yPos, pageWidth, yPos);
                yPos += 10;

                // Table Header
                g.DrawString("Rental ID | Amount | Type | Date", subHeaderFont, brush, leftMargin, yPos);
                yPos += 20;
                g.DrawLine(pen, leftMargin, yPos, pageWidth, yPos);
                yPos += 10;

                foreach (DataRow row in payments.Rows)
                {
                    string line = $"{row["RentalID"]} | ₱{row["Amount"]} | {row["Type"]} | {Convert.ToDateTime(row["PaymentDate"]).ToShortDateString()}";
                    g.DrawString(line, bodyFont, brush, leftMargin, yPos);
                    yPos += 15;
                    if (yPos > e.PageBounds.Height - 50) { e.HasMorePages = true; return; }
                }
            }

            // Footer
            yPos = e.PageBounds.Height - 50;
            g.DrawString("Generated on: " + DateTime.Now.ToString(), bodyFont, brush, leftMargin, yPos);
        }
    }
}