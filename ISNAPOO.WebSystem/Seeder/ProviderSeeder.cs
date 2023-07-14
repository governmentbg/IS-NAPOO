using Data.Models.Data.ProviderData;
using Data.Models.DB;

namespace ISNAPOO.WebSystem.Seeder
{
    public class ProviderSeeder
    {
        public static async Task ProvidersSeeder(WebApplication applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.Services.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();

                if (!context.Providers.Any())
                {
                    context.Providers.AddRange(new List<Provider>()
                    {
                        new Provider()
                        {
                            ProviderOwner = "Mega",
                            PoviderBulstat = "133122144",
                            ZipCode = "8000",
                            ProviderAddress = "bul. Bulgaria 100",
                            ProviderPhone = "0888502987",
                            ProviderPhoneCorrespondence = "0888505587",
                            ProviderFax = "02598752",
                            ProviderWeb = "www.test.test",
                            ProviderEmail = "mega@test.com",
                            //ProviderStatusId = 1,
                            LicenceStatusId = 1,
                            LicenceNumber = 123456789
                        },

                        new Provider()
                        {
                            ProviderOwner = "Sun",
                            PoviderBulstat = "133122144",
                            ZipCode = "8000",
                            ProviderAddress = "bul. Gotse Deltchev 100",
                            ProviderPhone = "0888502987",
                            ProviderPhoneCorrespondence = "0888505587",
                            ProviderFax = "02598752",
                            ProviderWeb = "www.test.test",
                            ProviderEmail = "sun@test.com",
                          //  ProviderStatusId = 2,
                            LicenceStatusId = 2,
                            LicenceNumber = 123456789
                        },

                        new Provider()
                        {
                            ProviderOwner = "Omega",
                            PoviderBulstat = "133122144",
                            ZipCode = "8000",
                            ProviderAddress = "bul. Simeonovsko Shose 100",
                            ProviderPhone = "0888502987",
                            ProviderPhoneCorrespondence = "0888505587",
                            ProviderFax = "02598752",
                            ProviderWeb = "www.test.test",
                            ProviderEmail = "omega@test.com",
                            //ProviderStatusId = 3,
                            LicenceStatusId = 3,
                            LicenceNumber = 123456789
                        },

                        new Provider()
                        {
                            ProviderOwner = "Alpha",
                            PoviderBulstat = "133122144",
                            ZipCode = "1000",
                            ProviderAddress = "bul. Arsenalski 22",
                            ProviderPhone = "0888502987",
                            ProviderPhoneCorrespondence = "0888505587",
                            ProviderFax = "02598752",
                            ProviderWeb = "www.test.test",
                            ProviderEmail = "alpha@test.com",
                           // ProviderStatusId = 4,
                            LicenceStatusId = 4,
                            LicenceNumber = 123456789
                        },

                        new Provider()
                        {
                            ProviderOwner = "Beta",
                            PoviderBulstat = "133122144",
                            ZipCode = "8000",
                            ProviderAddress = "ul. Ivan Rilski 56",
                            ProviderPhone = "0888502987",
                            ProviderPhoneCorrespondence = "0888505587",
                            ProviderFax = "02598752",
                            ProviderWeb = "www.test.test",
                            ProviderEmail = "beta@test.com",
                          //  ProviderStatusId = 5,
                            LicenceStatusId = 5,
                            LicenceNumber = 123456789
                        }
                    });

                    context.SaveChanges();
                }
            }
        }
    }
}
