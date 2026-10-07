/* INCLUTEC - esquema de INCLUTECBD con IDENTITY y datos de prueba.
   Recrea las 4 tablas. Correr solo con las tablas vacias. */

IF OBJECT_ID('dbo.RegistroAsistencia','U') IS NOT NULL DROP TABLE dbo.RegistroAsistencia;
GO
IF OBJECT_ID('dbo.Estudiante','U') IS NOT NULL DROP TABLE dbo.Estudiante;
GO
IF OBJECT_ID('dbo.Aula','U') IS NOT NULL DROP TABLE dbo.Aula;
GO
IF OBJECT_ID('dbo.ResponsableAula','U') IS NOT NULL DROP TABLE dbo.ResponsableAula;
GO

CREATE TABLE dbo.ResponsableAula (
    Id       int IDENTITY(1,1) NOT NULL CONSTRAINT PK_ResponsableAula PRIMARY KEY,
    Nombre   nvarchar(100) NOT NULL,
    Apellido nvarchar(100) NOT NULL,
    Rol      nvarchar(100) NOT NULL,
    Email    nvarchar(100) NULL
);
GO

CREATE TABLE dbo.Aula (
    Id            int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Aula PRIMARY KEY,
    Nombre        nvarchar(100) NOT NULL,
    AñoLectivo    date NOT NULL,
    ResponsableId int NOT NULL,
    EstadoActivo  bit NOT NULL CONSTRAINT DF_Aula_EstadoActivo DEFAULT (1),
    CONSTRAINT FK_Aula_ResponsableAula FOREIGN KEY (ResponsableId) REFERENCES dbo.ResponsableAula (Id)
);
GO
CREATE INDEX IX_Aula_ResponsableId ON dbo.Aula (ResponsableId);
GO

CREATE TABLE dbo.Estudiante (
    Id                      int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Estudiante PRIMARY KEY,
    Nombre                  nvarchar(100) NOT NULL,
    Apellido                nvarchar(100) NOT NULL,
    AvatarUrlPictogramaPath nvarchar(1000) NULL,
    AulaId                  int NOT NULL,
    EstadoActivo            bit NOT NULL CONSTRAINT DF_Estudiante_EstadoActivo DEFAULT (1),
    CONSTRAINT FK_Estudiante_Aula FOREIGN KEY (AulaId) REFERENCES dbo.Aula (Id)
);
GO
CREATE INDEX IX_Estudiante_AulaId ON dbo.Estudiante (AulaId);
GO

CREATE TABLE dbo.RegistroAsistencia (
    Id            int IDENTITY(1,1) NOT NULL CONSTRAINT PK_RegistroAsistencia PRIMARY KEY,
    Fecha         datetime NOT NULL CONSTRAINT DF_RegistroAsistencia_Fecha DEFAULT (GETDATE()),
    EstudianteId  int NOT NULL,
    EstaPresente  bit NOT NULL,
    Observaciones nvarchar(max) NULL,
    CONSTRAINT FK_RegistroAsistencia_Estudiante FOREIGN KEY (EstudianteId) REFERENCES dbo.Estudiante (Id)
);
GO
CREATE INDEX IX_RegistroAsistencia_EstudianteId ON dbo.RegistroAsistencia (EstudianteId);
GO

INSERT INTO dbo.ResponsableAula (Nombre, Apellido, Rol, Email) VALUES
 (N'Laura', N'Gimenez', N'Docente', N'laura.gimenez@escuela.edu.ar'),
 (N'Marcelo', N'Ponce', N'Preceptor', N'marcelo.ponce@escuela.edu.ar');
GO
INSERT INTO dbo.Aula (Nombre, AñoLectivo, ResponsableId, EstadoActivo) VALUES
 (N'Sala Inicial', '2026-01-01', 1, 1),
 (N'3 Grado', '2026-01-01', 2, 1);
GO
INSERT INTO dbo.Estudiante (Nombre, Apellido, AvatarUrlPictogramaPath, AulaId, EstadoActivo) VALUES
 (N'Mateo', N'Alvarez', N'/pictogramas/mateo.png', 1, 1),
 (N'Valentina', N'Suarez', N'/pictogramas/valentina.png', 1, 1),
 (N'Emma', N'Ledesma', NULL, 2, 1);
GO
INSERT INTO dbo.RegistroAsistencia (Fecha, EstudianteId, EstaPresente, Observaciones) VALUES
 ('2026-10-06', 1, 1, NULL),
 ('2026-10-06', 2, 0, N'Aviso la familia'),
 ('2026-10-06', 3, 1, NULL);
GO
