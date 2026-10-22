# PaymentSystem - API de Gestión de Pedidos y Pagos

Un sistema backend modular y de alto rendimiento para la gestión de órdenes de compra y simulación de procesamiento de pagos, construido con **.NET 8** y **SQLite**.

---

## 🛠️ Tecnologías Utilizadas

- **Plataforma**: .NET 8 (C# 12)
- **API Framework**: ASP.NET Core Minimal APIs
- **Persistencia / ORM**: Entity Framework Core 8 con SQLite
- **Manejo de Errores**: `IExceptionHandler` global middleware
- **Documentación & Pruebas Interactivas**: Swagger / OpenAPI (Swashbuckle)
- **Pruebas Unitarias**: xUnit, Moq, FluentAssertions

---

## 🏗️ Arquitectura del Proyecto

El proyecto sigue los principios de **Clean Architecture** (Arquitectura Limpia) y separación de responsabilidades en capas ligeras:

```text
PaymentSystem/
├── src/
│   ├── Domain/          # Entidades centrales (Product, Order) y lógica de negocio pura
│   ├── Application/     # Casos de uso, Servicios (OrderService) y DTOs
│   ├── Infrastructure/  # DbContext (AppDbContext), configuraciones de EF Core y persistencia SQLite
│   └── Api/             # Endpoints minimalistas, configuración de DI, Swagger y Middleware
└── tests/
    └── PaymentSystem.Tests/ # Pruebas unitarias aisladas con xUnit
```

---

## 🧠 Decisiones Técnicas y de Diseño

### 1. Minimal APIs en ASP.NET Core
Se optó por **Minimal APIs** en lugar de controladores tradicionales para reducir la sobrecarga de código (boilerplate), mejorar los tiempos de respuesta y lograr una estructura de mapeo clara utilizando métodos estáticos de extensión divididos por dominio (`OrderEndpoints.cs` y `ProductEndpoints.cs`).

### 2. Control de Concurrencia Optimista
Para prevenir el **overselling** (vender más stock del disponible cuando múltiples usuarios compran simultáneamente), se implementó un control de concurrencia optimista. Dado que **SQLite** no soporta de forma nativa el atributo `[Timestamp]` / `rowversion` basado en `byte[]` de SQL Server, se utilizó la anotación `[ConcurrencyCheck]` sobre una propiedad numéricas de versión (`int Version`). Con esto se garantiza la compatibilidad multi-plataforma e integridad de los datos.

### 3. Transacciones ACID
El proceso de reducción de stock y registro del pedido se ejecuta dentro de una transacción explícita de EF Core. Si la simulación del pago o el guardado del pedido falla, los cambios de stock se revierten automáticamente (Rollback).

### 4. Persistencia Liviana (Zero-Config)
Se utilizó **SQLite** para permitir que la solución sea completamente portable y ejecutable inmediatamente ("Zero-Config") por cualquier evaluador sin requerir contenedores Docker o servidores de bases de datos pesados como SQL Server o PostgreSQL.

---

## 🚀 Instrucciones de Ejecución

### Prerrequisitos
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) o superior.
- Visual Studio 2022 / JetBrains Rider / VS Code.

### Paso 1: Clonar el Repositorio
```bash
git clone <URL_DEL_REPOSITORIO>
cd PaymentSystem
```

### Paso 2: Restaurar Dependencias y Compilar
```bash
dotnet restore
dotnet build
```

### Paso 3: Ejecutar la API
Navega a la carpeta del proyecto API y ejecútalo:

```bash
cd src/Api
dotnet run
```

Una vez iniciada la aplicación, la terminal te indicará las URLs activas (por ejemplo `https://localhost:7228`).

### Paso 4: Explorar la API (Swagger)
Abre tu navegador de preferencia e ingresa a:
👉 **`https://localhost:7228/swagger`**

*(Si ejecutas mediante HTTP el puerto por defecto suele ser `http://localhost:5244/swagger`)*

---

## 📌 Endpoints Disponibles

| Método | Endpoint | Descripción |
| :--- | :--- | :--- |
| `POST` | `/api/orders` | Crea una nueva orden, procesa el pago y descuenta stock. |
| `GET` | `/api/orders` | Obtiene el listado de todas las órdenes realizadas. |
| `GET` | `/api/orders/{id}` | Obtiene los detalles de una orden específica por su ID. |
| `GET` | `/api/products` | Consulta los productos disponibles y su stock actual. |

---

## 🧪 Ejecución de Pruebas Unitarias

El proyecto incluye pruebas unitarias para validar las reglas de negocio, tales como validación de stock insuficiente y creación exitosa de órdenes.

Para ejecutar la suite de pruebas desde la terminal en la raíz de la solución:

```bash
dotnet test
```

---

## 🗄️ Inspección de la Base de Datos

La base de datos SQLite (`payment.db`) se crea automáticamente en la raíz del proyecto `Api` al arrancar la aplicación por primera vez con sus datos semilla (Seed Data).

Para explorar visualmente las tablas (`Products` y `Orders`):
1. Abre **[DB Browser for SQLite](https://sqlitebrowser.org/)**.
2. Abre el archivo `payment.db` ubicado en `src/Api/payment.db`.
3. Navega a la pestaña **Browse Data** (Examinar datos).