# Sistema de Gestión de Citas Médicas

Proyecto de Bases de Datos, tema 15: Persistencia y Mapeo Objeto-Relacional (Entity Framework Core).

## Tecnologías
- ASP.NET Core MVC (.NET 10)
- Entity Framework Core 10 + SQL Server 2022

## Estructura
- `scripts/01_crear_tablas.sql`: tablas, claves e integridad referencial
- `scripts/02_triggers.sql`: auditoría de citas y validación de consultas
- `Models/`, `Data/`: entidades y contexto generados con `dotnet ef dbcontext scaffold`
- `Controllers/`, `Views/`: CRUD, atención de citas con transacción y reportes LINQ

## Cómo ejecutarlo
1. Levantar SQL Server (Docker) y ejecutar los scripts de `scripts/` en orden.
2. Guardar la cadena de conexión en user-secrets:
   `dotnet user-secrets set "ConnectionStrings:CitasMedicas" "Server=localhost,1433;Database=CitasMedicas;User Id=sa;Password=TU_CLAVE;TrustServerCertificate=True"`
3. `dotnet run`
