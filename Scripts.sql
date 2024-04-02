
use IMDB;
go

-- Create MovieView
CREATE VIEW MovieView AS
SELECT 
    ROW_NUMBER() OVER (ORDER BY dbo.Titles.Tconst) AS ID, 
    dbo.Titles.Tconst, 
    dbo.Titles.PrimaryTitle, 
    dbo.Titles.OriginalTitle, 
    dbo.Titles.IsAdult, 
    dbo.Titles.StartYear, 
    dbo.Titles.EndYear, 
    dbo.Titles.RuntimeMinutes, 
    dbo.TitleTypes.Type, 
    STRING_AGG(dbo.Genres.GenreName, ', ') AS GenreNames 
FROM 
    dbo.Genres 
    INNER JOIN dbo.Title_Genres ON dbo.Genres.Id = dbo.Title_Genres.GenreId 
    INNER JOIN dbo.Titles ON dbo.Title_Genres.TitleTconst = dbo.Titles.Tconst 
    INNER JOIN dbo.TitleTypes ON dbo.Titles.TitleTypeId = dbo.TitleTypes.Id 
GROUP BY 
    dbo.Titles.Tconst, 
    dbo.Titles.PrimaryTitle, 
    dbo.Titles.OriginalTitle, 
    dbo.Titles.IsAdult, 
    dbo.Titles.StartYear, 
    dbo.Titles.EndYear, 
    dbo.Titles.RuntimeMinutes, 
    dbo.TitleTypes.Type;

    go

-- Create PersonView
CREATE VIEW PersonView AS
SELECT 
    ROW_NUMBER() OVER (ORDER BY c.Nconst) AS ID, 
    c.Nconst, 
    c.PrimaryName, 
    c.BirthYear, 
    c.DeathYear, 
    STRING_AGG(j.JobName, ', ') AS JobNames
FROM 
    dbo.Crews c 
    INNER JOIN dbo.Crew_Professions cp ON c.Nconst = cp.CrewNconst 
    INNER JOIN dbo.Jobs j ON cp.JobId = j.Id
GROUP BY 
    c.Nconst, 
    c.PrimaryName, 
    c.BirthYear, 
    c.DeathYear;

    go