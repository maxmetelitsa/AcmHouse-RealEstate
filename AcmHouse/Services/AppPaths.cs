using System;
using System.IO;

namespace AcmHouse.Services
{
    /// <summary>
    /// Folders the application reads from and writes to. When run from Visual Studio
    /// the executable lives in bin\Debug, so data folders are two levels up.
    /// </summary>
    public static class AppPaths
    {
        public static string ProjectDirectory =>
            Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;

        public static string ImagesDirectory => Path.Combine(ProjectDirectory, "Images");

        /// <summary>Contracts kept by the agency.</summary>
        public static string RentalsDirectory => Path.Combine(ProjectDirectory, "Rentals");

        /// <summary>Copies of contracts shown in the employee's profile.</summary>
        public static string UserRentalsDirectory => Path.Combine(ProjectDirectory, "UserRentals");

        public static string GetImagePath(int imageId)
        {
            return Path.Combine(ImagesDirectory, $"pic{imageId}.jpg");
        }
    }
}
