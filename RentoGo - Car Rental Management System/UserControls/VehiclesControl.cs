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
            @"Server=LAPTOP-8NVU0AIF\SQLEXPRESS;Database=RentoGoDB;Trusted_Connection=True;";

        public VehiclesControl()
        {
            InitializeComponent();
        }

        private void VehiclesControl_Load(object sender, EventArgs e)
        {
            configureGrid();
            loadVehicles();
        }

        // load
        private void loadVehicles(string searchTerm = "")
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = @"
                        SELECT VehicleID, Model, Type, PlateNo, RatePerDay, Status
                        FROM Vehicles
                        WHERE (@search = '' OR Model LIKE @search OR PlateNo LIKE @search)
                        ORDER BY VehicleID DESC";

                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    da.SelectCommand.Parameters.AddWithValue("@search", $"%{searchTerm}%");

                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvVehicles.DataSource = dt;
                }

                addButtons();
                styleButtons();
                dgvVehicles.ClearSelection();
                dgvVehicles.CurrentCell = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("⚠ unable to load vehicles:\n" + ex.Message,
                    "database error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // setup
        private void configureGrid()
        {
            dgvVehicles.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvVehicles.RowHeadersVisible = false;
            dgvVehicles.AllowUserToAddRows = false;
            dgvVehicles.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvVehicles.DefaultCellStyle.SelectionBackColor = Color.White;
            dgvVehicles.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvVehicles.BorderStyle = BorderStyle.None;
            dgvVehicles.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvVehicles.GridColor = Color.FromArgb(223, 245, 228);
            dgvVehicles.MultiSelect = false;

            dgvVehicles.EnableHeadersVisualStyles = false;
            dgvVehicles.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(46, 204, 113);
            dgvVehicles.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvVehicles.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 10);
            dgvVehicles.DefaultCellStyle.Font = new Font("Segoe UI", 9);
        }

        // buttons
        private void addButtons()
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
                Width = 80
            };
            dgvVehicles.Columns.Add(editCol);

            DataGridViewButtonColumn deleteCol = new DataGridViewButtonColumn
            {
                Name = "Delete",
                HeaderText = "",
                Text = "Delete",
                UseColumnTextForButtonValue = true,
                FlatStyle = FlatStyle.Flat,
                Width = 80
            };
            dgvVehicles.Columns.Add(deleteCol);

            dgvVehicles.Columns["Edit"].DisplayIndex = dgvVehicles.Columns.Count - 2;
            dgvVehicles.Columns["Delete"].DisplayIndex = dgvVehicles.Columns.Count - 1;
        }

        // style
        private void styleButtons()
        {
            dgvVehicles.Columns["Edit"].DefaultCellStyle.ForeColor = Color.FromArgb(39, 174, 96);
            dgvVehicles.Columns["Edit"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvVehicles.Columns["Edit"].DefaultCellStyle.SelectionBackColor = Color.White;

            dgvVehicles.Columns["Delete"].DefaultCellStyle.ForeColor = Color.FromArgb(192, 0, 0);
            dgvVehicles.Columns["Delete"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvVehicles.Columns["Delete"].DefaultCellStyle.SelectionBackColor = Color.White;
        }

        // search
        private void btnSearch_Click(object sender, EventArgs e)
        {
            string term = txtSearch.Text.Trim();
            loadVehicles(term);
        }

        // add
        private void btnAddVehicle_Click(object sender, EventArgs e)
        {
            using (AddEditVehicleForm addForm = new AddEditVehicleForm())
            {
                addForm.FormClosed += (s, args) => loadVehicles();
                addForm.ShowDialog();
            }
        }

        // click
        private void dgvVehicles_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string columnName = dgvVehicles.Columns[e.ColumnIndex].Name;
            int vehicleId = Convert.ToInt32(dgvVehicles.Rows[e.RowIndex].Cells["VehicleID"].Value);

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

            dgvVehicles.ClearSelection();
            dgvVehicles.CurrentCell = null;
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
            catch (Exception ex)
            {
                MessageBox.Show("⚠ error deleting vehicle:\n" + ex.Message,
                    "database error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
