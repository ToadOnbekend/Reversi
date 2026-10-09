using System;
using System.Drawing;
using System.Windows.Forms;

Application.Run(new Reversi());
public class Reversi : Form
{
    Button Nieuwspel;
    Button Help;
    Label Spelbord;
    Label Wie_aanzet;
    ComboBox Bordgrootte;
    int[,] Stenen;

    int diameter_steen = 20;

    int afstand_lijnen = 50;

    int Grootte = 8; 
    
    Steen[,] map_stenen;
    public Reversi()
    {   
        ClientSize = new Size(800,600);
        Spelbord = new Label();
        Spelbord.Size = new Size(500, 500);
        Spelbord.Location = new Point (000, 000);
        this.Controls.Add(Spelbord);
        Spelbord.Paint += TekenspelBord;
        map_stenen = new Steen[Grootte, Grootte];
                for(int t = 0; t < Grootte; t++)
        {
           for (int i = 0; i < Grootte; i++)
            {
                //int ber = (t+1)*afstand_lijnen;
                if (i>=Grootte){
                   Console.WriteLine("Beak--");
                   break;}
                
                
                map_stenen[i, t] = new Steen(
                                              new Point((i+1)*afstand_lijnen, (t+1)*afstand_lijnen),
                                              new Size(afstand_lijnen, afstand_lijnen),
                                              diameter_steen
                                            , t+i);
                this.Controls.Add(map_stenen[i,t]);
            }}


    }

    public void TekenspelBord(object o, PaintEventArgs pea)
    {   
        Graphics gr = pea.Graphics;
        Pen lijnen = new Pen(Color.Black, 1);
        for(int t = 1; t<= Grootte+1; t++)
        {
            gr.DrawLine(lijnen, t*afstand_lijnen, afstand_lijnen, t*afstand_lijnen, (Grootte+1)*afstand_lijnen);
       
            gr. DrawLine(lijnen, afstand_lijnen, t*afstand_lijnen, (Grootte+1)*afstand_lijnen, t*afstand_lijnen);
            
        }
        
        for(int t = 0; t < Grootte; t++)
        {
           for (int i = 0; i < Grootte; i++)
            {
                //int ber = (t+1)*afstand_lijnen;
        
                if (i>=Grootte){
                   Console.WriteLine("Beak--");
                   break;}
                
                map_stenen[i,t].Teken(gr);
                gr.FillRectangle(Brushes.Red, (i+1)*afstand_lijnen, (t+1)*afstand_lijnen, 4,4);
              
            
                Console.WriteLine ($"i: {i}, t: {t} Grootte: {i<Grootte-1}");
            }
        }
    }


}

public class Steen: Label
{
    int state = 1; //Lege staat = 0
    int diameter;
    int midden;
    int straal;

    int nummer;

    public Steen(Point pos, Size Sized, int diameter_steen, int nummer)
    {
        this.Location = pos;
        this.Size = Sized;
        diameter = diameter_steen;
        midden = this.Size.Width / 2;
        straal = diameter_steen/2;
        this.nummer = nummer;
        Console.WriteLine(this.Location.X + "Location" + this.Location.Y);
        
    }

    public void Teken(Graphics gr)
    {   
        Console.WriteLine(this.Location + ": Location steen van: " + nummer.ToString());
        if (state == 1)
        {
     
           Brush b = new SolidBrush(Color.Red);
           Console.WriteLine("X-");
           gr.FillRectangle(b, this.Location.X, this.Location.Y, 10, 10);
           gr.FillEllipse(b,this.Location.X + midden, this.Location.Y +midden, straal*2, straal*2);
        
        } else if (state == 2)
        {
             Brush b = new SolidBrush(Color.Blue);
           gr.FillEllipse(b,midden-straal, midden - straal, straal*2, straal*2);
        }
    }
}