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
using RentoGo___Car_Rental_Management_System.Forms;

namespace RentoGo___Car_Rental_Management_System.UserControls
{
    public partial class CustomerControl : UserControl
    {
        private readonly string connectionString =
            @"Server=(localdb)\MSSQLLocalDB;Database=RentoGoDB;Trusted_Connection=True;";

        public CustomerControl()
        {
            InitializeComponent();
        }

        private void CustomersControl_Load(object sender, EventArgs e)
        {
            configureGrid();
            loadCustomers();
        }

        // load
        private void loadCustomers(string searchTerm = "")
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = @"
                        SELECT CustomerID, FullName, Contact, Address, LicenseNo, CreatedAt
                        FROM Customers
                        WHERE (@search = '' OR FullName LIKE @search OR Contact LIKE @search OR LicenseNo LIKE @search)
                        ORDER BY CustomerID DESC";

                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    da.SelectCommand.Parameters.AddWithValue("@search", $"%{searchTerm}%");

                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvCustomers.DataSource = dt;
                }

                addButtons();
                styleButtons();

                this.BeginInvoke((Action)(() =>
                {
                    dgvCustomers.ClearSelection();
                    dgvCustomers.CurrentCell = null;
                }));

                dgvCustomers.GotFocus += (s, e) => dgvCustomers.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading customers:\n" + ex.Message,
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // setup
        private void configureGrid()
        {
            dgvCustomers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCustomers.RowHeadersVisible = false;
            dgvCustomers.AllowUserToAddRows = false;
            dgvCustomers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCustomers.MultiSelect = false;
            dgvCustomers.BorderStyle = BorderStyle.None;
            dgvCustomers.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvCustomers.GridColor = Color.FromArgb(223, 245, 228);

            dgvCustomers.EnableHeadersVisualStyles = false;
            dgvCustomers.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(223, 245, 228);
            dgvCustomers.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(64, 64, 64);
            dgvCustomers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 11);
            dgvCustomers.DefaultCellStyle.Font = new Font("Segoe UI", 9);
        }

        // buttons
        private void addButtons()
        {
            if (dgvCustomers.Columns.Contains("Edit")) dgvCustomers.Columns.Remove("Edit");
            if (dgvCustomers.Columns.Contains("Delete")) dgvCustomers.Columns.Remove("Delete");

            DataGridViewButtonColumn editCol = new DataGridViewButtonColumn
            {
                Name = "Edit",
                HeaderText = "Actions",
                Text = "Edit",
                UseColumnTextForButtonValue = true,
                FlatStyle = FlatStyle.Flat,
                Width = 70,
            };
            dgvCustomers.Columns.Add(editCol);

            DataGridViewButtonColumn deleteCol = new DataGridViewButtonColumn
            {
                Name = "Delete",
                HeaderText = "",
                Text = "Delete",
                UseColumnTextForButtonValue = true,
                FlatStyle = FlatStyle.Flat,
                Width = 70
            };
            dgvCustomers.Columns.Add(deleteCol);

            dgvCustomers.Columns["Edit"].DisplayIndex = dgvCustomers.Columns.Count - 2;
            dgvCustomers.Columns["Delete"].DisplayIndex = dgvCustomers.Columns.Count - 1;
        }

        // style
        private void styleButtons()
        {
            dgvCustomers.CellPainting += (s, e) =>
            {
                if (e.RowIndex >= 0 &&
                    (e.ColumnIndex == dgvCustomers.Columns["Edit"].Index ||
                     e.ColumnIndex == dgvCustomers.Columns["Delete"].Index))
                {
                    e.PaintBackground(e.CellBounds, true);

                    string text = e.Value?.ToString();
                    var color = e.ColumnIndex == dgvCustomers.Columns["Edit"].Index
                        ? Color.FromArgb(39, 174, 96)   // green
                        : Color.FromArgb(192, 0, 0);    // red

                    TextRenderer.DrawText(e.Graphics, text, new Font("Segoe UI", 10, FontStyle.Bold),
                        e.CellBounds, color, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                    e.Handled = true;
                }
            };

            dgvCustomers.DefaultCellStyle.SelectionBackColor = Color.White;
            dgvCustomers.DefaultCellStyle.SelectionForeColor = Color.Black;
        }

        // search
        private void btnSearch_Click(object sender, EventArgs e)
        {
            string term = txtSearch.Text.Trim();
            loadCustomers(term);
        }

        // add
        private void btnAddCustomer_Click(object sender, EventArgs e)
        {
            using (AddEditCustomerForm addForm = new AddEditCustomerForm())
            {
                addForm.FormClosed += (s, args) => loadCustomers();
                addForm.ShowDialog();
            }
        }

        // click
        private void dgvCustomers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string columnName = dgvCustomers.Columns[e.ColumnIndex].Name;
            int customerId = Convert.ToInt32(dgvCustomers.Rows[e.RowIndex].Cells["CustomerID"].Value);

            if (columnName == "Edit")
            {
                using (AddEditCustomerForm editForm = new AddEditCustomerForm(customerId))
                {
                    editForm.FormClosed += (s, args) => loadCustomers();
                    editForm.ShowDialog();
                }
            }
            else if (columnName == "Delete")
            {
                deleteCustomer(customerId);
            }

            this.BeginInvoke((Action)(() =>
            {
                dgvCustomers.ClearSelection();
                dgvCustomers.CurrentCell = null;
            }));
        }

        // delete
        private void deleteCustomer(int customerId)
        {
            DialogResult confirm = MessageBox.Show(
                "Are you sure you want to delete this customer?",
                "Confirm Deletion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (confirm != DialogResult.Yes) return;

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    // check if customer has active rentals
                    SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM Rentals WHERE CustomerID = @id", con);
                    checkCmd.Parameters.AddWithValue("@id", customerId);
                    int rentalCount = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (rentalCount > 0)
                    {
                        MessageBox.Show("This customer cannot be deleted because they have rental records.",
                            "Action Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    SqlCommand cmd = new SqlCommand("DELETE FROM Customers WHERE CustomerID = @id", con);
                    cmd.Parameters.AddWithValue("@id", customerId);
                    cmd.ExecuteNonQuery();
                }

                loadCustomers();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting customer:\n" + ex.Message,
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}