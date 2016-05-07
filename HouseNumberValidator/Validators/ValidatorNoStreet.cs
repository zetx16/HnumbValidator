using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsmSharp.Osm;

namespace HouseNumberValidator
{
    class ValidatorNoStreet: IValidator
    {
        string fileListEnd = ".nostreet.html";
        string fileMapEnd = ".nostreet.map.html";

        List<Error> errors;
        public List<Error> Errors { get { return errors; } }

        string description = @"Список номеров домов без ""addr:street"" и ""addr:place""<br>";
        public string DescriptionForList { get { return description; } }

        string descriptionForMap = @"Адреса без ""addr:street"" и ""addr:place""<br><br>"
            + @"<div class=""info-colour"" style=""background-color:orange;""></div> - есть ""addr:city"" или ""addr:suburb""<br>";
        public string DescriptionForMap { get { return descriptionForMap; } }

        List<string> addrkeys;


        public ValidatorNoStreet()
        {
            errors = new List<Error>(); 
            addrkeys = new List<string>{
				"addr:street",
				"addr:place",
                //"addr:quarter",
                //"addr:neighbourhood",
                "is_in:neighbourhood"
			};
        }

        public void ValidateObject(OsmGeo geo)
        {
            string value;
            if ( geo.Tags.TryGetValue( "addr:housenumber", out value ) && !geo.Tags.ContainsOneOfKeys( addrkeys ) )
            {
                if ( !GeoOperations.CountryRu( geo ) )
                    return;

                Error nostreet = new Error( geo, value );

                string city;

                if ( geo.Tags.TryGetValue( "addr:city", out city ) )
                    nostreet.Description += "<font color=\"gray\">city:</font> " + city;
                if ( geo.Tags.TryGetValue( "addr:suburb", out city ) )
                    if ( nostreet.Description != String.Empty )
                        nostreet.Description += "  |  " + "<font color=\"gray\">suburb: </font>" + city;
                    else
                        nostreet.Description += "<font color=\"gray\">suburb: </font>" + city;
                GeoOperations.GetCoordinates( geo, nostreet );
                errors.Add( nostreet );
            }
        }

        public void ValidateEndReadFile()
        {
            errors = errors.Except( GeoOperations.members ).ToList();
            errors = errors.OrderByDescending( x => x.TimeStump ).OrderBy( x => x.Description ).ToList();
        }

        public string GetPathList( string directory, string region )
        {
            return directory + region + fileListEnd;
        }

        public string GetPathMap( string directory, string region )
        {
            return directory + region + fileMapEnd;
        }

        public string[] GetTableHead()
        {
            string[] result = new string[ 2 ];
            result[ 0 ] = "Ошибка";
            result[ 1 ] = "Доп. информация";
            return result;
        }

        public IEnumerable<string[]> GetTableTr()
        {
            string[] result = new string[ 3 ];

            foreach ( var error in errors )
            {
                result[ 0 ] = String.Format( @"<a href=""http://127.0.0.1:8111/load_object?objects={0}{1}"" onClick=""open_josm('{0}{1}');return false;""><img src=icon_to_josm.png></a>", error.Type, error.Osmid );
                result[ 1 ] = String.Format( @"<a href=""http://osm.org/{0}/{1}"">{2}</a>", error.Type, error.Osmid, error.Value );
                result[ 2 ] = error.Description;
                yield return result;
            }
        }
    }
}
