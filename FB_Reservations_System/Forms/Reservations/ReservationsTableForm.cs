using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using FB_Reservations_System.DataAccess;

namespace FB_Reservations_System
{
    /// <summary>
    /// Display all reservations, filtered by day and field.
    /// </summary>
    public partial class ReservationsTableForm : Form
    {
        public ReservationsTableForm()
        {
            InitializeComponent();
        }

        private void ReservationsTableForm_Load(object sender, EventArgs e)
        {
            LoadFields();
            LoadDays();
            LoadReservations();
        }

        private void LoadFields()
        {
            try
            {
                var dt = DbHelper.ExecuteQuery("SELECT FieldID, FieldName FROM Fields ORDER BY FieldName", CommandType.Text);
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

        private void LoadDays()
        {
            try
            {
                var dt = DbHelper.ExecuteQuery("SELECT DISTINCT CAST(StartTime AS DATE) AS ReservationDate FROM Reservations ORDER BY ReservationDate", CommandType.Text);
                var table = new DataTable();
                table.Columns.Add("ReservationDate", typeof(object));
                table.Rows.Add(DBNull.Value);
                foreach (DataRow r in dt.Rows)
                {
                    table.Rows.Add(r["ReservationDate"]);
                }

                this.comboBoxDay.DisplayMember = "ReservationDate";
                this.comboBoxDay.ValueMember = "ReservationDate";
                this.comboBoxDay.DataSource = table;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading days: " + ex.Message);
            }
        }

        private void LoadReservations()
        {
            try
            {
                var dayVal = this.comboBoxDay.SelectedValue;
                var fieldVal = this.comboBoxField.SelectedValue;

                // Load all rows from the correct view name dbo.ViewReservations
                var dt = DbHelper.ExecuteQuery("SELECT ReservationID, CustomerName, Phone, FieldName, StartTime, EndTime, TotalPrice, Status FROM dbo.ViewReservations", CommandType.Text);

                // Apply client-side filters safely (no SQL concatenation)
                var dv = dt.DefaultView;
                var filters = new System.Collections.Generic.List<string>();
                if (dayVal != null && dayVal != DBNull.Value)
                {
                    var date = Convert.ToDateTime(dayVal).Date;
                    var nextDate = date.AddDays(1);

                    // Use date range filter because RowFilter doesn't support CONVERT/CAST.
                    // DataView RowFilter uses # delimiters for dates and expects US format MM/dd/yyyy.
                    filters.Add($"StartTime >= #{date:MM/dd/yyyy}# AND StartTime < #{nextDate:MM/dd/yyyy}#");
                }
                if (fieldVal != null && fieldVal != DBNull.Value)
                {
                    // FieldName match based on currently selected field text
                    filters.Add($"FieldName = '{this.comboBoxField.Text.Replace("'", "''")}'");
                }
                dv.RowFilter = string.Join(" AND ", filters);
                this.dataGridViewTable.DataSource = dv;
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("Database error while loading reservations: " + sqlEx.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading reservations: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void comboBoxField_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadReservations();
        }

        private void comboBoxDay_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadReservations();
        }
    }
}