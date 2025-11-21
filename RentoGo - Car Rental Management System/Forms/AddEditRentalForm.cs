using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace RentoGo___Car_Rental_Management_System.Forms
{
    public partial class AddEditRentalForm : Form
    {
        private readonly string connectionString =
            @"Server=(localdb)\MSSQLLocalDB;Database=RentoGoDB;Trusted_Connection=True;";

        private int rentalId = 0;

        public AddEditRentalForm()
        {
            InitializeComponent();
        }

        public AddEditRentalForm(int id)
        {
            InitializeComponent();
            rentalId = id;
        }

        private void AddEditRentalForm_Load(object sender, EventArgs e)
        {
            cbVehicle.DropDownStyle = ComboBoxStyle.DropDown;
            cbCustomer.DropDownStyle = ComboBoxStyle.DropDown;

            cbStatus.Items.Clear();
            if (rentalId == 0)
            {
                cbStatus.Items.AddRange(new string[] { "Reserved", "Active" });
                cbStatus.Enabled = true;
                cbStatus.SelectedIndex = 1;
            }
            else
            {
                cbStatus.Items.AddRange(new string[] { "Reserved", "Active", "Completed", "Cancelled" });
                cbStatus.Enabled = true;
            }

            loadAllVehicles(); // load all vehicles except maintenance
            loadAvailableCustomers();

            if (rentalId > 0)
            {
                lblTitle.Text = "Edit Rental";
                btnSave.Text = "Save";
                cbStatus.Enabled = true;
                loadRental();
            }
            else
            {
                lblTitle.Text = "Add Rental";
                btnSave.Text = "Add";
            }

            // events
            dtStart.ValueChanged += (s, a) => computeTotalCharge();
            dtEnd.ValueChanged += (s, a) => computeTotalCharge();
            cbVehicle.TextChanged += (s, a) => updateVehicleID();
            cbVehicle.SelectedIndexChanged += (s, a) => updateVehicleID();
            cbCustomer.TextChanged += (s, a) => updateCustomerID();
            cbCustomer.SelectedIndexChanged += (s, a) => updateCustomerID();
        }

        // load all vehicles except those in maintenance
        private void loadAllVehicles()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "SELECT VehicleID, Model FROM Vehicles WHERE Status != 'Maintenance' ORDER BY VehicleID"; // exclude maintenance
                    SqlCommand cmd = new SqlCommand(query, con);
                    SqlDataReader reader = cmd.ExecuteReader();

                    cbVehicle.Items.Clear();
                    while (reader.Read())
                    {
                        int id = Convert.ToInt32(reader["VehicleID"]);
                        string model = reader["Model"].ToString();
                        cbVehicle.Items.Add($"{id} - {model}"); // format: "id - model"
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading vehicles:\n" + ex.Message);
            }
        }

        // load available customers (no active rentals)
        private void loadAvailableCustomers()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = @"
                        SELECT c.CustomerID, c.FullName
                        FROM Customers c
                        WHERE c.CustomerID NOT IN (
                            SELECT r.CustomerID FROM Rentals r WHERE r.Status IN ('Reserved', 'Active')
                        )
                        ORDER BY c.CustomerID";
                    SqlCommand cmd = new SqlCommand(query, con);
                    SqlDataReader reader = cmd.ExecuteReader();

                    cbCustomer.Items.Clear();
                    while (reader.Read())
                    {
                        int id = Convert.ToInt32(reader["CustomerID"]);
                        string name = reader["FullName"].ToString();
                        cbCustomer.Items.Add($"{id} - {name}"); // format: "id - fullname"
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading available customers:\n" + ex.Message);
            }
        }

        // update txtVehicleID when cbVehicle changes
        private void updateVehicleID()
        {
            if (int.TryParse(cbVehicle.Text.Split('-')[0].Trim(), out int id))
            {
                txtVehicleID.Text = id.ToString();
                loadVehicle(); // update rate for calculation
            }
            else
            {
                txtVehicleID.Clear();
                txtRate.Clear();
            }
        }

        // update txtCustomerID when cbCustomer changes
        private void updateCustomerID()
        {
            if (int.TryParse(cbCustomer.Text.Split('-')[0].Trim(), out int id))
            {
                txtCustomerID.Text = id.ToString();
                // no need to load customer name since ComboBox shows it
            }
            else
            {
                txtCustomerID.Clear();
            }
        }

        private void loadRental()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string q = @"
                        SELECT CustomerID, VehicleID, StartDate, EndDate, TotalCharge, Status
                        FROM Rentals WHERE RentalID = @id";

                    SqlCommand cmd = new SqlCommand(q, con);
                    cmd.Parameters.AddWithValue("@id", rentalId);

                    SqlDataReader r = cmd.ExecuteReader();
                    if (r.Read())
                    {
                        txtCustomerID.Text = r["CustomerID"].ToString();
                        txtVehicleID.Text = r["VehicleID"].ToString();
                        // set ComboBox text to match the format (if available)
                        cbCustomer.Text = $"{r["CustomerID"]} - {getCustomerName(Convert.ToInt32(r["CustomerID"]))}";
                        cbVehicle.Text = $"{r["VehicleID"]} - {getVehicleModel(Convert.ToInt32(r["VehicleID"]))}";
                        dtStart.Value = Convert.ToDateTime(r["StartDate"]);
                        dtEnd.Value = Convert.ToDateTime(r["EndDate"]);
                        txtTotal.Text = r["TotalCharge"].ToString();
                        cbStatus.SelectedItem = r["Status"].ToString();
                    }
                    r.Close();
                }

                loadVehicle(); // load rate for calculation
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading rental:\n" + ex.Message);
            }
        }

        // helper to get customer name (for editing)
        private string getCustomerName(int id)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT FullName FROM Customers WHERE CustomerID = @id", con);
                    cmd.Parameters.AddWithValue("@id", id);
                    return cmd.ExecuteScalar()?.ToString() ?? "";
                }
            }
            catch
            {
                return "";
            }
        }

        // helper to get vehicle model (for editing)
        private string getVehicleModel(int id)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT Model FROM Vehicles WHERE VehicleID = @id", con);
                    cmd.Parameters.AddWithValue("@id", id);
                    return cmd.ExecuteScalar()?.ToString() ?? "";
                }
            }
            catch
            {
                return "";
            }
        }

        private void loadVehicle()
        {
            if (!int.TryParse(txtVehicleID.Text, out int id))
            {
                txtRate.Clear();
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT RatePerDay FROM Vehicles WHERE VehicleID = @id", con);
                    cmd.Parameters.AddWithValue("@id", id);
                    object result = cmd.ExecuteScalar();
                    txtRate.Text = result?.ToString() ?? "";
                }
            }
            catch { }

            computeTotalCharge();
        }

        private void computeTotalCharge()
        {
            if (!decimal.TryParse(txtRate.Text, out decimal rate))
            {
                txtTotal.Text = "";
                return;
            }

            int days = (dtEnd.Value.Date - dtStart.Value.Date).Days;
            if (days < 1) days = 1; // same-day rentals charge for 1 day

            txtTotal.Text = (rate * days).ToString("0.00");
        }

        private bool validateFields()
        {
            string err = "";

            if (!int.TryParse(txtCustomerID.Text, out _))
                err += "Customer ID is invalid.\n";

            if (!int.TryParse(txtVehicleID.Text, out _))
                err += "Vehicle ID is invalid.\n";

            // check if vehicle is in maintenance
            if (isVehicleInMaintenance())
                err += "This vehicle is currently under maintenance and cannot be rented.\n";

            if (!decimal.TryParse(txtRate.Text, out _))
                err += "Rate is invalid.\n";

            if (!decimal.TryParse(txtTotal.Text, out _))
                err += "Total charge is invalid.\n";

            // real-world date validations
            if (dtStart.Value.Date < DateTime.Today)
                err += "Start date cannot be in the past.\n";

            if (dtEnd.Value.Date < dtStart.Value.Date) // allow same-day (end == start)
                err += "End date must be on or after the start date.\n";

            if (rentalId > 0 && cbStatus.SelectedItem == null)
                err += "Please select a status.\n";

            // check for customer uniqueness (no multiple active rentals)
            if (isCustomerAlreadyRenting())
                err += "This customer already has an active rental.\n";

            if (err != "")
            {
                MessageBox.Show("Please fix the following:\n\n" + err,
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        // check if the vehicle is in maintenance
        private bool isVehicleInMaintenance()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT Status FROM Vehicles WHERE VehicleID = @id", con);
                    cmd.Parameters.AddWithValue("@id", int.Parse(txtVehicleID.Text));
                    string status = cmd.ExecuteScalar()?.ToString();
                    return status == "Maintenance";
                }
            }
            catch
            {
                return false; // allow if check fails
            }
        }

        // check if the vehicle is available during the selected dates
        private bool isVehicleAvailable()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    string query = @"
                        SELECT COUNT(*) FROM Rentals
                        WHERE VehicleID = @vid
                          AND Status IN ('Reserved', 'Active')
                          AND (
                              (StartDate <= @end AND EndDate >= @start)
                              OR (StartDate <= @start AND EndDate >= @end)
                              OR (StartDate >= @start AND EndDate <= @end)
                          )";

                    // exclude current rental if editing
                    if (rentalId > 0)
                        query += " AND RentalID != @rid";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@vid", int.Parse(txtVehicleID.Text));
                    cmd.Parameters.AddWithValue("@start", dtStart.Value.Date);
                    cmd.Parameters.AddWithValue("@end", dtEnd.Value.Date);
                    if (rentalId > 0)
                        cmd.Parameters.AddWithValue("@rid", rentalId);

                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    if (count > 0)
                    {
                        MessageBox.Show("This vehicle is already rented during the selected dates. Please choose different dates or another vehicle.",
                            "Vehicle Unavailable", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error checking vehicle availability:\n" + ex.Message);
                return false;
            }

            return true;
        }

        // check if the customer already has an active rental
        private bool isCustomerAlreadyRenting()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    string query = "SELECT COUNT(*) FROM Rentals WHERE CustomerID = @cid AND Status IN ('Reserved', 'Active')";

                    // exclude current rental if editing
                    if (rentalId > 0)
                        query += " AND RentalID != @rid";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@cid", int.Parse(txtCustomerID.Text));
                    if (rentalId > 0)
                        cmd.Parameters.AddWithValue("@rid", rentalId);

                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
            catch
            {
                return false; // allow save if check fails, but log if needed
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!validateFields()) return;
            // check vehicle availability before saving
            if (!isVehicleAvailable()) return;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlCommand cmd;
                    string selectedStatus = cbStatus.SelectedItem.ToString();
                    if (rentalId == 0)
                    {
                        cmd = new SqlCommand(@"
                            INSERT INTO Rentals (CustomerID, VehicleID, StartDate, EndDate, TotalCharge, Status)
                            VALUES (@c, @v, @s, @e, @t, @st)", con); // use selected status
                        cmd.Parameters.AddWithValue("@st", selectedStatus);
                    }
                    else
                    {
                        cmd = new SqlCommand(@"
                            UPDATE Rentals SET 
                                CustomerID=@c, VehicleID=@v, StartDate=@s, EndDate=@e, 
                                TotalCharge=@t, Status=@st
                            WHERE RentalID = @id", con);
                        cmd.Parameters.AddWithValue("@id", rentalId);
                        cmd.Parameters.AddWithValue("@st", selectedStatus);
                    }
                    cmd.Parameters.AddWithValue("@c", int.Parse(txtCustomerID.Text));
                    cmd.Parameters.AddWithValue("@v", int.Parse(txtVehicleID.Text));
                    cmd.Parameters.AddWithValue("@s", dtStart.Value);
                    cmd.Parameters.AddWithValue("@e", dtEnd.Value);
                    cmd.Parameters.AddWithValue("@t", decimal.Parse(txtTotal.Text));
                    cmd.ExecuteNonQuery();
                    // automatic update base sa rental status
                    string vehicleStatus;
                    if (selectedStatus == "Active")
                        vehicleStatus = "Rented";
                    else if (selectedStatus == "Reserved")
                        vehicleStatus = "Reserved";
                    else // complete or cancelled
                        vehicleStatus = "Available";
                    SqlCommand updateVehicle = new SqlCommand(
                        "UPDATE Vehicles SET Status = @status WHERE VehicleID = @vid", con);
                    updateVehicle.Parameters.AddWithValue("@status", vehicleStatus);
                    updateVehicle.Parameters.AddWithValue("@vid", int.Parse(txtVehicleID.Text));
                    updateVehicle.ExecuteNonQuery();

                    AppEvents.RaiseVehiclesUpdated();
                }
                MessageBox.Show(
                    rentalId == 0 ? "Rental added." : "Rental updated.",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
                AppEvents.RaiseRentalsUpdated();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving rental:\n" + ex.Message);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}