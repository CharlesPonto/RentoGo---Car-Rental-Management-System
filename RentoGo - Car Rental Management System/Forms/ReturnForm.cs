using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace RentoGo___Car_Rental_Management_System.Forms
{
    public partial class ReturnForm : Form
    {
        private readonly string connectionString =
            @"Server=(localdb)\MSSQLLocalDB;Database=RentoGoDB;Trusted_Connection=True;";

        private int rentalId = 0;
        private decimal dailyRate = 0;
        private decimal originalTotal = 0;
        private DateTime endDate;

        public ReturnForm(int id)
        {
            InitializeComponent();
            rentalId = id;
            txtLateFee.Text = "0.00";   
        }

        private void ReturnForm_Load(object sender, EventArgs e)
        {
            dtReturn.Value = DateTime.Today;
            loadRentalData();

            dtReturn.ValueChanged += (s, a) => computeFees();
            txtDamageFee.TextChanged += (s, a) => computeFees();
        }

        // load
        private void loadRentalData()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    string query = @"
                        SELECT 
                            r.CustomerID, c.FullName,
                            r.VehicleID, v.Model, v.RatePerDay,
                            r.StartDate, r.EndDate,
                            r.TotalCharge
                        FROM Rentals r
                        JOIN Customers c ON r.CustomerID = c.CustomerID
                        JOIN Vehicles v ON r.VehicleID = v.VehicleID
                        WHERE RentalID = @id";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@id", rentalId);

                    SqlDataReader r = cmd.ExecuteReader();
                    if (r.Read())
                    {
                        txtCustomer.Text = r["FullName"].ToString();
                        txtVehicle.Text = r["Model"].ToString();
                        txtRate.Text = r["RatePerDay"].ToString();

                        dailyRate = Convert.ToDecimal(r["RatePerDay"]);
                        originalTotal = Convert.ToDecimal(r["TotalCharge"]);

                        dtStart.Value = Convert.ToDateTime(r["StartDate"]);
                        dtEnd.Value = Convert.ToDateTime(r["EndDate"]);
                        endDate = dtEnd.Value;

                        txtOriginalTotal.Text = originalTotal.ToString("0.00");
                    }
                    r.Close();
                }

                computeFees();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading rental:\n" + ex.Message);
            }
        }

        // compute
        private void computeFees()
        {
            decimal damage = 0;
            decimal.TryParse(txtDamageFee.Text, out damage);

            int overdueDays = (dtReturn.Value.Date - endDate.Date).Days;
            if (overdueDays < 0) overdueDays = 0;

            decimal lateFee = overdueDays * dailyRate;
            txtLateFee.Text = lateFee.ToString("0.00");

            decimal totalBeforePayments = originalTotal + lateFee + damage;

            decimal totalPayments = getTotalPayments();

            decimal finalTotal = totalBeforePayments - totalPayments;
            // Ensure final total is not negative
            if (finalTotal < 0) finalTotal = 0;

            txtFinalTotal.Text = finalTotal.ToString("0.00");
        }

        // validate
        private bool validateFields()
        {
            if (!decimal.TryParse(txtDamageFee.Text, out _))
            {
                MessageBox.Show("damage fee must be a valid number.");
                return false;
            }

            return true;
        }

        private decimal getTotalPayments()
        {
            decimal totalPayments = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "SELECT ISNULL(SUM(Amount), 0) FROM Payments WHERE RentalID = @id";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@id", rentalId);
                    totalPayments = Convert.ToDecimal(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error calculating payments: " + ex.Message);
            }
            return totalPayments;
        }

        // save
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!validateFields()) return;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    decimal lateFee = decimal.Parse(txtLateFee.Text);
                    decimal damageFee = decimal.Parse(txtDamageFee.Text);
                    decimal finalTotal = decimal.Parse(txtFinalTotal.Text);

                    // update Rentals
                    string updateRental = @"
                UPDATE Rentals SET
                     LateFee=@late, DamageFee=@damage,
                    TotalCharge=@total,
                     ReturnDate=@return,
                    Status='Completed'
                WHERE RentalID = @id";
                    SqlCommand cmd1 = new SqlCommand(updateRental, con);
                    cmd1.Parameters.AddWithValue("@late", lateFee);
                    cmd1.Parameters.AddWithValue("@damage", damageFee);
                    cmd1.Parameters.AddWithValue("@total", finalTotal);
                    cmd1.Parameters.AddWithValue("@return", dtReturn.Value);
                    cmd1.Parameters.AddWithValue("@id", rentalId);
                    cmd1.ExecuteNonQuery();

                    // set vehicle available
                    SqlCommand cmd2 = new SqlCommand(@"
                UPDATE Vehicles 
                SET Status='Available'
                WHERE VehicleID = (SELECT VehicleID FROM Rentals WHERE RentalID=@id)", con);
                    cmd2.Parameters.AddWithValue("@id", rentalId);
                    cmd2.ExecuteNonQuery();
                }

                AppEvents.RaiseRentalsUpdated();

                MessageBox.Show("Vehicle returned successfully.", "success");
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error returning vehicle:\n" + ex.Message);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
