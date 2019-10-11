using System;

namespace DatingApp.API.Helpers
{
    public static class Extensions
    {
        public static int CalculateAge(this DateTime theDateTime) {
            var age=DateTime.Today.Year-theDateTime.Year;
            if(theDateTime.AddYears(age) > DateTime.Today) {
                age--;
            }
            return age;
        }
    }
}