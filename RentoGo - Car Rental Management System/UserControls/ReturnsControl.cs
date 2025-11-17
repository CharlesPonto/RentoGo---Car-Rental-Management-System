using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace RentoGo___Car_Rental_Management_System.UserControls
{
    public partial class ReturnsControl : UserControl
    {
        private readonly string connectionString =
            @"Server=(localdb)\MSSQLLocalDB;Database=RentoGoDB;Trusted_Connection=True;";

        public ReturnsControl()
        {
            InitializeComponent();
        }

        private void ReturnsControl_Load(object sender, EventArgs e)
        {
            configureGrid();
            loadReturns();
        }

        // load
        private void loadReturns(string search = "")
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    string query = @"
                        SELECT 
                            r.RentalID,
                            c.FullName AS Customer,
                            v.Model AS Vehicle,
                            r.StartDate,
                            r.EndDate,
                            r.ReturnDate,
                            r.LateFee,
                            r.DamageFee,
                            r.TotalCharge
                        FROM Rentals r
                        JOIN Customers c ON r.CustomerID = c.CustomerID
                        JOIN Vehicles v ON r.VehicleID = v.VehicleID
                        WHERE 
                            r.Status = 'Completed' AND
                            (@search = '' OR c.FullName LIKE @search OR v.Model LIKE @search)
                        ORDER BY r.ReturnDate DESC";

                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    da.SelectCommand.Parameters.AddWithValue("@search", $"%{search}%");

                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvReturns.DataSource = dt;
                }

                dgvReturns.ClearSelection();
                dgvReturns.CurrentCell = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading return history:\n" + ex.Message,
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // grid
        private void configureGrid()
        {
            dgvReturns.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvReturns.RowHeadersVisible = false;
            dgvReturns.AllowUserToAddRows = false;
            dgvReturns.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvReturns.MultiSelect = false;
            dgvReturns.BorderStyle = BorderStyle.None;
            dgvReturns.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvReturns.GridColor = Color.FromArgb(223, 245, 228);

            dgvReturns.EnableHeadersVisualStyles = false;
            dgvReturns.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(223, 245, 228);
            dgvReturns.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(64, 64, 64);
            dgvReturns.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 11);

            dgvReturns.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            dgvReturns.DefaultCellStyle.SelectionBackColor = Color.White;
            dgvReturns.DefaultCellStyle.SelectionForeColor = Color.Black;
        }

        // search
        private void btnSearch_Click(object sender, EventArgs e)
        {
            string input = txtSearch.Text.Trim();
            loadReturns(input);
        }

    }
}
