namespace Health_Booking_MVC.Models.ViewModels
{
    public class BookingViewModel
    {
        public List<Hospital> Hospitals { get; set; } = new();
        public List<Specialization> Specializations { get; set; } = new();
    }
}