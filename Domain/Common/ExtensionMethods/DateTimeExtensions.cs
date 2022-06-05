namespace Domain.Common.ExtensionMethods
{
    public static class DateTimeExtensions
    {
        public static int CalculateAge(this DateTime dob)
        {
            var today = DateTime.Today;
            var age = today.Year - dob.Year;
            // comparing date to see if the birthday is already gone this year
            if (dob.Date > today.AddYears(-age))
            {
                age--;
            }
            return age;
        }
    }
}
