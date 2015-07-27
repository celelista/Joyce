//Design a program to open the file StrategicPlan.txt, read the lines of text
// and produce output 
//which reports: number of lines with words starting with "innova", number of 
//lines with words starting with "entrepre", the beginning and ending line 
//numbers of a longest span of lines in the file not containing any words which 
//start with "innova" or "entrepre".
//assume: word is any sequence of chars seperated by white space, lines in file
//are numbered starting at 1, that there is capitalization variation.

using System;
using System.IO;
static class Program
{
    const string strategicFile = "StrategicPlan.txt";
    
    static void Main()
    {
        //figuring out how big the arrays have to be for the program
        int allLineNum = 0;
        using (StreamReader howBig = new StreamReader(strategicFile))
        {
            while(!howBig.EndOfStream)
            {
                howBig.ReadLine();
                allLineNum++;
            }
        }
        
        //are now declaring and initializing the arrays
        //bools are used because you either have or don't have the word
        bool [] withInnova = new bool[allLineNum];
        bool [] withWord = new bool[allLineNum];
        bool [] withEntrepre = new bool[allLineNum];
        
        int innovaIndex = 0;
        int entrepreIndex = 0;
        int lineNum = 0;
        
        // taking the file apart and analyzing it
        string line;
        string [] words;
        using (StreamReader sr = new StreamReader(strategicFile))
        {
            while(!sr.EndOfStream)
            {
                //disassemblying the file
                line = sr.ReadLine();
                words = line.Split (" ".ToCharArray());
                
                //testing for innova
                foreach (string word in words)
                {
                    
                    if(word.ToLower().StartsWith("innova"))
                    {
                        withInnova[innovaIndex] = true;
                        innovaIndex++;
                        withWord[lineNum] = true;
                        break;
                    
                    }
                    
                    
                }
                
                //testing for entrepre
                foreach (string word in words)
                {
                    
                    if(word.ToLower().StartsWith("entrepre"))
                    {
                        withEntrepre[entrepreIndex] = true;
                        entrepreIndex++;
                        if (withWord[lineNum]==false)
                        {
                            withWord[lineNum]= true;
                        }
                        break;
                    
                    }
                    
                }
                
               
                lineNum++;
                
            }// end of while loop
            
            
        }// finished using the files
        
        //we now have to count the total trues and falses in the two arrays 
        //we filled the values of.
        
        entrepreIndex = 0;
        innovaIndex = 0;
        
        int entrepreLine = 0;
        int innovaLine = 0;
        
        foreach (bool statement in withEntrepre)
        {
            if (withEntrepre[entrepreIndex])
            {
                entrepreLine++;    
            }
            
            entrepreIndex++;
        }
        
        foreach (bool statement in withInnova)
        {
            if (withInnova[innovaIndex])
            {
                innovaLine++;    
            }
            
            innovaIndex++;
        }
        
        Console.WriteLine("The total number of lines with words starting with"+
        "'innova' is {0}", innovaLine);
        Console.WriteLine("The total number of lines with words starting with"+
        "'entrepre' is {0}", entrepreLine);
        //we have to figure out the largest span of lines without either word
        //we now have to use the withWord array that we created to find the span
        
        //declaring variables
        lineNum = 0;
        int span = 0;
        int Start = 0;
        int End = 0;
        int bestStart = 0;
        int bestEnd = 0;
        int bestSpan = 0;
        
        while(true)
        {
            //finds the next false in the withWord array
            Start = Array.IndexOf(withWord, false, Start);
            if(Start == -1)
            {
                break;
            }
            
            //finds the next true in withWord, start at the position of false
            End = Array.IndexOf(withWord, true, Start);
            if(End == -1)
            {
                break;
            }
            
            //update length
            span = End - Start;
            
            if(span > bestSpan)
            {
                bestStart = Start;
                bestEnd = End;
                bestSpan = span;
            }
            
            Start = End;
        }
        Console.WriteLine("The longest span of lines in the file with neither "+
           "'innova' nor 'entrepre' words is from lines {0} to {1}", bestStart+1
           , bestEnd);
    }
}
