using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsmSharp.Osm;

namespace HouseNumberValidator
{
    class ValidatorOpeningDate: Validator
    {
        public ValidatorOpeningDate()
        {
            FileEnd = "opendate";
            Title = "Дата открытия";

            descriptionForList = "Строящиеся дороги у которых дата открытия прошла.<br>";
            descriptionForMap = "Строящиеся дороги у которых дата открытия прошла.<br>";
            errors = new List<Error>();
        }

        public override void ValidateObject( OsmGeo geo )
        {
            string value;
            if ( geo.Tags.TryGetValue( "opening_date", out value ) )
            {
                DateTime opendate;
                int openyear;

                if ( DateTime.TryParse( value, out opendate ) )
                {
                    if ( opendate <= DateTime.Now )
                    {
                        Error err = new Error( geo, value );
                        err.Description = "Дата открытия прошла";
                        errors.Add( err );
                    }
                }
                else if ( int.TryParse( value, out openyear ) )
                {
                    if ( openyear <= DateTime.Now.Year )
                    {
                        Error err = new Error( geo, value );
                        err.Description = "Дата открытия прошла";
                        errors.Add( err );
                    }
                }
                else
                {
                    Error err = new Error( geo, value );
                    err.Description = "Дата не распознана";
                    errors.Add( err );
                }
            }
        }
    }
}
