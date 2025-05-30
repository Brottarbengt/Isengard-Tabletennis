using DataAccessLayer.DTOs;
using DataAccessLayer.Models;
using Mapster;
using Tabletennis.ViewModels;

namespace Tabletennis.Mappings
{
    public static class MappingConfig
    {
        public static void Configure()
        {
            TypeAdapterConfig<PlayerCreateViewModel, PlayerDTO>.NewConfig()
            .Map(dest => dest.Birthday, src => DateOnly.FromDateTime(src.Birthday));

            TypeAdapterConfig<PlayerCreateViewModel, PlayerDTO>.NewConfig()
                .Map(dest => dest.Birthday, src => DateOnly.FromDateTime(src.Birthday));

            TypeAdapterConfig<PlayerDTO, PlayerCreateViewModel>.NewConfig()
                .Map(dest => dest.Birthday, src => src.Birthday.HasValue
                    ? src.Birthday.Value.ToDateTime(TimeOnly.MinValue)
                    : default);

            TypeAdapterConfig<Player, PlayerDTO>.NewConfig()
                .Map(dest => dest.FullName, src => $"{src.FirstName} {src.LastName}");

            // Mappningar för PlayerStatisticsViewModel
            TypeAdapterConfig<PlayerDTO, PlayerStatisticsViewModel>.NewConfig()
                .Map(dest => dest.FullName, src => src.BirthYear.HasValue 
                    ? $"{src.FirstName} {src.LastName} ({src.BirthYear})" 
                    : $"{src.FirstName} {src.LastName}");

            // Mappningar för Top10PlayersViewModel
            TypeAdapterConfig<PlayerDTO, Top10PlayersViewModel>.NewConfig()
                .Map(dest => dest.FullName, src => src.BirthYear.HasValue 
                    ? $"{src.FirstName} {src.LastName} ({src.BirthYear})" 
                    : $"{src.FirstName} {src.LastName}");

            // Mappningar för OpponentStatsViewModel
            TypeAdapterConfig<PlayerDTO, OpponentStatsViewModel>.NewConfig()
                .Map(dest => dest.FullName, src => src.BirthYear.HasValue 
                    ? $"{src.FirstName} {src.LastName} ({src.BirthYear})" 
                    : $"{src.FirstName} {src.LastName}");
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