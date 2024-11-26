using System;
using System.ComponentModel.Design.Serialization;
using System.IO;

class Program {
    static void Main() {
        string[,] data = Data.GetData();
        Data.PrintData(data);
    }
}

static class Globals { //constants to access columns of data
    public const int MISSION_TIME = 0;
    public const int POS_X = 1;
    public const int POS_Y = 2;
    public const int POS_Z = 3; 
    public const int VEL_X = 4;
    public const int VEL_Y = 5;
    public const int VEL_Z = 6;
    public const int MASS = 7;
    public const int WPSA = 8;
    public const int WPSA_RANGE = 9;
    public const int DS54 = 10;
    public const int DS54_RANGE = 11;
    public const int DS24 = 12;
    public const int DS24_RANGE = 13;
    public const int DS34 = 14;
    public const int DS34_RANGE = 15;
}

class Data {
    public static string[,] GetData() {
        string filePath = "FY25 ADC HS Data Updated.csv";
        
        
// if (File.Exists(filePath))
// {
//     Console.WriteLine("File found. Proceeding...");
//     // Continue with file operations
// }
// else
// {
//     Console.WriteLine("File not found!");
// }

        string[] lines = File.ReadAllLines(filePath);

        int rows = lines.Length;
        int cols = lines[0].Split(',').Length;

        string[,] csvData = new string[rows, cols];

        for(int i = 0; i < rows; i++) {
            string[] columns = lines[i].Split(','); // Split the row into columns
            for(int j = 0; j < cols; j++) {
                csvData[i, j] = columns[j]; // Assign to the 2D array
            }
        }
        return csvData;
    }

    public static void PrintData(string[,] data) {
        for(int i = 0; i < data.GetLength(0); i++) {
            for(int j = 0; j < data.GetLength(1); j++) {
                Console.Write(data[i,j] + "\t");
            }
            Console.WriteLine();
        }
    }
}

class Antenna {
    int dataIndex;
    int dataRange;
    int diameter;
    bool[] inRange;
    float[] linkBudget;

    public Antenna Create(int index, int range, int dataSize) {
        Antenna a = new Antenna();
        a.dataIndex = index;
        a.dataRange = range;
        a.inRange = new bool[dataSize];
        a.linkBudget = new float[dataSize];

        if(index == Globals.DS24 || index == Globals.DS34 || index == Globals.DS54)
            a.diameter = 34;
        else if(index == Globals.WPSA)
            a.diameter = 12;
        else
            a.diameter = -1;

        return a;
    }

    private void Range(string[,] data) {
        for(int i = 0; i < data.GetLength(0); i++){
            if(data[i, dataIndex] == "0")
                inRange[i] = false;
            else
                inRange[i] = true;
        } 
    }

    private void LinkBudget(string[,] data, Antenna a) {
        for(int i = 0; i < data.GetLength(0); i++){
            linkBudget[i] = (float)((Math.Pow(10, 10+9-19.43+10*Math.Log10(0.55*Math.Pow(3.14*a.diameter/0.136363636,2)-20*Math.Log10(4000*3.14*float.Parse(data[i,a.dataRange])/0.136363636)+228.6-10*Math.Log10(22))))/1000);
        }

    }
}