using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HnumbValidator
{
    static class ReportHtml
    {
        static string iconMap = @"<img border=0 width=16 height=16 src=icons/icon_map.png alt='map' title='Показать на карте'>";


        public static void SaveIndexList( List<Validator> validators )
        {
            string outFileName = "index.html";

            using ( StreamWriter fw = new StreamWriter( Paths.DirOut + outFileName, false, Encoding.UTF8 ) )
            {
                fw.WriteLine( @"<html><head>" );
                fw.WriteLine( @"<title>Валидатор номеров домов</title>" );
                fw.WriteLine( @"<link rel=""stylesheet"" href=""style/List.css"" />" );
                fw.WriteLine( @"</style></head>" );

                fw.WriteLine( @"<body>" );
                fw.WriteLine( @"<b>Валидатор номеров домов</b>" );
                fw.Write( @" | " );
                fw.WriteLine( @"<a href=""http://forum.openstreetmap.org/viewtopic.php?id=53343"">Форум</a>" );
                fw.Write( @" | " );
                fw.WriteLine( @"<a href=""http://wiki.openstreetmap.org/wiki/RU:Валидаторы"">Другие валидаторы</a>" );
                fw.Write( @" | " );
                fw.WriteLine( @"<a href=""download"">Скачать</a>" );
                fw.Write( @" | " );
                fw.WriteLine( @"<a href=""https://github.com/zetx16/HnumbValidator"">GitHub</a>" );
                fw.WriteLine( @"<br>" );

                fw.WriteLine( @"<p class=""description warn"">Не мапьте под валидатор!</p>" );

                fw.WriteLine( @"<table>" );
                fw.Write( @"<tr>" );
                fw.Write( @"<td><b>Регион</b></td>" );
                fw.Write( @"<td><b>Дата проверки</b></td>" );

                foreach ( var v in validators )
                    fw.Write( @"<td><b>{0}</b></td>", v.Title );

                fw.WriteLine( @"</tr>" );

                int n = 0;

                // формируем строки
                foreach ( var statReigon in Reports.RegionList.StatRegions )
                {
                    if ( n++ % 2 == 1 )
                        fw.Write( @"<tr>" );
                    else
                        fw.Write( @"<tr class=""clr"">" );

                    fw.Write( @"<td>{0}</td>",
                        Regions.RegionsDict.ContainsKey( statReigon.Region ) ?
                        Regions.RegionsDict[ statReigon.Region ].Replace( "автономный округ", "АО" ).Replace( "автономная область", "АО" ) :
                        statReigon.Region
                    );
                    fw.Write( @"<td>{0:dd-MMM-yyyy}</td>", statReigon.Stamp );

                    foreach ( var v in validators )
                    {
                        var stat = statReigon.Stats.Find( x => x.ErrorType == v.GetType().Name );

                        if ( stat == null )
                            fw.Write( "<td></td>" );
                        else
                            fw.Write( CreateIndexCell(
                                statReigon.Region,
                                stat.CountNewErrors,
                                stat.CountOldErrors,
                                v.FileEnd,
                                v.ListableReport,
                                v.MapableReport
                            ) );
                    }

                    fw.WriteLine( @"</tr>" );
                }
                fw.WriteLine( @"</table>" );
                fw.WriteLine( @"</body></html>" );
            }
        }

        private static string CreateIndexCell( string region, int value, int oldValuse, string file, bool listable, bool mapable )
        {
            string result = "<td>";

            if ( value > 0 )
            {
                result += String.Format( @"<a href=""{0}"">{1}</a> <a href=""{2}"">{3}</a>",
                    @"map/" + region + "." + file + ".map.html",
                    iconMap,
                    region + "." + file + ".html",
                    value > 0 ? value.ToString() : ""
                );
            }

            if ( value == 0 )
                result += "<font color=\"gold\">★</font>";

            if ( oldValuse >= 0 && oldValuse != value )
                result += String.Format( @" <span class=""{1}"">{0}</span>",
                    ( value - oldValuse ).ToString( "+#;−#;0" ), //−
                    value > oldValuse ? "countup" : "countdown"
                );

            result += "</td>";

            return result;
        }

        public static void SaveList( Validator validator, string region, DateTime dateDump )
        {
            if ( validator.Errors.Count == 0 ) return;

            using ( StreamWriter fw = new StreamWriter( validator.GetPathList( Paths.DirOut, region ), false, Encoding.UTF8 ) )
            {
                string regionName = Regions.RegionsDict.ContainsKey( region ) ? Regions.RegionsDict[ region ] : region;

                fw.WriteLine( @"<html><head>" );
                fw.WriteLine( @"<title>{0}</title>", regionName );

                fw.WriteLine( @"<style type=""text/css"">" );
                fw.WriteLine( @"</style>" );
                fw.WriteLine( @"<link rel=""stylesheet"" href=""style/List.css"" />" );
                fw.WriteLine( @"</head>" );

                fw.WriteLine( @"<body>" );
                fw.WriteLine( @"<img id=josm width=1 height=1 border=0 style='display:none' />" );

                fw.WriteLine( @"<script><!--" );
                fw.WriteLine( @"function open_josm(x) { document.getElementById('josm').src='http://127.0.0.1:8111/load_object?objects='+x; }" );
                fw.WriteLine( @"--></script>" );
                fw.WriteLine( @"<script src=""js/sorttable.js""></script>" );

                string elements = "";
                bool zapytaya = false;
                foreach ( Error err in validator.Errors )
                {
                    if ( zapytaya )
                        elements += ",";
                    else
                        zapytaya = true;
                    elements += err.TypeStringShort + err.Osmid;
                }

                fw.WriteLine( @"<b>{1}</b> | Дата проверки: {0:d MMM yyyy} | Ошибок: {2} | <a href=""{3}"">Карта</a><br>",
                    dateDump,
                    regionName,
                    validator.Errors.Count,
                    validator.GetPathMap( "map/", region )
                );

                if ( validator.DescriptionForList != string.Empty )
                    fw.WriteLine( @"<p class=""description"">" + validator.DescriptionForList + "</p>" );
                else
                    fw.Write( @"<br>" );


                fw.WriteLine( @"<table class=""sortable"">" );
                fw.Write( @"<tr>" );
                fw.Write( @"<td>" );
                fw.Write( @"<a href=""http://127.0.0.1:8111/load_object?objects=" );
                fw.Write( elements );
                fw.Write( @""" onClick=""open_josm('" );
                fw.Write( elements );
                fw.Write( @"');return false;""><img border=0 width=20 height=20 src=icons/icon_josm_all.png></a>" );
                fw.Write( @"</td>" );
                fw.Write( @"<td><b>DateStamp</b></td>" );
                foreach ( var head in validator.GetTableHead() )
                    fw.Write( @"<td><b>{0}</b></td>", head );
                fw.WriteLine( @"</tr>" );


                int n = 0;
                foreach ( Error err in validator.Errors )
                {
                    if ( n++ % 2 == 1 )
                        fw.Write( @"<tr>" );
                    else
                        fw.Write( @"<tr class=""clr"">" );
                    fw.Write( @"<td><a href=""http://127.0.0.1:8111/load_object?objects={0}{1}"" onClick=""open_josm('{0}{1}');return false;""><img src=icons/icon_to_josm.png></a>"
                        + @"&nbsp;<a href=""http://level0.osmz.ru/?url={0}{1}""><img src=icons/icon_to_level0.png></a></td>",
                        err.TypeStringShort, err.Osmid );
                    fw.Write( @"<td>{0:yyyy-MM-dd}</td>", err.TimeStump.Date);
                    fw.Write( @"<td><a href=""http://osm.org/{0}/{1}"">{2}</a></td>", 
                        err.TypeString, err.Osmid, err.Value == "" ? "[osm]" : err.Value );
                    fw.Write( @"<td>" + err.Description + @"</td>" );
                    fw.Write( @"</tr>" );
                    fw.WriteLine();
                }
            }
        }

        public static void SaveMap( Validator validator, string region )
        {
            if ( validator.Errors.Count == 0 ) return;

            string indexFile = validator.GetPathMap( Paths.DirOutMap, region );

            using ( StreamWriter wr = new StreamWriter( indexFile ) )
            {
                wr.Write( Resources.map_file_begin );

                wr.WriteLine( @"map.setView([{0}, {1}], 7);",
                    validator.Errors[ 0 ].lat.ToString().Replace( ',', '.' ),
                    validator.Errors[ 0 ].lon.ToString().Replace( ',', '.' ) );

                bool cluster = validator.Errors.Count > 150 ? true : false;
                bool nostreet = validator is ValidatorNoStreet;

                string elements = "";
                bool zapytaya = false;

                foreach ( var err in validator.Errors )
                {
                    if ( zapytaya )
                        elements += ",";
                    else
                        zapytaya = true;
                    elements += err.TypeStringShort + err.Osmid;

                    if ( err.lat == 0 )
                        continue;

                    string popupText = String.Format( @"<div class=""popup popup-edit"">"
                        + @"<a href=""http://127.0.0.1:8111/load_object?objects={3}{1}"" onClick=""open_josm('{3}{1}');return false;""><img src=../icons/icon_to_josm.png></a> "
                        + @"<a href=""http://level0.osmz.ru/?url={0}{1}""><img src=../icons/icon_to_level0.png></a>"
                        + @"</div><div class=""popup popup-link"">"
                        + @"<a href=""http://osm.org/{0}/{1}"">{2}</a><br>"
                        + @"</div><div class=""popup popup-description"">{4}</div>",
                        err.TypeString,
                        err.Osmid,
                        err.Value == "" ? "[osm]" : err.Value.Trim(),
                        err.TypeStringShort,
                        err.Description
                    ).Replace( "\\", "\\\\" ).Replace( "\"", "\\\"" );

                    string icon;
                    switch ( err.Level )
                    {
                        case ErrorLevel.Level0: icon = "iErr"; break;
                        case ErrorLevel.Level5: icon = "iWarn"; break;
                        default: icon = "iErr"; break;
                    }

                    wr.WriteLine( string.Format( @"L.marker([{0}, {1}],{{icon: {4}}}).addTo({2}).bindPopup(""{3}"");",
                        err.lat.ToString().Replace( ',', '.' ),
                        err.lon.ToString().Replace( ',', '.' ),
                        cluster ? "markerCluster" : "map",
                        popupText,
                        icon
                        ) );
                }

                string allToJosm = String.Format( @"<a href=""http://127.0.0.1:8111/load_object?objects={0}"" onClick=""open_josm('{0}');return false;""><p align=""right"">Открыть всё в Josm</p></a>",
                    elements 
                );

                wr.WriteLine( @"function showDisclaimer() {{ var div = document.getElementById(""info""); div.innerHTML = ""{1}<br><p>{0}</p>""; }} ",
                    validator.DescriptionForMap.Replace( "\"", "\\\"" ),
                    allToJosm.Replace( "\"", "\\\"" )
                );

                wr.Write( Resources.map_file_end );
            }

        }

        public static void SaveMapAll()
        {
            List<string> types = new List<string>{
                "errors",
                "warning",
                "nostreet"
            };

            foreach ( var type in types )
            {
                string outPath = Paths.DirOutMap + "RU." + type + ".map.html";
                List<string> errors = new List<string>();

                foreach ( var file in Directory.GetFiles( Paths.DirOutMap, "RU-*." + type + ".map.html" ) )
                {
                    using ( StreamReader sr = new StreamReader( file ) )
                    {
                        while ( !sr.EndOfStream )
                        {
                            string line = sr.ReadLine();
                            if ( line.StartsWith( "L.marker" ) )
                                errors.Add( line.Replace( "(map)", "(markerCluster)" ) );
                        }
                    }
                }
                errors = errors.Distinct().ToList();
                using ( StreamWriter wr = new StreamWriter( outPath ) )
                {
                    wr.Write( Resources.map_file_begin );

                    wr.WriteLine( @"map.setView([61.0455502, 83.6036577], 4);" );

                    foreach ( var line in errors )
                        wr.WriteLine( line );

                    wr.Write( Resources.map_file_end );
                }
            }
        }
    }
}
