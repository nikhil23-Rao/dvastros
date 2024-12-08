using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.IO;

class Program {
    static void Main() {
        string[,] data = Data.GetData();
        //Console.WriteLine(data[0,1]);
        Antenna a = Antenna.Create(Globals.WPSA, Globals.WPSA_RANGE, data.GetLength(0), data);
        Antenna b = Antenna.Create(Globals.DS24, Globals.DS24_RANGE, data.GetLength(0), data);
        Antenna c = Antenna.Create(Globals.DS34, Globals.DS34_RANGE, data.GetLength(0), data);
        Antenna d = Antenna.Create(Globals.DS54, Globals.DS54_RANGE, data.GetLength(0), data);

        //Antenna.PrintFloat(a.linkBudget);
        for(int i=0;i<data.Length;i++) {
            if(Antenna.Priority(a, b, c, d, i)==null) {
                Console.Write(""+i+" ");
                Console.WriteLine("No anntenna available");
            } else {
                Console.Write(""+i+" ");
                Antenna.PrintPriority(Antenna.Priority(a, b, c, d, i));
                Console.WriteLine();
            }
            //Console.WriteLine(data[i, Globals.WPSA_RANGE]);
        }
        //Data.PrintData(data);
        
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
        
        
        if (File.Exists(filePath))
        {
            Console.WriteLine("File found. Proceeding...");
            // Continue with file operations
        }
        else
        {
            Console.WriteLine("File not found!");
        }

        string[] lines = File.ReadAllLines(filePath);

        int rows = lines.Length-1;
        int cols = lines[0].Split(',').Length;

        string[,] csvData = new string[rows, cols];

        for(int i = 1; i < lines.Length; i++) {
            string[] columns = lines[i].Split(','); // Split the row into columns
            for(int j = 0; j < cols; j++) {
                csvData[i-1, j] = columns[j]; // Assign to the 2D array
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
    public int dataIndex;
    public int dataRange;
    public int diameter;
    public bool[] inRange;
    public float[] linkBudget;

    public static Antenna Create(int index, int range, int dataSize, string[,] data) {
        Antenna a = new Antenna {
            dataIndex = index,
            dataRange = range,
            inRange = new bool[dataSize],
            linkBudget = new float[dataSize]
        };

        if (index == Globals.DS24 || index == Globals.DS34 || index == Globals.DS54)
            a.diameter = 34;
        else if (index == Globals.WPSA)
            a.diameter = 12;
        else
            a.diameter = -1;

        a.Range(data);       
        a.LinkBudget(data);  

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

    private void LinkBudget(string[,] data) {
        for(int i = 0; i < data.GetLength(0); i++){
            if (float.TryParse(data[i, dataRange], out float range)) {
                linkBudget[i] = (float)((Math.Pow(10, (10+9-19.43+10*Math.Log10(0.55*Math.Pow(Math.PI*diameter/0.136363636,2))-20*Math.Log10(4000*Math.PI*float.Parse(data[i,dataRange])/0.136363636)+228.6-10*Math.Log10(22))/10))/1000);

                //linkBudget[i] = (float)((Math.Pow(10, (10+9-19.43+10*Math.Log10(0.55*Math.Pow(Math.PI*diameter/0.136363636,2)-20*Math.Log10(4000*Math.PI*float.Parse(data[i,dataRange])/0.136363636)+228.6-10*Math.Log10(22)))/10))/1000);
            } else {
                linkBudget[i] = float.MinValue;
            }
        }

    }

    public static Antenna[] Priority(Antenna a, Antenna b, Antenna c, Antenna d, int index) {
        Antenna[] arr = new Antenna[4];
        arr[0] = a;
        arr[1] = b;
        arr[2] = c;
        arr [3] = d;

        Antenna storage;
        if(a.inRange[index] || b.inRange[index] || c.inRange[index] || d.inRange[index]) {
            for(int i=0; i<3; i++) {
                for(int j=0; j<3-i; j++) {
                    if(arr[j].linkBudget[index]<arr[j+1].linkBudget[index]) {
                        storage = arr[j];
                        arr[j] = arr[j+1];
                        arr[j+1] = storage;
                    }
                }
            }
        } else {
            return null;
        }

        return arr;
    }

    public static void PrintBool(bool[] arr) {
        for(int i=0; i<arr.Length; i++) {
            Console.WriteLine(""+i+" "+arr[i]);
        }
    }

    public static void PrintFloat(float[] arr) {
        for(int i=0; i<arr.Length; i++) {
            Console.WriteLine(""+i+" "+arr[i]);
        }
    }

    public static void PrintPriority(Antenna[] arr) {
        for(int i=0; i<arr.Length; i++) {
            Console.Write(""+arr[i].dataIndex+" ");
        }
    }
}