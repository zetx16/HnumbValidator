using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsmSharp.Osm;

namespace HouseNumberValidator
{
    class ValidatorFlats : Validator
    {
        public ValidatorFlats()
        {
            fileListEnd = ".flats.html";
            fileMapEnd = ".flats.map.html";
            description = "Не правильно обозначенные номера квартир в подъезде.<br>"
                + "Правильный формат записи addr:flats=3-7;10;14;16-18<br>";
            descriptionForMap = "Не правильно обозначенные номера квартир в подъезде.<br>"
                + "Правильный формат записи addr:flats=3-7;10;14;16-18<br>";
            errors = new List<Error>();
        }

        public override void ValidateObject(OsmGeo geo)
        {
            string value;
            if ( geo.Tags.TryGetValue( "addr:flats", out value ) )
            {
                Error flat = new Error( geo, value );
                if ( !ValidateFlats( flat ) )
                {
                    GeoOperations.GetCoordinates( geo, flat );
                    errors.Add( flat );
                }
            }
        }

        private bool ValidateFlats( Error hn )
        {
            if ( Regexes.CheckPattern( Regexes.flat, hn.Value ) )
                return true;

            if ( hn.Value.Contains( ',' ) )
                hn.Description += "Заменить ',' на ';'";

            return false;
        }

        public override string[] GetTableHead()
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
