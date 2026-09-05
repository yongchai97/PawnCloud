using Abp.BlobStoring;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawnCloud.CustomerPictures;
using PawnCloud.CustomerPictures.Dto;
using PawnCloud.Customers;
using PawnCloud.PawnTicketDocuments.Dto;
using PawnCloud.PawnTickets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.PawnTicketDocuments
{
    public class PawnTicketDocumentAppService : PawnCloudAppServiceBase
    {
        private readonly IRepository<PawnTicket, int> _pawnTicketRepository;

        private readonly IRepository<PawnTicketDocument, int>
            _pawnTicketDocumentRepository;

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
        public PawnTicketDocumentAppService(
            IRepository<PawnTicket, int> pawnTicketRepository,
            IRepository<PawnTicketDocument, int> pawnTicketDocumentRepository,
            IBlobContainer blobContainer)
        {
            _pawnTicketRepository = pawnTicketRepository;
            _pawnTicketDocumentRepository =
                pawnTicketDocumentRepository;
            _blobContainer = blobContainer;
        }
        public async Task<PawnTicketDocument> UploadAsync(
            [FromForm] UploadPawnTicketDocumentInput input)
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

            // Make sure the pawn ticket exists.
            await _pawnTicketRepository.GetAsync(
                input.PawnTicket);



            int? tenantId = AbpSession.TenantId == null ? null :
                AbpSession.TenantId.Value;

            var blobName =
                GenerateBlobName(
                    tenantId,
                    input.PawnTicket,
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

                var document = new PawnTicketDocument
                {
                    TenantId = tenantId,
                    PawnTicket = input.PawnTicket,
                    BlobName = blobName,
                    OriginalFileName = input.File.FileName,
                    ContentType = input.File.ContentType,
                    FileSize = input.File.Length
                };

                var documentId =
                    await _pawnTicketDocumentRepository     
                        .InsertAndGetIdAsync(document);

                return document;
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
            var document =
                await _pawnTicketDocumentRepository.GetAsync(id);

            await _blobContainer.DeleteAsync(
                document.BlobName);

            await _pawnTicketDocumentRepository.DeleteAsync(
                document);
        }
        public async Task<FileResult> DownloadAsync(
    int id)
        {
            var document =
                await _pawnTicketDocumentRepository.GetAsync(id);

            var stream =
                await _blobContainer.GetAsync(
                    document.BlobName);

            return new FileStreamResult(
                stream,
                document.ContentType)
            {
                FileDownloadName =
                    document.OriginalFileName
            };
        }
        public async Task<List<PawnTicketDocument>>
    GetListAsync(int pawnTicket)
        {
            // Ensure pawn ticket exists.
            await _pawnTicketRepository.GetAsync(pawnTicket);

            var documents =
                await _pawnTicketDocumentRepository
                    .GetAll()
                    .Where(x => x.PawnTicket == pawnTicket)
                    .OrderByDescending(x => x.CreationTime)
                    .ToListAsync();

            return documents;
        }
        public async Task<PawnTicketDocument>
    GetAsync(int id)
        {
            var document =
                await _pawnTicketDocumentRepository.GetAsync(id);

            return document;
        }
        private string GenerateBlobName(
    int? tenantId,
    long pawnTicketId,
    string extension)
        {
            return
                $"tenant-{tenantId}/pawnTickets/" +
                $"{pawnTicketId}/documents/" +
                $"{Guid.NewGuid():N}{extension}";
        }
    }
}
