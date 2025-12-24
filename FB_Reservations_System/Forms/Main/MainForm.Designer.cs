using System;
using System.Windows.Forms;

namespace FB_Reservations_System
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private Button buttonNewReservation;
        private Button buttonReservationsTable;

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
            this.buttonNewReservation = new System.Windows.Forms.Button();
            this.buttonReservationsTable = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonNewReservation
            // 
            this.buttonNewReservation.Location = new System.Drawing.Point(57, 191);
            this.buttonNewReservation.Name = "buttonNewReservation";
            this.buttonNewReservation.Size = new System.Drawing.Size(220, 45);
            this.buttonNewReservation.TabIndex = 0;
            this.buttonNewReservation.Text = "New Reservation";
            this.buttonNewReservation.UseVisualStyleBackColor = true;
            this.buttonNewReservation.Click += new System.EventHandler(this.buttonNewReservation_Click);
            // 
            // buttonReservationsTable
            // 
            this.buttonReservationsTable.Location = new System.Drawing.Point(383, 191);
            this.buttonReservationsTable.Name = "buttonReservationsTable";
            this.buttonReservationsTable.Size = new System.Drawing.Size(220, 45);
            this.buttonReservationsTable.TabIndex = 1;
            this.buttonReservationsTable.Text = "Reservations Table";
            this.buttonReservationsTable.UseVisualStyleBackColor = true;
            this.buttonReservationsTable.Click += new System.EventHandler(this.buttonReservationsTable_Click);
            // 
            // MainForm
            // 
            this.BackgroundImage = global::FB_Reservations_System.Properties.Resources.img1;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(675, 394);
            this.Controls.Add(this.buttonReservationsTable);
            this.Controls.Add(this.buttonNewReservation);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "R";
            this.ResumeLayout(false);

        }
    }
}