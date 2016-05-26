using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseNumberValidator
{
    static class ReportHtml
    {
        static string iconMap = @"<img border=0 width=16 height=16 src=icon_map.png alt='map' title='Показать на карте'>";

        public static void SaveIndexList()
        {
            string outFileName = "v.html";

            using ( StreamWriter fw = new StreamWriter( Paths.DirOut + outFileName, false, Encoding.UTF8 ) )
            {
                fw.WriteLine( @"<html><head>" );
                fw.WriteLine( @"<title>Валидатор номеров домов</title>" );
                fw.WriteLine( @"<link rel=""stylesheet"" href=""style/style.css"" />" );
                fw.WriteLine( @"</style></head>" );

                fw.WriteLine( @"<body>" );
                fw.WriteLine( @"<b>Валидатор номеров домов</b> | " );
                fw.WriteLine( @"<a href=""http://forum.openstreetmap.org/viewtopic.php?id=53343"">Форум</a>" );
                fw.Write( @" | " );
                fw.WriteLine( @"<a href=""http://wiki.openstreetmap.org/wiki/RU:Валидаторы"">Другие валидаторы</a>" );
                fw.Write( @"<br><br>" );
                fw.WriteLine( @"<table>" );
                fw.Write( @"<tr>" );
                fw.Write( @"<td><b>Регион</b></td>" );
                fw.Write( @"<td><b>Дата проверки</b></td>" );
                fw.Write( @"<td><b>Ошибки</b></td>" );
                fw.Write( @"<td><b>Квартиры</b></td>" );
                fw.Write( @"<td><b>Нет улицы</b></td>" );
                fw.Write( @"<td><b>Нет тега</b></td>" );
                fw.Write( @"<td><b>Дублирование</b></td>" );
                fw.Write( @"</tr>" );

                fw.WriteLine();
                int n = 0;

                string icon = @"<img border=0 width=16 height=16 src=icon_map.png alt='map' title='Показать на карте'>";

                foreach ( RegionalReport rep in Reports.Reps )
                {
                    if ( n++ % 2 == 1 )
                        fw.Write( @"<tr>" );
                    else
                        fw.Write( @"<tr class=""clr"">" );

                    fw.Write( @"<td>{0}</td>",
                        Regions.RegionsDict.ContainsKey( rep.Region ) ? Regions.RegionsDict[ rep.Region ] : rep.Region );
                    fw.Write( @"<td>{0:dd-MMM-yyyy}</td>", rep.Stamp );

                    //--------------Ошибки--------------

                    if ( rep.Errors > 0 )
                        fw.Write( @"<td><a href=""{2}"">{3}</a> <a href=""{0}"">{1}</a>",
                            rep.Region + ".html",
                            rep.Errors > 0 ? rep.Errors.ToString() : "",
                            @"map/" + rep.Region + ".errors.map.html",
                            icon
                            );
                    if ( rep.Errors == 0 )
                        fw.Write( @"<td>{0}", "<font color=\"gold\">★</font>" );
                    fw.Write( @" <font color=""{1}"" size=""2.3"">{0}</font></td>",
                        rep.ErrorsOld >= 0 && rep.ErrorsOld != rep.Errors ? String.Format( "{0}", ( rep.Errors - rep.ErrorsOld ).ToString( "+#;−#;0" ) ) : "",
                        rep.Errors - rep.ErrorsOld > 0 ? "red" : rep.Errors - rep.ErrorsOld < 0 ? "green" : "black"
                        );

                    //--------------Квартиры--------------

                    if ( rep.Flats > 0 )
                        fw.Write( @"<td><a href=""{0}"">{1}</a>",
                            rep.Region + ".flats.html",
                            rep.Flats > 0 ? rep.Flats.ToString() : ""
                            );
                    if ( rep.Flats == 0 )
                        fw.Write( @"<td>{0}", "<font color=\"gold\">★</font>" );
                    fw.Write( @" <font color=""{1}"" size=""2.3"">{0}</font></td>",
                        rep.FlatsOld >= 0 && rep.FlatsOld != rep.Flats ? String.Format( "{0}", ( rep.Flats - rep.FlatsOld ).ToString( "+#;−#;0" ) ) : "",
                        rep.Flats - rep.FlatsOld > 0 ? "red" : rep.Flats - rep.FlatsOld < 0 ? "green" : "black"
                        );

                    //--------------Нет улицы--------------

                    if ( rep.NoStreet > 0 )
                        fw.Write( @"<td><a href=""{2}"">{3}</a> <a href=""{0}"">{1}</a>",
                            rep.Region + ".nostreet.html",
                            rep.NoStreet > 0 ? rep.NoStreet.ToString() : "",
                            @"map/" + rep.Region + ".nostreet.map.html",
                            icon
                            );
                    if ( rep.NoStreet == 0 )
                        fw.Write( @"<td>{0}", "<font color=\"gold\">★</font>" );
                    fw.Write( @" <font color=""{1}"" size=""2.3"">{0}</font></td>",
                        rep.NoStreetOld >= 0 && rep.NoStreetOld != rep.NoStreet ? String.Format( "{0}", ( rep.NoStreet - rep.NoStreetOld ).ToString( "+#;−#;0" ) ) : "",
                        rep.NoStreet - rep.NoStreetOld > 0 ? "red" : rep.NoStreet - rep.NoStreetOld < 0 ? "green" : "black"
                        );

                    //--------------Нет тега--------------

                    if ( rep.Names > 0 )
                        fw.Write( @"<td><a href=""{2}"">{3}</a> <a href=""{0}"">{1}</a>",
                            rep.Region + ".names.html",
                            rep.Names > 0 ? rep.Names.ToString() : "",
                            @"map/" + rep.Region + ".names.map.html",
                            icon
                            );
                    if ( rep.Names == 0 )
                        fw.Write( @"<td>{0}", "<font color=\"gold\">★</font>" );
                    fw.Write( @" <font color=""{1}"" size=""2.3"">{0}</font></td>",
                        rep.NamesOld >= 0 && rep.NamesOld != rep.Names ? String.Format( "{0}", ( rep.Names - rep.NamesOld ).ToString( "+#;−#;0" ) ) : "",
                        rep.Names - rep.NamesOld > 0 ? "red" : rep.Names - rep.NamesOld < 0 ? "green" : "black"
                        );

                    //--------------Дублирование--------------

                    if ( rep.Double > 0 )
                        fw.Write( @"<td><a href=""{2}"">{3} {1}</a>",
                            rep.Region + ".double.html",
                            rep.Double,
                            @"map/" + rep.Region + ".double.map.html",
                            icon
                            );
                    if ( rep.Double == 0 )
                        fw.Write( @"<td>{0}", "<font color=\"gold\">★</font>" );
                    fw.Write( @" <font color=""{1}"" size=""2.3"">{0}</font></td>",
                        rep.DoubleOld >= 0 && rep.DoubleOld != rep.Double ? String.Format( "{0}", ( rep.Double - rep.DoubleOld ).ToString( "+#;−#;0" ) ) : "",
                        rep.Double - rep.DoubleOld > 0 ? "red" : rep.Double - rep.DoubleOld < 0 ? "green" : "black"
                        );

                    fw.Write( @"</tr>" );
                    fw.WriteLine();
                }

                fw.WriteLine( @"</table>" );
                fw.WriteLine( @"</body></html>" );
            }
            File.Copy( Paths.DirOut + outFileName, Paths.DirOut + "index.html", true );
        }

        private static string CreateIndexCell(string region, int value, int oldValuse, string file)
        {
            string result = "";

            if ( value > 0 )
                result += String.Format( @"<td><a href=""{2}"">{3}</a> <a href=""{0}"">{1}</a>",
                    region + ( file == "errors" ? "" : ( "." + file ) ) + ".html",
                    value > 0 ? value.ToString() : "",
                    @"map/" + region + "." + file + ".map.html",
                    iconMap
                );
            if ( value == 0 )
                result += String.Format( @"<td>{0}", "<font color=\"gold\">★</font>" );
            result += String.Format( @" <font color=""{1}"" size=""2.3"">{0}</font></td>",
                oldValuse >= 0 && oldValuse != value ? String.Format( "{0}", ( value - oldValuse ).ToString( "+#;−#;0" ) ) : "",
                value - oldValuse > 0 ? "red" : value - oldValuse < 0 ? "green" : "black"
            );

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
                    fw.WriteLine( "<br>" + validator.DescriptionForList + "<br>" );


                fw.WriteLine( @"<table>" );
                fw.Write( @"<tr>" );
                fw.Write( @"<td>" );
                fw.Write( @"<a href=""http://127.0.0.1:8111/load_object?objects=" );
                fw.Write( elements );
                fw.Write( @""" onClick=""open_josm('" );
                fw.Write( elements );
                fw.Write( @"');return false;""><img border=0 width=20 height=20 src=icon_josm_all.png></a>" );
                fw.Write( @"</td>" );
                foreach ( var header in validator.GetTableHead() )
                    fw.Write( @"<td><b>{0}</b></td>", header );


                int n = 0;
                foreach ( Error err in validator.Errors )
                {
                    if ( n++ % 2 == 1 )
                        fw.Write( @"<tr>" );
                    else
                        fw.Write( @"<tr class=""clr"">" );
                    fw.Write( @"<td><a href=""http://127.0.0.1:8111/load_object?objects={0}{1}"" onClick=""open_josm('{0}{1}');return false;""><img src=icon_to_josm.png></a>"
                        + @"&nbsp;<a href=""http://level0.osmz.ru/?url={0}{1}""><img src=icon_to_level0.png></a></td>",
                        err.TypeStringShort, err.Osmid );
                    //fw.Write( @"<td><a href=""http://level0.osmz.ru/?url={0}{1}""><img src=icon_to_level0.png></a></td>",
                    //    err.TypeStringShort, err.Osmid );
                    fw.Write( @"<td><a href=""http://osm.org/{0}/{1}"">{2}</a></td>", 
                        err.TypeString, err.Osmid, err.Value );
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
                using ( StreamReader rd = new StreamReader( Paths.FileIndexBegin ) )
                    wr.Write( rd.ReadToEnd() );

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

                    string popupText = String.Format(
                        @"<a href=\""http://127.0.0.1:8111/load_object?objects={3}{1}\"" onClick=\""open_josm('{3}{1}');return false;\""><img src=icon_to_josm.png></a> "
                        + @"<a href=\""http://osm.org/{0}/{1}\"">{2}</a><br>",
                        err.TypeString,
                        err.Osmid,
                        err.Value == "" ? "[osm]" : err.Value.Replace( "\\", "\\\\" ).Replace( "\"", "\\\"" ),
                        err.TypeStringShort
                        );

                    string icon;
                    switch ( err.Level )
                    {
                        case ErrorLevel.Level0: icon = "iErr"; break;
                        case ErrorLevel.Level5: icon = "iWarn"; break;
                        default: icon = "iErr"; break;
                    }

                    if ( nostreet && err.Description != string.Empty )
                    {
                        foreach ( var place in err.Description.Split( '|' ) )
                            popupText += place.Replace( ":", " =" ).Replace( "\"", "\\\"" ).Trim();
                        icon = "iWarn";
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

                using ( StreamReader rd = new StreamReader( Paths.FileIndexEnd ) )
                    wr.Write( rd.ReadToEnd() );
            }

        }

        public static void SaveMapAll()
        {
            List<string> types = new List<string>{
                "errors",
                "warning",
                "nostreet",
                "names"
            };

            foreach ( var type in types )
            {
                string indexFile = Paths.DirOutMap + "RU." + type + ".map.html";
                string indexBeinFile = @"D:\OSM\ValidatorTT\index_begin.html";
                string indexEndFile = @"D:\OSM\ValidatorTT\index_end.html";
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
                using ( StreamWriter wr = new StreamWriter( indexFile ) )
                {
                    using ( StreamReader rd = new StreamReader( indexBeinFile ) )
                        wr.Write( rd.ReadToEnd() );

                    wr.WriteLine( @"map.setView([61.0455502, 83.6036577], 4);" );

                    foreach ( var line in errors )
                        wr.WriteLine( line );

                    using ( StreamReader rd = new StreamReader( indexEndFile ) )
                        wr.Write( rd.ReadToEnd() );
                }
            }
        }
    }
}
