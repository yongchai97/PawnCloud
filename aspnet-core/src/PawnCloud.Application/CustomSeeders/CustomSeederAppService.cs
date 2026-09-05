using Abp.Application.Services;
using Abp.Domain.Repositories;
using PawnCloud.BasicCodes;
using PawnCloud.Countries;
using PawnCloud.GoldTypes;
using PawnCloud.ItemStatuses;
using PawnCloud.MiscMasterConfigs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.CustomSeeders
{
    public class CustomSeederAppService : ApplicationService
    {
        private readonly IRepository<BasicCode, int> _BasicCode;
        private readonly IRepository<MiscMasterConfig, int> _MiscMasterConfig;
        private readonly IRepository<GoldType, int> _GoldType;
        private readonly IRepository<ItemStatus, int> _ItemStatus;
        private readonly IRepository<Country, int> _Country;

        public CustomSeederAppService(IRepository<BasicCode, int> BasicCode, IRepository<MiscMasterConfig, int> MiscMasterConfig, 
            IRepository<GoldType, int> GoldType, IRepository<ItemStatus, int> ItemStatus, IRepository<Country, int> country)
        {
            _BasicCode = BasicCode;
            _MiscMasterConfig = MiscMasterConfig;
            _GoldType = GoldType;
            _ItemStatus = ItemStatus;
            _Country = country;
        }
        public async Task SeedRootDb()
        {
            await SeedMiscMasterConfig();
            await SeedGoldType();
            await SeedItemStatus();
            await SeedCountry();
        }
        public async Task SeedSubDb()
        {
            await SeedBasicCode();
        }
        private async Task SeedBasicCode()
        {
            var allMiscMasterConfigs = await _MiscMasterConfig.GetAllListAsync();
            var raceMiscMasterConfig = allMiscMasterConfigs.FirstOrDefault(m =>m.category == "RACE");
            if (raceMiscMasterConfig != null)
            {
                await AddBasicCodeIfNotExist("BUMIPUTERA", "Bumiputera", true, raceMiscMasterConfig.Id);
                await AddBasicCodeIfNotExist("MELAYU", "Melayu", true, raceMiscMasterConfig.Id);
                await AddBasicCodeIfNotExist("CINA", "Cina", true, raceMiscMasterConfig.Id);
                await AddBasicCodeIfNotExist("INDIA", "India", true, raceMiscMasterConfig.Id);
                await AddBasicCodeIfNotExist("LAIN-LAIN", "Lain-lain", true, raceMiscMasterConfig.Id);


            }
            var genderMiscMasterConfig = allMiscMasterConfigs.FirstOrDefault(m => m.category == "GENDER");
            if (genderMiscMasterConfig != null) 
            {
                await AddBasicCodeIfNotExist("LELAKI", "Male", true, genderMiscMasterConfig.Id);
                await AddBasicCodeIfNotExist("PEREMPUAN", "Female", true, genderMiscMasterConfig.Id);
            }
            var businessNatureMiscMasterConfig = allMiscMasterConfigs.FirstOrDefault(m => m.category == "BUSINESS NATURE");
            if(businessNatureMiscMasterConfig != null)
            {
                await AddBasicCodeIfNotExist("INFORMATION TECHNOLOGY", "Information Technology", true, businessNatureMiscMasterConfig.Id); 

            }
            var maritalStatusMiscMasterConfig = allMiscMasterConfigs.FirstOrDefault(m => m.category == "MARITAL STATUS");
            if (maritalStatusMiscMasterConfig != null)
            {
                await AddBasicCodeIfNotExist("SINGLE", "Single", true, maritalStatusMiscMasterConfig.Id);
                await AddBasicCodeIfNotExist("MARRIED", "Married", true, maritalStatusMiscMasterConfig.Id);
            }
            var paymentMethodMiscMasterConfig = allMiscMasterConfigs.FirstOrDefault(m => m.category == "PAYMENT METHOD");
            if (paymentMethodMiscMasterConfig != null)
            {
                await AddBasicCodeIfNotExist("CASH", "Cash", true, paymentMethodMiscMasterConfig.Id);
                await AddBasicCodeIfNotExist("BANK TRANSFER", "Bank Transfer", true, paymentMethodMiscMasterConfig.Id);
                await AddBasicCodeIfNotExist("TOUCH N GO", "Touch N Go", true, paymentMethodMiscMasterConfig.Id);
            }
            var includedItemMiscMasterConfig = allMiscMasterConfigs.FirstOrDefault(m => m.category == "INCLUDED ITEM");
            if (includedItemMiscMasterConfig != null)
            {
                await AddBasicCodeIfNotExist("STONE", "Stone", false, includedItemMiscMasterConfig.Id);
                await AddBasicCodeIfNotExist("RIBBON", "Ribbon", false, includedItemMiscMasterConfig.Id);
                await AddBasicCodeIfNotExist("DIAMOND", "Diamond", false, includedItemMiscMasterConfig.Id);
            }
        }
        private async Task AddBasicCodeIfNotExist(string codeName, string codeDescription, bool systemProvidedValue, int? MiscMasterConfig)
        {
            var basicCodeChecker = await _BasicCode.FirstOrDefaultAsync(bc => bc.codeName == codeName && bc.MiscMasterConfig == MiscMasterConfig);
            if(basicCodeChecker != null)
            {
                return;
            }
            var basicCode = new BasicCode(codeName, codeDescription, systemProvidedValue, MiscMasterConfig);
            basicCode.TenantId = AbpSession.TenantId;
            await _BasicCode.InsertAsync(basicCode);
        }
        private async Task SeedCountry()
        {
            await TaskAddCountryIfNotExist("Afghanistan", "AF", "AFG", "004", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Åland Islands", "AX", "ALA", "248", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Albania", "AL", "ALB", "008", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Algeria", "DZ", "DZA", "012", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("American Samoa", "AS", "ASM", "016", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Andorra", "AD", "AND", "020", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Angola", "AO", "AGO", "024", "HIGH", true, 2026);
            await TaskAddCountryIfNotExist("Anguilla", "AI", "AIA", "660", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Antarctica", "AQ", "ATA", "010", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Antigua and Barbuda", "AG", "ATG", "028", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Argentina", "AR", "ARG", "032", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Armenia", "AM", "ARM", "051", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Aruba", "AW", "ABW", "533", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Australia", "AU", "AUS", "036", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Austria", "AT", "AUT", "040", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Azerbaijan", "AZ", "AZE", "031", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Bahamas (the)", "BS", "BHS", "044", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Bahrain", "BH", "BHR", "048", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Bangladesh", "BD", "BGD", "050", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Barbados", "BB", "BRB", "052", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Belarus", "BY", "BLR", "112", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Belgium", "BE", "BEL", "056", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Belize", "BZ", "BLZ", "084", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Benin", "BJ", "BEN", "204", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Bermuda", "BM", "BMU", "060", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Bhutan", "BT", "BTN", "064", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Bolivia (Plurinational State of)", "BO", "BOL", "068", "HIGH", true, 2026);
            await TaskAddCountryIfNotExist("Bonaire, Sint Eustatius and Saba", "BQ", "BES", "535", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Bosnia and Herzegovina", "BA", "BIH", "070", "HIGH", true, 2026);
            await TaskAddCountryIfNotExist("Botswana", "BW", "BWA", "072", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Bouvet Island", "BV", "BVT", "074", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Brazil", "BR", "BRA", "076", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("British Indian Ocean Territory (the)", "IO", "IOT", "086", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Brunei Darussalam", "BN", "BRN", "096", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Bulgaria", "BG", "BGR", "100", "HIGH", true, 2026);
            await TaskAddCountryIfNotExist("Burkina Faso", "BF", "BFA", "854", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Burundi", "BI", "BDI", "108", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Cabo Verde", "CV", "CPV", "132", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Cambodia", "KH", "KHM", "116", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Cameroon", "CM", "CMR", "120", "HIGH", true, 2026);
            await TaskAddCountryIfNotExist("Canada", "CA", "CAN", "124", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Cayman Islands (the)", "KY", "CYM", "136", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Central African Republic (the)", "CF", "CAF", "140", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Chad", "TD", "TCD", "148", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Chile", "CL", "CHL", "152", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("China", "CN", "CHN", "156", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Christmas Island", "CX", "CXR", "162", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Cocos (Keeling) Islands (the)", "CC", "CCK", "166", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Colombia", "CO", "COL", "170", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Comoros (the)", "KM", "COM", "174", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Congo (the Democratic Republic of the)", "CD", "COD", "180", "HIGH", true, 2026);
            await TaskAddCountryIfNotExist("Congo (the)", "CG", "COG", "178", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Cook Islands (the)", "CK", "COK", "184", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Costa Rica", "CR", "CRI", "188", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Côte d'Ivoire", "CI", "CIV", "384", "HIGH", true, 2026);
            await TaskAddCountryIfNotExist("Croatia", "HR", "HRV", "191", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Cuba", "CU", "CUB", "192", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Curaçao", "CW", "CUW", "531", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Cyprus", "CY", "CYP", "196", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Czechia", "CZ", "CZE", "203", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Denmark", "DK", "DNK", "208", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Djibouti", "DJ", "DJI", "262", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Dominica", "DM", "DMA", "212", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Dominican Republic (the)", "DO", "DOM", "214", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Ecuador", "EC", "ECU", "218", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Egypt", "EG", "EGY", "818", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("El Salvador", "SV", "SLV", "222", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Equatorial Guinea", "GQ", "GNQ", "226", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Eritrea", "ER", "ERI", "232", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Estonia", "EE", "EST", "233", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Eswatini", "SZ", "SWZ", "748", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Ethiopia", "ET", "ETH", "231", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Falkland Islands (the) [Malvinas]", "FK", "FLK", "238", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Faroe Islands (the)", "FO", "FRO", "234", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Fiji", "FJ", "FJI", "242", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Finland", "FI", "FIN", "246", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("France", "FR", "FRA", "250", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("French Guiana", "GF", "GUF", "254", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("French Polynesia", "PF", "PYF", "258", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("French Southern Territories (the)", "TF", "ATF", "260", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Gabon", "GA", "GAB", "266", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Gambia (the)", "GM", "GMB", "270", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Georgia", "GE", "GEO", "268", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Germany", "DE", "DEU", "276", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Ghana", "GH", "GHA", "288", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Gibraltar", "GI", "GIB", "292", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Greece", "GR", "GRC", "300", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Greenland", "GL", "GRL", "304", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Grenada", "GD", "GRD", "308", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Guadeloupe", "GP", "GLP", "312", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Guam", "GU", "GUM", "316", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Guatemala", "GT", "GTM", "320", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Guernsey", "GG", "GGY", "831", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Guinea", "GN", "GIN", "324", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Guinea-Bissau", "GW", "GNB", "624", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Guyana", "GY", "GUY", "328", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Haiti", "HT", "HTI", "332", "HIGH", true, 2026);
            await TaskAddCountryIfNotExist("Heard Island and McDonald Islands", "HM", "HMD", "334", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Holy See (the)", "VA", "VAT", "336", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Honduras", "HN", "HND", "340", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Hong Kong", "HK", "HKG", "344", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Hungary", "HU", "HUN", "348", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Iceland", "IS", "ISL", "352", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("India", "IN", "IND", "356", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Indonesia", "ID", "IDN", "360", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Iran (Islamic Republic of)", "IR", "IRN", "364", "HIGH", true, 2026);
            await TaskAddCountryIfNotExist("Iraq", "IQ", "IRQ", "368", "HIGH", true, 2026);
            await TaskAddCountryIfNotExist("Ireland", "IE", "IRL", "372", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Isle of Man", "IM", "IMN", "833", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Israel", "IL", "ISR", "376", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Italy", "IT", "ITA", "380", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Jamaica", "JM", "JAM", "388", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Japan", "JP", "JPN", "392", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Jersey", "JE", "JEY", "832", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Jordan", "JO", "JOR", "400", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Kazakhstan", "KZ", "KAZ", "398", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Kenya", "KE", "KEN", "404", "HIGH", true, 2026);
            await TaskAddCountryIfNotExist("Kiribati", "KI", "KIR", "296", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Korea (the Democratic People's Republic of)", "KP", "PRK", "408", "HIGH", true, 2026);
            await TaskAddCountryIfNotExist("Korea (the Republic of)", "KR", "KOR", "410", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Kuwait", "KW", "KWT", "414", "HIGH", true, 2026);
            await TaskAddCountryIfNotExist("Kyrgyzstan", "KG", "KGZ", "417", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Lao People's Democratic Republic (the)", "LA", "LAO", "418", "HIGH", true, 2026);
            await TaskAddCountryIfNotExist("Latvia", "LV", "LVA", "428", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Lebanon", "LB", "LBN", "422", "HIGH", true, 2026);
            await TaskAddCountryIfNotExist("Lesotho", "LS", "LSO", "426", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Liberia", "LR", "LBR", "430", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Libya", "LY", "LBY", "434", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Liechtenstein", "LI", "LIE", "438", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Lithuania", "LT", "LTU", "440", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Luxembourg", "LU", "LUX", "442", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Macao", "MO", "MAC", "446", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("North Macedonia", "MK", "MKD", "807", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Madagascar", "MG", "MDG", "450", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Malawi", "MW", "MWI", "454", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Malaysia", "MY", "MYS", "458", "LOW", true, 2026);
            await TaskAddCountryIfNotExist("Maldives", "MV", "MDV", "462", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Mali", "ML", "MLI", "466", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Malta", "MT", "MLT", "470", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Marshall Islands (the)", "MH", "MHL", "584", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Martinique", "MQ", "MTQ", "474", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Mauritania", "MR", "MRT", "478", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Mauritius", "MU", "MUS", "480", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Mayotte", "YT", "MYT", "175", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Mexico", "MX", "MEX", "484", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Micronesia (Federated States of)", "FM", "FSM", "583", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Moldova (the Republic of)", "MD", "MDA", "498", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Monaco", "MC", "MCO", "492", "HIGH", true, 2026);
            await TaskAddCountryIfNotExist("Mongolia", "MN", "MNG", "496", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Montenegro", "ME", "MNE", "499", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Montserrat", "MS", "MSR", "500", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Morocco", "MA", "MAR", "504", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Mozambique", "MZ", "MOZ", "508", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Myanmar", "MM", "MMR", "104", "HIGH", true, 2026);
            await TaskAddCountryIfNotExist("Namibia", "NA", "NAM", "516", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Nauru", "NR", "NRU", "520", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Nepal", "NP", "NPL", "524", "HIGH", true, 2026);
            await TaskAddCountryIfNotExist("Netherlands (the)", "NL", "NLD", "528", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("New Caledonia", "NC", "NCL", "540", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("New Zealand", "NZ", "NZL", "554", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Nicaragua", "NI", "NIC", "558", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Niger (the)", "NE", "NER", "562", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Nigeria", "NG", "NGA", "566", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Niue", "NU", "NIU", "570", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Norfolk Island", "NF", "NFK", "574", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Northern Mariana Islands (the)", "MP", "MNP", "580", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Norway", "NO", "NOR", "578", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Oman", "OM", "OMN", "512", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Pakistan", "PK", "PAK", "586", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Palau", "PW", "PLW", "585", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Palestine, State of", "PS", "PSE", "275", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Panama", "PA", "PAN", "591", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Papua New Guinea", "PG", "PNG", "598", "HIGH", true, 2026);
            await TaskAddCountryIfNotExist("Paraguay", "PY", "PRY", "600", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Peru", "PE", "PER", "604", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Philippines (the)", "PH", "PHL", "608", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Pitcairn", "PN", "PCN", "612", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Poland", "PL", "POL", "616", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Portugal", "PT", "PRT", "620", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Puerto Rico", "PR", "PRI", "630", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Qatar", "QA", "QAT", "634", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Réunion", "RE", "REU", "638", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Romania", "RO", "ROU", "642", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Russian Federation (the)", "RU", "RUS", "643", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Rwanda", "RW", "RWA", "646", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Saint Barthélemy", "BL", "BLM", "652", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Saint Helena, Ascension and Tristan da Cunha", "SH", "SHN", "654", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Saint Kitts and Nevis", "KN", "KNA", "659", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Saint Lucia", "LC", "LCA", "662", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Saint Martin (French part)", "MF", "MAF", "663", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Saint Pierre and Miquelon", "PM", "SPM", "666", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Saint Vincent and the Grenadines", "VC", "VCT", "670", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Samoa", "WS", "WSM", "882", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("San Marino", "SM", "SMR", "674", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Sao Tome and Principe", "ST", "STP", "678", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Saudi Arabia", "SA", "SAU", "682", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Senegal", "SN", "SEN", "686", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Serbia", "RS", "SRB", "688", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Seychelles", "SC", "SYC", "690", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Sierra Leone", "SL", "SLE", "694", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Singapore", "SG", "SGP", "702", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Sint Maarten (Dutch part)", "SX", "SXM", "534", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Slovakia", "SK", "SVK", "703", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Slovenia", "SI", "SVN", "705", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Solomon Islands", "SB", "SLB", "090", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Somalia", "SO", "SOM", "706", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("South Africa", "ZA", "ZAF", "710", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("South Georgia and the South Sandwich Islands", "GS", "SGS", "239", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("South Sudan", "SS", "SSD", "728", "HIGH", true, 2026);
            await TaskAddCountryIfNotExist("Spain", "ES", "ESP", "724", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Sri Lanka", "LK", "LKA", "144", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Sudan (the)", "SD", "SDN", "729", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Suriname", "SR", "SUR", "740", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Svalbard and Jan Mayen", "SJ", "SJM", "744", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Sweden", "SE", "SWE", "752", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Switzerland", "CH", "CHE", "756", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Syrian Arab Republic", "SY", "SYR", "760", "HIGH", true, 2026);
            await TaskAddCountryIfNotExist("Taiwan (Province of China)", "TW", "TWN", "158", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Tajikistan", "TJ", "TJK", "762", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Tanzania, United Republic of", "TZ", "TZA", "834", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Thailand", "TH", "THA", "764", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Timor-Leste", "TL", "TLS", "626", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Togo", "TG", "TGO", "768", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Tokelau", "TK", "TKL", "772", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Tonga", "TO", "TON", "776", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Trinidad and Tobago", "TT", "TTO", "780", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Tunisia", "TN", "TUN", "788", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Türkiye", "TR", "TUR", "792", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Turkmenistan", "TM", "TKM", "795", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Turks and Caicos Islands (the)", "TC", "TCA", "796", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Tuvalu", "TV", "TUV", "798", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Uganda", "UG", "UGA", "800", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Ukraine", "UA", "UKR", "804", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("United Arab Emirates (the)", "AE", "ARE", "784", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("United Kingdom of Great Britain and Northern Ireland (the)", "GB", "GBR", "826", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("United States Minor Outlying Islands (the)", "UM", "UMI", "581", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("United States of America (the)", "US", "USA", "840", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Uruguay", "UY", "URY", "858", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Uzbekistan", "UZ", "UZB", "860", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Vanuatu", "VU", "VUT", "548", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Venezuela (Bolivarian Republic of)", "VE", "VEN", "862", "HIGH", true, 2026);
            await TaskAddCountryIfNotExist("Viet Nam", "VN", "VNM", "704", "HIGH", true, 2026);
            await TaskAddCountryIfNotExist("Virgin Islands (British)", "VG", "VGB", "092", "HIGH", true, 2026);
            await TaskAddCountryIfNotExist("Virgin Islands (U.S.)", "VI", "VIR", "850", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Wallis and Futuna", "WF", "WLF", "876", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Western Sahara", "EH", "ESH", "732", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Yemen", "YE", "YEM", "887", "HIGH", true, 2026);
            await TaskAddCountryIfNotExist("Zambia", "ZM", "ZMB", "894", "MEDIUM", true, 2026);
            await TaskAddCountryIfNotExist("Zimbabwe", "ZW", "ZWE", "716", "MEDIUM", true, 2026);

        }
        private async Task TaskAddCountryIfNotExist(string countryName, string countryCodeTwoAlphabet, string countryCodeThreeAlphabet, string sequenceNumber, string riskLevel, bool help, int effectiveYear)
        {
            var country = await _Country.FirstOrDefaultAsync(x => x.countryName == countryName && x.effectiveYear == effectiveYear);
            if(country == null) {
                var newCountry = new Country
                {
                    countryName = countryName,
                    countryCodeTwoAlphabet = countryCodeTwoAlphabet,
                    countryCodeThreeAlphabet = countryCodeThreeAlphabet,
                    sequenceNumber = sequenceNumber,
                    riskLevel = riskLevel,
                    help = help,
                    effectiveYear = effectiveYear
                };
                newCountry.TenantId = AbpSession.TenantId;
                await _Country.InsertAsync(newCountry);
            }
        }
        private async Task SeedItemStatus()
        {
            await AddItemStatusIfNotExist("3", "(ROSAK PATAH KURANG)");
            await AddItemStatusIfNotExist("4", "(ROSAK PATAH KURANG BENGKOK)");
            await AddItemStatusIfNotExist("5", "(ROSAK PATAH KURANG BENGKOK BASAH)");
            await AddItemStatusIfNotExist("B", "(BENGKOK)");
            await AddItemStatusIfNotExist("BBS", "(BENGKOK BASAH)");
            await AddItemStatusIfNotExist("BS", "(BASAH)");
            await AddItemStatusIfNotExist("CAL", "(CALAR)");
            await AddItemStatusIfNotExist("K", "(KURANG)");
            await AddItemStatusIfNotExist("KB", "(KURANG BENGKOK)");
            await AddItemStatusIfNotExist("KBS", "(KURANG BASAH)");
            await AddItemStatusIfNotExist("P", "(PATAH)");
            await AddItemStatusIfNotExist("PBS", "(PATAH BASAH)");
            await AddItemStatusIfNotExist("PK", "(PATAH KURANG)");
            await AddItemStatusIfNotExist("R", "(ROSAK)");
            await AddItemStatusIfNotExist("RB", "(ROSAK BENGKOK)");
            await AddItemStatusIfNotExist("RBS", "(ROSAK BASAH)");
            await AddItemStatusIfNotExist("RC", "(ROSAK CALAR)");
            await AddItemStatusIfNotExist("RK", "(ROSAK KURANG)");
            await AddItemStatusIfNotExist("RP", "(ROSAK PATAH)");
        }
        private async Task AddItemStatusIfNotExist(string code, string description)
        {
            var itemStatus = await _ItemStatus.FirstOrDefaultAsync(x => x.code == code);
            if (itemStatus == null) 
            {
                var newItemStatus = new ItemStatus
                {
                    code = code,
                    description = description
                };
                newItemStatus.TenantId = AbpSession.TenantId;
                await _ItemStatus.InsertAsync(newItemStatus);
            }
        }
        private async Task SeedGoldType()
        {
            await AddGoldTypeIfNotExist("375", "375", (decimal)37.5);
            await AddGoldTypeIfNotExist("585", "585", (decimal)58.5);
            await AddGoldTypeIfNotExist("750", "750", (decimal)75.0);
            await AddGoldTypeIfNotExist("800", "800", (decimal)80.0);
            await AddGoldTypeIfNotExist("835", "835", (decimal)83.5);
            await AddGoldTypeIfNotExist("875", "875", (decimal)87.5);
            await AddGoldTypeIfNotExist("900", "900", (decimal)90.0);
            await AddGoldTypeIfNotExist("916", "916", (decimal)91.6);
            await AddGoldTypeIfNotExist("950", "950", (decimal)95.0);
            await AddGoldTypeIfNotExist("965", "965", (decimal)96.5);
            await AddGoldTypeIfNotExist("999", "999", (decimal)99.9);
            await AddGoldTypeIfNotExist("G", "STAINLESS STEEL", 0);
            await AddGoldTypeIfNotExist("H", "YELLOW GOLD", (decimal)75.0);
            await AddGoldTypeIfNotExist("I", "WHITE GOLD", 750);
            await AddGoldTypeIfNotExist("J", "EVEROSE GOLD", 750);
            await AddGoldTypeIfNotExist("K", "PLATINUM", 0);
            await AddGoldTypeIfNotExist("L", "DIAMOND", 0);
            await AddGoldTypeIfNotExist("M", "SS DIAMOND", 0);
            await AddGoldTypeIfNotExist("N", "YG DIAMOND", 0);
            await AddGoldTypeIfNotExist("O", "WG DIAMOND", 0);
            await AddGoldTypeIfNotExist("P", "PT DIAMOND", 0);
            await AddGoldTypeIfNotExist("PP", "PAPER", 0);

        }
        private async Task AddGoldTypeIfNotExist(string purity, string description, decimal defaultPercentage)
        {
            var goldType = await _GoldType.FirstOrDefaultAsync(x => x.purity == purity);
            if (goldType == null) 
            {
                var newGoldType = new GoldType
                {
                    purity = purity,
                    description = description,
                    defaultPercentage = defaultPercentage,
                    active = true
                };
                newGoldType.TenantId = AbpSession.TenantId;
                await _GoldType.InsertAsync(newGoldType);
            }
        }
        private async Task SeedMiscMasterConfig()
        {
            //await AddMiscMasterConfigIfNotExist("NATIONALITY", false);
            //await AddMiscMasterConfigIfNotExist("COUNTRY", false);
            await AddMiscMasterConfigIfNotExist("RACE", false);
            await AddMiscMasterConfigIfNotExist("GENDER", false);
            await AddMiscMasterConfigIfNotExist("BUSINESS NATURE", true);
            await AddMiscMasterConfigIfNotExist("MARITAL STATUS", false);
            await AddMiscMasterConfigIfNotExist("PAYMENT METHOD", false);
            await AddMiscMasterConfigIfNotExist("INCLUDED ITEM", true);

        }
        private async Task AddMiscMasterConfigIfNotExist(string category, bool availableForUser)
        {
            var miscMasterConfig = await _MiscMasterConfig.FirstOrDefaultAsync(x => x.category == category);
            if (miscMasterConfig == null) 
            {
                var newMiscMasterConfig = new MiscMasterConfig
                {
                    category = category,
                    availableForUser = availableForUser
                };
                newMiscMasterConfig.TenantId = AbpSession.TenantId;
                await _MiscMasterConfig.InsertAsync(newMiscMasterConfig);
            }
        }
    }
}
