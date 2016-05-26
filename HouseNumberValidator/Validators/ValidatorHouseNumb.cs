using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OsmSharp.Osm;

namespace HouseNumberValidator
{
    public class ValidatorHouseNumb : Validator
    {
        public ValidatorHouseNumb()
        {
            fileListEnd = ".html";
            fileMapEnd = ".errors.map.html";
            description = "";
            descriptionForMap = "";

            errors = new List<Error>();
        }

        public override void ValidateObject( OsmGeo geo )
        {
            string value;
            for ( int i = 1; i <= 9; i++ )
            {
                if ( geo.Tags.TryGetValue( String.Format( "addr{0}:housenumber", i > 1 ? i.ToString() : "" ), out value ) )
                {
                    if ( !GeoOperations.CountryRu( geo ) )
                        continue;

                    var error = new Error( geo, value );

                    bool valid_error = ValidateHouseNumbOnError( error );
                    bool valid_warn = ValidateHouseNumbOnWarn( error );

                    if ( !valid_error || !valid_warn )
                    {
                        GeoOperations.GetCoordinates( geo, error );
                        if ( !valid_warn )
                            error.Level = ErrorLevel.Level5;
                        if ( !valid_error )
                            error.Level = ErrorLevel.Level0;
                        errors.Add( error );
                    }
                    error.Description += GeoOperations.GetNotes( geo.Tags );
                }
                else
                    break;
            }
        }

        private bool ValidateHouseNumbOnWarn( Error hn )
        {
            Match downRes = Regexes.down.Match( hn.Value );
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

            //------------------------------------------
            /*
            if ( Regexes.CheckPattern( Regexes.korp, hn.Value ) )
                hn.Description += "Неверно обозначен корпус, надо: " + Regexes.korp.Replace( hn.Value, Regexes.korpReplace ) + "<br>";

            if ( Regexes.CheckPattern( Regexes.stroy, hn.Value ) )
                hn.Description += "Неверно обозначено строение, надо: " + Regexes.stroy.Replace( hn.Value, Regexes.stroyReplace ) + "<br>";

            if ( Regexes.CheckPattern( Regexes.soor, hn.Value ) )
                hn.Description += "Неверно обозначено сооружение, надо: " + Regexes.soor.Replace( hn.Value, Regexes.soorReplace ) + "<br>";
            */
            //------------------------------------------

            Match fullRes = Regexes.full2.Match( hn.Value );
            if ( fullRes.Success && fullRes.Length == hn.Value.Length )
            {
                string result = "";
                if ( fullRes.Groups[ 1 ].Success )
                {
                    hn.Description += "Неверно обозначено:";

                    Match downRes = Regexes.down.Match( fullRes.Groups[ 1 ].Value );
                    if ( downRes.Success && downRes.Groups[ 1 ].Length + downRes.Groups[ 2 ].Length == fullRes.Groups[ 1 ].Value.Length )
                    {
                        result += downRes.Groups[ 1 ].Value + downRes.Groups[ 2 ].Value.ToUpper()/* + downRes.Groups[ 3 ]*/;
                    }
                    else
                        result += fullRes.Groups[ 1 ].Value;
                }
                if ( fullRes.Groups[ 2 ].Success )
                {
                    hn.Description += " корпус";
                    result += " к" + fullRes.Groups[ 2 ].Value;
                }
                if ( fullRes.Groups[ 3 ].Success )
                {
                    hn.Description += " строение";
                    result += " с" + fullRes.Groups[ 3 ].Value;
                }
                if ( fullRes.Groups[ 4 ].Success )
                {
                    hn.Description += " сооружение";
                    result += " соор" + fullRes.Groups[ 4 ].Value;
                }
                hn.Description += ". Надо: <b>" + result + "</b><br>";
            }
            CorrectHouseNumb( hn.Value );

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
                    //hn.Description += "Неверно обозначено:";

                    Match downRes = Regexes.down.Match( fullRes.Groups[ 1 ].Value );
                    if ( downRes.Success && downRes.Groups[ 1 ].Length + downRes.Groups[ 2 ].Length == fullRes.Groups[ 1 ].Value.Length )
                    {
                        result += downRes.Groups[ 1 ].Value + downRes.Groups[ 2 ].Value.ToUpper()/* + downRes.Groups[ 3 ]*/;
                    }
                    else
                        result += fullRes.Groups[ 1 ].Value;
                }
                if ( fullRes.Groups[ 2 ].Success )
                {
                    //hn.Description += " корпус";
                    result += " к" + fullRes.Groups[ 2 ].Value;
                }
                if ( fullRes.Groups[ 3 ].Success )
                {
                    //hn.Description += " строение";
                    result += " с" + fullRes.Groups[ 3 ].Value;
                }
                if ( fullRes.Groups[ 4 ].Success )
                {
                    //hn.Description += " сооружение";
                    result += " соор" + fullRes.Groups[ 4 ].Value;
                }
                //hn.Description += ". Надо: <b>" + result + "</b><br>";
            }
            return result;
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
