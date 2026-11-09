IF DB_ID('CitasMedicas') IS NULL CREATE DATABASE CitasMedicas;
GO
USE CitasMedicas;
GO

CREATE TABLE Especialidades (
    EspecialidadId INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(80) NOT NULL UNIQUE,
    Descripcion NVARCHAR(200) NULL
);

CREATE TABLE Medicos (
    MedicoId INT IDENTITY(1,1) PRIMARY KEY,
    EspecialidadId INT NOT NULL,
    Nombres NVARCHAR(80) NOT NULL,
    Apellidos NVARCHAR(80) NOT NULL,
    NumeroJvpm NVARCHAR(20) NOT NULL UNIQUE,
    Telefono NVARCHAR(15) NULL,
    Correo NVARCHAR(100) NULL,
    Activo BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Medicos_Especialidades
        FOREIGN KEY (EspecialidadId) REFERENCES Especialidades(EspecialidadId)
);

CREATE TABLE Pacientes (
    PacienteId INT IDENTITY(1,1) PRIMARY KEY,
    Dui NVARCHAR(10) NULL,
    Nombres NVARCHAR(80) NOT NULL,
    Apellidos NVARCHAR(80) NOT NULL,
    FechaNacimiento DATE NOT NULL,
    Sexo CHAR(1) NOT NULL CHECK (Sexo IN ('M','F')),
    Telefono NVARCHAR(15) NULL,
    Correo NVARCHAR(100) NULL,
    Direccion NVARCHAR(200) NULL,
    FechaRegistro DATETIME2 NOT NULL DEFAULT SYSDATETIME()
);
CREATE UNIQUE INDEX UX_Pacientes_Dui ON Pacientes(Dui) WHERE Dui IS NOT NULL;

CREATE TABLE Consultorios (
    ConsultorioId INT IDENTITY(1,1) PRIMARY KEY,
    Numero NVARCHAR(20) NOT NULL UNIQUE,
    Piso INT NOT NULL,
    Activo BIT NOT NULL DEFAULT 1
);

CREATE TABLE Citas (
    CitaId INT IDENTITY(1,1) PRIMARY KEY,
    PacienteId INT NOT NULL,
    MedicoId INT NOT NULL,
    ConsultorioId INT NOT NULL,
    FechaHora DATETIME2 NOT NULL,
    DuracionMin INT NOT NULL DEFAULT 30,
    Estado NVARCHAR(15) NOT NULL DEFAULT 'Programada'
        CHECK (Estado IN ('Programada','Atendida','Cancelada')),
    Motivo NVARCHAR(200) NULL,
    CONSTRAINT FK_Citas_Pacientes FOREIGN KEY (PacienteId) REFERENCES Pacientes(PacienteId),
    CONSTRAINT FK_Citas_Medicos FOREIGN KEY (MedicoId) REFERENCES Medicos(MedicoId),
    CONSTRAINT FK_Citas_Consultorios FOREIGN KEY (ConsultorioId) REFERENCES Consultorios(ConsultorioId),
    CONSTRAINT UX_Citas_Medico_FechaHora UNIQUE (MedicoId, FechaHora)
);

CREATE TABLE Consultas (
    ConsultaId INT IDENTITY(1,1) PRIMARY KEY,
    CitaId INT NOT NULL UNIQUE,
    Diagnostico NVARCHAR(500) NOT NULL,
    Observaciones NVARCHAR(MAX) NULL,
    FechaRegistro DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    CONSTRAINT FK_Consultas_Citas FOREIGN KEY (CitaId) REFERENCES Citas(CitaId)
);

CREATE TABLE Medicamentos (
    MedicamentoId INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL UNIQUE,
    Presentacion NVARCHAR(60) NULL
);

CREATE TABLE Recetas (
    ConsultaId INT NOT NULL,
    MedicamentoId INT NOT NULL,
    Dosis NVARCHAR(60) NOT NULL,
    Frecuencia NVARCHAR(60) NOT NULL,
    DuracionDias INT NOT NULL,
    CONSTRAINT PK_Recetas PRIMARY KEY (ConsultaId, MedicamentoId),
    CONSTRAINT FK_Recetas_Consultas FOREIGN KEY (ConsultaId) REFERENCES Consultas(ConsultaId),
    CONSTRAINT FK_Recetas_Medicamentos FOREIGN KEY (MedicamentoId) REFERENCES Medicamentos(MedicamentoId)
);
GO
