using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;

public class Reversi : Form
{
    Button Help;
    Label Spelbord;
    Label player1;
    Label player2;
    Label wie_aanzet;
    Label wie_gewonnen;
    MenuStrip menuStrip;
    ToolStripDropDownItem menu;
    int diameter_steen = 45;
    int afstand_lijnen = 50;
    int Grootte = 6;
    Gameloop gameloop;
    Steen[,] map_stenen;
    int wissel_speler;
    bool spel_geinitialiseerd = false;
    bool hint_aan = false;
    int speler1_stenen = 2;
    int speler2_stenen = 2;
    Button nieuw_spel_knop;

    public Reversi(){   
        ClientSize = new Size(800,800);

        menuStrip = new MenuStrip();
        menu = new ToolStripDropDownButton("Nieuw spel: kies grootte");
        menu.DropDownItems.Add("Nieuw 4x4", null, (object o, EventArgs ea)=>{nieuw_spel(4);});
        menu.DropDownItems.Add("Nieuw 6x6", null, (object o, EventArgs ea)=>{nieuw_spel(6);});
        menu.DropDownItems.Add("Nieuw 8x8", null, (object o, EventArgs ea)=>{nieuw_spel(8);});
        menu.DropDownItems.Add("Nieuw 10x10", null, (object o, EventArgs ea)=>{nieuw_spel(10);});
        menuStrip.Items.Add(menu);


        Spelbord = new Label();
        Spelbord.Size = new Size(600, 600);
        Spelbord.Location = new Point (60, 60);
    

        player1 = new Label();
        player2 = new Label();
        player1.Size = new Size(250, 20);
        player2.Size = new Size(250, 20);
        player1.Text = $"Speler 1 stenen: {speler1_stenen}";
        player2.Text = $"Speler 2 stenen: {speler2_stenen}";
        player1.ForeColor = Color.Red;
        player2.ForeColor = Color.Blue;
        player1.Font = new Font("Arial", 14, FontStyle.Bold);
        player2.Font = new Font("Arial", 14, FontStyle.Bold);
        player1.Location =  new Point(100, 50);
        player2.Location =  new Point(100, 75);
        player1.Font = new Font("Arial", 14, FontStyle.Bold);
        player2.Font = new Font("Arial", 14, FontStyle.Bold);

        Help = new Button();
        Help.Text = "Help";
        Help.Size = new Size(100, 30);
        Help.Location = new Point(320, 75);
        Help.Cursor = Cursors.Help;
        Help.Click += help_functie_aanuitzetten;   

        nieuw_spel_knop = new Button();
        nieuw_spel_knop.Text = "Nieuw spel";
        nieuw_spel_knop.Size = new Size(100, 30);
        nieuw_spel_knop.Location = new Point(320, 45);
        nieuw_spel_knop.Cursor = Cursors.Hand;
        nieuw_spel_knop.Click += knop_nieuw_spel;
     

        wie_aanzet = new Label();
        wie_aanzet.Size = new Size(40, 65);
        wie_aanzet.Location = new Point(55, 40);
       
        wie_gewonnen = new Label();
        wie_gewonnen.Size = new Size(300, 30);
        wie_gewonnen.Location = new Point(430, 63);
        wie_gewonnen.Font = new Font("Arial", 16, FontStyle.Bold);
        wie_aanzet.Paint += teken_wie_aanzet;

        menuStrip.Items.Add(menu);
        Controls.Add(nieuw_spel_knop);
        Controls.Add(wie_gewonnen);
        Controls.Add(wie_aanzet);
        Controls.Add(Help);
        Controls.Add(player1);
        Controls.Add(player2);
        Controls.Add(menuStrip);
        
        Controls.Add(Spelbord);    
        
        Spelbord.Paint += Initialiseer_bord;
        map_stenen = new Steen[Grootte, Grootte];     

        wissel_speler = 2;             
    }

    void knop_nieuw_spel(object o, EventArgs e){
        nieuw_spel(Grootte);
    }

    //Maakt een nieuw spel aan en reset het bord naar de juiste groote
    private void nieuw_spel(int grootte){
        Spelbord.Controls.Clear();
        spel_geinitialiseerd = false;
        Grootte = grootte;
        wissel_speler = 2;
        update_wie_aanzet();
        speler1_stenen = 2;
        speler2_stenen = 2;
        wie_gewonnen.Text = "";
        player1.Text = $"Speler 1 stenen: {speler1_stenen}";
        player2.Text = $"Speler 2 stenen: {speler2_stenen}";
        map_stenen = new Steen[Grootte, Grootte];
        Spelbord.Invalidate();
    }

    //Zet de hint stenen aan of uit
    public void help_functie_aanuitzetten(object o, EventArgs ea){
        hint_aan = !hint_aan; //Zet aan/uit
        if (hint_aan){
            foreach (Steen steen in map_stenen){
                if (steen.vertel_aanklikbaar()){
                    steen.geef_state(3);
                    steen.Invalidate(); //Update de staat direct
                }
            }
        }else{
            zet_hint_stenen_uit();
        }
    }

    //Tekent spel bord en maakt gameloop object    
    public void Initialiseer_bord(object o, PaintEventArgs pea){
        
        // Zorgt ervoor dat de stenen niet worden overschreven als het bord opnieuw wordt getekend.
        if (!spel_geinitialiseerd){
            Maak_en_plaats_begin_stenen();
            spel_geinitialiseerd = true; //Initaliseerd de stenen maar 1 keer
            gameloop = new Gameloop(this);
            //Zet direct de valide zetten al neer;
            zet_aangegeven_stenen_op_aanklikbaar(gameloop.valide_zetten().Item1, wissel_speler);
        }
        TekenspelBord(o, pea);
    }

    //Plaats de stenen aan het begin van het spel
    public void Maak_en_plaats_begin_stenen(){
        //Houd bij waar de rode en blauwe steen moeten staan
        int[] speler_plek_begin = [2,1,1,2]; int w = 0;
        map_stenen = new Steen[Grootte, Grootte];

        for(int t = 0; t < Grootte; t++){
           for (int i = 0; i < Grootte; i++){
                map_stenen[i, t] = new Steen( new Point((i+1)*afstand_lijnen+1, (t+1)*afstand_lijnen+1),
                                              new Size(afstand_lijnen-1, afstand_lijnen-1),
                                              diameter_steen, (i, t), this);
                map_stenen[i,t].Cursor = Cursors.Hand; //Krijg hand cursor als je over een vakje gaat

                // Plaats de begin stenen met de juiste kleur
                if ((i+1 == Grootte/2 || i+1 == Grootte/2+1) && (t+1 == Grootte/2 || t+1 == Grootte / 2 + 1)){  
                    map_stenen[i,t].geef_state(speler_plek_begin[w]);
                    map_stenen[i,t].Invalidate();
                    w++;
                }
                Spelbord.Controls.Add(map_stenen[i,t]);
            }
        }
    }

    // Tekent het spelbord met lijnen en vakken
    public void TekenspelBord(object o, PaintEventArgs pea){   
        Graphics gr = pea.Graphics;
        Pen lijnen = new Pen(Color.Black, 1);
        for(int t = 1; t<= Grootte+1; t++){
            gr.DrawLine(lijnen, t*afstand_lijnen, afstand_lijnen, t*afstand_lijnen, (Grootte+1)*afstand_lijnen);
            gr. DrawLine(lijnen, afstand_lijnen, t*afstand_lijnen, (Grootte+1)*afstand_lijnen, t*afstand_lijnen);   
        }
    }

    public void zet_alle_stenen_op_niet_aanklikbaar(){
        foreach (Steen steen in map_stenen){
            steen.set_aanklikbaar(false);
        }
    }

    //Zet het bolletje bij de juiste speler die aan zet i
    public void teken_wie_aanzet(object o, PaintEventArgs pea){
        Graphics gr = pea.Graphics;
        gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        if (wissel_speler == 1){
            gr.FillEllipse(Brushes.Red, 18, 11, 20, 20);
        } else if (wissel_speler == 2){
            gr.FillEllipse(Brushes.Blue, 18, 36, 20, 20);
        }
    }

    //Vraag de status van alle stenen op en return een 2D array met de status van alle stenen
    public int[,] alle_stenen_states(){
        int[,] temp = new int[Grootte, Grootte];
        for(int t = 0; t < Grootte; t++){
            for (int i = 0; i < Grootte; i++){
                temp[i,t] = map_stenen[i,t].geef_state_terug();
            }
        }
        return temp;
    }

    //Zet de stenen die in de lijst staan op aanklikbaar en zet de hint aan als de hint aan staat
    public void zet_aangegeven_stenen_op_aanklikbaar(List<(int,int)> Z, int speler){
        foreach((int,int) z in Z){   
            if (hint_aan)
                map_stenen[z.Item1, z.Item2].stel_in_op_valide_hint(speler);
            map_stenen[z.Item1, z.Item2].set_aanklikbaar(true);
        } 
    }

    //Zet de hint stenen uit (hint = 3)
    public void zet_hint_stenen_uit(){
        foreach (Steen steen in map_stenen){
            if (steen.geef_state_terug() == 3){
                steen.geef_state(0); steen.Invalidate();
            }
        }
    }
    
    //Telt de stenen van beide spelers en zet dit in de labels
    public void tel_stenen(){   
        int steen_speler1 = 0; int steen_speler2 = 0;
        foreach (Steen steen in map_stenen){
            switch (steen.geef_state_terug()){
                case 1:
                    steen_speler1++;
                    break;
                case 2:
                    steen_speler2++;
                    break;    
            }
        }
        player1.Text = $"Speler 1 stenen: {steen_speler1}"; player2.Text = $"Speler 2 stenen: {steen_speler2}";
        speler1_stenen = steen_speler1; speler2_stenen = steen_speler2;
    }


    //Bepaalt wie de winnaar is en zet dit in de label wie_gewonnen, tel eerst de stenen
    public void bepaal_winnaar()
    {
        tel_stenen();
        if (speler1_stenen > speler2_stenen){
            wie_gewonnen.ForeColor = Color.Red;
            wie_gewonnen.Text = "Speler 1 heeft gewonnen!";
        } else if (speler2_stenen > speler1_stenen){
            wie_gewonnen.ForeColor = Color.Blue;
            wie_gewonnen.Text = "Speler 2 heeft gewonnen!";
        } else{
            wie_gewonnen.ForeColor = Color.Black;
            wie_gewonnen.Text = "Remise!";
        }
    }

    //Flip de stenen die in de dictionary staan bij de locatie van de aangeklikte steen (key)
    public void flip_stenen(Dictionary<string, List<(int,int)>> alle_combinaties, (int,int) locatie_steen){ 

        //Maak de key aan om deze in dictianary op te zoeken, de key is de locatie van de aangeklikte steen  
        string key = $"{locatie_steen.Item1}-{locatie_steen.Item2}"; 

        if (alle_combinaties.TryGetValue(key, out List<(int, int)> stenen_die_veranderen)){
            foreach (var steen in stenen_die_veranderen){
                map_stenen[steen.Item1, steen.Item2].geef_state(wissel_speler);
                map_stenen[steen.Item1, steen.Item2].Invalidate();
            }
        }
    }

    //Steen roept deze functie aan als hij is aangeklikt, deze functie roept dan de gameloop object aan.
    public void ontvangen((int, int) a, Steen s, MouseEventArgs _){
        gameloop.speler_aanzet(a, s);
    }

    public int geef_grootte_bord(){
        return Grootte;
    }

    public void update_wie_aanzet(){
        wie_aanzet.Invalidate();
    }

    public void wie_is_er_aan_zet(int speler){
        wissel_speler = speler;
    }
}
