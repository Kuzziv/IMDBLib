
use IMDB;
go

CREATE TABLE [dbo].[Titles] (
    [Tconst]         NVARCHAR (450) NOT NULL,
    [TitleType]      NVARCHAR (MAX) NOT NULL,
    [PrimaryTitle]   NVARCHAR (MAX) NOT NULL,
    [OriginalTitle]  NVARCHAR (MAX) NOT NULL,
    [IsAdult]        BIT            NOT NULL,
    [StartYear]      INT            NOT NULL,
    [EndYear]        INT            NULL,
    [RuntimeMinutes] INT            NOT NULL,
    CONSTRAINT [PK_Titles] PRIMARY KEY CLUSTERED ([Tconst] ASC)
);

CREATE TABLE [dbo].[TitleGenres] (
    [Tconst]  NVARCHAR (450) NOT NULL,
    [GenreId] INT            NOT NULL,
    CONSTRAINT [PK_TitleGenres] PRIMARY KEY CLUSTERED ([Tconst] ASC, [GenreId] ASC),
    CONSTRAINT [FK_TitleGenres_Genres_GenreId] FOREIGN KEY ([GenreId]) REFERENCES [dbo].[Genres] ([GenreId]) ON DELETE CASCADE,
    CONSTRAINT [FK_TitleGenres_Titles_Tconst] FOREIGN KEY ([Tconst]) REFERENCES [dbo].[Titles] ([Tconst]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_TitleGenres_GenreId]
    ON [dbo].[TitleGenres]([GenreId] ASC);

CREATE TABLE [dbo].[Genres] (
    [GenreId]   INT            IDENTITY (1, 1) NOT NULL,
    [GenreName] NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_Genres] PRIMARY KEY CLUSTERED ([GenreId] ASC)
);

CREATE TABLE [dbo].[TitlePersons] (
    [Tconst] NVARCHAR (450) NOT NULL,
    [Nconst] NVARCHAR (450) NOT NULL,
    CONSTRAINT [PK_TitlePersons] PRIMARY KEY CLUSTERED ([Tconst] ASC, [Nconst] ASC),
    CONSTRAINT [FK_TitlePersons_Persons_Nconst] FOREIGN KEY ([Nconst]) REFERENCES [dbo].[Persons] ([Nconst]) ON DELETE CASCADE,
    CONSTRAINT [FK_TitlePersons_Titles_Tconst] FOREIGN KEY ([Tconst]) REFERENCES [dbo].[Titles] ([Tconst]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_TitlePersons_Nconst]
    ON [dbo].[TitlePersons]([Nconst] ASC);



CREATE TABLE [dbo].[Persons] (
    [Nconst]      NVARCHAR (450) NOT NULL,
    [PrimaryName] NVARCHAR (MAX) NOT NULL,
    [BirthYear]   INT            NOT NULL,
    [DeathYear]   INT            NULL,
    CONSTRAINT [PK_Persons] PRIMARY KEY CLUSTERED ([Nconst] ASC)
);

CREATE TABLE [dbo].[PersonProfessions] (
    [Nconst]       NVARCHAR (450) NOT NULL,
    [ProfessionId] INT            NOT NULL,
    CONSTRAINT [PK_PersonProfessions] PRIMARY KEY CLUSTERED ([Nconst] ASC, [ProfessionId] ASC),
    CONSTRAINT [FK_PersonProfessions_Persons_Nconst] FOREIGN KEY ([Nconst]) REFERENCES [dbo].[Persons] ([Nconst]) ON DELETE CASCADE,
    CONSTRAINT [FK_PersonProfessions_Professions_ProfessionId] FOREIGN KEY ([ProfessionId]) REFERENCES [dbo].[Professions] ([ProfessionId]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_PersonProfessions_ProfessionId]
    ON [dbo].[PersonProfessions]([ProfessionId] ASC);

CREATE TABLE [dbo].[Professions] (
    [ProfessionId]   INT            IDENTITY (1, 1) NOT NULL,
    [ProfessionName] NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_Professions] PRIMARY KEY CLUSTERED ([ProfessionId] ASC)
);





-- Create MovieView
CREATE VIEW MovieView AS
SELECT 
    t.Tconst,
    t.TitleType,
    t.PrimaryTitle,
    t.OriginalTitle,
    t.IsAdult,
    t.StartYear,
    t.EndYear,
    t.RuntimeMinutes,
    STRING_AGG(g.GenreName, ', ') AS GenreNames
FROM 
    Titles t
JOIN 
    TitleGenres tg ON t.Tconst = tg.Tconst
JOIN 
    Genres g ON tg.GenreId = g.GenreId
GROUP BY
    t.Tconst,
    t.TitleType,
    t.PrimaryTitle,
    t.OriginalTitle,
    t.IsAdult,
    t.StartYear,
    t.EndYear,
    t.RuntimeMinutes;
    go

-- Create PersonView
CREATE VIEW PersonView AS
SELECT 
    p.Nconst,
    p.PrimaryName,
    p.BirthYear,
    p.DeathYear,
    STRING_AGG(pr.ProfessionName, ', ') AS ProfessionNames
FROM 
    Persons p
JOIN 
    PersonProfessions pp ON p.Nconst = pp.Nconst
JOIN 
    Professions pr ON pp.ProfessionId = pr.ProfessionId
GROUP BY
    p.Nconst,
    p.PrimaryName,
    p.BirthYear,
    p.DeathYear;
    go