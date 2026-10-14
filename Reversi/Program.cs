using System;
using System.Drawing;
using System.Windows.Forms;

namespace ReversiGame {
public class Program
{ 
    static void Main(string[] str)
    {
        Application.EnableVisualStyles();
        Application.Run(new Reversi());

    }
}
}
