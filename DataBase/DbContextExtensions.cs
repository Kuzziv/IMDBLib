using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace IMDBLib.DataBase
{
    public class DbContextExtensions
    {
        //public static IQueryable<YourViewModel> GetYourView(this DbContext context)
        //{
        //    return context.Set<YourViewModel>().FromSqlRaw(@"
        //    SELECT 
        //        ROW_NUMBER() OVER (ORDER BY dbo.Titles.Tconst) AS ID, 
        //        dbo.Titles.Tconst, 
        //        dbo.Titles.PrimaryTitle, 
        //        dbo.Titles.OriginalTitle, 
        //        dbo.Titles.IsAdult, 
        //        dbo.Titles.StartYear, 
        //        dbo.Titles.EndYear, 
        //        dbo.Titles.RuntimeMinutes, 
        //        dbo.TitleTypes.Type, 
        //        STRING_AGG(dbo.Genres.GenreName, ', ') AS GenreNames 
        //    FROM 
        //        dbo.Genres 
        //        INNER JOIN dbo.Title_Genres ON dbo.Genres.Id = dbo.Title_Genres.GenreId 
        //        INNER JOIN dbo.Titles ON dbo.Title_Genres.TitleTconst = dbo.Titles.Tconst 
        //        INNER JOIN dbo.TitleTypes ON dbo.Titles.TitleTypeId = dbo.TitleTypes.Id 
        //    GROUP BY 
        //        dbo.Titles.Tconst, 
        //        dbo.Titles.PrimaryTitle, 
        //        dbo.Titles.OriginalTitle, 
        //        dbo.Titles.IsAdult, 
        //        dbo.Titles.StartYear, 
        //        dbo.Titles.EndYear, 
        //        dbo.Titles.RuntimeMinutes, 
        //        dbo.TitleTypes.Type
        //");
        //}

    }
}
