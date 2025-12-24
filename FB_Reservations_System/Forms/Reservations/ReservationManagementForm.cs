using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using FB_Reservations_System.DataAccess;

namespace FB_Reservations_System
{
    /// <summary>
    /// Search and manage reservations.
    /// </summary>
    public partial class ReservationManagementForm : Form
    {
        public ReservationManagementForm()
        {
            InitializeComponent();
        }

        private void ReservationManagementForm_Load(object sender, EventArgs e)
        {
            LoadFields();
            this.dateTimePickerDay.Value = DateTime.Today;
            LoadSearchResults();
            this.buttonCancelReservation.Enabled = false;
        }

        private void LoadFields()
        {
            try
            {
                var dt = DbHelper.ExecuteQuery("SELECT FieldID, FieldName FROM Fields WHERE IsActive = 1 ORDER BY FieldName", CommandType.Text);
                var row = dt.NewRow();
                row["FieldID"] = DBNull.Value;
                row["FieldName"] = "All Fields";
                dt.Rows.InsertAt(row, 0);

                this.comboBoxField.DisplayMember = "FieldName";
                this.comboBoxField.ValueMember = "FieldID";
                this.comboBoxField.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading fields: " + ex.Message);
            }
        }

        // Reusable search caller that uses dbo.SearchReservations with exact parameters
        private DataTable ExecuteSearchReservations(DateTime? dateFilter, int? fieldIdFilter, string customerNameFilter)
        {
            var dt = new DataTable();
            var connString = ConfigurationManager.ConnectionStrings["FootballDB"].ConnectionString;

            using (var conn = new SqlConnection(connString))
            using (var cmd = new SqlCommand("dbo.SearchReservations", conn))
            using (var adapter = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // @Date parameter (nullable)
                if (dateFilter.HasValue)
                    cmd.Parameters.Add(new SqlParameter("@Date", SqlDbType.DateTime) { Value = dateFilter.Value.Date });
                else
                    cmd.Parameters.Add(new SqlParameter("@Date", SqlDbType.DateTime) { Value = DBNull.Value });

                // @FieldID parameter (nullable)
                if (fieldIdFilter.HasValue)
                    cmd.Parameters.Add(new SqlParameter("@FieldID", SqlDbType.Int) { Value = fieldIdFilter.Value });
                else
                    cmd.Parameters.Add(new SqlParameter("@FieldID", SqlDbType.Int) { Value = DBNull.Value });

                // @CustomerName parameter (nullable) - caller supplies pattern
                if (!string.IsNullOrWhiteSpace(customerNameFilter))
                    cmd.Parameters.Add(new SqlParameter("@CustomerName", SqlDbType.NVarChar, 200) { Value = "%" + customerNameFilter.Trim() + "%" });
                else
                    cmd.Parameters.Add(new SqlParameter("@CustomerName", SqlDbType.NVarChar, 200) { Value = DBNull.Value });

                adapter.Fill(dt);
            }

            return dt;
        }

        private void LoadSearchResults()
        {
            try
            {
                DateTime? dateFilter = this.dateTimePickerDay.Checked ? (DateTime?)this.dateTimePickerDay.Value.Date : null;
                int? fieldIdFilter = (this.comboBoxField.SelectedValue == null || this.comboBoxField.SelectedValue == DBNull.Value)
                    ? (int?)null
                    : Convert.ToInt32(this.comboBoxField.SelectedValue);
                string customerFilter = string.IsNullOrWhiteSpace(this.textBoxCustomer.Text) ? null : this.textBoxCustomer.Text.Trim();

                var results = ExecuteSearchReservations(dateFilter, fieldIdFilter, customerFilter);
                this.dataGridViewResults.DataSource = results;

                if (this.dataGridViewResults.Columns.Contains("ReservationID"))
                    this.dataGridViewResults.Columns["ReservationID"].ReadOnly = true;

                this.buttonCancelReservation.Enabled = false;
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("Database error while searching reservations: " + sqlEx.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected error while searching reservations: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            LoadSearchResults();
        }

        private void dataGridViewResults_SelectionChanged(object sender, EventArgs e)
        {
            this.buttonCancelReservation.Enabled = this.dataGridViewResults.SelectedRows.Count > 0;
        }

        private void buttonCancelReservation_Click(object sender, EventArgs e)
        {
            if (this.dataGridViewResults.SelectedRows.Count == 0)
            {
                return;
            }

            var row = this.dataGridViewResults.SelectedRows[0];
            var idObj = row.Cells["ReservationID"].Value;
            if (idObj == null || idObj == DBNull.Value)
            {
                MessageBox.Show("Invalid reservation selection.");
                return;
            }

            var reservationId = Convert.ToInt32(idObj);

            var confirm = MessageBox.Show("Are you sure you want to cancel this reservation?", "Confirm Cancel", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                // Use fully qualified stored procedure name and exact parameter name
                var parameters = new[]
                {
                    new SqlParameter("@ReservationID", SqlDbType.Int) { Value = reservationId },
                };

                DbHelper.ExecuteNonQuery("dbo.CancelReservation", CommandType.StoredProcedure, parameters);
                MessageBox.Show("Reservation canceled successfully.");
                LoadSearchResults();
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("Database error while canceling reservation: " + sqlEx.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error canceling reservation: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonCreateNew_Click(object sender, EventArgs e)
        {
            using (var frm = new CreateReservationForm())
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    LoadSearchResults();
                }
            }
        }
    }
}