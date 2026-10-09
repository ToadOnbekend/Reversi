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

    int diameter_steen = 45;

    int afstand_lijnen = 50;

    int Grootte = 8; 
    
    Steen[,] map_stenen;
    public Reversi()
    {   
        ClientSize = new Size(800,600);
        Spelbord = new Label();
        Spelbord.Size = new Size(500, 500);
        Spelbord.Location = new Point (000, 000);

        Spelbord.Paint += TekenspelBord;
        // Spelbord.MouseClick += (object o, MouseEventArgs mea) =>
        // {
        //     Console.WriteLine("In veld geklikt");
            
            
        // };
            
        this.Controls.Add(Spelbord);    
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
                                              new Point((i+1)*afstand_lijnen+1, (t+1)*afstand_lijnen+1),
                                              new Size(afstand_lijnen-1, afstand_lijnen-1),
                                              diameter_steen,
                                              (i, t), this);
                Spelbord.Controls.Add(map_stenen[i,t]);
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
    }

    public void ontvangen((int, int) a){
        Console.WriteLine($"\e[31ma: {a}\e[0m");
    }


}

public class Steen: Label
{
    int state = 0; //Lege staat = 0
    int diameter;
    int midden;
    bool steen_aanklikbaar;

    (int, int) plek;

    Reversi bord_waar_steen_staat;

    public Steen(Point pos, Size Sized, int diameter_steen, (int, int) nummer, Reversi r)
    {
        this.Location = pos;
        this.Size = Sized;
        diameter = diameter_steen;
  
        midden = (this.Width - diameter)/2;
        this.plek = nummer;
        bord_waar_steen_staat = r;
        Console.WriteLine(this.Location.X + "Location" + this.Location.Y);
        this.MouseClick += Teken;
        this.MouseClick += vertel_positie;
        this.Paint += Teken;
        steen_aanklikbaar = true;
        
    }



    public void Teken(object o, PaintEventArgs pea)
    {   
        Graphics gr = pea.Graphics;
        Console.WriteLine(this.Location + ": Location steen van: " + plek.ToString());
        if (state == 1)
        {
           Brush b = new SolidBrush(Color.Red);
           gr.FillEllipse(b,midden, midden, diameter, diameter);
        
        } else if (state == 2)
        {
           Brush b = new SolidBrush(Color.Blue);
           gr.FillEllipse(b,midden, midden, diameter, diameter);
        } else if (state == 3)
        {
            gr.DrawEllipse(Pens.Black,midden, midden, diameter, diameter);
        }

    }
    public void Teken(object o, MouseEventArgs mea)
    {
        if (steen_aanklikbaar)
            state = 3;
            this.Invalidate();
    }
    
    public void vertel_positie(object o, MouseEventArgs mea)
    {
        bord_waar_steen_staat.ontvangen(plek);
    }

    public void set_aanklikbaar(bool steen_aanklikbaar)
    {
        this.steen_aanklikbaar = steen_aanklikbaar;
    }

    public int krijg_state()
    {
        return state;
    }


}

