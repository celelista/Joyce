using System;
using System.IO;

static class Program
{
    const string intensityFile = "WorldCupIllumination.csv";  
    
    static void Main( )
    {
        int[ ][ ] intensities;
        
        // Read the array of intensities from the file.
        
        using( StreamReader sr = new StreamReader( intensityFile ) )
        {
            int rows = int.Parse( sr.ReadLine( ) );
            int cols = int.Parse( sr.ReadLine( ) );
            
            intensities = new int[ rows ][ ];
            for( int row = 0; row < rows; row ++ )
            {
                intensities[ row ] = new int[ cols ];
                string[ ] words = sr.ReadLine( ).Split( ",".ToCharArray( ) );
                for( int col = 0; col < cols; col ++ )
                {
                    intensities[ row ][ col ] = int.Parse( words[ col ] );
                }
            }
        }
            
        // Find minimal and maximal intensities.
        int minIntensity;
        int maxIntensity;
        
        FindMinMax( intensities, out minIntensity, out maxIntensity );
        
        Console.WriteLine( );
        Console.WriteLine( "minimal intensity = {0}", minIntensity );
        Console.WriteLine( "maximal intensity = {0}", maxIntensity );
    }
 // -------------------------------------------------------------------
// Biomedical Engineering Program
// Department of Systems Design Engineering
// University of Waterloo
//
// Student Name:     <JOYCE ZIYI ZHANG>
// Userid:           <J495ZHAN>
//
// Assignment:       <Weekly Assignment 5>
// Submission Date:  <Oct 25>
// 
// I declare that, other than the acknowledgements listed below, 
// this  is my original work.
//
// Acknowledgements:
// <N/A>
// -------------------------------------------------------------------

//============================================================================
//the static helper method I wrote to find the max and min values in the array
    public static void FindMinMax(int[][] intensities, out int min, out int max)
    {
        int maxComparison = intensities[0][0];
        int minComparison = intensities[0][0];
        
        // looping through the different arrays in the jagged array
        for (int i= 0; i< intensities.Length; i++)
        {
        // looping through the elements in an array
            for(int j = 0; j< intensities[i].Length; j++)
            {
                //sorting through the max and min values
                if ( intensities[i][j] > maxComparison)
                {
                    maxComparison = intensities[i][j];
                }
                else if( intensities[i][j] < minComparison)
                {
                    minComparison = intensities[i][j];
                }
            }
        }
        // assigning the results of the sorting to the variables
        max = maxComparison;
        min = minComparison;
    }
// end of static helper method
//=============================================================================
}
