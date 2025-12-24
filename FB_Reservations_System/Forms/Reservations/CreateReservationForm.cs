using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using FB_Reservations_System.DataAccess;

namespace FB_Reservations_System
{
    /// <summary>
    /// Create a new reservation and call dbo.AddReservation stored procedure.
    /// </summary>
    public partial class CreateReservationForm : Form
    {
        public CreateReservationForm()
        {
            InitializeComponent();
        }

        private void CreateReservationForm_Load(object sender, EventArgs e)
        {
            LoadFields();
            this.dateTimePickerStart.Value = DateTime.Now.Date.AddHours(9);
            this.dateTimePickerEnd.Value = this.dateTimePickerStart.Value.AddHours(1);
            UpdatePriceDisplay();
        }

        private void LoadFields()
        {
            try
            {
                var dt = DbHelper.ExecuteQuery("SELECT FieldID, FieldName, PricePerHour FROM Fields WHERE IsActive = 1 ORDER BY FieldName", CommandType.Text);
                this.comboBoxField.DisplayMember = "FieldName";
                this.comboBoxField.ValueMember = "FieldID";
                this.comboBoxField.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading fields: " + ex.Message);
            }
        }

        private void UpdatePriceDisplay()
        {
            if (this.comboBoxField.SelectedValue == null)
            {
                this.labelPrice.Text = "$0.00";
                return;
            }

            try
            {
                var row = (this.comboBoxField.SelectedItem as DataRowView)?.Row;
                if (row == null)
                {
                    this.labelPrice.Text = "$0.00";
                    return;
                }

                var pricePerHour = Convert.ToDecimal(row["PricePerHour"]);
                var start = this.dateTimePickerStart.Value;
                var end = this.dateTimePickerEnd.Value;
                if (end <= start)
                {
                    this.labelPrice.Text = "End time must be after start time";
                    return;
                }

                var duration = (decimal)(end - start).TotalHours;
                var total = Math.Round(pricePerHour * duration, 2);
                this.labelPrice.Text = total.ToString("C2");
            }
            catch (Exception)
            {
                this.labelPrice.Text = "Error";
            }
        }

        private void dateTimePickerStart_ValueChanged(object sender, EventArgs e)
        {
            UpdatePriceDisplay();
        }

        private void dateTimePickerEnd_ValueChanged(object sender, EventArgs e)
        {
            UpdatePriceDisplay();
        }

        private void comboBoxField_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdatePriceDisplay();
        }

        private void buttonConfirm_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.textBoxCustomerName.Text))
            {
                MessageBox.Show("Customer name is required.");
                return;
            }

            if (this.comboBoxField.SelectedValue == null)
            {
                MessageBox.Show("Please select a field.");
                return;
            }

            var start = this.dateTimePickerStart.Value;
            var end = this.dateTimePickerEnd.Value;
            if (end <= start)
            {
                MessageBox.Show("End time must be after start time.");
                return;
            }

            try
            {
                var fieldId = Convert.ToInt32(this.comboBoxField.SelectedValue);
                var customerName = this.textBoxCustomerName.Text.Trim();
                var phone = this.textBoxPhone.Text.Trim();

                // Calculate price client-side for display; send calculated price to DB
                var row = (this.comboBoxField.SelectedItem as DataRowView)?.Row;
                var pricePerHour = Convert.ToDecimal(row["PricePerHour"]);
                var duration = (decimal)(end - start).TotalHours;
                var totalPrice = Math.Round(pricePerHour * duration, 2);

                // Call stored procedure dbo.AddReservation with exact parameter names
                var parameters = new[]
                {
                    new SqlParameter("@CustomerName", SqlDbType.NVarChar, 200) { Value = customerName },
                    new SqlParameter("@Phone", SqlDbType.NVarChar, 50) { Value = string.IsNullOrEmpty(phone) ? (object)DBNull.Value : phone },
                    new SqlParameter("@FieldID", SqlDbType.Int) { Value = fieldId },
                    new SqlParameter("@StartTime", SqlDbType.DateTime) { Value = start },
                    new SqlParameter("@EndTime", SqlDbType.DateTime) { Value = end },
                    new SqlParameter("@TotalPrice", SqlDbType.Decimal) { Precision = 10, Scale = 2, Value = totalPrice }
                };

                // Use fully qualified stored procedure name
                DbHelper.ExecuteNonQuery("dbo.AddReservation", CommandType.StoredProcedure, parameters);

                MessageBox.Show("Reservation created successfully.");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("Database error creating reservation: " + sqlEx.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error creating reservation: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}