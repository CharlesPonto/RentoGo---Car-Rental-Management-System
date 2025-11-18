using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace RentoGo___Car_Rental_Management_System.Forms
{
    public partial class AddPaymentForm : Form
    {
        private readonly string connectionString =
            @"Server=(localdb)\MSSQLLocalDB;Database=RentoGoDB;Trusted_Connection=True;";

        public AddPaymentForm()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(txtRentalID.Text.Trim(), out int rentalId))
                {
                    MessageBox.Show("Invalid Rental ID.");
                    return;
                }

                if (!decimal.TryParse(txtAmount.Text.Trim(), out decimal amount))
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
                    MessageBox.Show("Select a payment type.");
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

                    if (amountPaid + amount > totalCharge)
                    {
                        MessageBox.Show("Payment exceeds total balance.");
                        return;
                    }

                    SqlTransaction trans = con.BeginTransaction();

                    string insertPayment = @"
                        INSERT INTO Payments (RentalID, Amount, Type)
                        VALUES (@RentalID, @Amount, @Type)";

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

                AppEvents.RaiseRentalsUpdated();

                MessageBox.Show("Payment recorded successfully.", "Success");
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding payment:\n" + ex.Message);
            }
        }
    }
}
