
using System;
using System.Collections;
using System.IO;

// Hold information about one baby name.
class BabyName
{
    string name;
    int numFemales;
    int numMales;
    
    public BabyName( string name )
    {
        if( string.IsNullOrWhiteSpace( name ) ) 
            throw new ArgumentException( "Invalid baby name", "name" );
            
        this.name = name;
        numFemales = 0;
        numMales = 0;
    }
    
    public string Name { get { return name; } }
    public int NumFemales { get { return numFemales; } }
    public int NumMales { get { return numMales; } }
    
    public void IncrementFemales( int number ) { numFemales += number; }
    public void IncrementMales( int number ) { numMales += number; }
}


// A linked list carrying data of type BabyName.
class List
{
    class Node
    {
        Node link;
        BabyName data;
        
        public Node( BabyName data ) { link = null; this.data = data; }
        
        public Node Link { get { return link; } set { link = value; } }
        public BabyName Data { get { return data; } }
    }
    
    Node tail;
    Node head;
    
    public List( ) { tail = null; head = null; }
    
    //Beginning of Add Method
    //===================================================
    
    // Add new baby-name data to the linked list.
    public void Add( string name, int countFemales, int countMales )
    {
        if( string.IsNullOrWhiteSpace( name ) ) 
            throw new ArgumentException( "Invalid baby name", "name" );
        Node current = head;
        
        //beginning the list
        if (head == null)
        {
            // creating an object
            BabyName b = new BabyName(name);
            // creating the first node
            head = new Node(b);
            head.Data.IncrementFemales(countFemales);
            head.Data.IncrementMales(countMales);
            tail = head;
        }
        
        bool DoesContain = false;
        
        //search through the list and look for the name
            while( current != null )
            {
                if (current.Data.Name == name )
                {
                    // when it exits the loop, we have the position of the node
                    // with the object we are looking for
                    DoesContain = true;
                    break;
                }

                current = current.Link;
            }

        // case 1: baby name is in name and the number of males and females 
        //have to be updated
        if (DoesContain)
        {
            // updates the information
            current.Data.IncrementFemales(countFemales);
            current.Data.IncrementMales(countMales); 
        }
        
        // case 2: name has not been seen before, so create a new object
        // and add it to the list
        else if(current != head)
        {
            
            // creating an object
            BabyName b = new BabyName(name);

            // creating a new node
            Node newNode = new Node(b);

            //adding information to the object
            newNode.Data.IncrementFemales(countFemales);
            newNode.Data.IncrementMales(countMales);

            // placing the node at the end of the list
            tail.Link = newNode;

            // updating the tail
            tail = newNode;
        }
        // TO DO:  Complete this method.
    }
    
    
    
    //===========================================================
    
    // Convert the linked list to an array of its data elements.
    public BabyName[ ] ToArray( )
    {
        // Collect data objects from the linked list.
        ArrayList objects = new ArrayList( );
        
        Node current = head;
        while( current != null )
        {
            objects.Add( current.Data );
            current = current.Link;
        }
        
        // Covert the collection of objects to an array of strings.
        BabyName[ ] results = new BabyName[ objects.Count ];
        
        for( int i = 0; i < results.Length; i ++ ) 
        {
            results[ i ] = ( BabyName ) objects[ i ];
        }
        
        return results;
    }
}


// Program to extract information from a file of baby-name data.
static class Program
{
    static string file = "Baby_Names__Beginning_2007.csv";
    
    static void Main( )
    {
        // Use a linked list to hold the baby-name data.
        List babies = new List( );
        
        // Read the baby-name data from the file.
        int count = 0;
        
        using( StreamReader sr = new StreamReader( file ) )
        {
            sr.ReadLine( );  // skip header line
            
            while( ! sr.EndOfStream )
            {
                string line = sr.ReadLine( );
                string[ ] words = line.Split( ',' );
                count ++;
                
                if( words.Length != 5 )
                {
                    Console.WriteLine( "{0}: {1}", count, line );
                }
                
                string year = words[ 0 ];
                string name = words[ 1 ];
                string county = words[ 2 ];
                string gender = words[ 3 ];
                int number = int.Parse( words[ 4 ] );
                
                if( gender == "M" ) babies.Add( name, 0, number );
                if( gender == "F" ) babies.Add( name, number, 0 );
            }
        }
        
        Console.WriteLine( );
        Console.WriteLine( "Read {0} records from \"{1}\"", count, file );
        
        // Display the baby-name data for names that are used for both genders.
        Console.WriteLine( );
        Console.WriteLine( 
            "The following names were popularly used for both genders:" );
            
        foreach( BabyName baby in babies.ToArray( ) )
        if( baby.NumFemales > 0 && baby.NumMales > 0 )
        {
            Console.WriteLine( "  {0}: {1} F, {2} M", 
                baby.Name, baby.NumFemales, baby.NumMales );
        }
    }
}