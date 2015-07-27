
using System;

// Class Sweater holds the stock warehouse description of a sweater.
class Sweater
{
    private string style;
    private string colours;
    private int size;
    private double price;  // CDN dollars
    private bool isInStock;
    
    public Sweater( string style, string colours, int size, double price, 
        bool isInStock )
    {
        this.style = style;
        this.colours = colours;
        this.size = size;
        this.price = price;
        this.isInStock = isInStock;
    }
    //This section is doing the "micro formatting" of the individual strings
    public override string ToString( )
    {
            string Available="";
            string output = String.Format( "{0,-12} {1,-13}size {2,3}  $ 
            {3, 5:F2} {4,-13}", style, colours, size, price, (this.isInStock)? 
            "": "Not Available");
            
            return output;
    }
}

static class Program
{
    // Method Main builds and displays a list of sweaters.
    static void Main( )
    {
        //Now we are print out the output and doing the "macroformatting" or 
        //formatting the overall structure of the output
        Sweater s1, s2, s3, s4;
        
        s1 = new Sweater( "Turtleneck", "Green",        12, 59.95, true  );
        s2 = new Sweater( "V-neck",     "Beige/black",   8, 32.00, false );
        s3 = new Sweater( "Crew-neck",  "Purple/white", 10,  9.95, true  );
        s4 = new Sweater( "V-neck",     "Beige/black",  10, 32.00, true  );
        
        Console.WriteLine( );
        Console.WriteLine( "Sweaters" );
        Console.WriteLine( new String( '-', 57 ) );
        Console.WriteLine( s1 );
        Console.WriteLine( s2 );
        Console.WriteLine( s3 );
        Console.WriteLine( s4 );
        Console.WriteLine( new String( '-', 57 ) );
    }
}