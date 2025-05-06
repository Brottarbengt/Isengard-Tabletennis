namespace Tabletennis.Mappings
{
    public static class MappingConfig
    {
        public static void Configure()
        {

        }

        private static string CalculateAge(DateOnly? birthday)
        {
            if (!birthday.HasValue)
                return "Okänd";

            var today = DateOnly.FromDateTime(DateTime.Today);
            var age = today.Year - birthday.Value.Year;
            if (birthday.Value.DayNumber > today.AddYears(-age).DayNumber) age--;
            return age.ToString();
        }
    }
}