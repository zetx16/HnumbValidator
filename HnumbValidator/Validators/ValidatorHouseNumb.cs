using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OsmSharp.Osm;

namespace HnumbValidator
{
    public class ValidatorHouseNumb : Validator
    {
        public ValidatorHouseNumb()
        {
            FileEnd = "errors";
            Title = "Ошибки";
            descriptionForList = "Номера домов не соответствующие принятой схеме.";
            descriptionForMap = "Номера домов не соответствующие принятой схеме.";

            errors = new List<Error>();
        }

        public override void ValidateObject( OsmGeo geo )
        {
            string value;
            for ( int i = 1; i <= 9; i++ )
            {
                if ( geo.Tags.TryGetValue( String.Format( "addr{0}:housenumber", i > 1 ? i.ToString() : "" ), out value ) )
                {
                    if ( !GeoCollections.CountryRu( geo ) )
                        continue;

                    var error = new Error( geo, value );

                    bool valid_error = ValidateHouseNumbOnError( error );
                    bool valid_warn = ValidateHouseNumbOnWarn( error );

                    if ( !valid_error || !valid_warn )
                    {
                        if ( !valid_warn )
                            error.Level = ErrorLevel.Level5;
                        if ( !valid_error )
                            error.Level = ErrorLevel.Level0;
                        errors.Add( error );
                    }
                    error.Description += GeoCollections.GetNotes( geo.Tags );
                }
                else
                    break;
            }
        }

        private bool ValidateHouseNumbOnWarn( Error hn )
        {
            Match downRes = Regexes.low.Match( hn.Value );
            if ( downRes.Success && downRes.Length == hn.Value.Length )
            {
                hn.Description += "После номера должна быть заглавная буква: ";
                hn.Description += downRes.Groups[ 1 ] + downRes.Groups[ 2 ].Value.ToUpper() + downRes.Groups[ 3 ] + "<br>";
                return false;
            }

            return true;
        }

        private bool ValidateHouseNumbOnError( Error hn )
        {
            if ( Regexes.CheckPattern( Regexes.numb, hn.Value ) )
                return true;

            if ( Regexes.CheckPattern( Regexes.defis, hn.Value ) )
                return true;

            if ( Regexes.CheckPattern( Regexes.nch, hn.Value ) )
                return true;

            if ( Regexes.CheckPattern( Regexes.rim, hn.Value ) )
                return true;

            if ( Regexes.CheckPattern( Regexes.zyab, hn.Value ) )
                return true;

            if ( Regexes.CheckPattern( Regexes.c, hn.Value ) )
                return true;

            if ( Regexes.CheckPattern( Regexes.zgrad, hn.Value ) )
                return true;

            if ( Regexes.MatchSuccess( Regexes.latin, hn.Value ) )
                hn.Description += "Содержится английская буква<br>";

            if ( Regexes.CheckPattern( Regexes.space, hn.Value ) )
                hn.Description += "Пробел между номером и буквой, надо: " + Regexes.space.Replace( hn.Value, Regexes.spaceReplace ).ToUpper() + "<br>";
            
            string correct = CorrectHouseNumb( hn.Value );
            if(!correct.IsEmpty())
                hn.Description += "Надо: <b>" + correct + "</b><br>";
            
            return false;
        }

        public string CorrectHouseNumb( string hn )
        {
            Match fullRes = Regexes.full2.Match( hn );
            string result = "";
            if ( fullRes.Success && fullRes.Length == hn.Length )
            {
                if ( fullRes.Groups[ 1 ].Success )
                {
                    Match downRes = Regexes.low.Match( fullRes.Groups[ 1 ].Value );
                    if ( downRes.Success && downRes.Groups[ 1 ].Length + downRes.Groups[ 2 ].Length == fullRes.Groups[ 1 ].Value.Length )
                    {
                        result += downRes.Groups[ 1 ].Value + downRes.Groups[ 2 ].Value.ToUpper();
                    }
                    else
                        result += fullRes.Groups[ 1 ].Value;
                }
                if ( fullRes.Groups[ 2 ].Success )
                {
                    result += " к" + fullRes.Groups[ 2 ].Value;
                }
                if ( fullRes.Groups[ 3 ].Success )
                {
                    result += " с" + fullRes.Groups[ 3 ].Value;
                }
                if ( fullRes.Groups[ 4 ].Success )
                {
                    result += " соор" + fullRes.Groups[ 4 ].Value;
                }
                if ( fullRes.Groups[ 5 ].Success )
                {
                    result += " лит" + fullRes.Groups[ 5 ].Value;
                }
                if ( fullRes.Groups[ 6 ].Success )
                {
                    result += " ф" + fullRes.Groups[ 6 ].Value;
                }
            }
            return result;
        }
    }
}
