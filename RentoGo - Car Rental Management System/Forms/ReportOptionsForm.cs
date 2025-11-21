using System;
using System.Windows.Forms;

namespace RentoGo___Car_Rental_Management_System.Forms
{
    public partial class ReportOptionsForm : Form
    {
        public string ReportType { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }

        public ReportOptionsForm()
        {
            InitializeComponent();
            cbReportType.Items.AddRange(new string[]
            {
                "Weekly",
                "Monthly",
                "Quarterly",
                "Yearly",
                "Custom Date Range",
                "Customer" // Added Customer option
            });
            cbReportType.SelectedIndex = 0;
            dtStart.Enabled = false;
            dtEnd.Enabled = false;
        }

        private void cbReportType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selected = cbReportType.SelectedItem.ToString();

            // Disable date selection if Customer is selected
            if (selected == "Customer")
            {
                dtStart.Enabled = false;
                dtEnd.Enabled = false;
                return; // Skip others
            }

            if (selected == "Weekly")
            {
                DateTime today = DateTime.Now;
                int daysToMonday = (int)today.DayOfWeek - (int)DayOfWeek.Monday;
                if (daysToMonday < 0) daysToMonday += 7; // Adjust for Sunday
                dtStart.Value = today.AddDays(-daysToMonday);
                dtEnd.Value = dtStart.Value.AddDays(6);
                dtStart.Enabled = false;
                dtEnd.Enabled = false;
            }
            else if (selected == "Monthly")
            {
                dtStart.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                dtEnd.Value = dtStart.Value.AddMonths(1).AddDays(-1);
                dtStart.Enabled = false;
                dtEnd.Enabled = false;
            }
            else if (selected == "Quarterly")
            {
                int quarter = (DateTime.Now.Month - 1) / 3 + 1;
                dtStart.Value = new DateTime(DateTime.Now.Year, (quarter - 1) * 3 + 1, 1);
                dtEnd.Value = dtStart.Value.AddMonths(3).AddDays(-1);
                dtStart.Enabled = false;
                dtEnd.Enabled = false;
            }
            else if (selected == "Yearly")
            {
                dtStart.Value = new DateTime(DateTime.Now.Year, 1, 1);
                dtEnd.Value = new DateTime(DateTime.Now.Year, 12, 31);
                dtStart.Enabled = false;
                dtEnd.Enabled = false;
            }
            else if (selected == "Custom Date Range")
            {
                dtStart.Enabled = true;
                dtEnd.Enabled = true;
            }
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            // No date validation needed if "Customer" is selected
            if (cbReportType.SelectedItem.ToString() != "Customer" && dtStart.Value > dtEnd.Value)
            {
                MessageBox.Show("Start date cannot be after end date.", "Invalid Dates", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ReportType = cbReportType.SelectedItem.ToString();
            StartDate = dtStart.Value;
            EndDate = dtEnd.Value;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
