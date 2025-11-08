using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace RentoGo___Car_Rental_Management_System.Forms
{
    public partial class AddEditVehicleForm : Form
    {
        private int vehicleId = 0;
        private readonly string connectionString =
            @"Server=LAPTOP-8NVU0AIF\SQLEXPRESS;Database=RentoGoDB;Trusted_Connection=True;";

        public AddEditVehicleForm()
        {
            InitializeComponent();
        }

        public AddEditVehicleForm(int id)
        {
            InitializeComponent();
            vehicleId = id;
        }

        private void AddEditVehicleForm_Load(object sender, EventArgs e)
        {
            cbStatus.Items.Clear();
            cbStatus.Items.AddRange(new string[]
            {
                "Available",
                "Reserved",
                "Rented",
                "Maintenance"
            });

            if (vehicleId > 0)
            {
                Text = "edit vehicle";
                btnSave.Text = "save changes";
                loadVehicle();
            }
            else
            {
                Text = "add vehicle";
                btnSave.Text = "add vehicle";
            }
        }

        private void loadVehicle()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "SELECT Model, Type, PlateNo, RatePerDay, Status FROM Vehicles WHERE VehicleID = @id";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@id", vehicleId);

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        txtModel.Text = reader["Model"].ToString();
                        txtType.Text = reader["Type"].ToString();
                        txtPlateNo.Text = reader["PlateNo"].ToString();
                        txtRate.Text = reader["RatePerDay"].ToString();
                        cbStatus.SelectedItem = reader["Status"].ToString();
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("error loading vehicle:\n" + ex.Message,
                    "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // validate
        private bool validateFields()
        {
            string error = "";

            if (string.IsNullOrWhiteSpace(txtModel.Text))
                error += "model is required.\n";

            if (string.IsNullOrWhiteSpace(txtType.Text))
                error += "type is required.\n";

            if (string.IsNullOrWhiteSpace(txtPlateNo.Text))
                error += "plate number is required.\n";

            if (string.IsNullOrWhiteSpace(txtRate.Text))
                error += "rate per day is required.\n";
            else if (!decimal.TryParse(txtRate.Text, out _))
                error += "rate per day must be a valid number.\n";

            if (cbStatus.SelectedItem == null)
                error += "please select a vehicle status.\n";

            if (!string.IsNullOrEmpty(error))
            {
                MessageBox.Show("please fix the following:\n\n" + error,
                    "validation error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        // save
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!validateFields())
                return;

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlCommand cmd;

                    if (vehicleId == 0)
                    {
                        // add
                        cmd = new SqlCommand(@"
                            INSERT INTO Vehicles (Model, Type, PlateNo, RatePerDay, Status)
                            VALUES (@model, @type, @plate, @rate, @status)", con);
                    }
                    else
                    {
                        // update
                        cmd = new SqlCommand(@"
                            UPDATE Vehicles 
                            SET Model = @model, Type = @type, PlateNo = @plate,
                                RatePerDay = @rate, Status = @status
                            WHERE VehicleID = @id", con);
                        cmd.Parameters.AddWithValue("@id", vehicleId);
                    }

                    cmd.Parameters.AddWithValue("@model", txtModel.Text.Trim());
                    cmd.Parameters.AddWithValue("@type", txtType.Text.Trim());
                    cmd.Parameters.AddWithValue("@plate", txtPlateNo.Text.Trim());
                    cmd.Parameters.AddWithValue("@rate", decimal.Parse(txtRate.Text.Trim()));
                    cmd.Parameters.AddWithValue("@status", cbStatus.SelectedItem.ToString());
                    cmd.ExecuteNonQuery();

                    MessageBox.Show(vehicleId == 0
                        ? "vehicle added successfully."
                        : "vehicle updated successfully.",
                        "success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("error saving vehicle:\n" + ex.Message,
                    "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
