using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace RentoGo___Car_Rental_Management_System.Forms
{
    public partial class AddPaymentForm : Form
    {
        private readonly string connectionString =
            @"Server=(localdb)\MSSQLLocalDB;Database=RentoGoDB;Trusted_Connection=True;";
        private decimal _originalBalance = 0;

        public AddPaymentForm()
        {
            InitializeComponent();
        }

        private void AddPaymentForm_Load(object sender, EventArgs e)
        {
            cbRental.DropDownStyle = ComboBoxStyle.DropDown; 
            cbType.Items.AddRange(new string[] { "Partial", "Full" });
            cbType.SelectedIndex = 0; 

            txtTotalCharge.ReadOnly = true;
            txtBalance.ReadOnly = true;
            txtCustomerName.ReadOnly = true;

            loadAvailableRentals();

            // Events
            cbRental.TextChanged += (s, a) => updateRentalDetails();
            cbRental.SelectedIndexChanged += (s, a) => updateRentalDetails();
            txtAmountPaid.TextChanged += (s, a) => updateBalance(); 
        }

        private void loadAvailableRentals()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = @"
                        SELECT r.RentalID, c.FullName AS Customer, v.Model AS Vehicle, r.TotalCharge
                        FROM Rentals r
                        JOIN Customers c ON r.CustomerID = c.CustomerID
                        JOIN Vehicles v ON r.VehicleID = v.VehicleID
                        WHERE r.Status IN ('Active', 'Reserved')
                        ORDER BY r.RentalID";

                    SqlCommand cmd = new SqlCommand(query, con);
                    SqlDataReader reader = cmd.ExecuteReader();

                    cbRental.Items.Clear();
                    while (reader.Read())
                    {
                        int id = Convert.ToInt32(reader["RentalID"]);
                        string customer = reader["Customer"].ToString();
                        string vehicle = reader["Vehicle"].ToString();
                        decimal total = Convert.ToDecimal(reader["TotalCharge"]);
                        cbRental.Items.Add($"{id} - {customer} - {vehicle} - ₱{total:N2}"); 
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading rentals:\n" + ex.Message);
            }
        }

        private void updateRentalDetails()
        {
            if (!int.TryParse(cbRental.Text.Split('-')[0].Trim(), out int rentalId))
            {
                txtTotalCharge.Clear();
                txtAmountPaid.Clear();
                txtBalance.Clear();
                txtCustomerName.Clear();
                _originalBalance = 0;
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = @"
                        SELECT r.TotalCharge, ISNULL(r.AmountPaid, 0) AS AmountPaid, c.FullName AS Customer
                        FROM Rentals r
                        JOIN Customers c ON r.CustomerID = c.CustomerID
                        WHERE r.RentalID = @id";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@id", rentalId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        decimal total = Convert.ToDecimal(reader["TotalCharge"]);
                        decimal paid = Convert.ToDecimal(reader["AmountPaid"]);
                        string customer = reader["Customer"].ToString();

                        _originalBalance = total - paid; 
                        txtTotalCharge.Text = total.ToString("N2");
                        txtAmountPaid.Clear(); 
                        txtBalance.Text = _originalBalance.ToString("N2");
                        txtCustomerName.Text = customer;
                    }
                    else
                    {
                        txtTotalCharge.Clear();
                        txtAmountPaid.Clear();
                        txtBalance.Clear();
                        txtCustomerName.Clear();
                        _originalBalance = 0;
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading rental details:\n" + ex.Message);
            }
        }

        // update balance  based on input amount
        private void updateBalance()
        {
            if (!decimal.TryParse(txtAmountPaid.Text, out decimal inputAmount))
            {
                txtBalance.Text = _originalBalance.ToString("N2");
                return;
            }

            decimal newBalance = _originalBalance - inputAmount;
            txtBalance.Text = newBalance.ToString("N2");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(cbRental.Text.Split('-')[0].Trim(), out int rentalId))
                {
                    MessageBox.Show("Invalid Rental ID.");
                    return;
                }

                if (!decimal.TryParse(txtAmountPaid.Text.Trim(), out decimal amount)) // Parse from input field
                {
                    MessageBox.Show("Invalid amount.");
                    return;
                }

                if (amount <= 0)
                {
                    MessageBox.Show("Amount must be greater than 0.");
                    return;
                }

                if (cbType.SelectedItem == null)
                {
                    MessageBox.Show("Select a payment type (Partial or Full).");
                    return;
                }

                string type = cbType.SelectedItem.ToString();

                decimal totalCharge = 0;
                decimal amountPaid = 0;
                string rentalStatus = "";

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    string checkRental = @"
                        SELECT TotalCharge, ISNULL(AmountPaid,0), Status
                        FROM Rentals
                        WHERE RentalID = @RentalID";

                    using (SqlCommand cmd = new SqlCommand(checkRental, con))
                    {
                        cmd.Parameters.AddWithValue("@RentalID", rentalId);
                        SqlDataReader rd = cmd.ExecuteReader();

                        if (!rd.Read())
                        {
                            MessageBox.Show("Rental not found.");
                            return;
                        }

                        totalCharge = rd.GetDecimal(0);
                        amountPaid = rd.GetDecimal(1);
                        rentalStatus = rd.GetString(2);

                        rd.Close();
                    }

                    if (rentalStatus == "Completed" || rentalStatus == "Cancelled")
                    {
                        MessageBox.Show("Cannot accept payments for completed or cancelled rentals.");
                        return;
                    }

                    decimal balance = totalCharge - amountPaid;
                    if (balance <= 0)
                    {
                        MessageBox.Show("Rental is fully paid. No payment needed.");
                        return;
                    }

                    if (amount > balance)
                    {
                        MessageBox.Show("Payment exceeds remaining balance.");
                        return;
                    }

                    SqlTransaction trans = con.BeginTransaction();

                    string insertPayment = @"
                        INSERT INTO Payments (RentalID, Amount, Type)
                        VALUES (@RentalID, @Amount, @Type)"; // Removed PaymentDate

                    using (SqlCommand cmd = new SqlCommand(insertPayment, con, trans))
                    {
                        cmd.Parameters.AddWithValue("@RentalID", rentalId);
                        cmd.Parameters.AddWithValue("@Amount", amount);
                        cmd.Parameters.AddWithValue("@Type", type);
                        cmd.ExecuteNonQuery();
                    }

                    string updateRental = @"
                        UPDATE Rentals
                        SET AmountPaid = AmountPaid + @Amount
                        WHERE RentalID = @RentalID";

                    using (SqlCommand cmd = new SqlCommand(updateRental, con, trans))
                    {
                        cmd.Parameters.AddWithValue("@Amount", amount);
                        cmd.Parameters.AddWithValue("@RentalID", rentalId);
                        cmd.ExecuteNonQuery();
                    }

                    trans.Commit();
                }

                AppEvents.RaisePaymentsUpdated();

                MessageBox.Show("Payment recorded successfully.", "Success");
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding payment:\n" + ex.Message);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
