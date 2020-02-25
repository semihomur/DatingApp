namespace DatingApp.API.Models
{
    public class Reports
    {
        public int Id { get; set; }
        public int ReporterId { get; set; }
        public User Reporter  { get; set; }
        public int ReportedUserId { get; set; }
        public User ReportedUser { get; set; }
        public string Reason { get; set; }
        public bool IsDeleted { get; set; }
    }
}