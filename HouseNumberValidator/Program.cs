using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Resources;

namespace HouseNumberValidator
{
    class Program
    {
        static void Main( string[] args )
        {
            ParseArgs( args );
            
            Work p = new Work();
            p.Validate();

            Console.WriteLine( "Press Enter to exit..." );
            //Console.ReadLine();
        }

        private static void ParseArgs( string[] args )
        {
            bool commRegion = false;
            bool commDirectoryInput = false;
            bool commDirectoryOutput = false;
            foreach ( var arg in args )
            {
                if ( arg == "-download" || arg == "-down" || arg == "-d" )
                    Options.DownloadFromGislab = true;

                if ( commRegion )
                {
                    Options.Regions = ParseRegions( arg );
                    commRegion = false;
                }
                if ( arg == "-regions" || arg == "-region" || arg == "-r" )
                    commRegion = true;

                if ( commDirectoryInput )
                {
                    Options.DirectoryInput = arg;
                    commDirectoryInput = false;
                }
                if ( arg == "-indir" || arg == "-i" )
                    commDirectoryInput = true;

                if ( commDirectoryOutput )
                {
                    Options.DirectoryOutput = arg;
                    commDirectoryOutput = false;
                }
                if ( arg == "-outdir" || arg == "-o" )
                    commDirectoryOutput = true;

                if ( arg == "-upload" || arg == "-u" )
                    Options.UploadToFtp = true;
            }

            if ( Options.DirectoryInput.IsEmpty() )
                Options.DirectoryInput = Directory.GetCurrentDirectory();

            if ( Options.DirectoryOutput.IsEmpty() )
                Options.DirectoryOutput = Directory.GetCurrentDirectory();
        }

        private static string[] ParseRegions( string str )
        {
            var result = new List<string>();
            var regions = str.Split( new char[] { ' ', ',' } );

            foreach ( var region in regions )
                if ( region.Trim() != "" )
                    result.Add( region.Trim() );

            return result.ToArray();
        }
    }
}