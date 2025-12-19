using Google.Cloud.Firestore;
using VotoSeguro.API.DTOs;
using VotoSeguro.API.Models;
using Firebase.Storage;

namespace VotoSeguro.API.Services;

public interface ICandidateService
{
    Task<List<CandidateDto>> GetAllCandidates();
    Task<CandidateDto?> GetCandidateById(string candidateId);
    Task<CandidateDto> CreateCandidate(CreateCandidateDto createDto, string createdBy);
    Task<CandidateDto> UpdateCandidate(string candidateId, UpdateCandidateDto updateDto);
    Task DeleteCandidate(string candidateId);
}

public class CandidateService : ICandidateService
{
    private readonly FirebaseService _firebaseService;
    private const string StorageBucket = "votoseguro-21502.appspot.com";

    public CandidateService(FirebaseService firebaseService)
    {
        _firebaseService = firebaseService;
    }

    public async Task<List<CandidateDto>> GetAllCandidates()
    {
        var snapshot = await _firebaseService.CandidatesCollection.GetSnapshotAsync();
        var candidates = new List<CandidateDto>();

        foreach (var doc in snapshot.Documents)
        {
            var candidate = doc.ConvertTo<Candidate>();
            candidates.Add(MapToDto(candidate));
        }

        return candidates.OrderBy(c => c.Name).ToList();
    }

    public async Task<CandidateDto?> GetCandidateById(string candidateId)
    {
        var doc = await _firebaseService.CandidatesCollection.Document(candidateId).GetSnapshotAsync();
        if (!doc.Exists)
        {
            return null;
        }

        var candidate = doc.ConvertTo<Candidate>();
        return MapToDto(candidate);
    }

    public async Task<CandidateDto> CreateCandidate(CreateCandidateDto createDto, string createdBy)
    {
        var candidateId = Guid.NewGuid().ToString();

        // Subir imágenes a Firebase Storage o usar URL directa
        var photoUrl = !string.IsNullOrEmpty(createDto.PhotoBase64)
            ? await UploadImageToStorage(createDto.PhotoBase64, $"candidates/photos/{candidateId}.jpg")
            : createDto.ImageUrl ?? "";

        var logoUrl = !string.IsNullOrEmpty(createDto.LogoBase64)
            ? await UploadImageToStorage(createDto.LogoBase64, $"candidates/logos/{candidateId}.jpg")
            : "";

        var candidate = new Candidate
        {
            Id = candidateId,
            Name = createDto.Name,
            Party = createDto.Party,
            Description = createDto.Description,
            PhotoUrl = photoUrl,
            LogoUrl = logoUrl,
            Proposals = createDto.Proposals,
            VoteCount = 0,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        await _firebaseService.CandidatesCollection.Document(candidateId).SetAsync(candidate);

        return MapToDto(candidate);
    }

    public async Task<CandidateDto> UpdateCandidate(string candidateId, UpdateCandidateDto updateDto)
    {
        var doc = await _firebaseService.CandidatesCollection.Document(candidateId).GetSnapshotAsync();
        if (!doc.Exists)
        {
            throw new InvalidOperationException("Candidato no encontrado");
        }

        var candidate = doc.ConvertTo<Candidate>();

        // Actualizar campos
        candidate.Name = updateDto.Name;
        candidate.Party = updateDto.Party;
        candidate.Description = updateDto.Description;
        candidate.Proposals = updateDto.Proposals;

        // Si se proporcionan nuevas imágenes, actualizarlas
        if (!string.IsNullOrEmpty(updateDto.PhotoBase64))
        {
            candidate.PhotoUrl = await UploadImageToStorage(updateDto.PhotoBase64, $"candidates/photos/{candidateId}.jpg");
        }

        if (!string.IsNullOrEmpty(updateDto.LogoBase64))
        {
            candidate.LogoUrl = await UploadImageToStorage(updateDto.LogoBase64, $"candidates/logos/{candidateId}.jpg");
        }
        else if (!string.IsNullOrEmpty(updateDto.ImageUrl) && string.IsNullOrEmpty(candidate.PhotoUrl))
        {
            // Fallback simplistic
            candidate.PhotoUrl = updateDto.ImageUrl;
        }

        await _firebaseService.CandidatesCollection.Document(candidateId).SetAsync(candidate);

        return MapToDto(candidate);
    }

    public async Task DeleteCandidate(string candidateId)
    {
        var doc = await _firebaseService.CandidatesCollection.Document(candidateId).GetSnapshotAsync();
        if (!doc.Exists)
        {
            throw new InvalidOperationException("Candidato no encontrado");
        }

        var candidate = doc.ConvertTo<Candidate>();

        // Verificar que no tenga votos
        if (candidate.VoteCount > 0)
        {
            throw new InvalidOperationException("No se puede eliminar un candidato que ya tiene votos");
        }

        // Eliminar de Firestore
        await _firebaseService.CandidatesCollection.Document(candidateId).DeleteAsync();

        // Eliminar imágenes de Storage
        try
        {
            var storage = new FirebaseStorage(StorageBucket);

            if (!string.IsNullOrEmpty(candidate.PhotoUrl))
            {
                // Extraer nombre del archivo de la URL o usar el ID
                await storage.Child($"candidates/photos/{candidateId}.jpg").DeleteAsync();
            }

            if (!string.IsNullOrEmpty(candidate.LogoUrl))
            {
                await storage.Child($"candidates/logos/{candidateId}.jpg").DeleteAsync();
            }
        }
        catch (Exception)
        {
            // Ignorar errores de borrado de imagen si el candidato ya fue borrado de BD
            // Esto evita inconsistencias
        }
    }

    private async Task<string> UploadImageToStorage(string base64Image, string path)
    {
        try
        {
            // Remover el prefijo de la imagen base64 (data:image/jpeg;base64,)
            var base64Data = base64Image;
            if (base64Image.Contains(","))
            {
                base64Data = base64Image.Split(',')[1];
            }

            var imageBytes = Convert.FromBase64String(base64Data);

            // Subir a Firebase Storage
            var stream = new MemoryStream(imageBytes);
            var storage = new FirebaseStorage(StorageBucket);

            var downloadUrl = await storage
                .Child(path)
                .PutAsync(stream);

            return downloadUrl;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error al subir imagen: {ex.Message}");
        }
    }

    private CandidateDto MapToDto(Candidate candidate)
    {
        return new CandidateDto
        {
            Id = candidate.Id,
            Name = candidate.Name,
            Party = candidate.Party,
            Description = candidate.Description,
            PhotoUrl = candidate.PhotoUrl,
            LogoUrl = candidate.LogoUrl,
            Proposals = candidate.Proposals,
            VoteCount = candidate.VoteCount,
            CreatedAt = candidate.CreatedAt
        };
    }
}
