
using System;

class Angle
{
    private double radians;
    public double Radians
    {
        get { return this.radians; }
        set { this.radians = value; }
    }
    
    //converts degrees to radians
    public double Degrees
    {
        get{return( this.radians * 180d / Math.PI );}
        set{this.radians = ( value * Math.PI / 180d );}
    }

    public double Sin { get { return Math.Sin( this.radians ); } }
    public double Cos { get { return Math.Cos( this.radians ); } }
    public double Tan { get { return Math.Tan( this.radians ); } }
    
    public override string ToString( )
    {
        //this was written to match the example in the PDF
        return string.Format( "{0} ({1}\u00b0)", this.radians, this.Degrees );
    }

 }

