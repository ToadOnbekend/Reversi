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

    Gameloop gameloop;
    Steen[,] map_stenen;
    public Reversi()
    {   
        ClientSize = new Size(800,600);
        Spelbord = new Label();
        Spelbord.Size = new Size(500, 500);
        Spelbord.Location = new Point (000, 000);

        Spelbord.Paint += TekenspelBord;

        gameloop = new Gameloop(this);

        this.Controls.Add(Spelbord);    

        int[] speler_plek_begin = [2,1,1,2];
        int w = 0;
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
                                              (i, t), 
                                              this);
                if ((i+1 == Grootte/2 || i+1 == Grootte/2+1)  && (t+1 == Grootte/2 || t+1 == Grootte / 2 + 1))
                {  
                    map_stenen[i,t].set_aanklikbaar(false);
                    map_stenen[i,t].geef_state(speler_plek_begin[w]);
                    
                    map_stenen[i,t].Invalidate();
                    w++;
                }
                
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

    public void zet_alle_stenen_op_niet_aanklikbaar()
    {
        foreach (Steen steen in map_stenen)
        {
            steen.set_aanklikbaar(false);
        }
    }

    public void ontvangen((int, int) a, Steen s){
        Console.WriteLine($"\e[31ma: {a}\e[0m");
        int wissel_speler = gameloop.wissel_speler();
        Console.WriteLine($"Speler nu aanzet: {wissel_speler}");
        s.geef_state(wissel_speler);
        zet_alle_stenen_op_niet_aanklikbaar();
    }
}

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

public class Steen: Label
{
    int state = 0; //Lege staat = 0
    int diameter;
    int midden;
    int straal;
    (int, int) plek;

    bool steen_aanklikbaar;

    Reversi bord_waar_steen_staat;

    public Steen(Point pos, Size Sized, int diameter_steen, (int, int) nummer, Reversi r)
    {
        this.Location = pos;
        this.Size = Sized;
        diameter = diameter_steen;
        straal = diameter/2;
        midden = (this.Width - diameter)/2;
        this.plek = nummer;
        bord_waar_steen_staat = r;
        Console.WriteLine(this.Location.X + "Location" + this.Location.Y);
        this.MouseClick += Teken;
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
        {   // TODO: Zet straal/2 in constructor
            gr.DrawEllipse(Pens.Black,midden+straal/2, midden+straal/2, straal, straal);
        }

    }
    public void Teken(object o, MouseEventArgs mea)
    {
        if (steen_aanklikbaar){
            vertel_positie();
        
            this.Invalidate();
        }
    }
    
    public void vertel_positie()
    {
        bord_waar_steen_staat.ontvangen(plek, this);
    }

    public void set_aanklikbaar(bool steen_aanklikbaar_)
    {
        this.steen_aanklikbaar = steen_aanklikbaar_;
    }

    public int geef_state_terug()
    {
        return state;
    }

    public void geef_state(int staat)
    {
        this.state = staat;
    }
}

