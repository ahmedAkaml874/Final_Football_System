using System;
using System.Windows.Forms;

namespace FB_Reservations_System
{
    partial class ReservationManagementForm
    {
        private System.ComponentModel.IContainer components = null;
        private ComboBox comboBoxField;
        private DateTimePicker dateTimePickerDay;
        private TextBox textBoxCustomer;
        private Button buttonSearch;
        private DataGridView dataGridViewResults;
        private Button buttonCancelReservation;
        private Button buttonCreateNew;
        private Label labelField;
        private Label labelDay;
        private Label labelCustomer;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.comboBoxField = new System.Windows.Forms.ComboBox();
            this.dateTimePickerDay = new System.Windows.Forms.DateTimePicker();
            this.textBoxCustomer = new System.Windows.Forms.TextBox();
            this.buttonSearch = new System.Windows.Forms.Button();
            this.dataGridViewResults = new System.Windows.Forms.DataGridView();
            this.buttonCancelReservation = new System.Windows.Forms.Button();
            this.buttonCreateNew = new System.Windows.Forms.Button();
            this.labelField = new System.Windows.Forms.Label();
            this.labelDay = new System.Windows.Forms.Label();
            this.labelCustomer = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewResults)).BeginInit();
            this.SuspendLayout();
            // 
            // comboBoxField
            // 
            this.comboBoxField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxField.Location = new System.Drawing.Point(75, 12);
            this.comboBoxField.Name = "comboBoxField";
            this.comboBoxField.Size = new System.Drawing.Size(200, 24);
            this.comboBoxField.TabIndex = 6;
            // 
            // dateTimePickerDay
            // 
            this.dateTimePickerDay.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePickerDay.Location = new System.Drawing.Point(340, 12);
            this.dateTimePickerDay.Name = "dateTimePickerDay";
            this.dateTimePickerDay.Size = new System.Drawing.Size(110, 22);
            this.dateTimePickerDay.TabIndex = 5;
            // 
            // textBoxCustomer
            // 
            this.textBoxCustomer.Location = new System.Drawing.Point(540, 12);
            this.textBoxCustomer.Name = "textBoxCustomer";
            this.textBoxCustomer.Size = new System.Drawing.Size(200, 22);
            this.textBoxCustomer.TabIndex = 4;
            // 
            // buttonSearch
            // 
            this.buttonSearch.Location = new System.Drawing.Point(760, 10);
            this.buttonSearch.Name = "buttonSearch";
            this.buttonSearch.Size = new System.Drawing.Size(90, 23);
            this.buttonSearch.TabIndex = 3;
            this.buttonSearch.Text = "Search";
            this.buttonSearch.Click += new System.EventHandler(this.buttonSearch_Click);
            // 
            // dataGridViewResults
            // 
            this.dataGridViewResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewResults.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewResults.ColumnHeadersHeight = 29;
            this.dataGridViewResults.Location = new System.Drawing.Point(15, 50);
            this.dataGridViewResults.MultiSelect = false;
            this.dataGridViewResults.Name = "dataGridViewResults";
            this.dataGridViewResults.ReadOnly = true;
            this.dataGridViewResults.RowHeadersWidth = 51;
            this.dataGridViewResults.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewResults.Size = new System.Drawing.Size(835, 350);
            this.dataGridViewResults.TabIndex = 2;
            this.dataGridViewResults.SelectionChanged += new System.EventHandler(this.dataGridViewResults_SelectionChanged);
            // 
            // buttonCancelReservation
            // 
            this.buttonCancelReservation.Location = new System.Drawing.Point(15, 410);
            this.buttonCancelReservation.Name = "buttonCancelReservation";
            this.buttonCancelReservation.Size = new System.Drawing.Size(160, 32);
            this.buttonCancelReservation.TabIndex = 1;
            this.buttonCancelReservation.Text = "Cancel Reservation";
            this.buttonCancelReservation.Click += new System.EventHandler(this.buttonCancelReservation_Click);
            // 
            // buttonCreateNew
            // 
            this.buttonCreateNew.Location = new System.Drawing.Point(690, 410);
            this.buttonCreateNew.Name = "buttonCreateNew";
            this.buttonCreateNew.Size = new System.Drawing.Size(160, 32);
            this.buttonCreateNew.TabIndex = 0;
            this.buttonCreateNew.Text = "Create New Reservation";
            this.buttonCreateNew.Click += new System.EventHandler(this.buttonCreateNew_Click);
            // 
            // labelField
            // 
            this.labelField.AutoSize = true;
            this.labelField.Location = new System.Drawing.Point(12, 15);
            this.labelField.Name = "labelField";
            this.labelField.Size = new System.Drawing.Size(40, 16);
            this.labelField.TabIndex = 9;
            this.labelField.Text = "Field:";
            // 
            // labelDay
            // 
            this.labelDay.AutoSize = true;
            this.labelDay.Location = new System.Drawing.Point(295, 15);
            this.labelDay.Name = "labelDay";
            this.labelDay.Size = new System.Drawing.Size(35, 16);
            this.labelDay.TabIndex = 8;
            this.labelDay.Text = "Day:";
            // 
            // labelCustomer
            // 
            this.labelCustomer.AutoSize = true;
            this.labelCustomer.Location = new System.Drawing.Point(470, 15);
            this.labelCustomer.Name = "labelCustomer";
            this.labelCustomer.Size = new System.Drawing.Size(67, 16);
            this.labelCustomer.TabIndex = 7;
            this.labelCustomer.Text = "Customer:";
            // 
            // ReservationManagementForm
            // 
            this.BackColor = System.Drawing.Color.Sienna;
            this.ClientSize = new System.Drawing.Size(864, 460);
            this.Controls.Add(this.buttonCreateNew);
            this.Controls.Add(this.buttonCancelReservation);
            this.Controls.Add(this.dataGridViewResults);
            this.Controls.Add(this.buttonSearch);
            this.Controls.Add(this.textBoxCustomer);
            this.Controls.Add(this.dateTimePickerDay);
            this.Controls.Add(this.comboBoxField);
            this.Controls.Add(this.labelCustomer);
            this.Controls.Add(this.labelDay);
            this.Controls.Add(this.labelField);
            this.Name = "ReservationManagementForm";
            this.Text = "Reservation Management";
            this.Load += new System.EventHandler(this.ReservationManagementForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewResults)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}