using Abp.Application.Services;
using Abp.Domain.Repositories;
using Microsoft.Extensions.Configuration;
using PawnCloud.CustomerCDDs.Dto;
using PawnCloud.Customers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static Azure.Core.HttpHeader;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PawnCloud.CustomerCDDs
{
    public class CustomerCDDAppService : ApplicationService
    {
        private readonly IRepository<Customer> _repository;
        private readonly IRepository<CustomerCDD> _customerCDDRepository;
        private readonly IConfiguration _configuration;

        private readonly string _1988list;
        private readonly string _1718sanctionlist;
        private readonly string _mohasanctionlist;  
        private readonly string _unscrsanctionlist;
        private readonly string _ISILlist;
        public CustomerCDDAppService(IRepository<Customer> repository, IRepository<CustomerCDD> customerCDDRepository, 
            IConfiguration configuration)
        {
            _repository = repository;
            _customerCDDRepository = customerCDDRepository;
            _configuration = configuration;
            _customerCDDRepository = customerCDDRepository;
            _1988list = "1988sanction.xml";
            _1718sanctionlist = "1718sanction.xml";
            _mohasanctionlist = "SENARAI_KDN.xml";
            _unscrsanctionlist = "consolidatedLegacyByNAME.xml";
            _ISILlist = "ISILsanction.xml";
        }
        public async Task<CreateOrEditCustomerCDDDto> CheckCustomerCDD(SearchCDDDto input)
        {
            CreateOrEditCustomerCDDDto createOrEditCustomerCDDDto = input.CreateOrEditCustomerCDDDto;
            //get the path
            var rootPath = _configuration["ExternalXml:FolderPath"];
            //get the current date
            DateTime checkDate = DateTime.Now;
            var dateFolder = checkDate.ToString("yyyyMMdd");
            var folderPath = Path.Combine(
                rootPath,
                dateFolder);
            // Folder doesn't exist -> simply skip
            if (!Directory.Exists(folderPath))
            {
                bool folderFound = false;
                //check until got one, countdown 10 days
                for (int i = 0; i < 10; i++)
                {
                    var checkDateBackTrack = DateTime.Now.AddDays(-i);
                    var dateFolderBackTrack = checkDateBackTrack.ToString("yyMMdd");
                    folderPath = Path.Combine(
                        rootPath,
                        "AMLA",
                        dateFolderBackTrack);
                    if (Directory.Exists(folderPath))
                    {
                        folderFound = true;
                        break;
                    }
                }
                if(!folderFound)
                {
                    return createOrEditCustomerCDDDto;
                }
            }
            //based on the found folder, start the 5 xml checks
            createOrEditCustomerCDDDto = CheckSENARAIKdnAsync(folderPath, input.customerName, createOrEditCustomerCDDDto);
            createOrEditCustomerCDDDto = CheckConsolidatedLegacyAsync(folderPath, input.customerName, createOrEditCustomerCDDDto);
            createOrEditCustomerCDDDto = Check1988SanctionAsync(folderPath, input.customerName, createOrEditCustomerCDDDto);
            createOrEditCustomerCDDDto = CheckISILSanctionAsync(folderPath, input.customerName, createOrEditCustomerCDDDto);
            createOrEditCustomerCDDDto = Check1718SanctionAsync(folderPath, input.customerName, createOrEditCustomerCDDDto);

            return createOrEditCustomerCDDDto;
        }
        public async Task<CustomerCDD> GetViaCustomerId(int? Customer)
        {
            var entity = await _customerCDDRepository.FirstOrDefaultAsync(x => x.Customer == Customer);
            return entity;
        }
        public async Task CreateOrEdit(CreateOrEditCustomerCDDDto input)
        {
            if(input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }
        private async Task Create(CreateOrEditCustomerCDDDto input)
        {
            var customerCDD = new CustomerCDD(input.Customer, input.highNetWorth, input.businessSize, input.businessType,
                input.publicResearchCompany, input.researchDescription, input.allowAnomaly, input.measureCustomer, input.offerUnusualTransaction,
                input.nomineeService, input.nomineeCustomer, input.crossBorderCustomer, input.PaymentMode, input.DeliveryChannel,
                input.UNSCRMatching, input.MOHAMatching, input.otherMatching, input.matchingID, input.approval, input.approvedBy,
                input.matchingDescription);
            customerCDD.TenantId = AbpSession.TenantId;
            await _customerCDDRepository.InsertAsync(customerCDD);
        }
        private async Task Update(CreateOrEditCustomerCDDDto input)
        {
            var customerCDD = await _customerCDDRepository.FirstOrDefaultAsync(x => x.Id == input.Id);
            if (customerCDD != null)
            {
                customerCDD.Customer = input.Customer;
                customerCDD.highNetWorth = input.highNetWorth;
                customerCDD.businessSize = input.businessSize;
                customerCDD.businessType = input.businessType;
                customerCDD.publicResearchCompany = input.publicResearchCompany;
                customerCDD.researchDescription = input.researchDescription;
                customerCDD.allowAnomaly = input.allowAnomaly;
                customerCDD.measureCustomer = input.measureCustomer;
                customerCDD.offerUnusualTransaction = input.offerUnusualTransaction;
                customerCDD.nomineeService = input.nomineeService;
                customerCDD.nomineeCustomer = input.nomineeCustomer;
                customerCDD.crossBorderCustomer = input.crossBorderCustomer;
                customerCDD.PaymentMode = input.PaymentMode;
                customerCDD.DeliveryChannel = input.DeliveryChannel;
                customerCDD.UNSCRMatching = input.UNSCRMatching;
                customerCDD.MOHAMatching = input.MOHAMatching;
                customerCDD.otherMatching = input.otherMatching;
                customerCDD.matchingID = input.matchingID;
                customerCDD.approval = input.approval;
                customerCDD.approvedBy = input.approvedBy;
                customerCDD.matchingDescription = input.matchingDescription;
                await _customerCDDRepository.UpdateAsync(customerCDD);
            }
        }
        private CreateOrEditCustomerCDDDto CheckSENARAIKdnAsync(string folderPath,string name, CreateOrEditCustomerCDDDto createOrEditCustomerCDDDto)
        {
            var filePath = Path.Combine(
                folderPath,
                _mohasanctionlist);

            if (!File.Exists(filePath))
            {
                return createOrEditCustomerCDDDto;
            }

            var document = XDocument.Load(filePath);

            // SENARAI_KDN specific logic here
            var entries = document.Descendants("entry");

            foreach (var entry in entries)
            {
                var nameList = entry
                    .Elements("field")
                    .FirstOrDefault(x =>
                        NormalizeFieldName(x.Attribute("name")?.Value) == "(3)Name")
                    ?.Value
                    .Trim();

                if (string.IsNullOrWhiteSpace(nameList))
                {
                    continue;
                }

                if (IsNameMatch(nameList, name))
                {
                    createOrEditCustomerCDDDto.MOHAMatching = true;

                    createOrEditCustomerCDDDto.matchingID =
                        "MOHA:" + (entry.Attribute("id")?.Value ?? "");

                    createOrEditCustomerCDDDto.matchingDescription =
                        "MOHA REFERENCE:" +
                        (entry.Elements("field")
                            .FirstOrDefault(x =>
                                NormalizeFieldName(x.Attribute("name")?.Value) == "(2)Reference")
                            ?.Value ?? "");

                    return createOrEditCustomerCDDDto;
                }
            }
            /*
            var entries = document.Descendants("entry");

            foreach (var entry in entries)
            {
                var nameList = entry
                    .Elements("field")
                    .FirstOrDefault(x =>
                        x.Attribute("name")?.Value == "(3) Name")
                    ?.Value
                    .Trim();

                if (string.IsNullOrWhiteSpace(name))
                {
                    continue;
                }

                if (IsNameMatch(nameList, name))
                {

                    createOrEditCustomerCDDDto.MOHAMatching = true;
                    createOrEditCustomerCDDDto.matchingID = "MOHA:" + (entry.Attribute("id")?.Value ?? "");
                    createOrEditCustomerCDDDto.matchingDescription = "MOHA REFERENCE:" + entry.Elements("field")
                    .FirstOrDefault(x =>
                        x.Attribute("name")?.Value == "(2) Reference")
                    ?.Value ?? "";
                    return createOrEditCustomerCDDDto;
                }
            }
            */
            return createOrEditCustomerCDDDto;
        }
        private CreateOrEditCustomerCDDDto CheckConsolidatedLegacyAsync(string folderPath,string name, CreateOrEditCustomerCDDDto createOrEditCustomerCDDDto)
        {
            var filePath = Path.Combine(
                folderPath,
                _unscrsanctionlist);

            if (!File.Exists(filePath))
            {
                return createOrEditCustomerCDDDto;
            }

            var document = XDocument.Load(filePath);
            // consolidatedLegacyByNAME specific logic here
            foreach (var individual in document.Descendants("INDIVIDUAL"))
            {
                var dataId = individual
                    .Element("DATAID")
                    ?.Value
                    .Trim() ?? "";
                var reference = individual
                    .Element("REFERENCE_NUMBER")
                    ?.Value
                    .Trim() ?? "";
                // --------------------------------
                // Full name
                // --------------------------------

                var firstName = individual
                    .Element("FIRST_NAME")
                    ?.Value
                    .Trim() ?? "";

                var secondName = individual
                    .Element("SECOND_NAME")
                    ?.Value
                    .Trim() ?? "";

                var thirdName = individual
                    .Element("THIRD_NAME")
                    ?.Value
                    .Trim() ?? "";

                var fullName = string.Join(
                    " ",
                    new[]
                    {
                firstName,
                secondName,
                thirdName
                    }
                    .Where(x => !string.IsNullOrWhiteSpace(x)));

                if (IsNameMatch(fullName, name))
                {
                    createOrEditCustomerCDDDto.UNSCRMatching = true;
                    createOrEditCustomerCDDDto.matchingID += " UNSCR2231:" + dataId;
                    createOrEditCustomerCDDDto.matchingDescription += " UNSCR 2231 REFERENCE:" + reference;

                    return createOrEditCustomerCDDDto;
                }

                // --------------------------------
                // Aliases
                // --------------------------------

                foreach (var alias in individual.Elements("INDIVIDUAL_ALIAS"))
                {
                    var aliasName = alias
                        .Element("ALIAS_NAME")
                        ?.Value
                        .Trim() ?? "";

                    if (string.IsNullOrWhiteSpace(aliasName))
                    {
                        continue;
                    }

                    if (IsNameMatch(aliasName, name))
                    {
                        createOrEditCustomerCDDDto.UNSCRMatching = true;
                        createOrEditCustomerCDDDto.matchingID += " UNSCR2231:" + dataId;
                        createOrEditCustomerCDDDto.matchingDescription += " UNSCR 2231 REFERENCE:" + reference;

                        return createOrEditCustomerCDDDto;
                    }
                }
            }
            foreach (var entity in document.Descendants("ENTITY"))
            {
                var dataId = entity
                    .Element("DATAID")
                    ?.Value
                    .Trim() ?? "";

                var entityName = entity
                    .Element("FIRST_NAME")
                    ?.Value
                    .Trim() ?? "";
                var reference = entity
                    .Element("REFERENCE_NUMBER")
                    ?.Value
                    .Trim() ?? "";

                if (!string.IsNullOrWhiteSpace(entityName) &&
                    IsNameMatch(entityName, name))
                {
                    createOrEditCustomerCDDDto.UNSCRMatching = true;
                    createOrEditCustomerCDDDto.matchingID += " UNSCR2231:" + dataId;
                    createOrEditCustomerCDDDto.matchingDescription += " UNSCR 2231 ENTITY REFERENCE:" + reference;

                    return createOrEditCustomerCDDDto;
                }

                foreach (var alias in entity.Elements("ENTITY_ALIAS"))
                {
                    var aliasName = alias
                        .Element("ALIAS_NAME")
                        ?.Value
                        .Trim() ?? "";

                    if (string.IsNullOrWhiteSpace(aliasName))
                    {
                        continue;
                    }

                    if (IsNameMatch(aliasName, name))
                    {
                        createOrEditCustomerCDDDto.UNSCRMatching = true;
                        createOrEditCustomerCDDDto.matchingID += " UNSCR2231:" + dataId;
                        createOrEditCustomerCDDDto.matchingDescription += " UNSCR 2231 ENTITY REFERENCE:" + reference;

                        return createOrEditCustomerCDDDto;
                    }
                }
            }
            return createOrEditCustomerCDDDto;
        }
        private CreateOrEditCustomerCDDDto Check1988SanctionAsync(string folderPath,string name, CreateOrEditCustomerCDDDto createOrEditCustomerCDDDto)
        {
            var filePath = Path.Combine(
                folderPath,
                _1988list);

            if (!File.Exists(filePath))
            {
                return createOrEditCustomerCDDDto;
            }

            var document = XDocument.Load(filePath);

            // 1988sanction specific logic here
            foreach (var individual in document.Descendants("INDIVIDUAL"))
            {
                var dataId = individual
                    .Element("DATAID")
                    ?.Value
                    .Trim() ?? "";
                var reference = individual
                    .Element("REFERENCE_NUMBER")
                    ?.Value
                    .Trim() ?? "";
                // --------------------------------
                // Full name
                // --------------------------------

                var firstName = individual
                    .Element("FIRST_NAME")
                    ?.Value
                    .Trim() ?? "";

                var secondName = individual
                    .Element("SECOND_NAME")
                    ?.Value
                    .Trim() ?? "";

                var thirdName = individual
                    .Element("THIRD_NAME")
                    ?.Value
                    .Trim() ?? "";

                var fullName = string.Join(
                    " ",
                    new[]
                    {
                firstName,
                secondName,
                thirdName
                    }
                    .Where(x => !string.IsNullOrWhiteSpace(x)));

                if (IsNameMatch(fullName, name))
                {
                    createOrEditCustomerCDDDto.UNSCRMatching = true;
                    createOrEditCustomerCDDDto.matchingID += " UNSCR1988:" + dataId;
                    createOrEditCustomerCDDDto.matchingDescription += " UNSCR 1988 REFERENCE:" + reference;

                    return createOrEditCustomerCDDDto;
                }

                // --------------------------------
                // Aliases
                // --------------------------------

                foreach (var alias in individual.Elements("INDIVIDUAL_ALIAS"))
                {
                    var aliasName = alias
                        .Element("ALIAS_NAME")
                        ?.Value
                        .Trim() ?? "";

                    if (string.IsNullOrWhiteSpace(aliasName))
                    {
                        continue;
                    }

                    if (IsNameMatch(aliasName, name))
                    {
                        createOrEditCustomerCDDDto.UNSCRMatching = true;
                        createOrEditCustomerCDDDto.matchingID += " UNSCR1988:" + dataId;
                        createOrEditCustomerCDDDto.matchingDescription += " UNSCR 1988 REFERENCE:" + reference;

                        return createOrEditCustomerCDDDto;
                    }
                }
            }
            foreach (var entity in document.Descendants("ENTITY"))
            {
                var dataId = entity
                    .Element("DATAID")
                    ?.Value
                    .Trim() ?? "";

                var entityName = entity
                    .Element("FIRST_NAME")
                    ?.Value
                    .Trim() ?? "";
                var reference = entity
                    .Element("REFERENCE_NUMBER")
                    ?.Value
                    .Trim() ?? "";

                if (!string.IsNullOrWhiteSpace(entityName) &&
                    IsNameMatch(entityName, name))
                {
                    createOrEditCustomerCDDDto.UNSCRMatching = true;
                    createOrEditCustomerCDDDto.matchingID += " UNSCR1988:" + dataId;
                    createOrEditCustomerCDDDto.matchingDescription += " UNSCR 1988 ENTITY REFERENCE:" + reference;

                    return createOrEditCustomerCDDDto;
                }

                foreach (var alias in entity.Elements("ENTITY_ALIAS"))
                {
                    var aliasName = alias
                        .Element("ALIAS_NAME")
                        ?.Value
                        .Trim() ?? "";

                    if (string.IsNullOrWhiteSpace(aliasName))
                    {
                        continue;
                    }

                    if (IsNameMatch(aliasName, name))
                    {
                        createOrEditCustomerCDDDto.UNSCRMatching = true;
                        createOrEditCustomerCDDDto.matchingID += " UNSCR1988:" + dataId;
                        createOrEditCustomerCDDDto.matchingDescription += " UNSCR 1988 ENTITY REFERENCE:" + reference;

                        return createOrEditCustomerCDDDto;
                    }
                }
            }
            return createOrEditCustomerCDDDto;
        }
        private CreateOrEditCustomerCDDDto CheckISILSanctionAsync(string folderPath,string name, CreateOrEditCustomerCDDDto createOrEditCustomerCDDDto)
        {
            var filePath = Path.Combine(
                folderPath,
                _ISILlist);

            if (!File.Exists(filePath))
            {
                return createOrEditCustomerCDDDto;
            }

            var document = XDocument.Load(filePath);

            // isil sanction specific logic here
            foreach (var individual in document.Descendants("INDIVIDUAL"))
            {
                var dataId = individual
                    .Element("DATAID")
                    ?.Value
                    .Trim() ?? "";
                var reference = individual
                    .Element("REFERENCE_NUMBER")
                    ?.Value
                    .Trim() ?? "";
                // --------------------------------
                // Full name
                // --------------------------------

                var firstName = individual
                    .Element("FIRST_NAME")
                    ?.Value
                    .Trim() ?? "";

                var secondName = individual
                    .Element("SECOND_NAME")
                    ?.Value
                    .Trim() ?? "";

                var thirdName = individual
                    .Element("THIRD_NAME")
                    ?.Value
                    .Trim() ?? "";

                var fullName = string.Join(
                    " ",
                    new[]
                    {
                firstName,
                secondName,
                thirdName
                    }
                    .Where(x => !string.IsNullOrWhiteSpace(x)));

                if (IsNameMatch(fullName, name))
                {
                    createOrEditCustomerCDDDto.UNSCRMatching = true;
                    createOrEditCustomerCDDDto.matchingID += " UNSCR1267:" + dataId;
                    createOrEditCustomerCDDDto.matchingDescription += " UNSCR 1267 REFERENCE:" + reference;

                    return createOrEditCustomerCDDDto;
                }

                // --------------------------------
                // Aliases
                // --------------------------------

                foreach (var alias in individual.Elements("INDIVIDUAL_ALIAS"))
                {
                    var aliasName = alias
                        .Element("ALIAS_NAME")
                        ?.Value
                        .Trim() ?? "";

                    if (string.IsNullOrWhiteSpace(aliasName))
                    {
                        continue;
                    }

                    if (IsNameMatch(aliasName, name))
                    {
                        createOrEditCustomerCDDDto.UNSCRMatching = true;
                        createOrEditCustomerCDDDto.matchingID += " UNSCR1267:" + dataId;
                        createOrEditCustomerCDDDto.matchingDescription += " UNSCR 1267 REFERENCE:" + reference;

                        return createOrEditCustomerCDDDto;
                    }
                }
            }
            foreach (var entity in document.Descendants("ENTITY"))
            {
                var dataId = entity
                    .Element("DATAID")
                    ?.Value
                    .Trim() ?? "";

                var entityName = entity
                    .Element("FIRST_NAME")
                    ?.Value
                    .Trim() ?? "";
                var reference = entity
                    .Element("REFERENCE_NUMBER")
                    ?.Value
                    .Trim() ?? "";

                if (!string.IsNullOrWhiteSpace(entityName) &&
                    IsNameMatch(entityName, name))
                {
                    createOrEditCustomerCDDDto.UNSCRMatching = true;
                    createOrEditCustomerCDDDto.matchingID += " UNSCR1267:" + dataId;
                    createOrEditCustomerCDDDto.matchingDescription += " UNSCR 1267 ENTITY REFERENCE:" + reference;

                    return createOrEditCustomerCDDDto;
                }

                foreach (var alias in entity.Elements("ENTITY_ALIAS"))
                {
                    var aliasName = alias
                        .Element("ALIAS_NAME")
                        ?.Value
                        .Trim() ?? "";

                    if (string.IsNullOrWhiteSpace(aliasName))
                    {
                        continue;
                    }

                    if (IsNameMatch(aliasName, name))
                    {
                        createOrEditCustomerCDDDto.UNSCRMatching = true;
                        createOrEditCustomerCDDDto.matchingID += " UNSCR1267:" + dataId;
                        createOrEditCustomerCDDDto.matchingDescription += " UNSCR 1267 ENTITY REFERENCE:" + reference;

                        return createOrEditCustomerCDDDto;
                    }
                }
            }
            return createOrEditCustomerCDDDto;
        }
        private CreateOrEditCustomerCDDDto Check1718SanctionAsync(string folderPath,string name, CreateOrEditCustomerCDDDto createOrEditCustomerCDDDto)
        {
            var filePath = Path.Combine(
                folderPath,
                _1718sanctionlist);

            if (!File.Exists(filePath))
            {
                return createOrEditCustomerCDDDto;
            }

            var document = XDocument.Load(filePath);

            // 1718sanction specific logic here
            foreach (var individual in document.Descendants("INDIVIDUAL"))
            {
                var dataId = individual
                    .Element("DATAID")
                    ?.Value
                    .Trim() ?? "";
                var reference = individual
                    .Element("REFERENCE_NUMBER")
                    ?.Value
                    .Trim() ?? "";
                // --------------------------------
                // Full name
                // --------------------------------

                var firstName = individual
                    .Element("FIRST_NAME")
                    ?.Value
                    .Trim() ?? "";

                var secondName = individual
                    .Element("SECOND_NAME")
                    ?.Value
                    .Trim() ?? "";

                var thirdName = individual
                    .Element("THIRD_NAME")
                    ?.Value
                    .Trim() ?? "";

                var fullName = string.Join(
                    " ",
                    new[]
                    {
                firstName,
                secondName,
                thirdName
                    }
                    .Where(x => !string.IsNullOrWhiteSpace(x)));

                if (IsNameMatch(fullName, name))
                {
                    createOrEditCustomerCDDDto.UNSCRMatching = true;
                    createOrEditCustomerCDDDto.matchingID += " UNSCR1718:" + dataId;
                    createOrEditCustomerCDDDto.matchingDescription += " UNSCR 1718 REFERENCE:" + reference;

                    return createOrEditCustomerCDDDto;
                }

                // --------------------------------
                // Aliases
                // --------------------------------

                foreach (var alias in individual.Elements("INDIVIDUAL_ALIAS"))
                {
                    var aliasName = alias
                        .Element("ALIAS_NAME")
                        ?.Value
                        .Trim() ?? "";

                    if (string.IsNullOrWhiteSpace(aliasName))
                    {
                        continue;
                    }

                    if (IsNameMatch(aliasName, name))
                    {
                        createOrEditCustomerCDDDto.UNSCRMatching = true;
                        createOrEditCustomerCDDDto.matchingID += " UNSCR1718:" + dataId;
                        createOrEditCustomerCDDDto.matchingDescription += " UNSCR 1718 REFERENCE:" + reference;

                        return createOrEditCustomerCDDDto;
                    }
                }
            }
            foreach (var entity in document.Descendants("ENTITY"))
            {
                var dataId = entity
                    .Element("DATAID")
                    ?.Value
                    .Trim() ?? "";

                var entityName = entity
                    .Element("FIRST_NAME")
                    ?.Value
                    .Trim() ?? "";
                var reference = entity
                    .Element("REFERENCE_NUMBER")
                    ?.Value
                    .Trim() ?? "";

                if (!string.IsNullOrWhiteSpace(entityName) &&
                    IsNameMatch(entityName, name))
                {
                    createOrEditCustomerCDDDto.UNSCRMatching = true;
                    createOrEditCustomerCDDDto.matchingID += " UNSCR1718:" + dataId;
                    createOrEditCustomerCDDDto.matchingDescription += " UNSCR 1718 ENTITY REFERENCE:" + reference;

                    return createOrEditCustomerCDDDto;
                }

                foreach (var alias in entity.Elements("ENTITY_ALIAS"))
                {
                    var aliasName = alias
                        .Element("ALIAS_NAME")
                        ?.Value
                        .Trim() ?? "";

                    if (string.IsNullOrWhiteSpace(aliasName))
                    {
                        continue;
                    }

                    if (IsNameMatch(aliasName, name))
                    {
                        createOrEditCustomerCDDDto.UNSCRMatching = true;
                        createOrEditCustomerCDDDto.matchingID += " UNSCR1718:" + dataId;
                        createOrEditCustomerCDDDto.matchingDescription += " UNSCR 1718 ENTITY REFERENCE:" + reference;

                        return createOrEditCustomerCDDDto;
                    }
                }
            }
            return createOrEditCustomerCDDDto;
        }
        private string NormalizeName(string value)
        {
            if(string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }
            return string.Concat(
                    value.Where(c => !char.IsWhiteSpace(c)))
                .ToUpperInvariant();
        }
        private bool IsNameMatch(string xmlName, string searchName)
        {
            var normalizedXmlName = NormalizeName(xmlName);
            var normalizedSearchName = NormalizeName(searchName);

            return normalizedXmlName.Contains(normalizedSearchName);
                //|| normalizedSearchName.Contains(normalizedXmlName);
        }
        private string NormalizeFieldName(string? value)
        {
            return string.Concat(
                value?.Where(c => !char.IsWhiteSpace(c))
                ?? []);
        }
    }
}
