using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using OsmSharp.Osm.PBF.Streams;

namespace HouseNumberValidator
{
    class Work
    {
        List<IValidator> validators;

        public void Validate()
        {
            if ( Options.DownloadFromGislab )
                DownloadFiles( Options.Regions );
            else
                ReadFiles( Options.Regions );
        }

        private string[] GetDownFiles( int parts )
        {
            int n;
            var r = Reports.Reps;
            var minDate = r.Min( x => x.Stamp );
            int firsIndex = 0;

            for ( int i = 0; i < r.Count; i++ )
            {
                if ( r[ i ].Stamp > minDate )
                    continue;

                if ( i > 0 && r[ i - 1 ].Stamp == minDate )
                    continue;

                if ( r[ i ].Stamp == minDate )
                    firsIndex = i;
            }

            if ( firsIndex == 0 )
                n = 85 - ( 85 / parts ) * ( parts - 1 );
            else
                n = 85 / parts;

            var res = r.Skip( firsIndex ).Take( n );
            var count = res.Count();

            if ( count < n )
                res = res.Concat( r.Take( n - count ) );

            var result = res.Select( x => x.Region ).ToArray();

            return result;
        }

        public void DownloadFiles( string[] regions )
        {
            if ( regions == null )
                regions = GetDownFiles( 5 );
            //string[] regions = { /* "RU-TVE", "RU-RYA", "RU-SAM", "RU-AD", "RU-AL", "RU-ALT", "RU-AMU", */"RU-KYA" };

            string typefile = ".osm.pbf";
            WebClient webClient = new WebClient();

            foreach ( string region in regions )
            {
                Console.Write( "Download {0}", region );
                webClient.DownloadFile( @"http://data.gis-lab.info/osm_dump/dump/latest/" + region + typefile,
                    Paths.DirIn + region + typefile + ".down" );
                File.Delete( Paths.DirIn + region + typefile );
                File.Move( Paths.DirIn + region + typefile + ".down", Paths.DirIn + region + typefile );

                FileInfo file = new FileInfo( Paths.DirIn + region + typefile );

                Console.CursorLeft = 0;
                Console.Write( "Validate {0}", region );

                Stopwatch stw = new Stopwatch();
                stw.Start();
                ReadPbfFile( file );
                stw.Stop();
                //Console.WriteLine( stw.Elapsed );

                WriteReports( region, file.LastWriteTime );
                UploadToFtp( region );
            }
        }

        public void ReadFiles( string[] regions )
        {
            string typefile = ".osm.pbf";

            if ( !Directory.Exists( Paths.DirOut ) )
                Directory.CreateDirectory( Paths.DirOut );

            foreach ( string fl in Directory.GetFiles( Paths.DirIn, "*" + typefile ) )
            {
                FileInfo file = new FileInfo( fl );
                string region = file.Name.Split( '.' )[ 0 ];
                //DateTime stump = Reports.GetRegion( region ).Stamp;


                if ( regions != null && regions.Length > 0 && !regions.Contains( region ) ) 
                    continue;

                Console.CursorLeft = 0;
                Console.Write( "Validate {0}\t", region );

                Stopwatch stw = new Stopwatch();
                stw.Start();
                ReadPbfFile( file );
                stw.Stop();
                //Console.WriteLine( stw.Elapsed );


                WriteReports( region, file.LastWriteTime );
            }
        }

        public void ReadPbfFile( FileInfo pbfile )
        {
            validators = new List<IValidator>();
            validators.Add( new ValidatorHouseNumb() );
            validators.Add( new ValidatorFlats() );
            validators.Add( new ValidatorNoStreet() );
            validators.Add( new ValidatorNames() );
            validators.Add( new ValidatorDoubleTag() );
            validators.Add( new ValidatorUnorrectTag() );
            GeoOperations.ClearCollections( pbfile.Length );

            using ( FileStream fileStream = pbfile.OpenRead() )
            {
                PBFOsmStreamSource reader = new PBFOsmStreamSource( fileStream );
                foreach ( var geo in reader )
                {
                    if ( geo.Id.HasValue )
                    {
                        validators.ForEach( x => x.ValidateObject( geo ) );
                        GeoOperations.Add( geo );
                    }
                }
            }

            validators.ForEach( x => x.ValidateEndReadFile() );
        }

        private void WriteReports( string region, DateTime dateDump )
        {
            Console.CursorLeft = 0;
            Console.WriteLine( "{0}:\t{1,4} err, {2,3} flats, {3,4} strt, {4,5} names", region,
                validators.Find( x => x is ValidatorHouseNumb ).Errors.Count,
                validators.Find( x => x is ValidatorFlats ).Errors.Count,
                validators.Find( x => x is ValidatorNoStreet ).Errors.Count,
                validators.Find( x => x is ValidatorNames ).Errors.Count
                );

            Reports.Edit( new RegionalReport( region,
                validators.Find( x => x is ValidatorHouseNumb ).Errors.Count,
                validators.Find( x => x is ValidatorFlats ).Errors.Count,
                validators.Find( x => x is ValidatorNoStreet ).Errors.Count,
                validators.Find( x => x is ValidatorNames ).Errors.Count,
                validators.Find( x => x is ValidatorDoubleTag ).Errors.Count,
                dateDump
                ) );


            validators.ForEach( x => ReportHtml.SaveList( x, region, dateDump ) );
            validators.ForEach( x => ReportHtml.SaveMap( x, region ) );

            ReportHtml.SaveIndexList();
        }

        private void UploadToFtp( string region )
        {
            if ( !Options.UploadToFtp )
                return;

            string ftpUrl, ftpLogin, ftpPass;

            if ( !File.Exists( "ftp.txt" ) )
            {
                Console.WriteLine( "Not found \"ftp.txt\"" );
                return;
            }

            using ( StreamReader rd = new StreamReader( "ftp.txt" ) )
            {
                try
                {
                    ftpUrl = rd.ReadLine();
                    ftpLogin = rd.ReadLine();
                    ftpPass = rd.ReadLine();
                }
                catch ( Exception ex )
                {
                    Console.WriteLine( "Error read \"ftp.txt\"" );
                    return;
                }
            }

            List<string> files = new List<string>{
                "v",
                "",
                "warning",
                "flats",
                "nostreet",
                "names",
                "errors.map",
                "warning.map",
                "nostreet.map",
                "names.map",
                "double.map"
            };


            foreach ( var fl in files )
            {
                string filename = region + "." + fl + ".html";
                if ( fl == "" )
                    filename = region + ".html";
                if ( fl == "v" )
                    filename = "v.html";
                if ( fl.EndsWith( "map" ) )
                    filename = @"map/" + region + "." + fl + ".html";

                FileStream fileStream;
                try { fileStream = File.OpenRead( Paths.DirOut + filename ); }
                catch ( FileNotFoundException ex ) { continue; }

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create( ftpUrl + filename );
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential( ftpLogin, ftpPass );
                Stream ftpStream = request.GetRequestStream();

                byte[] buffer = new byte[ 1024 ];
                int bytesRead = 0;
                do
                {
                    bytesRead = fileStream.Read( buffer, 0, 1024 );
                    ftpStream.Write( buffer, 0, bytesRead );
                }
                while ( bytesRead != 0 );
                fileStream.Close();
                ftpStream.Close();
            }
        }
    }
}
