using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using RentoGo___Car_Rental_Management_System.Forms;

namespace RentoGo___Car_Rental_Management_System.UserControls
{
    public partial class VehiclesControl : UserControl
    {
        private readonly string connectionString =
            @"Server=(localdb)\MSSQLLocalDB;Database=RentoGoDB;Trusted_Connection=True;";

        public VehiclesControl()
        {
            InitializeComponent();
        }

        private void VehiclesControl_Load(object sender, EventArgs e)
        {
            try
            {
                configureGrid();
                loadVehicles();
                cbStatusFilter.Items.AddRange(new string[] { "All", "Available", "Reserved", "Rented" });
                cbStatusFilter.SelectedIndex = 0;
                AppEvents.VehiclesUpdated += () => loadVehicles();
            }
            catch (Exception ex)
            {
                MessageBox.Show("failed to load vehicle control:\n" + ex.Message,
                    "system error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // load vehicles
        private void loadVehicles(string searchTerm = "", string statusFilter = "")
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con == null)
                        throw new InvalidOperationException("database connection could not be created.");

                    con.Open();

                    string query = @"
                        SELECT VehicleID, Model, Type, PlateNo, RatePerDay, Status
                        FROM Vehicles
                        WHERE 
                            (@search = '' OR Model LIKE @search OR PlateNo LIKE @search)
                            AND (@status = '' OR Status = @status)
                        ORDER BY VehicleID DESC";

                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    da.SelectCommand.Parameters.AddWithValue("@search", $"%{searchTerm}%");
                    da.SelectCommand.Parameters.AddWithValue("@status", statusFilter ?? "");

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgvVehicles.DataSource = dt;

                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("No vehicle records found for the given filters.",
                            "no results", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                addButtons();
                styleButtons();

                this.BeginInvoke((Action)(() =>
                {
                    dgvVehicles.ClearSelection();
                    dgvVehicles.CurrentCell = null;
                }));

                dgvVehicles.GotFocus += (s, e) => dgvVehicles.ClearSelection();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("database operation failed:\n" + ex.Message,
                    "database error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show("invalid operation:\n" + ex.Message,
                    "logic error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("unexpected error:\n" + ex.Message,
                    "system error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // setup grid
        private void configureGrid()
        {
            dgvVehicles.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvVehicles.RowHeadersVisible = false;
            dgvVehicles.AllowUserToAddRows = false;
            dgvVehicles.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvVehicles.MultiSelect = false;
            dgvVehicles.BorderStyle = BorderStyle.None;
            dgvVehicles.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvVehicles.GridColor = Color.FromArgb(223, 245, 228);

            dgvVehicles.EnableHeadersVisualStyles = false;
            dgvVehicles.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(223, 245, 228);
            dgvVehicles.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(64, 64, 64);
            dgvVehicles.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 11);
        }

        // actions buttton
        private void addButtons()
        {
            try
            {
                if (dgvVehicles.Columns.Contains("Edit")) dgvVehicles.Columns.Remove("Edit");
                if (dgvVehicles.Columns.Contains("Delete")) dgvVehicles.Columns.Remove("Delete");

                DataGridViewButtonColumn editCol = new DataGridViewButtonColumn
                {
                    Name = "Edit",
                    HeaderText = "Actions",
                    Text = "Edit",
                    UseColumnTextForButtonValue = true,
                    FlatStyle = FlatStyle.Flat,
                    Width = 70,
                };
                dgvVehicles.Columns.Add(editCol);

                DataGridViewButtonColumn deleteCol = new DataGridViewButtonColumn
                {
                    Name = "Delete",
                    HeaderText = "",
                    Text = "Delete",
                    UseColumnTextForButtonValue = true,
                    FlatStyle = FlatStyle.Flat,
                    Width = 70
                };
                dgvVehicles.Columns.Add(deleteCol);

                dgvVehicles.Columns["Edit"].DisplayIndex = dgvVehicles.Columns.Count - 2;
                dgvVehicles.Columns["Delete"].DisplayIndex = dgvVehicles.Columns.Count - 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("failed to add action buttons:\n" + ex.Message,
                    "ui setup error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // style
        private void styleButtons()
        {
            dgvVehicles.CellPainting += (s, e) =>
            {
                if (e.RowIndex >= 0 &&
                    (e.ColumnIndex == dgvVehicles.Columns["Edit"].Index ||
                     e.ColumnIndex == dgvVehicles.Columns["Delete"].Index))
                {
                    e.PaintBackground(e.CellBounds, true);

                    string text = e.Value?.ToString();
                    var color = e.ColumnIndex == dgvVehicles.Columns["Edit"].Index
                        ? Color.FromArgb(39, 174, 96)
                        : Color.FromArgb(192, 0, 0);

                    TextRenderer.DrawText(e.Graphics, text, new Font("Segoe UI", 10, FontStyle.Bold),
                        e.CellBounds, color, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                    e.Handled = true;
                }
            };

            dgvVehicles.DefaultCellStyle.SelectionBackColor = Color.White;
            dgvVehicles.DefaultCellStyle.SelectionForeColor = Color.Black;
        }

        // filter
        private void cbStatusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string search = txtSearch.Text.Trim();
                string selected = cbStatusFilter.SelectedItem?.ToString() ?? "All";
                string status = selected == "All" ? "" : selected;
                loadVehicles(search, status);
            }
            catch (Exception ex)
            {
                MessageBox.Show("failed to apply filter:\n" + ex.Message,
                    "filter error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // search
        private void btnSearch_Click(object sender, EventArgs e)
        {
            string term = txtSearch.Text.Trim();
            string selected = cbStatusFilter.SelectedItem.ToString();
            string status = selected == "All" ? "" : selected;
            loadVehicles(term, status);
        }

        // add
        private void btnAddVehicle_Click(object sender, EventArgs e)
        {
            try
            {
                using (AddEditVehicleForm addForm = new AddEditVehicleForm())
                {
                    addForm.FormClosed += (s, args) => loadVehicles();
                    addForm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("error opening add vehicle form:\n" + ex.Message,
                    "ui error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // click
        private void dgvVehicles_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0) return;

                string columnName = dgvVehicles.Columns[e.ColumnIndex].Name;
                if (!int.TryParse(dgvVehicles.Rows[e.RowIndex].Cells["VehicleID"].Value?.ToString(), out int vehicleId))
                {
                    MessageBox.Show("invalid vehicle selection. Please try again.",
                        "data error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (columnName == "Edit")
                {
                    using (AddEditVehicleForm editForm = new AddEditVehicleForm(vehicleId))
                    {
                        editForm.FormClosed += (s, args) => loadVehicles();
                        editForm.ShowDialog();
                    }
                }
                else if (columnName == "Delete")
                {
                    deleteVehicle(vehicleId);
                }

                this.BeginInvoke((Action)(() =>
                {
                    dgvVehicles.ClearSelection();
                    dgvVehicles.CurrentCell = null;
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show("an error occurred while processing your action:\n" + ex.Message,
                    "action error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // delete
        private void deleteVehicle(int vehicleId)
        {
            DialogResult confirm = MessageBox.Show(
                "Are you sure you want to delete this vehicle?",
                "confirm deletion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (confirm != DialogResult.Yes) return;

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("DELETE FROM Vehicles WHERE VehicleID = @id", con);
                    cmd.Parameters.AddWithValue("@id", vehicleId);
                    cmd.ExecuteNonQuery();
                }

                loadVehicles();
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                MessageBox.Show("This vehicle cannot be deleted because it is linked to rental records.",
                    "delete blocked", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (SqlException ex)
            {
                MessageBox.Show("SQL error during deletion:\n" + ex.Message,
                    "database error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("unexpected error during deletion:\n" + ex.Message,
                    "system error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
