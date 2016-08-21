using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsmSharp.Osm;
using System.Collections;

namespace HnumbValidator
{
    class ValidatorOpeningDate: Validator
    {
        public ValidatorOpeningDate()
        {
            FileEnd = "opendate";
            Title = "Дата открытия";

            descriptionForList = "Строящиеся объекты у которых дата открытия прошла.";
            descriptionForMap = descriptionForList;
            errors = new List<Error>();
        }

        public override IEnumerable GetTableHead()
        {
            yield return "Дата открытия";
            yield return "Доп. информация";
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
                        AddErrorToList( geo, value, "Дата открытия прошла." );
                    else if ( ( opendate - DateTime.Now ).Days < 7 )
                        AddErrorToList( geo, value, "До открытия меньше недели." );
                }
                else if ( int.TryParse( value, out openyear ) )
                {
                    if ( openyear <= DateTime.Now.Year )
                        AddErrorToList( geo, value, "Дата открытия прошла." );
                }
                else
                    AddErrorToList( geo, value, "Дата не распознана." );
            }
        }

        private void AddErrorToList( OsmGeo geo, string value, string description )
        {
            Error err = new Error( geo, value );
            err.Description = description;
            errors.Add( err );
        }
    }
}
