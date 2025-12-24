using System;

namespace FB_Reservations_System.Models
{
    public class Field
    {
        public int FieldID { get; set; }
        public string FieldName { get; set; }
        public decimal PricePerHour { get; set; }
        public bool IsActive { get; set; }
    }

    public class Reservation
    {
        public int ReservationID { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string FieldName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
    }
}