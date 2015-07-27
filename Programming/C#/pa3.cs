// -------------------------------------------------------------------
// Biomedical Engineering Program
// Department of Systems Design Engineering
// University of Waterloo
//
// Student Name:     <JOYCE ZHANG>
// Userid:           <J495ZHAN>
//
// Assignment:       <PA3>
// Submission Date:  <November 14th, 2014>
// 
// I declare that, other than the acknowledgements listed below, 
// this program is my original work.
//
// Acknowledgements:
// <N/A>
// -------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

// Class holds the sequence description of a protein.
class Protein
{
    // this declares the properties of the Protein class
    string id;
    string name;
    string sequence;
    
    // creating the protein object
    public Protein( string id, string name, string sequence )
    {
        this.id = id;
        this.name = name;
        this.sequence = sequence;
    }
    
    public string Id { get { return id; } }
    public string Name { get { return name; } }
    
    // Does the protein sequence contain a specified subsequence?
    // first helper method
    public bool ContainsSubsequence( string sub )
    {
        if (this.sequence.IndexOf(sub) != -1)
            {
                return true;
            
            }
        else
        {
            return false;
        }
        
        
    }
    
    // How often does a specified subsequence occur in the protein?
    // helper method 2
    public int CountSubsequence( string sub )
    {
        bool containsSub = ContainsSubsequence(sub); 
        if(!containsSub)
        {
            return 0;
        }
        int position = 0;
        int counter = 0;
        
        //use a loop to go through the rest of the sequence by analyzing the
        //position and starting at the next possible position.
        while ( (position = this.sequence.IndexOf( sub, position)) != -1)
        {
            position+= sub.Length;
            counter++;
        }
        return counter;
    }
    
    
    // Output a string showing only the location of a specified subsequence
    // in the sequence which specifies the protein.
    public string LocateSubsequence( string sub )
    {
        bool containsSub = ContainsSubsequence(sub); 
        if (!containsSub)
        {
            return null;
        }
        // create a copy of the this.sequence
        
        string targetSequence = String.Copy(this.sequence); 
        
        //returns a copy string of this.sequence where anything not matching 
        //the subsquence gets replaced by a .
        // copy the positions of the substrings into an array for later use
        // the "empty" array spaces are set to zero

        int[] positions = new int[targetSequence.Length]; 
       
       // filling up the array
        int i = 0;
        
        // finding the first occurance
        int position = targetSequence.IndexOf(sub);
       while ( targetSequence.IndexOf( sub, position) != -1)
        {
          positions [i] = position;
          position += sub.Length - 1;
          i++;
        }
       //turn the this.sequence into a char array
       char[] charSequence  = targetSequence.ToCharArray();
       
       // turn all the elements in the char array into a "." except for that 
       //containing the substring
       // use the position char array to find this information
        i = 0;
        
        //create a parallel bool array for the conversion
        bool[] hasSeq = new bool[targetSequence.Length];
        
        foreach (int pos in positions)
        {
            if(pos != 0)
            {
                for (int k = pos; k <= (pos + sub.Length -1); k++)
                {
                    hasSeq[k] = true;
                }
            }
        }
        
        for(int j = 0; j < targetSequence.Length;j++)
        {
            if( hasSeq[j] == false)
            {
                charSequence[j] = '.';
            }
        }
        
        targetSequence = new string(charSequence);
        return targetSequence;
    }

    // Write the FASTA description of the protein to a given text stream.
    public void WriteFasta( TextWriter output ) // a reference to Console.Out
    {
        Console.WriteLine(">{0} {1}", this.id, this.name);
        
        int a = 0;
        int b = 80;
        while(true)
        {
            if(a+80 >= this.sequence.Length)
            {
                Console.WriteLine(
                    this.sequence.Substring(a, sequence.Length - a));
            break;
            }
            Console.WriteLine(this.sequence.Substring(a,b));
            a += 80;
        }
    }

    
}// end of Protein class

// Read a protein file into a collection and test the functionality of
// methods in the Protein class.
static class Program
{
    static string fastaFile = "protein.fasta";
  
    static List< Protein > proteins = new List< Protein >( );
    
    static void Main( )
    {
        
        // Read proteins in FASTA format from the file.
        using( StreamReader sr = new StreamReader( fastaFile ) )
        {
           
           string id = null;
            string name = null;
            
            string line = sr.ReadLine();
            //while line not null
            while( line != null)
            {
                
                //set header to null
                string header = null;
                
                //while header null and line not null
                while ((line != null) && (header == null))
                {   
                    
                   
                // if empty line, skip it (i.e., read next line)
                    if (string.IsNullOrWhiteSpace(line)== true)
                    {
                        line = sr.ReadLine();
                    }
                
                // else if header line, set header, read next line
                    else if (line.StartsWith(">"))
                    {
                        header = line; 
                        //split the header into the id and the chemical name
                        id = header.Substring(1,10);
                        name = header.Substring(12);
                        line = sr.ReadLine();
                    }
                // else throw an exception (expected a header line)
                    else 
                    {
                       throw new Exception("expected a header line");
                        
                    }
                }
                
                //set sequence to null, not complete
                bool IsComplete = false;
                string sequence = null;
                
                // it's in a infinite loop right nowwwww
                
                //while sequence not complete and line not null
                while( (!IsComplete) && (line != null) )
                {
                    
                //| if empty line, skip it (i.e., read next line)
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        line = sr.ReadLine();
                    }
             //else if not header line, concatenate to sequence, read next line                
                    else if(!line.StartsWith(">"))
                    {
                        sequence = sequence + line.Trim();
                        line = sr.ReadLine();
                    }
              //else (is header line) if sequence not null, sequence is complete
                    else if (sequence != null)
                    {
                        // if(sequence != null)
                        // {
                            IsComplete = true;
                        // }
                    }
                //| else throw an exception (expected a sequence line)
                    else
                    {
                        throw new Exception
                            (" Expected sequence line");
                    }
                }
                
                
                //if header not null
                if (header != null)
                {
                    //| if sequence not null
                    if(sequence != null)
                    {
                    //| | add protein to collection
                    // This is how to create a new object
                    Protein p = new Protein( id, name, sequence);
                    proteins.Add(p);
                    }
                //| else throw an exception (header with missing sequence)
                    else 
                    {
                         throw new Exception ("Header with missing sequence");
                    }
                }
        }
        
        
        // Report count of proteins loaded.
        Console.WriteLine( );
        Console.WriteLine( "Loaded {0} proteins from the {1} file.", 
            proteins.Count, fastaFile );
          
        // Report proteins containing a specified sequence.
        Console.WriteLine( );
        Console.WriteLine( "Proteins containing sequence RILED:" );
        foreach( Protein p in proteins )
        {
            if( p.ContainsSubsequence( "RILED" ) )
            {
                Console.WriteLine( p.Name );
            }
        }
        
        // Report proteins containing a repeated sequence.
        Console.WriteLine( );
        Console.WriteLine( 
            "Proteins containing sequence SNL more than 5 times:" );
        foreach( Protein p in proteins )
        {
            if( p.CountSubsequence( "SNL" ) > 5 )
            {
                Console.WriteLine( p.Name );
            }
        }
        
        // Locate the specified sequence in proteins containing it.
        Console.WriteLine( );
        Console.WriteLine( "Proteins containing sequence DEVGG:" );
        foreach( Protein p in proteins )
        {
            if( p.ContainsSubsequence( "DEVGG" ) )
            {
                Console.WriteLine( p.Name );
                Console.WriteLine( p.LocateSubsequence( "DEVGG" ) );
            }
        }
        
        // Show FASTA output for proteins containing a specified sequence.
        Console.WriteLine( );
        Console.WriteLine( "Proteins containing sequence DEVGG:" );
        foreach( Protein p in proteins )
        {
            if( p.ContainsSubsequence( "DEVGG" ) )
            {
                p.WriteFasta( Console.Out );
            }
        }
        
        }// done using file
    }// end of Main()
}// end of the Program class
