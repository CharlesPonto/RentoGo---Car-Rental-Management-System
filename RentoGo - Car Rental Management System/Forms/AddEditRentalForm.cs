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
            cbStatus.Items.Clear();
            cbStatus.Items.AddRange(new string[]
            {
                "Reserved", "Active", "Completed", "Cancelled"
            });

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
                cbStatus.Enabled = false;
            }

            // events
            dtStart.ValueChanged += (s, a) => computeTotalCharge();
            dtEnd.ValueChanged += (s, a) => computeTotalCharge();
            txtCustomerID.TextChanged += (s, a) => loadCustomer();
            txtVehicleID.TextChanged += (s, a) => loadVehicle();
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
                        dtStart.Value = Convert.ToDateTime(r["StartDate"]);
                        dtEnd.Value = Convert.ToDateTime(r["EndDate"]);
                        txtTotal.Text = r["TotalCharge"].ToString();
                        cbStatus.SelectedItem = r["Status"].ToString();
                    }
                    r.Close();
                }

                loadCustomer();
                loadVehicle();
            }
            catch (Exception ex)
            {
                MessageBox.Show("error loading rental:\n" + ex.Message);
            }
        }

        private void loadCustomer()
        {
            if (!int.TryParse(txtCustomerID.Text, out int id))
            {
                txtCustomerName.Clear();
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    SqlCommand cmd = new SqlCommand(
                        "SELECT FullName FROM Customers WHERE CustomerID = @id", con);
                    cmd.Parameters.AddWithValue("@id", id);

                    object result = cmd.ExecuteScalar();
                    txtCustomerName.Text = result?.ToString() ?? "";
                }
            }
            catch { }
        }

        private void loadVehicle()
        {
            if (!int.TryParse(txtVehicleID.Text, out int id))
            {
                txtVehicleName.Clear();
                txtRate.Clear();
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    SqlCommand cmd = new SqlCommand(
                        "SELECT Model, RatePerDay FROM Vehicles WHERE VehicleID = @id", con);
                    cmd.Parameters.AddWithValue("@id", id);

                    SqlDataReader r = cmd.ExecuteReader();
                    if (r.Read())
                    {
                        txtVehicleName.Text = r["Model"].ToString();
                        txtRate.Text = r["RatePerDay"].ToString();
                    }
                    else
                    {
                        txtVehicleName.Clear();
                        txtRate.Clear();
                    }
                    r.Close();
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
            if (days < 1) days = 1;

            txtTotal.Text = (rate * days).ToString("0.00");
        }


        private bool validateFields()
        {
            string err = "";

            if (!int.TryParse(txtCustomerID.Text, out _))
                err += "customer id is invalid.\n";

            if (string.IsNullOrWhiteSpace(txtCustomerName.Text))
                err += "customer does not exist.\n";

            if (!int.TryParse(txtVehicleID.Text, out _))
                err += "vehicle id is invalid.\n";

            if (string.IsNullOrWhiteSpace(txtVehicleName.Text))
                err += "vehicle does not exist.\n";

            if (!decimal.TryParse(txtRate.Text, out _))
                err += "rate is invalid.\n";

            if (!decimal.TryParse(txtTotal.Text, out _))
                err += "total charge is invalid.\n";

            if (rentalId > 0 && cbStatus.SelectedItem == null)
                err += "please select a status.\n";

            if (err != "")
            {
                MessageBox.Show("please fix the following:\n\n" + err,
                    "validation error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!validateFields()) return;

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    SqlCommand cmd;

                    if (rentalId == 0)
                    {
                        cmd = new SqlCommand(@"
                            INSERT INTO Rentals (CustomerID, VehicleID, StartDate, EndDate, TotalCharge, Status)
                            VALUES (@c, @v, @s, @e, @t, 'Reserved')", con);
                    }
                    else
                    {
                        cmd = new SqlCommand(@"
                            UPDATE Rentals SET 
                                CustomerID=@c, VehicleID=@v, StartDate=@s, EndDate=@e, 
                                TotalCharge=@t, Status=@st
                            WHERE RentalID = @id", con);

                        cmd.Parameters.AddWithValue("@id", rentalId);
                        cmd.Parameters.AddWithValue("@st", cbStatus.SelectedItem.ToString());
                    }

                    cmd.Parameters.AddWithValue("@c", int.Parse(txtCustomerID.Text));
                    cmd.Parameters.AddWithValue("@v", int.Parse(txtVehicleID.Text));
                    cmd.Parameters.AddWithValue("@s", dtStart.Value);
                    cmd.Parameters.AddWithValue("@e", dtEnd.Value);
                    cmd.Parameters.AddWithValue("@t", decimal.Parse(txtTotal.Text));

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show(
                    rentalId == 0 ? "Rental added." : "Rental updated.",
                    "success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("error saving rental:\n" + ex.Message);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
