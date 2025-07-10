# API de Control de Gastos - Proyecto Azul School

Este repositorio contiene el código fuente de una API RESTful para el control de gastos personales, desarrollada como práctica para el curso de Web API con ASP.NET Core de Azul School.

La API permite a los usuarios registrarse, gestionar sus cuentas de dinero, registrar transacciones (ingresos y egresos) y transferir fondos entre sus cuentas.

## ✨ Características Principales

- **Autenticación Segura:** Implementación de autenticación basada en JSON Web Tokens (JWT).
- **Gestión de Usuarios:** Creación y gestión de perfiles de usuario.
- **Cuentas de Dinero:** Creación de múltiples "apartados" o cuentas de dinero por usuario.
- **Registro de Transacciones:** Registro de ingresos y gastos asociados a categorías.
- **Transferencias Internas:** Movimiento de fondos entre las cuentas del propio usuario.
- **Consultas y Listados:** Endpoints para consultar transacciones por categoría y transferencias.
- **Documentación de API:** Interfaz de Swagger UI para explorar y probar los endpoints de forma interactiva.

## 🛠️ Tecnologías Utilizadas

- **Framework:** .NET 8.0
- **Lenguaje:** C#
- **Base de Datos:** PostgreSQL
- **ORM:** Entity Framework Core 8
- **Autenticación:** JWT (JSON Web Tokens)
- **Arquitectura:** API RESTful

---

## 🚀 Cómo Empezar (Configuración Local)

Sigue estos pasos para clonar y ejecutar el proyecto en tu máquina local.

### Prerrequisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/download/) instalado y corriendo.
- Un editor de código como [Visual Studio Code](https://code.visualstudio.com/) o [Visual Studio](https://visualstudio.microsoft.com/).
- [Git](https://git-scm.com/downloads)

### 1. Clonar el Repositorio

```bash
git clone <URL_DEL_REPOSITORIO>
cd AzulSchoolProject
```

### 2. Configurar Secretos de la Aplicación

Por seguridad, las claves secretas y la cadena de conexión no se guardan en el repositorio. Usamos la herramienta `dotnet user-secrets` para gestionarlas localmente.

**a. Inicializa los secretos de usuario:**

```bash
dotnet user-secrets init
```

**b. Agrega la Cadena de Conexión a tu base de datos PostgreSQL:**

> **Nota:** Asegúrate de haber creado una base de datos vacía en PostgreSQL para este proyecto.

```bash
# Reemplaza los valores con los de tu configuración de PostgreSQL
dotnet user-secrets set "ConnectionStrings:ProjectServer" "Host=localhost;Port=5432;Database=NombreDeTuDB;Username=tu_usuario;Password=tu_contraseña"
```

**c. Agrega la Clave Secreta para JWT:**

La clave debe ser larga y aleatoria. Puedes generar una con el siguiente comando en PowerShell y usar el resultado:

```powershell
# Comando para generar una clave segura en PowerShell
[Convert]::ToBase64String((1..32 | % { [byte](Get-Random -Maximum 256) }))
```

Usa la clave generada en el siguiente comando:

```bash
dotnet user-secrets set "Jwt:Key" "PEGA_AQUI_TU_CLAVE_GENERADA"
```

### 3. Aplicar las Migraciones de la Base de Datos

Esto creará el esquema de la base de datos (tablas, relaciones, etc.) basado en los modelos de la aplicación.

```bash
dotnet ef database update
```

### 4. Ejecutar la Aplicación

```bash
dotnet run
```

La API estará disponible en `https://localhost:7038` (o el puerto que se indique en la consola).

## 📖 Uso de la API

1.  **Explora los Endpoints:** Navega a `https://localhost:7038/swagger` para ver la documentación interactiva de la API.
2.  **Regístrate:** Crea un nuevo usuario usando el endpoint `POST /api/Users`.
3.  **Inicia Sesión:** Usa el endpoint `POST /api/Authentication/login` con el email y contraseña del usuario que creaste. La respuesta contendrá tu token JWT.
4.  **Autoriza tus Peticiones:** En la parte superior derecha de Swagger, haz clic en el botón `Authorize`. En el diálogo, escribe `Bearer ` seguido de tu token (ej: `Bearer eyJhbGciOi...`) y haz clic en "Authorize".
5.  **Usa los Endpoints Protegidos:** Ahora puedes usar el resto de los endpoints que requieren autenticación. Swagger incluirá automáticamente el token en cada petición.