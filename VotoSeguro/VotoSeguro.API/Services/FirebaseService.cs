using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;

namespace VotoSeguro.API.Services;

public class FirebaseService
{
    private readonly FirestoreDb _firestoreDb;
    private readonly string _projectId;

    public FirebaseService(IConfiguration configuration, IWebHostEnvironment environment)
    {
        _projectId = configuration["Firebase:ProjectId"]
            ?? throw new InvalidOperationException("Firebase ProjectID no configurado");

        // 1. Construir ruta al archivo de credenciales
        var contentRoot = environment.ContentRootPath;
        var credentialPath = Path.Combine(contentRoot, "Config", "firebase-credentials.json");

        // 2. Verificar existencia
        if (!File.Exists(credentialPath))
        {
            throw new FileNotFoundException($"No se encontró el archivo de credenciales en: {credentialPath}");
        }

        // 3. Inicializar Firebase App
        if (FirebaseApp.DefaultInstance == null)
        {
            GoogleCredential credential;

            try
            {
                using (var stream = File.OpenRead(credentialPath))
                {
                    credential = GoogleCredential.FromStream(stream);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error leyendo credenciales: {ex.Message}", ex);
            }

            FirebaseApp.Create(new AppOptions
            {
                Credential = credential,
                ProjectId = _projectId
            });

            // 4. Crear instancia de Firestore usando la misma credencial
            _firestoreDb = new FirestoreDbBuilder
            {
                ProjectId = _projectId,
                Credential = credential
            }.Build();
        }
        else
        {
            // Si ya existe la App, intentamos obtener Firestore de forma estándar
            // o podrías guardarlo si lo prefieres.
            _firestoreDb = FirestoreDb.Create(_projectId);
        }
    }

    public FirestoreDb GetFirestoreDb() => _firestoreDb;

    public CollectionReference GetCollection(string collectionName)
    {
        return _firestoreDb.Collection(collectionName);
    }

    // Colecciones específicas
    public CollectionReference UsersCollection => _firestoreDb.Collection("users");
    public CollectionReference CandidatesCollection => _firestoreDb.Collection("candidates");
    public CollectionReference VotesCollection => _firestoreDb.Collection("votes");
}
