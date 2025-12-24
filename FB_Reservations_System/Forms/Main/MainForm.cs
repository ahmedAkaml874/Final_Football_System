using System;
using System.Windows.Forms;

namespace FB_Reservations_System
{
    /// <summary>
    /// Main menu with navigation to Reservation Management and Reservations Table.
    /// </summary>
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void buttonNewReservation_Click(object sender, EventArgs e)
        {
            using (var frm = new ReservationManagementForm())
            {
                frm.ShowDialog(this);
            }
        }

        private void buttonReservationsTable_Click(object sender, EventArgs e)
        {
            using (var frm = new ReservationsTableForm())
            {
                frm.ShowDialog(this);
            }
        }
    }
}