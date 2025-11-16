using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using RentoGo___Car_Rental_Management_System.Forms;

namespace RentoGo___Car_Rental_Management_System.UserControls
{
    public partial class RentalsControl : UserControl
    {
        private readonly string connectionString =
            @"Server=(localdb)\MSSQLLocalDB;Database=RentoGoDB;Trusted_Connection=True;";

        public RentalsControl()
        {
            InitializeComponent();
        }

        private void RentalsControl_Load(object sender, EventArgs e)
        {
            grid();
            summary();
            load();
            statusFilter();
        }

        // summary
        private void summary()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    lblActive.Text = $"Active: {count(con, "Active")}";
                    lblReserved.Text = $"Reserved: {count(con, "Reserved")}";
                    lblCompleted.Text = $"Completed: {count(con, "Completed")}";
                    //lblRevenue.Text = "₱" + revenue(con).ToString("N2");
                }
            }
            catch { }
        }

        private int count(SqlConnection con, string status)
        {
            SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Rentals WHERE Status=@status", con);
            cmd.Parameters.AddWithValue("@status", status);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        private decimal revenue(SqlConnection con)
        {
            SqlCommand cmd = new SqlCommand("SELECT ISNULL(SUM(TotalCharge),0) FROM Rentals WHERE Status='Completed'", con);
            return Convert.ToDecimal(cmd.ExecuteScalar());
        }

        // load
        private void load(string search = "", string status = "")
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
                            r.TotalCharge,
                            r.Status
                        FROM Rentals r
                        JOIN Customers c ON r.CustomerID = c.CustomerID
                        JOIN Vehicles v ON r.VehicleID = v.VehicleID
                        WHERE 
                            (@s='' OR c.FullName LIKE @s OR v.Model LIKE @s)
                            AND (@st='' OR r.Status=@st)
                        ORDER BY r.StartDate DESC";

                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    da.SelectCommand.Parameters.AddWithValue("@s", $"%{search}%");
                    da.SelectCommand.Parameters.AddWithValue("@st", status);

                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvRentals.DataSource = dt;
                }

                buttons();
                dgvRentals.ClearSelection();
                dgvRentals.CurrentCell = null;

                dgvRentals.CellPainting -= paint;
                dgvRentals.CellPainting += paint;

                dgvRentals.CellFormatting -= statusColor;
                dgvRentals.CellFormatting += statusColor;
            }
            catch { }
        }

        // grid
        private void grid()
        {
            dgvRentals.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvRentals.RowHeadersVisible = false;
            dgvRentals.AllowUserToAddRows = false;
            dgvRentals.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRentals.MultiSelect = false;
            dgvRentals.BorderStyle = BorderStyle.None;
            dgvRentals.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvRentals.GridColor = Color.FromArgb(223, 245, 228);

            dgvRentals.EnableHeadersVisualStyles = false;
            dgvRentals.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(223, 245, 228);
            dgvRentals.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(64, 64, 64);
            dgvRentals.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 11);
            dgvRentals.DefaultCellStyle.Font = new Font("Segoe UI", 9);
        }

        // buttons
        private void buttons()
        {
            if (dgvRentals.Columns.Contains("Edit")) dgvRentals.Columns.Remove("Edit");
            if (dgvRentals.Columns.Contains("Return")) dgvRentals.Columns.Remove("Return");
            if (dgvRentals.Columns.Contains("Delete")) dgvRentals.Columns.Remove("Delete");

            DataGridViewButtonColumn edit = new DataGridViewButtonColumn
            {
                Name = "Edit",
                HeaderText = "Actions",
                Text = "Edit",
                UseColumnTextForButtonValue = true,
                FlatStyle = FlatStyle.Flat,
                Width = 70
            };
            dgvRentals.Columns.Add(edit);

            DataGridViewButtonColumn ret = new DataGridViewButtonColumn
            {
                Name = "Return",
                HeaderText = "",
                Text = "Return",
                UseColumnTextForButtonValue = true,
                FlatStyle = FlatStyle.Flat,
                Width = 80
            };
            dgvRentals.Columns.Add(ret);

            DataGridViewButtonColumn del = new DataGridViewButtonColumn
            {
                Name = "Delete",
                HeaderText = "",
                Text = "Delete",
                UseColumnTextForButtonValue = true,
                FlatStyle = FlatStyle.Flat,
                Width = 70
            };
            dgvRentals.Columns.Add(del);

            dgvRentals.Columns["Edit"].DisplayIndex = dgvRentals.Columns.Count - 3;
            dgvRentals.Columns["Return"].DisplayIndex = dgvRentals.Columns.Count - 2;
            dgvRentals.Columns["Delete"].DisplayIndex = dgvRentals.Columns.Count - 1;
        }

        // paint
        private void paint(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            int edit = dgvRentals.Columns["Edit"].Index;
            int ret = dgvRentals.Columns["Return"].Index;
            int del = dgvRentals.Columns["Delete"].Index;

            if (e.ColumnIndex != edit && e.ColumnIndex != ret && e.ColumnIndex != del) return;

            string text = e.Value?.ToString() ?? "";
            Color color = Color.Black;

            if (e.ColumnIndex == edit) color = Color.FromArgb(39, 174, 96);
            if (e.ColumnIndex == ret) color = Color.FromArgb(241, 196, 15);
            if (e.ColumnIndex == del) color = Color.FromArgb(192, 0, 0);

            e.PaintBackground(e.CellBounds, true);

            TextRenderer.DrawText(
                e.Graphics,
                text,
                new Font("Segoe UI", 9, FontStyle.Bold),  
                e.CellBounds,
                color,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
            );

            e.Handled = true;
        }

        // status
        private void statusColor(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvRentals.Columns[e.ColumnIndex].Name != "Status") return;
            if (e.Value == null) return;

            string s = e.Value.ToString();

            if (s == "Active") e.CellStyle.ForeColor = Color.FromArgb(30, 132, 73);
            else if (s == "Reserved") e.CellStyle.ForeColor = Color.FromArgb(185, 119, 14);
            else if (s == "Completed") e.CellStyle.ForeColor = Color.FromArgb(97, 106, 107);
            else if (s == "Cancelled") e.CellStyle.ForeColor = Color.FromArgb(146, 43, 33);
        }

        // filter
        private void statusFilter()
        {
            cbStatusFilter.Items.Clear();
            cbStatusFilter.Items.AddRange(new string[]
            {
                "All","Active","Reserved","Completed","Cancelled"
            });
            cbStatusFilter.SelectedIndex = 0;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string term = txtSearch.Text.Trim();
            string s = cbStatusFilter.SelectedItem.ToString();
            string status = s == "All" ? "" : s;
            load(term, status);
        }

        private void cbStatusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnSearch.PerformClick();
        }

        // click
        private void dgvRentals_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string col = dgvRentals.Columns[e.ColumnIndex].Name;
            int id = Convert.ToInt32(dgvRentals.Rows[e.RowIndex].Cells["RentalID"].Value);

            if (col == "Edit")
            {
                using (AddEditRentalForm f = new AddEditRentalForm(id))
                {
                    f.FormClosed += (s, a) => { summary(); load(); };
                    f.ShowDialog();
                }
            }
            else if (col == "Return")
            {
                using (ReturnForm r = new ReturnForm(id))
                {
                    r.FormClosed += (s, a) => { summary(); load(); };
                    r.ShowDialog();
                }
            }
            else if (col == "Delete")
            {
                remove(id);
            }

            dgvRentals.ClearSelection();
            dgvRentals.CurrentCell = null;
        }

        // delete
        private void remove(int id)
        {
            DialogResult c = MessageBox.Show("Delete this rental?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (c != DialogResult.Yes) return;

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    SqlCommand chk = new SqlCommand("SELECT Status FROM Rentals WHERE RentalID=@id", con);
                    chk.Parameters.AddWithValue("@id", id);
                    string s = chk.ExecuteScalar()?.ToString();

                    if (s == "Active" || s == "Completed")
                    {
                        MessageBox.Show("You cannot delete active or completed rentals.");
                        return;
                    }

                    SqlCommand cmd = new SqlCommand("DELETE FROM Rentals WHERE RentalID=@id", con);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }

                summary();
                load();
            }
            catch { }
        }
    }
}
