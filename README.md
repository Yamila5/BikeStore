# BikeStore

Solución para la gestión de bicicletas, clientes, categorías, ventas e inventario.

## Proyectos

- `BikeStore/`: API REST ASP.NET Core y acceso a SQL Server.
- `BikeStore.Web/`: sitio Web ASP.NET Core MVC que consume la API mediante `HttpClient`.
- `Database/BikeStore.sql`: creación de la base, tablas, PK, FK y datos de prueba.

## Ejecución

1. Abra `Database/BikeStore.sql` en SQL Server Management Studio y ejecútelo completo.
2. Revise la conexión de `BikeStore/appsettings.json`; por defecto usa SQL Server local (`Server=.`).
3. Configure ambos proyectos como proyectos de inicio múltiple en Visual Studio, iniciando primero `BikeStore` y después `BikeStore.Web`.
4. La API queda en `https://localhost:7265` y Swagger en `https://localhost:7265/swagger` durante desarrollo. El sitio Web inicia en `https://localhost:7275`.

El valor `ApiSettings:BaseUrl` de `BikeStore.Web/appsettings.json` debe coincidir con la URL HTTPS de la API.
