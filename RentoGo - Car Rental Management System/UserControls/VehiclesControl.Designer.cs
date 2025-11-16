namespace RentoGo___Car_Rental_Management_System.UserControls
{
    partial class VehiclesControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.label1 = new System.Windows.Forms.Label();
            this.guna2CustomGradientPanel6 = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            this.cbStatusFilter = new Guna.UI2.WinForms.Guna2ComboBox();
            this.dgvVehicles = new Guna.UI2.WinForms.Guna2DataGridView();
            this.btnSearch = new Guna.UI2.WinForms.Guna2GradientButton();
            this.txtSearch = new Guna.UI2.WinForms.Guna2TextBox();
            this.btnAddVehicle = new Guna.UI2.WinForms.Guna2GradientButton();
            this.guna2CustomGradientPanel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVehicles)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Semibold", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label1.Location = new System.Drawing.Point(43, 63);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(261, 46);
            this.label1.TabIndex = 1;
            this.label1.Text = "Vehicle Records";
            // 
            // guna2CustomGradientPanel6
            // 
            this.guna2CustomGradientPanel6.BorderRadius = 15;
            this.guna2CustomGradientPanel6.Controls.Add(this.cbStatusFilter);
            this.guna2CustomGradientPanel6.Controls.Add(this.dgvVehicles);
            this.guna2CustomGradientPanel6.Controls.Add(this.btnSearch);
            this.guna2CustomGradientPanel6.Controls.Add(this.txtSearch);
            this.guna2CustomGradientPanel6.Location = new System.Drawing.Point(51, 126);
            this.guna2CustomGradientPanel6.Margin = new System.Windows.Forms.Padding(5);
            this.guna2CustomGradientPanel6.Name = "guna2CustomGradientPanel6";
            this.guna2CustomGradientPanel6.ShadowDecoration.Parent = this.guna2CustomGradientPanel6;
            this.guna2CustomGradientPanel6.Size = new System.Drawing.Size(1296, 700);
            this.guna2CustomGradientPanel6.TabIndex = 7;
            // 
            // cbStatusFilter
            // 
            this.cbStatusFilter.BackColor = System.Drawing.Color.Transparent;
            this.cbStatusFilter.BorderRadius = 4;
            this.cbStatusFilter.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbStatusFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbStatusFilter.FocusedColor = System.Drawing.Color.Empty;
            this.cbStatusFilter.FocusedState.Parent = this.cbStatusFilter;
            this.cbStatusFilter.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cbStatusFilter.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.cbStatusFilter.FormattingEnabled = true;
            this.cbStatusFilter.HoverState.Parent = this.cbStatusFilter;
            this.cbStatusFilter.ItemHeight = 30;
            this.cbStatusFilter.Items.AddRange(new object[] {
            "All",
            "Available",
            "Reserved",
            "Rented",
            "Maintenance"});
            this.cbStatusFilter.ItemsAppearance.Parent = this.cbStatusFilter;
            this.cbStatusFilter.Location = new System.Drawing.Point(285, 24);
            this.cbStatusFilter.Name = "cbStatusFilter";
            this.cbStatusFilter.ShadowDecoration.Parent = this.cbStatusFilter;
            this.cbStatusFilter.Size = new System.Drawing.Size(168, 36);
            this.cbStatusFilter.TabIndex = 10;
            this.cbStatusFilter.SelectedIndexChanged += new System.EventHandler(this.cbStatusFilter_SelectedIndexChanged);
            // 
            // dgvVehicles
            // 
            this.dgvVehicles.AllowUserToAddRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
            this.dgvVehicles.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvVehicles.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvVehicles.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvVehicles.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvVehicles.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvVehicles.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(245)))), ((int)(((byte)(228)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(245)))), ((int)(((byte)(228)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvVehicles.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvVehicles.ColumnHeadersHeight = 40;
            this.dgvVehicles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvVehicles.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvVehicles.EnableHeadersVisualStyles = false;
            this.dgvVehicles.GridColor = System.Drawing.SystemColors.Window;
            this.dgvVehicles.Location = new System.Drawing.Point(39, 82);
            this.dgvVehicles.Margin = new System.Windows.Forms.Padding(4);
            this.dgvVehicles.Name = "dgvVehicles";
            this.dgvVehicles.ReadOnly = true;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.GradientActiveCaption;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvVehicles.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvVehicles.RowHeadersVisible = false;
            this.dgvVehicles.RowHeadersWidth = 51;
            this.dgvVehicles.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvVehicles.RowTemplate.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.LightBlue;
            this.dgvVehicles.RowTemplate.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.dgvVehicles.RowTemplate.Height = 35;
            this.dgvVehicles.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvVehicles.Size = new System.Drawing.Size(1207, 588);
            this.dgvVehicles.TabIndex = 7;
            this.dgvVehicles.Theme = Guna.UI2.WinForms.Enums.DataGridViewPresetThemes.Default;
            this.dgvVehicles.ThemeStyle.AlternatingRowsStyle.BackColor = System.Drawing.Color.White;
            this.dgvVehicles.ThemeStyle.AlternatingRowsStyle.Font = null;
            this.dgvVehicles.ThemeStyle.AlternatingRowsStyle.ForeColor = System.Drawing.Color.Empty;
            this.dgvVehicles.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = System.Drawing.Color.Empty;
            this.dgvVehicles.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = System.Drawing.Color.Empty;
            this.dgvVehicles.ThemeStyle.BackColor = System.Drawing.SystemColors.Window;
            this.dgvVehicles.ThemeStyle.GridColor = System.Drawing.SystemColors.Window;
            this.dgvVehicles.ThemeStyle.HeaderStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(245)))), ((int)(((byte)(228)))));
            this.dgvVehicles.ThemeStyle.HeaderStyle.BorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvVehicles.ThemeStyle.HeaderStyle.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvVehicles.ThemeStyle.HeaderStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.dgvVehicles.ThemeStyle.HeaderStyle.HeaightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvVehicles.ThemeStyle.HeaderStyle.Height = 40;
            this.dgvVehicles.ThemeStyle.ReadOnly = true;
            this.dgvVehicles.ThemeStyle.RowsStyle.BackColor = System.Drawing.Color.White;
            this.dgvVehicles.ThemeStyle.RowsStyle.BorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvVehicles.ThemeStyle.RowsStyle.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            this.dgvVehicles.ThemeStyle.RowsStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.dgvVehicles.ThemeStyle.RowsStyle.Height = 35;
            this.dgvVehicles.ThemeStyle.RowsStyle.SelectionBackColor = System.Drawing.Color.Empty;
            this.dgvVehicles.ThemeStyle.RowsStyle.SelectionForeColor = System.Drawing.Color.Empty;
            this.dgvVehicles.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvVehicles_CellContentClick);
            // 
            // btnSearch
            // 
            this.btnSearch.BorderRadius = 4;
            this.btnSearch.CheckedState.Parent = this.btnSearch;
            this.btnSearch.CustomImages.Parent = this.btnSearch;
            this.btnSearch.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.btnSearch.Font = new System.Drawing.Font("Segoe UI Semibold", 10.8F, System.Drawing.FontStyle.Bold);
            this.btnSearch.ForeColor = System.Drawing.Color.White;
            this.btnSearch.HoverState.Parent = this.btnSearch;
            this.btnSearch.Location = new System.Drawing.Point(460, 24);
            this.btnSearch.Margin = new System.Windows.Forms.Padding(4);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.ShadowDecoration.Parent = this.btnSearch;
            this.btnSearch.Size = new System.Drawing.Size(133, 38);
            this.btnSearch.TabIndex = 9;
            this.btnSearch.Text = "Search";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.BorderRadius = 4;
            this.txtSearch.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtSearch.DefaultText = "";
            this.txtSearch.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtSearch.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtSearch.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtSearch.DisabledState.Parent = this.txtSearch;
            this.txtSearch.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtSearch.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtSearch.FocusedState.Parent = this.txtSearch;
            this.txtSearch.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtSearch.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtSearch.HoverState.Parent = this.txtSearch;
            this.txtSearch.Location = new System.Drawing.Point(39, 24);
            this.txtSearch.Margin = new System.Windows.Forms.Padding(5);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.PasswordChar = '\0';
            this.txtSearch.PlaceholderText = "";
            this.txtSearch.SelectedText = "";
            this.txtSearch.ShadowDecoration.Parent = this.txtSearch;
            this.txtSearch.Size = new System.Drawing.Size(238, 38);
            this.txtSearch.TabIndex = 8;
            // 
            // btnAddVehicle
            // 
            this.btnAddVehicle.BorderRadius = 4;
            this.btnAddVehicle.CheckedState.Parent = this.btnAddVehicle;
            this.btnAddVehicle.CustomImages.Parent = this.btnAddVehicle;
            this.btnAddVehicle.FillColor = System.Drawing.Color.MediumSeaGreen;
            this.btnAddVehicle.FillColor2 = System.Drawing.Color.MediumSeaGreen;
            this.btnAddVehicle.Font = new System.Drawing.Font("Segoe UI Semibold", 10.8F, System.Drawing.FontStyle.Bold);
            this.btnAddVehicle.ForeColor = System.Drawing.Color.White;
            this.btnAddVehicle.HoverState.Parent = this.btnAddVehicle;
            this.btnAddVehicle.Location = new System.Drawing.Point(1069, 63);
            this.btnAddVehicle.Margin = new System.Windows.Forms.Padding(4);
            this.btnAddVehicle.Name = "btnAddVehicle";
            this.btnAddVehicle.ShadowDecoration.Parent = this.btnAddVehicle;
            this.btnAddVehicle.Size = new System.Drawing.Size(165, 46);
            this.btnAddVehicle.TabIndex = 10;
            this.btnAddVehicle.Text = "+ Add Vehicle";
            this.btnAddVehicle.Click += new System.EventHandler(this.btnAddVehicle_Click);
            // 
            // VehiclesControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.Controls.Add(this.btnAddVehicle);
            this.Controls.Add(this.guna2CustomGradientPanel6);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "VehiclesControl";
            this.Size = new System.Drawing.Size(1407, 875);
            this.Load += new System.EventHandler(this.VehiclesControl_Load);
            this.guna2CustomGradientPanel6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvVehicles)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private Guna.UI2.WinForms.Guna2CustomGradientPanel guna2CustomGradientPanel6;
        private Guna.UI2.WinForms.Guna2DataGridView dgvVehicles;
        private Guna.UI2.WinForms.Guna2TextBox txtSearch;
        private Guna.UI2.WinForms.Guna2GradientButton btnSearch;
        private Guna.UI2.WinForms.Guna2GradientButton btnAddVehicle;
        private Guna.UI2.WinForms.Guna2ComboBox cbStatusFilter;
    }
}
