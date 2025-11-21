using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace RentoGo___Car_Rental_Management_System.Forms
{
    public partial class AddEditVehicleForm : Form
    {
        private int vehicleId = 0;
        private readonly string connectionString =
            @"Server=(localdb)\MSSQLLocalDB;Database=RentoGoDB;Trusted_Connection=True;";

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
                lblTitle.Text = "Edit Vehicle";
                btnSave.Text = "Save";
                loadVehicle();
            }
            else
            {
                lblTitle.Text = "Add Vehicle";
                btnSave.Text = "Add";
            }
        }

        // load
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
                MessageBox.Show("Error loading vehicle details:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // validate
        private bool validateFields()
        {
            string error = "";

            if (string.IsNullOrWhiteSpace(txtModel.Text))
                error += "Model is required.\n";

            if (string.IsNullOrWhiteSpace(txtType.Text))
                error += "Type is required.\n";

            // Plate number required + format check
            if (string.IsNullOrWhiteSpace(txtPlateNo.Text))
            {
                error += "Plate number is required.\n";
            }
            else
            {
                string plate = txtPlateNo.Text.Trim().ToUpper();

                // Example format: ABC-123
                string platePattern = @"^[A-Z]{3}-\d{3}$";

                if (!Regex.IsMatch(plate, platePattern))
                {
                    error += "Plate number must be in the format ABC-123 (3 letters, a dash, and 3 digits).\n";
                }
                else
                {
                    // Normalize text box value (uppercased, trimmed, valid)
                    txtPlateNo.Text = plate;
                }
            }

            if (string.IsNullOrWhiteSpace(txtRate.Text))
                error += "Rate per day is required.\n";
            else if (!decimal.TryParse(txtRate.Text, out decimal rate))
                error += "Rate per day must be a valid number.\n";
            else if (rate <= 0)
                error += "Rate per day must be greater than zero.\n";
            else if (rate < 200 || rate > 20000)
                error += "Rate per day seems unrealistic. Please double-check.\n";

            if (cbStatus.SelectedItem == null)
                error += "Please select a vehicle status.\n";

            if (!string.IsNullOrEmpty(error))
            {
                MessageBox.Show("Please fix the following:\n\n" + error,
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

                    // check for duplicate plate numbers
                    SqlCommand checkCmd = new SqlCommand(
                        "SELECT COUNT(*) FROM Vehicles WHERE PlateNo = @plate AND VehicleID != @id", con);
                    checkCmd.Parameters.AddWithValue("@plate", txtPlateNo.Text.Trim());
                    checkCmd.Parameters.AddWithValue("@id", vehicleId);
                    int exists = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (exists > 0)
                    {
                        MessageBox.Show("A vehicle with this plate number already exists.",
                            "Duplicate Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

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
                        ? "Vehicle added successfully."
                        : "Vehicle updated successfully.",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.Close();
                    AppEvents.RaiseVehiclesUpdated();
                }
            }
            catch (SqlException ex)
            {
                if (ex.Message.Contains("PRIMARY KEY") || ex.Message.Contains("UNIQUE"))
                {
                    MessageBox.Show("A vehicle with this plate number already exists.",
                        "Duplicate Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show("Database connection error:\n" + ex.Message,
                        "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected error:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // cancel
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
