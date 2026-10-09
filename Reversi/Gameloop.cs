using System;
using System.Windows.Forms;


public class Gameloop
{
    int speler = 2;  
    Reversi reversi;

    public Gameloop(Reversi r)
    {
        reversi = r;
    }

    public int wissel_speler()
    {   
        Console.WriteLine("Gewisseld");
        if (speler == 1){
            speler =2;
            return 2;
        } else { 
            speler = 1;
            return 1;
        }
    }

    

}