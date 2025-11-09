using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace RentoGo___Car_Rental_Management_System.Forms
{
    public partial class AddEditCustomerForm : Form
    {
        private int customerId = 0;
        private readonly string connectionString =
            @"Server=(localdb)\MSSQLLocalDB;Database=RentoGoDB;Trusted_Connection=True;";

        public AddEditCustomerForm()
        {
            InitializeComponent();
        }
        public AddEditCustomerForm(int id)
        {
            InitializeComponent();
            customerId = id;
        }
        private void AddEditCustomerForm_Load(object sender, EventArgs e)
        {
            if (customerId > 0)
            {
                lblTitle.Text = "Edit Customer";
                btnSave.Text = "Save";
                loadCustomer();
            }
            else
            {
                lblTitle.Text = "Add Customer";
                btnSave.Text = "Add";
            }
        }

        private void loadCustomer()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "SELECT FullName, Contact, Address, LicenseNo FROM Customers WHERE CustomerID = @id";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@id", customerId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        txtFullName.Text = reader["FullName"].ToString();
                        txtContact.Text = reader["Contact"].ToString();
                        txtAddress.Text = reader["Address"].ToString();
                        txtLicenseNo.Text = reader["LicenseNo"].ToString();
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading customer details:\n" + ex.Message,
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // validate
        private bool validateFields()
        {
            string error = "";

            if (string.IsNullOrWhiteSpace(txtFullName.Text))
                error += "Full name is required.\n";

            if (string.IsNullOrWhiteSpace(txtContact.Text))
                error += "Contact number is required.\n";
            else if (!long.TryParse(txtContact.Text, out _))
                error += "Contact number must contain digits only.\n";
            else if (txtContact.Text.Length != 11)
                error += "Contact number must be exactly 11 digits long.\n";
            else if (!txtContact.Text.StartsWith("09"))
                error += "Contact number must start with '09'.\n";

            if (string.IsNullOrWhiteSpace(txtAddress.Text))
                error += "Address is required.\n";

            if (string.IsNullOrWhiteSpace(txtLicenseNo.Text))
                error += "License number is required.\n";

            if (!string.IsNullOrEmpty(error))
            {
                MessageBox.Show("Please fix the following:\n\n" + error,
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

                    // check duplicate license number
                    SqlCommand checkCmd = new SqlCommand(
                        "SELECT COUNT(*) FROM Customers WHERE LicenseNo = @license AND CustomerID != @id", con);
                    checkCmd.Parameters.AddWithValue("@license", txtLicenseNo.Text.Trim());
                    checkCmd.Parameters.AddWithValue("@id", customerId);
                    int exists = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (exists > 0)
                    {
                        MessageBox.Show("A customer with this license number already exists.",
                            "Duplicate Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    SqlCommand cmd;

                    if (customerId == 0)
                    {
                        cmd = new SqlCommand(@"
                            INSERT INTO Customers (FullName, Contact, Address, LicenseNo)
                            VALUES (@name, @contact, @address, @license)", con);
                    }
                    else
                    {
                        cmd = new SqlCommand(@"
                            UPDATE Customers
                            SET FullName = @name, Contact = @contact, Address = @address, LicenseNo = @license
                            WHERE CustomerID = @id", con);
                        cmd.Parameters.AddWithValue("@id", customerId);
                    }

                    cmd.Parameters.AddWithValue("@name", txtFullName.Text.Trim());
                    cmd.Parameters.AddWithValue("@contact", txtContact.Text.Trim());
                    cmd.Parameters.AddWithValue("@address", txtAddress.Text.Trim());
                    cmd.Parameters.AddWithValue("@license", txtLicenseNo.Text.Trim());
                    cmd.ExecuteNonQuery();

                    MessageBox.Show(customerId == 0
                        ? "Customer added successfully."
                        : "Customer updated successfully.",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving customer:\n" + ex.Message,
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

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
