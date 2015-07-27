// -------------------------------------------------------------------
// Biomedical Engineering Program
// Department of Systems Design Engineering
// University of Waterloo
//
// Student Name:     <JOYCE ZHANG>
// Userid:           <J495ZHAN>
//
// Assignment:       <PA4>
// Submission Date:  <NOV 30 2014>
// 
// I declare that, other than the acknowledgements listed below, 
// this program is my original work.
//
// Acknowledgements:
// <N/A>
// -------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.IO;

// -----------------------------------------------------------------------------
// A Drug object holds information about one fee-for-service outpatient drug 
// reimbursed by Medi-Cal (California's Medicaid program) to pharmacies.
class Drug
{
    string name;            // brand name, strength, dosage form
    string id;              // national drug code number
    double size;            // package size
    string unit;            // unit of measurement
    double quantity;        // number of units dispensed
    double lowest;          // price Medi-Cal is willing to pay
    double ingredientCost;  // estimated ingredient cost
    int numTar;             // number of claims with a treatment auth. request
    double totalPaid;       // total amount paid
    double averagePaid;     // average paid per prescription
    int daysSupply;         // total days supply
    int claimLines;         // total number of claim lines
    
    // Properties providing read-only access to every field.
    public string Name { get { return name; } }               
    public string Id { get { return id; } }                 
    public double Size { get { return size; } }             
    public string Unit { get { return unit; } }             
    public double Quantity { get { return quantity; } }         
    public double Lowest { get { return lowest; } }             
    public double IngredientCost { get { return ingredientCost; } }    
    public int NumTar { get { return numTar; } }                
    public double TotalPaid { get { return totalPaid; } }          
    public double AveragePaid { get { return averagePaid; } }        
    public int DaysSupply { get { return daysSupply; } }            
    public int ClaimLines { get { return claimLines; } }            
    
    public Drug ( string name, string id, double size, string unit, 
        double quantity, double lowest, double ingredientCost, int numTar, 
        double totalPaid, double averagePaid, int daysSupply, int claimLines )
    {
        this.name = name;
        this.id = id;
        this.size = size;
        this.unit = unit;
        this.quantity = quantity;
        this.lowest = lowest;
        this.ingredientCost = ingredientCost;
        this.numTar = numTar;
        this.totalPaid = totalPaid;
        this.averagePaid = averagePaid;
        this.daysSupply = daysSupply;
        this.claimLines = claimLines;
    }

    // Simple string for debugging purposes, showing only selected fields.
    public override string ToString( )
    { 
        return string.Format( 
            "{0}: {1}, {2}", id, name, size ); 
    }
}

// -----------------------------------------------------------------------------
// Linked list of Drugs.  A list object holds references to its head and tail
// Nodes and a count of the number of Nodes.
class DrugList
{
    // Nodes form the singly linked list.  Each node holds one Drug item.
    class Node
    {
        Node next;
        Drug data;
        
        public Node( Drug data ) { next = null; this.data = data; }
        
        public Node Next{ get { return next; } set { next = value; } }
        public Drug Data{ get { return data; } }
    }
    
    Node tail;
    Node head;
    int count;
    
    public int Count { get { return count; } }
    
    // Constructors:
    public DrugList( ) { tail = null; head = null; count = 0; }
    public DrugList( string drugFile ) : this( ) { AppendFromFile( drugFile ); }
   
    // Methods which add elements:
    // Build this list from a specified drug file.
    public void AppendFromFile( string drugFile )
    {
        using( StreamReader sr = new StreamReader( drugFile ) )
        {
            while( ! sr.EndOfStream )
            {
                string line = sr.ReadLine( );
                
                // Extract drug information
                string name = line.Substring( 7, 30 ).Trim( );
                string id = line.Substring( 37, 13 ).Trim( );
                string temp = line.Substring( 50, 14 ).Trim( );
                double size 
                    = double.Parse( temp.Substring( 0 , temp.Length - 2 ) );
                string unit = temp.Substring( temp.Length - 2, 2 );
                double quantity = double.Parse( line.Substring( 64, 16 ) );
                double lowest = double.Parse( line.Substring( 80, 10 ) );
                double ingredientCost 
                    = double.Parse( line.Substring( 90, 12 ) );
                int numTar = int.Parse( line.Substring( 102, 8 ) );
                double totalPaid = double.Parse( line.Substring( 110, 14 ) );
                double averagePaid = double.Parse( line.Substring( 124, 10 ) );
                int daysSupply 
                    = ( int ) double.Parse( line.Substring( 134, 14 ) );
                int claimLines = int.Parse( line.Substring( 148 ) );
                
                // Put drug onto this list of drugs.
                Append( new Drug( name, id, size, unit, quantity, lowest, 
                    ingredientCost, numTar, totalPaid, averagePaid, 
                    daysSupply, claimLines ) );
            }
        }
    }
// Add a new Drug item to the end of this linked list.
// Append: Given a Drug object, the Append method places that Drug object in a
// new Node of the linked list after its last element. If a null Drug object is 
//passed to the Append method, it should just return without changing the list.
    public void Append( Drug data )
    {
        if (data == null)
        {
            return;
            
        }
        // Starting the list by creating the first Node
        else if (head == null)
        {
            head = new Node(data);
            count = count++;
            tail = head;
        }
        else
        {
        // creating a new node
            Node newNode = new Node(data);
            count = count++;
            // placing the node at the end of the list
            tail.Next= newNode;

            // updating the tail
            tail = newNode;
            
        }
    }
    
// Add a new Drug in order based on a user-supplied comparison method.
// The new Drug goes just before the first one which tests greater than it.
// InsertInOrder: Given a Drug object and a Func< Drug, Drug, int > comparison 
//method, the InsertInOrder method places the Drug object in a new Node of the
// linked list just before the first element greater than the new element under
// the comparison method. If a null Drug object is passed to the InsertInOrder 
    public void InsertInOrder( Drug data, Func< Drug, Drug, int > userCompare)
    {
        // checking if the object exists
        if(data == null)
        {
            return;
        }
        // checking if the list has any entries yet
        if(head == null)
        {
            // the list is empty, thus the insertee is the first element
            head = new Node(data);
            tail = head;
            head.Next = null;
            return;
        }
        
        // when it reaches this part of the code, it means that there are 
        // entries in the list, thus we have to find the correct place to put 
        // the insertee
        Node insertee = new Node (data); 
        Node current = head;
        Node previous = null;
        while((current != null) && (userCompare(data, current.Data) > 0) )
        {
            previous = current;
            // updating the node
            current = current.Next;
        }
        // the insertion place is the last one in the list
        if(previous == null)
        {
            insertee.Next = head;
            head = insertee; 
            return;
        }
        // if the insertion place has been found
        else
        {
            // the insertion place is the last place
            if(previous.Next == null)
            {
                tail = insertee;
            }
            insertee.Next = previous.Next;
            previous.Next = insertee;
        }
    }
//========================================================================
    // Methods which remove elements:
    
// RemoveFirst: The RemoveFirst method deletes the first Node of the linked list
// and returns its Drug object. If the list has no elements, the RemoveFirst 
//method should return null.
    public Drug RemoveFirst( )
    {
        if (head == null)
        {
            return null;
        }
        
        // creating a copy of the object
        Drug firstDrug = head.Data;
        Node temp = head.Next;
        head = null;
        head = temp;
        return firstDrug;
    }
    // RemoveMin: Given a Func< Drug, Drug, int > comparison method, the
// RemoveMin method removes the first Node holding a minimal element under the 
//comparison method, and returns its Drug object. If the list has no elements, 
//the RemoveMin method should return null.

    public Drug RemoveMin( Func< Drug, Drug, int > userCompare )
    {
        Node minPrevious = null;
        Node previousCompare = head;
        Node min = head;
        Node current = head.Next;
        bool IsLast = false;
        if(head == tail)
        {
            //case 1: the list doesn't exist yet
            if(head == null)
            {
                return null;
            }
            // case two: the list has only one node
            else
            {
                if(IsLast)
                {
                    //deleting the node
                    head = null;
                    tail = null;
                    return null;
                }
                IsLast = true;
                // returning the data in the last node
                return head.Data;
            }
            
        }
        
        // looking for the min in a list
        while(current != null)
        {
            // if the current is smaller than the min now, the min is updated
            if (userCompare(current.Data, min.Data) < 0)
            {
                //updating the previous min node
                minPrevious = previousCompare;
                // updating the minimum
                min = current;
                
            }
            //updating the loop
            previousCompare = current;
            current = current.Next;
        }
        
        // the minimum has been found in this case and now has to be returned 
        // and deleted
        //case 1: the min is at the head
        if(head == min)
        {
            head = min.Next;
        }
        else
        {
            // the next two cases use the minPrevious node:
            
            // if the min is at the tail
            if(tail == min)
            {
                // deleting the tail
                tail = minPrevious;
            }
            //if the min is in the middle; skipping over min
            minPrevious.Next = min.Next;
        }
        return min.Data;
        
        
    }
//========================================================================    
    // Methods which sort the list:
    
// SelectSort: Given a Func< Drug, Drug, int > comparison method, the SelectSort
// method uses repeated calls to RemoveMin and Append to convert the linked list
// to a sorted linked list ordered by the comparison method completed, not 
//tested
    public void SelectSort( Func< Drug, Drug, int > userCompare)
    {
        Node current = head;
        int count = 0;
        
        //counting the number of elements in the list
        while(current != null)
        {
            current = current.Next;
            count++;
        }

        //creating a new empty list
        DrugList temp = new DrugList();
        
        Drug temporary = RemoveMin(userCompare);
        // removing the first minimum value from the list 
        //and making a copy of it
        
        while (0 <= count)
        {
            // adding the head to the temp list
            temp.Append(temporary);
            temporary = RemoveMin(userCompare);
            
            // if there's nothing left in the list we're sorting from
            if (temporary == null)
            {
                break;
            }
            
            count--;
        }
        
        head =  temp.head;
        tail = temp.tail;

    }
    
// Sort this list by insertion sort with a user-specified comparison method.
// InsertSort: Given a Func< Drug, Drug, int > comparison method, the InsertSort
// method uses repeated callsto RemoveFirst and InsertInOrder to convert the 
//linked list to a sorted linked list ordered by the comparison method.
    public void InsertSort( Func< Drug, Drug, int > userCompare)
    {
        DrugList temp = new DrugList();
        
        Drug tempCompare = RemoveFirst();
        while(tempCompare != null)
        {
            temp.InsertInOrder(tempCompare, userCompare);
            tempCompare = RemoveFirst();
        }
        head = temp.head;
        tail = temp.tail;
    }
    
//=================================================================
    // Methods which extract the Drugs:
    
    // Return, as an array, references to all the Drug objects on the list.
    public Drug[ ] ToArray( )
    {
        Drug[ ] result = new Drug[ count ];
        int nextIndex = 0;
        Node current = head;
        while( current != null )
        {
            result[ nextIndex ] = current.Data;
            nextIndex ++;
            current = current.Next;
        }
        return result;
    }
    
    // Return, as an array, references to those Drub objects on the list meeting 
    // a condition specified by a user-supplied method.
    public Drug[ ] ToArray( Func< Drug, bool > userTest )
    {
        // Count the number of elements meeting the condition.
        int number = 0;
        foreach( Drug d in Enumeration ) if( userTest( d ) ) number ++;
        
        // Collect the elements meeting the condition.
        Drug[ ] result = new Drug[ number ];
        int nextIndex = 0;
        foreach( Drug d in Enumeration ) 
        {
            if( userTest( d ) )
            {
                result[ nextIndex ] = d;
                nextIndex ++;
            }
        }
        return result;
    }
    
    // Return a collection of references to the Drug items on this list which 
    // can be used in a foreach loop.  Understanding enumerations and the 
    // 'yield return' statement is beyond the scope of the course.
    public IEnumerable< Drug > Enumeration
    {
        get
        {
            Node current = head;
            while( current != null )
            {
                yield return current.Data;
                current = current.Next;
            }
        }
    }
}

// -----------------------------------------------------------------------------
// Test the linked list of Drugs.
static class Program
{
    static void Main( )
    {
        DrugList drugs = new DrugList( "RXQT1402.txt" );
        drugs.InsertSort( CompareByName );
        foreach( Drug d in drugs.ToArray( TestByName ) ) Console.WriteLine( d );
    }
        
    static int CompareByName( Drug lhs, Drug rhs )
        { return lhs.Name.CompareTo( rhs.Name ); }
        
    static bool TestByName( Drug d ) 
        { return d.Name.Contains( "MEL" ); }
}