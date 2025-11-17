using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace RentoGo___Car_Rental_Management_System.UserControls
{
    public partial class PaymentsControl : UserControl
    {
        private readonly string connectionString =
            @"Server=(localdb)\MSSQLLocalDB;Database=RentoGoDB;Trusted_Connection=True;";

        public PaymentsControl()
        {
            InitializeComponent();
        }

        private void PaymentsControl_Load(object sender, EventArgs e)
        {
            configureGrid();
            loadPayments();
        }

        // load
        private void loadPayments(string search = "")
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    string query = @"
                        SELECT 
                            p.PaymentID,
                            r.RentalID,
                            c.FullName AS Customer,
                            v.Model AS Vehicle,
                            p.PaymentDate,
                            p.Amount,
                            p.Method,
                            p.Remarks
                        FROM Payments p
                        JOIN Rentals r ON p.RentalID = r.RentalID
                        JOIN Customers c ON r.CustomerID = c.CustomerID
                        JOIN Vehicles v ON r.VehicleID = v.VehicleID
                        WHERE 
                            (@search = '' OR c.FullName LIKE @search OR v.Model LIKE @search)
                        ORDER BY p.PaymentDate DESC";

                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    da.SelectCommand.Parameters.AddWithValue("@search", $"%{search}%");

                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvPayments.DataSource = dt;
                }

                // remove selection
                dgvPayments.ClearSelection();
                dgvPayments.CurrentCell = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading payments:\n" + ex.Message,
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // grid
        private void configureGrid()
        {
            dgvPayments.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvPayments.RowHeadersVisible = false;
            dgvPayments.AllowUserToAddRows = false;
            dgvPayments.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPayments.MultiSelect = false;

            dgvPayments.BorderStyle = BorderStyle.None;
            dgvPayments.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvPayments.GridColor = Color.FromArgb(223, 245, 228);

            dgvPayments.EnableHeadersVisualStyles = false;
            dgvPayments.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(223, 245, 228);
            dgvPayments.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(64, 64, 64);
            dgvPayments.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 11);

            dgvPayments.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            dgvPayments.DefaultCellStyle.SelectionBackColor = Color.White;
            dgvPayments.DefaultCellStyle.SelectionForeColor = Color.Black;
        }

        // search
        private void btnSearch_Click(object sender, EventArgs e)
        {
            string term = txtSearch.Text.Trim();
            loadPayments(term);
        }
    }
}
