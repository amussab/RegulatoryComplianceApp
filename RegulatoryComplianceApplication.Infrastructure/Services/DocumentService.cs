using Microsoft.EntityFrameworkCore;
using RegulatoryComplianceApplication.Core.Entities;
using RegulatoryComplianceApplication.Core.Interfaces;
using RegulatoryComplianceApplication.Infrastructure.Data;

namespace RegulatoryComplianceApplication.Infrastructure.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly AppDbContext _context;
        private readonly IAuditLogger _auditLogger;

        public DocumentService(AppDbContext context, IAuditLogger auditLogger)
        {
            _context = context;
            _auditLogger = auditLogger;
        }

        public async Task<Document?> GetByIdAsync(int documentId)
        {
            return await _context.Documents
                .Include(d => d.CurrentVersion)
                .Include(d => d.ResponsibleUsers).ThenInclude(r => r.User)
                .Include(d => d.Versions)
                .FirstOrDefaultAsync(d => d.DocumentId == documentId);
        }

        public async Task<IEnumerable<Document>> GetAllAsync()
        {
            return await _context.Documents
                .Include(d => d.CurrentVersion)
                .Where(d => !d.IsDeleted)
                .ToListAsync();
        }

        public async Task<Document> CreateAsync(Document document, DocumentVersion firstVersion, int uploadedByUserId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Documents.Add(document);
                await _context.SaveChangesAsync();

                firstVersion.DocumentId = document.DocumentId;
                firstVersion.VersionNumber = 1;
                firstVersion.UploadedByUserId = uploadedByUserId;
                firstVersion.UploadedAt = DateTime.UtcNow;
                _context.DocumentVersions.Add(firstVersion);
                await _context.SaveChangesAsync();

                document.CurrentVersionId = firstVersion.DocumentVersionId;

                _context.DocumentResponsibleUsers.Add(new DocumentResponsibleUser
                {
                    DocumentId = document.DocumentId,
                    UserId = uploadedByUserId,
                    IsUploader = true,
                    AddedByUserId = uploadedByUserId,
                    AddedAt = DateTime.UtcNow
                });

                await _context.SaveChangesAsync();
                await _auditLogger.LogAsync(uploadedByUserId, "Create", "Document", document.DocumentId);

                await transaction.CommitAsync();
                return document;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<DocumentVersion> RenewAsync(int documentId, DocumentVersion newVersion, int uploadedByUserId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var document = await _context.Documents.FindAsync(documentId)
                    ?? throw new InvalidOperationException("Document not found.");

                var lastVersionNumber = await _context.DocumentVersions
                    .Where(v => v.DocumentId == documentId)
                    .MaxAsync(v => v.VersionNumber);

                newVersion.DocumentId = documentId;
                newVersion.VersionNumber = lastVersionNumber + 1;
                newVersion.UploadedByUserId = uploadedByUserId;
                newVersion.UploadedAt = DateTime.UtcNow;

                _context.DocumentVersions.Add(newVersion);
                await _context.SaveChangesAsync();

                var oldVersionId = document.CurrentVersionId;
                document.CurrentVersionId = newVersion.DocumentVersionId;
                await _context.SaveChangesAsync();

                await _auditLogger.LogAsync(uploadedByUserId, "Renew", "Document", documentId,
                    "CurrentVersionId", oldVersionId?.ToString(), newVersion.DocumentVersionId.ToString());

                await transaction.CommitAsync();
                return newVersion;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<Document>> GetExpiringSoonAsync(int daysThreshold)
        {
            var cutoff = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(daysThreshold));

            return await _context.Documents
                .Include(d => d.CurrentVersion)
                .Where(d => !d.IsDeleted
                    && d.CurrentVersion != null
                    && d.CurrentVersion.ExpiryDate <= cutoff
                    && d.CurrentVersion.ExpiryDate >= DateOnly.FromDateTime(DateTime.UtcNow))
                .ToListAsync();
        }
    }
}