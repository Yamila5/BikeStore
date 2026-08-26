CREATE DATABASE BikeStore;
GO

USE BikeStore;
GO

-- =============================================
-- TABLA CATEGORIAS
-- =============================================
CREATE TABLE Categorias (
    IdCategoria INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Descripcion VARCHAR(250),
    Activo BIT NOT NULL DEFAULT 1
);
GO

-- =============================================
-- TABLA BICICLETAS
-- =============================================
CREATE TABLE Bicicletas (
    IdBicicleta INT IDENTITY(1,1) PRIMARY KEY,
    IdCategoria INT NOT NULL,
    Marca VARCHAR(100) NOT NULL,
    Modelo VARCHAR(100) NOT NULL,
    Precio DECIMAL(10,2) NOT NULL,
    Stock INT NOT NULL,
    Estado VARCHAR(20) NOT NULL DEFAULT 'Disponible',

    CONSTRAINT FK_Bicicletas_Categorias
        FOREIGN KEY (IdCategoria)
        REFERENCES Categorias(IdCategoria),

    CONSTRAINT CK_Bicicletas_Precio
        CHECK (Precio >= 0),

    CONSTRAINT CK_Bicicletas_Stock
        CHECK (Stock >= 0)
);
GO

-- =============================================
-- TABLA CLIENTES
-- =============================================
CREATE TABLE Clientes (
    IdCliente INT IDENTITY(1,1) PRIMARY KEY,
    Cedula VARCHAR(10) NOT NULL UNIQUE,
    Nombres VARCHAR(100) NOT NULL,
    Apellidos VARCHAR(100) NOT NULL,
    Telefono VARCHAR(20),
    Correo VARCHAR(150)
);
GO

-- =============================================
-- TABLA VENTAS
-- =============================================
CREATE TABLE Ventas (
    IdVenta INT IDENTITY(1,1) PRIMARY KEY,
    Fecha DATETIME NOT NULL DEFAULT GETDATE(),
    IdCliente INT NOT NULL,
    Subtotal DECIMAL(10,2) NOT NULL,
    IVA DECIMAL(10,2) NOT NULL,
    Total DECIMAL(10,2) NOT NULL,

    CONSTRAINT FK_Ventas_Clientes
        FOREIGN KEY (IdCliente)
        REFERENCES Clientes(IdCliente)
);
GO

-- =============================================
-- TABLA DETALLE DE VENTAS
-- =============================================
CREATE TABLE DetalleVentas (
    IdDetalle INT IDENTITY(1,1) PRIMARY KEY,
    IdVenta INT NOT NULL,
    IdBicicleta INT NOT NULL,
    Cantidad INT NOT NULL,
    Precio DECIMAL(10,2) NOT NULL,
    Subtotal DECIMAL(10,2) NOT NULL,

    CONSTRAINT FK_DetalleVentas_Ventas
        FOREIGN KEY (IdVenta)
        REFERENCES Ventas(IdVenta),

    CONSTRAINT FK_DetalleVentas_Bicicletas
        FOREIGN KEY (IdBicicleta)
        REFERENCES Bicicletas(IdBicicleta),

    CONSTRAINT CK_DetalleVentas_Cantidad
        CHECK (Cantidad > 0)
);
GO

USE BikeStore;

INSERT INTO Categorias (Nombre, Descripcion, Activo)
VALUES
('Montaña', 'Bicicletas para terrenos montañosos y caminos difíciles', 1),
('Ruta', 'Bicicletas diseñadas para carretera y velocidad', 1),
('BMX', 'Bicicletas para trucos y deportes extremos', 1),
('Eléctricas', 'Bicicletas con asistencia mediante motor eléctrico', 1),
('Infantiles', 'Bicicletas diseñadas para niños', 1);

INSERT INTO Bicicletas
(IdCategoria, Marca, Modelo, Precio, Stock, Estado)
VALUES
(1, 'Trek', 'Marlin 5', 750.00, 10, 'Disponible'),
(1, 'Giant', 'Talon 3', 680.00, 8, 'Disponible'),
(2, 'Specialized', 'Allez', 1200.00, 5, 'Disponible'),
(2, 'Scott', 'Speedster 40', 1100.00, 7, 'Disponible'),
(3, 'Mongoose', 'Legion L20', 450.00, 6, 'Disponible'),
(4, 'Trek', 'Verve+ 2', 2300.00, 4, 'Disponible'),
(5, 'GW', 'Raptor Kids', 280.00, 3, 'Disponible');


INSERT INTO Clientes
(Cedula, Nombres, Apellidos, Telefono, Correo)
VALUES
('0102030405', 'Juan', 'Perez', '0991111111', 'juan@gmail.com'),
('0102030406', 'Maria', 'Gomez', '0992222222', 'maria@gmail.com'),
('0102030407', 'Carlos', 'Lopez', '0993333333', 'carlos@gmail.com');


SELECT * FROM Categorias;

SELECT * FROM Bicicletas;

SELECT * FROM Clientes;

SELECT * FROM Ventas;

SELECT * FROM DetalleVentas;
