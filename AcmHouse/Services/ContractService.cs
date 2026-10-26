using System.IO;
using AcmHouse.AccommodationExtraInfo;
using AcmHouse.AccommodationInfo;
using AcmHouse.CompanyInfo;
using AcmHouse.Domain;
using AcmHouse.UsersInfo;

namespace AcmHouse.Services
{
    public interface IContractService
    {
        /// <summary>Creates the rental contract (.docx) for the agency and the employee's copy.</summary>
        void CreateRentalContracts(int orderId, Accommodation accommodation, AccommodationDetails details,
            CurrentUser employee, RentalForm form, RentalQuote quote);
    }

    public class ContractService : IContractService
    {
        public void CreateRentalContracts(int orderId, Accommodation accommodation, AccommodationDetails details,
            CurrentUser employee, RentalForm form, RentalQuote quote)
        {
            Directory.CreateDirectory(AppPaths.RentalsDirectory);
            Directory.CreateDirectory(AppPaths.UserRentalsDirectory);

            string[] outputPaths =
            {
                Path.Combine(AppPaths.RentalsDirectory, $"rental_order{orderId}.docx"),
                Path.Combine(AppPaths.UserRentalsDirectory, $"user_order{orderId}.docx")
            };

            var company = new Company();
            foreach (string path in outputPaths)
            {
                company.GenerateRentalDocument(path, accommodation, details, employee,
                    form.Surname, form.Initials, form.Email, form.Phone, form.PassportNumber, form.IdentificationNumber,
                    form.StartDate.Value, form.EndDate.Value, quote.Days, quote.Total);
            }
        }
    }
}
