using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HnumbValidator
{
    class WorkAsync
    {
        Queue<string> toDownload;
        Queue<string> toValidate;
        bool validating = false;
        bool downloading = false;
        Work work;

        string typefile = ".osm.pbf";
        WebClient webClient = new WebClient();
        Task validatingTask;

        public WorkAsync( Work w )
        {
            work = w;

            toDownload = new Queue<string>( Options.Regions ?? work.GetDownFiles( 5 ) );
            toValidate = new Queue<string>();
        }

        public void Start()
        {
            Download().Wait();
            validatingTask.Wait();
        }

        private async Task Download()
        {
            if ( downloading )
                return;

            while ( toDownload.Count != 0 )
            {
                downloading = true;
                string region = toDownload.Peek();
                var uri = new Uri( @"http://data.gis-lab.info/osm_dump/dump/latest/" + region + typefile );

                await Task.Run( () => webClient.DownloadFile( uri, Paths.DirIn + region + typefile + ".down" ) );

                File.Delete( Paths.DirIn + region + typefile );
                File.Move( Paths.DirIn + region + typefile + ".down", Paths.DirIn + region + typefile );

                Console.WriteLine( "Downloaded {0}", region );

                toValidate.Enqueue( toDownload.Dequeue() );
                if ( !validating )
                    validatingTask = Validate();
            }
        }

        private async Task Validate()
        {
            if ( validating )
                return;

            if ( toValidate.Count == 0 && toDownload.Count == 0 )
                return;

            while ( toValidate.Count != 0 )
            {
                validating = true;
                await Task.Run( () => Validate( toValidate.Dequeue() ) );
            }
            validating = false;
        }

        private void Validate( string region )
        {
            FileInfo file = new FileInfo( Paths.DirIn + region + typefile );
            work.ReadPbfFile( file );
            work.WriteReports( region, file.LastWriteTime );
            work.UploadToFtp( region );
            Console.WriteLine( "Validated  {0}", region );
        }
    }
}
