using System;
using DatingApp.API.Models;

namespace DatingApp.API.Dtos
{
    public class PhotosForReturnWithUserDto
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsMain { get; set; }
        public bool isApproved { get; set; }
        public int UserId { get; set; }
        public UserForDetailedDto User { get; set; }
    }
}