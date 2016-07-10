using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsmSharp.Osm;

namespace HnumbValidator
{
    class ValidatorFlats : Validator
    {
        public ValidatorFlats()
        {
            FileEnd = "flats";
            Title = "Квартиры";

            descriptionForList = "Не правильно обозначенные номера квартир в подъезде.<br>"
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
                    errors.Add( flat );
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
    }
}
