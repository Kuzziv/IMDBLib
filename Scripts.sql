
use IMDB;
go

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