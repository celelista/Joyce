// -------------------------------------------------------------------
// Biomedical Engineering Program
// Department of Systems Design Engineering
// University of Waterloo
//
// Student Name:     <JOYCE ZIYI ZHANG>
// Userid:           <J495ZHAN>
//
// Assignment:       <WEEKLY ASSIGNMENT 3>
// Submission Date:  <SEPT 27 20>
// 
// I declare that, other than the acknowledgements listed below, 
// this program is my original work.
//
// Acknowledgements:
// <N/A>

using System;
static class Sequence
{
    static void Main( )
    {
	    Console.WriteLine("{0,8}{1,8}","n","a(n)");// The header
        long m = 2L;
        //outer loop is generating numbers from 10001 and 10025
        for (long n = 10001L ; n <= 10025L ; n++)
        {
            //This inner loop is testing for appropriate m value.
            for (m = 2L; (Math.Pow(m, 3L) - 1L) % n != 0L; m++)
                {
                // tests for m until conditions are satisfied
                }
                string nConvert;
            nConvert = n.ToString("N0");
            string mConvert;
            mConvert = m.ToString("N0"); 
            // Conversions are for the sake of formatting the numbers. 
            Console.WriteLine("{0,8} {1,8}",nConvert,mConvert);
        }
    }
}


