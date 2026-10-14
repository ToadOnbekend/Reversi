using System;
using System.Windows.Forms;
using System.Drawing;

  public class Steen: Label
{
    int state = 0; //Lege staat = 0 (wit), Speler rood = 1, Speler blauw = 2, Hint = 3
    int diameter;
    int midden;
    int straal;
    bool steen_aanklikbaar;
    (int, int) plek;
    Reversi bord_waar_steen_staat;

    public Steen(Point pos, Size Size, int diameter_steen, (int, int) nummer, Reversi r)
    {
        this.Location = pos;
        this.Size = Size;
        diameter = diameter_steen;
        straal = diameter/2;
        midden = (this.Width - diameter)/2;
        this.plek = nummer;

        //De steen moet de Reversie class kunnen aanroepen als er op de steen is geklikt
        bord_waar_steen_staat = r;

        this.MouseClick += Teken;
        this.Paint += Tekensteen;
        steen_aanklikbaar = false;  
    } 

    //De steen tekent zichzelf op het bord met d ejuiste staat
    public void Tekensteen(object o, PaintEventArgs pea)
    {   
        Graphics gr = pea.Graphics;

        //Manier om anti-aliasing te gebruiken zodat de cirkels er mooi uitzien
        gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        switch (state){
            case 1:
                gr.FillEllipse(Brushes.Red,midden, midden, diameter, diameter);  
                break;      
            case 2:
        
                gr.FillEllipse(Brushes.Blue,midden, midden, diameter, diameter);
                break;
            case 3:  
                gr.DrawEllipse(Pens.Black,midden+straal/2, midden+straal/2, straal, straal);
                break;
            }
    }

    public void Teken(object o, MouseEventArgs mea)
    {   
        if (steen_aanklikbaar){
            roep_game_aan(mea);
            Console.WriteLine($"Steen aangeklikt: {plek} met state: {state}");
            this.Invalidate();
            
        }
    }

    //Roept de reversi class aan geeft zich zelf mee en de plek van de steen die is aangeklikt
    public void roep_game_aan(MouseEventArgs mea){
        bord_waar_steen_staat.ontvangen(plek, this, mea);
    }

    public void set_aanklikbaar(bool steen_aanklikbaar_){
        this.steen_aanklikbaar = steen_aanklikbaar_;
    }

    public int geef_state_terug(){
        return state;
    }

    public void geef_state(int staat){
        this.state = staat;
    }
    
    public bool vertel_aanklikbaar(){
        return steen_aanklikbaar;
    }

    //Functie om de steen in de hint staat te zetten en direct te tekenen
    public void stel_in_op_valide_hint(int speler){
        state = 3;
        steen_aanklikbaar = true;
        Invalidate();      
    }
}


