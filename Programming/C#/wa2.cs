// Weekly Assignment 2: Calc the BMI for a person given weight in kg and height in cm.
//Behaviour as follows: title display, prompt to enter weight in kg, read input string and store as
//double, then prompt to enter height in cm, store and read string as double, calc the bmi
//using the weight in kg /divided by square of height in meters. 
//Output the bmi with two digits after decimal point.
//Assume: input is pos and real.

using System;
static class BmiCalculator
	{
		static void Main()
		{
			Console.WriteLine("This is a BMI Calculator.");
			
			Console.WriteLine("Please enter your weight in kg below; press 'ENTER' when done.");
			string w= Console.ReadLine();
			double weight= double.Parse(w);
			
			Console.WriteLine("Please enter your height in cm below; press 'ENTER' when done.");
			string h= Console.ReadLine();
			double height= double.Parse(h);
			
			double BMI= Math.Round(weight/(Math.Pow(height,2d)),2);
			Console.WriteLine("Your BMI is: "+ BMI);
		}
	}
			