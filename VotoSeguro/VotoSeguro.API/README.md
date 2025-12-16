# VotoSeguro - Backend API

Sistema de votación digital seguro desarrollado con ASP.NET Core Web API y Firebase.

## 🚀 Tecnologías

- **ASP.NET Core 8.0** - Web API
- **Firebase Firestore** - Base de datos NoSQL
- **Firebase Storage** - Almacenamiento de imágenes
- **JWT** - Autenticación y autorización
- **BCrypt** - Hash de contraseñas

## 📋 Estructura del Proyecto

```
VotoSeguro.API/
├── Controllers/          # Endpoints de la API
│   ├── AuthController.cs
│   ├── CandidatesController.cs
│   ├── VotesController.cs
│   ├── UsersController.cs
│   └── ReportsController.cs
├── Services/             # Lógica de negocio
│   ├── FirebaseService.cs
│   ├── AuthService.cs
│   ├── CandidateService.cs
│   ├── VoteService.cs
│   ├── UserService.cs
│   └── ReportService.cs
├── Models/               # Modelos de datos
│   ├── User.cs
│   ├── Candidate.cs
│   ├── Vote.cs
│   └── VoteStatistics.cs
├── DTOs/                 # Data Transfer Objects
│   ├── AuthDto.cs
│   ├── CandidateDto.cs
│   ├── VoteDto.cs
│   └── UserDto.cs
└── Config/               # Archivos de configuración
    └── firebase-credentials.json
```

## 🔧 Configuración Inicial

### 1. Configurar Firebase

1. Ir a [Firebase Console](https://console.firebase.google.com/)
2. Crear un nuevo proyecto llamado `votoseguro-2025` (o el nombre que prefieras)
3. Habilitar **Firestore Database**
4. Habilitar **Storage**
5. Ir a **Project Settings** → **Service Accounts**
6. Click en "Generate new private key"
7. Guardar el archivo JSON descargado como `Config/firebase-credentials.json`

### 2. Actualizar Configuración

En `Services/FirebaseService.cs`, cambiar:
```csharp
private const string ProjectId = "votoseguro-2025"; // Tu ID de proyecto
```

En `Services/CandidateService.cs`, cambiar:
```csharp
private const string StorageBucket = "votoseguro-2025.appspot.com"; // Tu bucket
```

### 3. Instalar Dependencias

```bash
dotnet restore
```

### 4. Ejecutar la API

```bash
dotnet run
```

La API estará disponible en `https://localhost:5001` o `http://localhost:5000`

## 📡 Endpoints de la API

### **Autenticación** (`/api/auth`)

#### POST `/api/auth/register`
Registrar un nuevo usuario (votante por defecto)

**Request:**
```json
{
  "email": "usuario@example.com",
  "password": "contraseña123",
  "fullName": "Juan Pérez"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "userId": "abc123",
  "email": "usuario@example.com",
  "fullName": "Juan Pérez",
  "role": "votante",
  "hasVoted": false
}
```

#### POST `/api/auth/login`
Iniciar sesión

**Request:**
```json
{
  "email": "usuario@example.com",
  "password": "contraseña123"
}
```

---

### **Candidatos** (`/api/candidates`)

#### GET `/api/candidates`
Obtener todos los candidatos (público)

#### GET `/api/candidates/{id}`
Obtener un candidato específico

#### POST `/api/candidates` 🔒 (Administrador)
Crear un nuevo candidato

**Request:**
```json
{
  "name": "María González",
  "party": "Partido Verde",
  "photoBase64": "data:image/jpeg;base64,/9j/4AAQSkZJRg...",
  "logoBase64": "data:image/png;base64,iVBORw0KGgo...",
  "proposals": [
    "Reducir la contaminación",
    "Aumentar áreas verdes",
    "Transporte público gratuito"
  ]
}
```

#### PUT `/api/candidates/{id}` 🔒 (Administrador)
Actualizar un candidato

#### DELETE `/api/candidates/{id}` 🔒 (Administrador)
Eliminar un candidato (solo si no tiene votos)

---

### **Votos** (`/api/votes`)

#### POST `/api/votes` 🔒 (Votante)
Emitir un voto

**Request:**
```json
{
  "candidateId": "candidate-id-123"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Voto registrado exitosamente",
  "candidateName": "María González",
  "timestamp": "2025-12-15T21:30:00Z"
}
```

#### GET `/api/votes/status` 🔒 (Votante)
Verificar si ya votó

**Response:**
```json
{
  "hasVoted": true,
  "votedFor": "candidate-id-123",
  "votedForName": "María González",
  "voteTimestamp": "2025-12-15T21:30:00Z"
}
```

---

### **Usuarios** (`/api/users`)

#### GET `/api/users` 🔒 (Administrador)
Obtener lista de todos los votantes

#### GET `/api/users/{id}` 🔒 (Administrador)
Obtener información de un votante específico

---

### **Reportes** (`/api/reports`)

#### GET `/api/reports/statistics` 🔒 (Administrador)
Obtener estadísticas completas del proceso electoral

**Response:**
```json
{
  "totalVoters": 150,
  "totalVotes": 95,
  "participationPercentage": 63.33,
  "candidateResults": [
    {
      "id": "candidate-1",
      "name": "María González",
      "party": "Partido Verde",
      "photoUrl": "https://...",
      "logoUrl": "https://...",
      "votes": 45,
      "percentage": 47.37
    }
  ],
  "trends": [
    {
      "timestamp": "2025-12-15T10:00:00Z",
      "candidateName": "María González",
      "cumulativeVotes": 1
    }
  ]
}
```

---

## 🔐 Autenticación y Autorización

La API utiliza **JWT (JSON Web Tokens)** para autenticación.

### Roles:
- **votante**: Puede votar una vez
- **administrador**: Gestiona candidatos, ve reportes y lista de votantes

### Cómo usar el token:

1. Obtener token con `/api/auth/login` o `/api/auth/register`
2. Incluir en headers:
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### En Swagger:
1. Click en "Authorize" 🔓
2. Ingresar: `Bearer tu-token-aqui`
3. Click "Authorize"

---

## 🗄️ Estructura de Firebase

### Colecciones de Firestore

#### **users**
```json
{
  "id": "user-123",
  "email": "usuario@example.com",
  "passwordHash": "$2a$11$...",
  "fullName": "Juan Pérez",
  "role": "votante",
  "hasVoted": true,
  "votedFor": "candidate-456",
  "votedForName": "María González",
  "voteTimestamp": "2025-12-15T21:30:00Z",
  "createdAt": "2025-12-10T10:00:00Z"
}
```

#### **candidates**
```json
{
  "id": "candidate-456",
  "name": "María González",
  "party": "Partido Verde",
  "photoUrl": "https://firebasestorage.googleapis.com/...",
  "logoUrl": "https://firebasestorage.googleapis.com/...",
  "proposals": ["...", "...", "..."],
  "voteCount": 45,
  "createdAt": "2025-12-05T15:00:00Z",
  "createdBy": "admin-789"
}
```

#### **votes**
```json
{
  "id": "vote-001",
  "userId": "user-123",
  "userName": "Juan Pérez",
  "candidateId": "candidate-456",
  "candidateName": "María González",
  "timestamp": "2025-12-15T21:30:00Z"
}
```

### Firebase Storage
```
/candidates
  /photos
    - candidate-456.jpg
  /logos
    - candidate-456.jpg
```

---

## ✅ Validaciones de Seguridad

### Voto Único:
- ✅ Verificación antes de votar
- ✅ Transacciones atómicas de Firestore
- ✅ Campo `hasVoted` en usuario
- ✅ Validación en frontend y backend

### Seguridad:
- ✅ Contraseñas hasheadas con BCrypt
- ✅ Tokens JWT con expiración (24h)
- ✅ Roles y permisos
- ✅ CORS configurado
- ✅ Log de auditoría (colección votes)

---

## 🧪 Credenciales de Prueba

### Administrador
```
Email: admin@votoseguro.com
Password: Admin123!
```

### Votantes
```
Email: votante1@example.com
Password: Votante123!
```

**NOTA**: Debes crear estos usuarios manualmente usando el endpoint `/api/auth/register` y luego cambiar el rol en Firestore a "administrador" para el admin.

---

## 🔥 Crear Primer Administrador

1. Registrar un usuario normal:
```bash
POST /api/auth/register
{
  "email": "admin@votoseguro.com",
  "password": "Admin123!",
  "fullName": "Administrador"
}
```

2. Ir a Firebase Console → Firestore
3. Buscar el documento del usuario en la colección `users`
4. Cambiar campo `role` de `"votante"` a `"administrador"`

---

## 📦 Paquetes NuGet

- `Microsoft.AspNetCore.OpenApi` - OpenAPI/Swagger
- `Swashbuckle.AspNetCore` - Swagger UI
- `Google.Cloud.Firestore` - Firestore
- `FirebaseAdmin` - Firebase Admin SDK
- `FirebaseStorage.net` - Firebase Storage
- `Microsoft.AspNetCore.Authentication.JwtBearer` - JWT Auth
- `BCrypt.Net-Next` - Password hashing
- `System.IdentityModel.Tokens.Jwt` - JWT tokens

---

## 🐛 Troubleshooting

### Error: "Firebase credentials file not found"
- Verifica que `Config/firebase-credentials.json` existe
- Verifica que el archivo está configurado como "Copy to Output Directory: Always"

### Error: "Project ID mismatch"
- Actualiza `ProjectId` en `FirebaseService.cs`
- Actualiza `StorageBucket` en `CandidateService.cs`

### Error: "CORS policy"
- Verifica que Angular está corriendo en `http://localhost:4200`
- Actualiza la política CORS en `Program.cs` si usas otro puerto

---

## 📝 Notas Importantes

1. **No subir credenciales a GitHub**: El archivo `firebase-credentials.json` está en `.gitignore`
2. **Cambiar JWT Key en producción**: Actualizar en `appsettings.json`
3. **Firebase Storage**: Configurar reglas de seguridad en Firebase Console
4. **Índices de Firestore**: Firebase los creará automáticamente cuando sean necesarios

---

## 🚀 Siguientes Pasos

1. [✅] Backend API completo
2. [ ] Desarrollar Frontend con Angular
3. [ ] Integrar Frontend con Backend
4. [ ] Testing completo
5. [ ] Documentación de usuario
6. [ ] Video demostrativo

---

## 📞 Soporte

Para problemas o preguntas, revisar la documentación de:
- [Firebase](https://firebase.google.com/docs)
- [ASP.NET Core](https://docs.microsoft.com/aspnet/core)
- [JWT Authentication](https://jwt.io/)
