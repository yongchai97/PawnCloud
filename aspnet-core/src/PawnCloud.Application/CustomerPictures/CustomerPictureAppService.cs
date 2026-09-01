using Abp.BlobStoring;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawnCloud.CustomerPictures.Dto;
using PawnCloud.Customers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.CustomerPictures
{
    public class CustomerPictureAppService : PawnCloudAppServiceBase
    {
        private readonly IRepository<Customer, int> _customerRepository;

        private readonly IRepository<CustomerPicture, int>
            _customerPictureRepository;

        private readonly IBlobContainer _blobContainer;

        private static readonly string[] AllowedExtensions =
        {
        ".jpg",
        ".jpeg",
        ".png",
        ".webp",
        ".gif",
        ".pdf",
        ".doc",
        ".docx"
    };

        private const long MaxFileSize =
            5 * 1024 * 1024;

        public CustomerPictureAppService(
            IRepository<Customer, int> customerRepository,
            IRepository<CustomerPicture, int> customerPictureRepository,
            IBlobContainer blobContainer)
        {
            _customerRepository = customerRepository;
            _customerPictureRepository =
                customerPictureRepository;
            _blobContainer = blobContainer;
        }
        public async Task<CustomerPicture> UploadAsync(
            [FromForm] UploadCustomerPictureInput input)
        {
            if (input.File == null ||
                input.File.Length == 0)
            {
                throw new UserFriendlyException(
                    "Please select a picture.");
            }

            if (input.File.Length > MaxFileSize)
            {
                throw new UserFriendlyException(
                    "The picture cannot exceed 5 MB.");
            }

            var extension =
                Path.GetExtension(input.File.FileName)
                    .ToLowerInvariant();

            if (!AllowedExtensions.Contains(extension))
            {
                throw new UserFriendlyException(
                    "Only JPG, JPEG, PNG, WEBP, GIF, PDF, DOC and DOCX files are allowed.");
            }

            // Make sure the customer exists.
            await _customerRepository.GetAsync(
                input.Customer);

            

            int? tenantId = AbpSession.TenantId == null ? null :
                AbpSession.TenantId.Value;

            var blobName =
                GenerateBlobName(
                    tenantId,
                    input.Customer,
                    extension);

            var blobSaved = false;

            try
            {
                await using var stream =
                    input.File.OpenReadStream();

                await _blobContainer.SaveAsync(
                    blobName,
                    stream);

                blobSaved = true;

                var picture = new CustomerPicture
                {
                    TenantId = tenantId,
                    Customer = input.Customer,
                    BlobName = blobName,
                    OriginalFileName = input.File.FileName,
                    ContentType = input.File.ContentType,
                    FileSize = input.File.Length
                };

                var pictureId =
                    await _customerPictureRepository
                        .InsertAndGetIdAsync(picture);

                return picture;
            }
            catch
            {
                // Database insertion failed after blob was uploaded.
                // Remove the blob so we don't leave an orphan.
                if (blobSaved)
                {
                    await _blobContainer.DeleteAsync(
                        blobName);
                }

                throw;
            }
        }
        public async Task DeleteAsync(
    int id)
        {
            var picture =
                await _customerPictureRepository.GetAsync(id);

            await _blobContainer.DeleteAsync(
                picture.BlobName);

            await _customerPictureRepository.DeleteAsync(
                picture);
        }
        public async Task<FileResult> DownloadAsync(
    int id)
        {
            var picture =
                await _customerPictureRepository.GetAsync(id);

            var stream =
                await _blobContainer.GetAsync(
                    picture.BlobName);

            return new FileStreamResult(
                stream,
                picture.ContentType)
            {
                FileDownloadName =
                    picture.OriginalFileName
            };
        }
        public async Task<List<CustomerPicture>>
    GetListAsync(int customer)
        {
            // Ensure customer exists.
            await _customerRepository.GetAsync(customer);

            var pictures =
                await _customerPictureRepository
                    .GetAll()
                    .Where(x => x.Customer == customer)
                    .OrderByDescending(x => x.CreationTime)
                    .ToListAsync();

            return pictures;
        }
        public async Task<CustomerPicture>
    GetAsync(int id)
        {
            var picture =
                await _customerPictureRepository.GetAsync(id);

            return picture;
        }
        private string GenerateBlobName(
    int? tenantId,
    long customerId,
    string extension)
        {
            return
                $"tenant-{tenantId}/customers/" +
                $"{customerId}/pictures/" +
                $"{Guid.NewGuid():N}{extension}";
        }
    }
}
