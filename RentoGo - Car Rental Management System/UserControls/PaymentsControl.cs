using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using RentoGo___Car_Rental_Management_System.Forms;
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
                            p.Type
                        FROM Payments p
                        JOIN Rentals r ON p.RentalID = r.RentalID
                        JOIN Customers c ON r.CustomerID = c.CustomerID
                        JOIN Vehicles v ON r.VehicleID = v.VehicleID
                        WHERE (@search = '' OR c.FullName LIKE @search OR v.Model LIKE @search)
                        ORDER BY p.PaymentDate DESC";

                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    da.SelectCommand.Parameters.AddWithValue("@search", $"%{search}%");

                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvPayments.DataSource = dt;
                }

                dgvPayments.ClearSelection();
                dgvPayments.CurrentCell = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading payments:\n" + ex.Message);
            }
        }

        private void configureGrid()
        {
            dgvPayments.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvPayments.RowHeadersVisible = false;
            dgvPayments.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPayments.AllowUserToAddRows = false;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            loadPayments(txtSearch.Text.Trim());
        }

        private void btnAddPayment_Click(object sender, EventArgs e)
        {
            AddPaymentForm form = new AddPaymentForm();
            if (form.ShowDialog() == DialogResult.OK)
            AppEvents.RaiseCustomersUpdated();
                loadPayments();
        }
    }
}
