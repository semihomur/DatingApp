using DatingApp.API.Models;

namespace DatingApp.API.Dtos
{
    public class ReportReturnDto
    {
        public int Id { get; set; }
        public UserReportDto Reporter  { get; set; }
        public UserReportDto ReportedUser { get; set; }
        public string Reason { get; set; }
    }
}