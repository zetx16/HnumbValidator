using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace HouseNumberValidator
{
    public class RegionalReport
    {
        public string Region;

        public int Errors;
        public int ErrorsOld;

        public int Flats;
        public int FlatsOld;

        public int NoStreet;
        public int NoStreetOld;

        public int Names;
        public int NamesOld;

        public int Double;
        public int DoubleOld;

        public DateTime Stamp;
        
        public RegionalReport( string reg, DateTime dt, int er, int flt, int str, int nm, int dbl )
        {
            Region      = reg;

            Errors      = er;
            Flats       = flt;
            NoStreet    = str;
            Names       = nm;
            Double      = dbl;

            ErrorsOld   = -1;
            FlatsOld    = -1;
            NoStreetOld = -1;
            NamesOld    = -1;
            DoubleOld   = -1;

            Stamp       = dt;
        }

        public RegionalReport( string str )
        {
            string[] strs = str.Split( ',' );

            Region = strs[ 0 ];
            Stamp  = DateTime.Parse( strs[ 1 ] ); 

            Errors      = int.Parse( strs[ 2 ] );
            ErrorsOld   = int.Parse( strs[ 3 ] );
            Flats       = int.Parse( strs[ 4 ] );
            FlatsOld    = int.Parse( strs[ 5 ] );
            NoStreet    = int.Parse( strs[ 6 ] );
            NoStreetOld = int.Parse( strs[ 7 ] );
            Names       = int.Parse( strs[ 8 ] );
            NamesOld    = int.Parse( strs[ 9 ] );
            Double      = int.Parse( strs[ 10 ] );
            DoubleOld   = int.Parse( strs[ 11 ] );
        }

        public override string ToString()
        {
            return string.Format( "{0},\t{1:dd.MM.yyyy},{2,5},{3,5},{4,5},{5,5},{6,5},{7,5},{8,5},{9,5},{10,5},{11,5}",
                Region, Stamp,
                Errors, ErrorsOld,
                Flats, FlatsOld,
                NoStreet, NoStreetOld,
                Names, NamesOld,
                Double, DoubleOld 
                );
        }
    }

    public static class Reports
    {
        public static List<RegionalReport> Reps = new List<RegionalReport>();

        static Reports()
        {
            if ( !File.Exists( Paths.FileReport ) )
                return;

            using ( StreamReader rd = new StreamReader( Paths.FileReport ) )
            {
                while ( !rd.EndOfStream )
                {
                    Reps.Add( new RegionalReport( rd.ReadLine() ) );
                }
            }
        }

        public static void Edit( RegionalReport report )
        {
            bool finded = false;
            foreach ( RegionalReport r in Reps )
            {
                if ( r.Region == report.Region )
                {
                    if ( r.Stamp.Date != report.Stamp.Date )
                    {
                        r.ErrorsOld = r.Errors;
                        r.FlatsOld = r.Flats;
                        r.NoStreetOld = r.NoStreet;
                        r.NamesOld = r.Names;
                        r.DoubleOld = r.Double;
                    }

                    r.Errors = report.Errors;
                    r.Flats = report.Flats;
                    r.NoStreet = report.NoStreet;
                    r.Names = report.Names;
                    r.Stamp = report.Stamp;
                    r.Double = report.Double;

                    finded = true;
                    break;
                }
            }

            if ( !finded )
                Reps.Add( report );

            Save();
        }

        public static RegionalReport GetRegion( string region )
        {

            foreach ( RegionalReport r in Reps )
            {
                if ( r.Region == region )
                {
                    return r;
                }
            }
            return null;
        }

        public static void Save()
        {
            using ( StreamWriter wr = new StreamWriter( Paths.FileReport, false ) )
            {
                foreach ( RegionalReport r in Reps )
                {
                    wr.WriteLine( r.ToString() );
                }
            }
        }
    }
}
