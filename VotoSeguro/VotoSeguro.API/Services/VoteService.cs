using Google.Cloud.Firestore;
using VotoSeguro.API.DTOs;
using VotoSeguro.API.Models;

namespace VotoSeguro.API.Services;

public interface IVoteService
{
    Task<VoteResponseDto> CastVote(string userId, string candidateId);
    Task<bool> HasUserVoted(string userId);
    Task<User?> GetVoteStatus(string userId);
}

public class VoteService : IVoteService
{
    private readonly FirebaseService _firebaseService;
    private readonly IAuthService _authService;

    public VoteService(FirebaseService firebaseService, IAuthService authService)
    {
        _firebaseService = firebaseService;
        _authService = authService;
    }

    public async Task<VoteResponseDto> CastVote(string userId, string candidateId)
    {
        var db = _firebaseService.GetFirestoreDb();

        try
        {
            // Usar transacción para garantizar atomicidad
            var result = await db.RunTransactionAsync(async transaction =>
            {
                // 1. Obtener el usuario
                var userRef = _firebaseService.UsersCollection.Document(userId);
                var userSnapshot = await transaction.GetSnapshotAsync(userRef);

                if (!userSnapshot.Exists)
                {
                    throw new InvalidOperationException("Usuario no encontrado");
                }

                var user = userSnapshot.ConvertTo<User>();

                // 2. Verificar que no haya votado
                if (user.HasVoted)
                {
                    throw new InvalidOperationException("Ya has votado anteriormente");
                }

                // 3. Obtener el candidato
                var candidateRef = _firebaseService.CandidatesCollection.Document(candidateId);
                var candidateSnapshot = await transaction.GetSnapshotAsync(candidateRef);

                if (!candidateSnapshot.Exists)
                {
                    throw new InvalidOperationException("Candidato no encontrado");
                }

                var candidate = candidateSnapshot.ConvertTo<Candidate>();

                // 4. Actualizar el usuario
                var voteTimestamp = DateTime.UtcNow;
                transaction.Update(userRef, new Dictionary<string, object>
                {
                    { "hasVoted", true },
                    { "votedFor", candidateId },
                    { "votedForName", candidate.Name },
                    { "voteTimestamp", voteTimestamp }
                });

                // 5. Incrementar el contador del candidato
                transaction.Update(candidateRef, "voteCount", FieldValue.Increment(1));

                // 6. Crear registro de voto (log de auditoría)
                var vote = new Vote
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userId,
                    UserName = user.FullName,
                    CandidateId = candidateId,
                    CandidateName = candidate.Name,
                    Timestamp = voteTimestamp
                };

                var voteRef = _firebaseService.VotesCollection.Document(vote.Id);
                transaction.Set(voteRef, vote);

                return new VoteResponseDto
                {
                    Success = true,
                    Message = "Voto registrado exitosamente",
                    CandidateName = candidate.Name,
                    Timestamp = voteTimestamp
                };
            });

            return result;
        }
        catch (Exception ex)
        {
            return new VoteResponseDto
            {
                Success = false,
                Message = ex.Message
            };
        }
    }

    public async Task<bool> HasUserVoted(string userId)
    {
        var user = await _authService.GetUserById(userId);
        return user?.HasVoted ?? false;
    }

    public async Task<User?> GetVoteStatus(string userId)
    {
        return await _authService.GetUserById(userId);
    }
}
