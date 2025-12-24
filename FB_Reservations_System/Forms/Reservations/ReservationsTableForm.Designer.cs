using System;
using System.Windows.Forms;

namespace FB_Reservations_System
{
    partial class ReservationsTableForm
    {
        private System.ComponentModel.IContainer components = null;
        private ComboBox comboBoxDay;
        private ComboBox comboBoxField;
        private DataGridView dataGridViewTable;
        private Label labelDay;
        private Label labelField;

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
            this.comboBoxDay = new System.Windows.Forms.ComboBox();
            this.comboBoxField = new System.Windows.Forms.ComboBox();
            this.dataGridViewTable = new System.Windows.Forms.DataGridView();
            this.labelDay = new System.Windows.Forms.Label();
            this.labelField = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTable)).BeginInit();
            this.SuspendLayout();
            // 
            // comboBoxDay
            // 
            this.comboBoxDay.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDay.Location = new System.Drawing.Point(90, 12);
            this.comboBoxDay.Name = "comboBoxDay";
            this.comboBoxDay.Size = new System.Drawing.Size(160, 24);
            this.comboBoxDay.TabIndex = 2;
            this.comboBoxDay.SelectedIndexChanged += new System.EventHandler(this.comboBoxDay_SelectedIndexChanged);
            // 
            // comboBoxField
            // 
            this.comboBoxField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxField.Location = new System.Drawing.Point(345, 12);
            this.comboBoxField.Name = "comboBoxField";
            this.comboBoxField.Size = new System.Drawing.Size(200, 24);
            this.comboBoxField.TabIndex = 1;
            this.comboBoxField.SelectedIndexChanged += new System.EventHandler(this.comboBoxField_SelectedIndexChanged);
            // 
            // dataGridViewTable
            // 
            this.dataGridViewTable.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewTable.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewTable.ColumnHeadersHeight = 29;
            this.dataGridViewTable.Location = new System.Drawing.Point(15, 50);
            this.dataGridViewTable.Name = "dataGridViewTable";
            this.dataGridViewTable.ReadOnly = true;
            this.dataGridViewTable.RowHeadersWidth = 51;
            this.dataGridViewTable.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewTable.Size = new System.Drawing.Size(760, 400);
            this.dataGridViewTable.TabIndex = 0;
            // 
            // labelDay
            // 
            this.labelDay.AutoSize = true;
            this.labelDay.Location = new System.Drawing.Point(12, 15);
            this.labelDay.Name = "labelDay";
            this.labelDay.Size = new System.Drawing.Size(76, 16);
            this.labelDay.TabIndex = 4;
            this.labelDay.Text = "Select Day:";
            // 
            // labelField
            // 
            this.labelField.AutoSize = true;
            this.labelField.Location = new System.Drawing.Point(270, 15);
            this.labelField.Name = "labelField";
            this.labelField.Size = new System.Drawing.Size(81, 16);
            this.labelField.TabIndex = 3;
            this.labelField.Text = "Select Field:";
            // 
            // ReservationsTableForm
            // 
            this.BackgroundImage = global::FB_Reservations_System.Properties.Resources.img2;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(800, 470);
            this.Controls.Add(this.dataGridViewTable);
            this.Controls.Add(this.comboBoxField);
            this.Controls.Add(this.comboBoxDay);
            this.Controls.Add(this.labelField);
            this.Controls.Add(this.labelDay);
            this.Name = "ReservationsTableForm";
            this.Text = "Reservations Table";
            this.Load += new System.EventHandler(this.ReservationsTableForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTable)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}