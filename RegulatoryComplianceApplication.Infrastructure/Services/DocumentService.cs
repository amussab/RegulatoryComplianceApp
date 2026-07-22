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
                .Include(d => d.DocumentType)
                .Include(d => d.CurrentVersion)
                .Include(d => d.ResponsibleUsers).ThenInclude(r => r.User)
                .Include(d => d.Versions)
                .FirstOrDefaultAsync(d => d.DocumentId == documentId);
        }

        public async Task<IEnumerable<Document>> GetAllAsync()
        {
            return await _context.Documents
                .Include(d => d.DocumentType)
                .Include(d => d.CurrentVersion)
                .Include(d => d.ResponsibleUsers)
                    .ThenInclude(ru => ru.User)
                .Where(d => !d.IsDeleted)
                .ToListAsync();
        }

        public async Task<Document> CreateAsync(
            Document document,
            DocumentVersion firstVersion,
            int uploadedByUserId,
            int responsibleUserId)
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
                await _context.SaveChangesAsync();

                _context.DocumentResponsibleUsers.Add(new DocumentResponsibleUser
                {
                    DocumentId = document.DocumentId,
                    UserId = responsibleUserId,
                    IsUploader = responsibleUserId == uploadedByUserId,
                    AddedByUserId = uploadedByUserId,
                    AddedAt = DateTime.UtcNow
                });

                await _context.SaveChangesAsync();

                await _auditLogger.LogAsync(
                    uploadedByUserId,
                    "Create",
                    "Document",
                    document.DocumentId);

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
                
                .Include(d => d.DocumentType)
                .Include(d => d.CurrentVersion)
                .Where(d => !d.IsDeleted
                    && d.DocumentType.IsExpirable
                    && d.CurrentVersion != null
                    && d.CurrentVersion.ExpiryDate <= cutoff
                    && d.CurrentVersion.ExpiryDate >= DateOnly.FromDateTime(DateTime.UtcNow))
                .ToListAsync();
        }
        public async Task SoftDeleteAsync(int documentId, int deletedByUserId)
        {
            var document = await _context.Documents
                .FirstOrDefaultAsync(d => d.DocumentId == documentId);

            if (document == null)
                throw new InvalidOperationException("Document not found.");

            if (document.IsDeleted)
                return;

            document.IsDeleted = true;

            await _context.SaveChangesAsync();

            await _auditLogger.LogAsync(
                deletedByUserId,
                "Delete",
                "Document",
                document.DocumentId);
        }
        public async Task UpdateAsync(
            Document document,
            DocumentVersion currentVersion,
            int responsibleUserId,
            int editedByUserId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var existingDocument = await _context.Documents
                    .Include(d => d.DocumentType)
                    .Include(d => d.CurrentVersion)
                    .Include(d => d.ResponsibleUsers)
                    .FirstOrDefaultAsync(d => d.DocumentId == document.DocumentId);

                if (existingDocument == null)
                    throw new InvalidOperationException("Document not found.");

                var originalDocument = new Document
                {
                    DocumentId = existingDocument.DocumentId,
                    Title = existingDocument.Title,
                    DocumentNumber = existingDocument.DocumentNumber,
                    DocumentTypeId = existingDocument.DocumentTypeId
                };

                var originalVersion = new DocumentVersion
                {
                    IssueDate = existingDocument.CurrentVersion!.IssueDate,
                    ExpiryDate = existingDocument.CurrentVersion.ExpiryDate,
                    FilePath = existingDocument.CurrentVersion.FilePath
                };

                existingDocument.Title = document.Title;
                existingDocument.DocumentNumber = document.DocumentNumber;
                existingDocument.DocumentTypeId = document.DocumentTypeId;

                if (existingDocument.CurrentVersion == null)
                    throw new InvalidOperationException("Current document version not found.");

                existingDocument.CurrentVersion.IssueDate = currentVersion.IssueDate;
                existingDocument.CurrentVersion.ExpiryDate = currentVersion.ExpiryDate;

                if (!string.IsNullOrWhiteSpace(currentVersion.FilePath)) 
                {
                    existingDocument.CurrentVersion.FilePath = currentVersion.FilePath;
                }

                var oldResponsible = existingDocument.ResponsibleUsers.FirstOrDefault();
                var oldResponsibleId = oldResponsible?.UserId;

                if (oldResponsible != null)
                {
                    _context.DocumentResponsibleUsers.Remove(oldResponsible);

                    await _context.SaveChangesAsync();
                }

                _context.DocumentResponsibleUsers.Add(new DocumentResponsibleUser
                {
                    DocumentId = existingDocument.DocumentId,
                    UserId = responsibleUserId,
                    IsUploader = responsibleUserId == currentVersion.UploadedByUserId,
                    AddedByUserId = editedByUserId,
                    AddedAt = DateTime.UtcNow
                });

                await _context.SaveChangesAsync();

                if (oldResponsibleId != responsibleUserId)
                {
                    await _auditLogger.LogAsync(
                        editedByUserId,
                        "Edit",
                        "Document",
                        existingDocument.DocumentId,
                        "ResponsibleUser",
                        oldResponsibleId?.ToString(),
                        responsibleUserId.ToString());
                }

                await _auditLogger.LogChangesAsync(
                    editedByUserId,
                    "Edit",
                    "Document",
                    existingDocument.DocumentId,
                    originalDocument,
                    existingDocument,
                    nameof(Document.Title),
                    nameof(Document.DocumentNumber),
                    nameof(Document.DocumentTypeId));

                await _auditLogger.LogChangesAsync(
                    editedByUserId,
                    "Edit",
                    "Document",
                    existingDocument.DocumentId,
                    originalVersion,
                    existingDocument.CurrentVersion!,
                    nameof(DocumentVersion.IssueDate),
                    nameof(DocumentVersion.ExpiryDate),
                    nameof(DocumentVersion.FilePath));

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}