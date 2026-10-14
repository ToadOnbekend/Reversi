using System;
using System.Drawing;
using System.Windows.Forms;

public class Program{
    static void Main(string[] str){
    Application.EnableVisualStyles(); // Zorgt voor moderne Windows 10 ui elementen
    Application.Run(new Reversi());
    }
}

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

//De gameloop class is verantwoordelijk voor het bepalen van de valide zetten en het wisselen van spelers
public class Gameloop
{
    (List<(int,int)>, Dictionary<string, List<(int,int)>>) informatie;
    /* Voorbeeld van hoe deze  Dictionary<string, List<(int,int)>> eruit ziet:
    dict = {
        "1-2": [(1,3), (1,4)],
        "2-3": [(3,3), (4,4), (5,5)],
        ...
    }
    $"{x}-{y}" is de key (locatie van aangeklikte valide steen)
    */ 
    int speler = 2;     
    Reversi reversi;
    int[,] status_map;
    public Gameloop(Reversi r){
        reversi = r;
        informatie = valide_zetten();
    }

    //Wisse speler en geef de speler terug die nu aan de beurt is
    public int wissel_speler(){   
        Console.WriteLine("Gewisseld");
        switch(speler){
            case 1:
                speler = 2;
                return 2;
            case 2:
                speler = 1;
                return 1;
            default:
                return 1;    
        }
    }

    public int wissel(){
        if (speler == 1){
            return 2;
        } else { 
            return 1;
        }
    }

    //Bepaalt alle valide zetten van de huidige speler en welke stenen er bij welke valide zet moeten worden omgedraaid
    public (List<(int,int)>, Dictionary<string, List<(int,int)>>) valide_zetten(){   
        status_map = reversi.alle_stenen_states();
        int grootte = reversi.geef_grootte_bord();
 
        List<(int, int)> valide_plekken = new List<(int, int)>();
        List<(int, int)> tijdelijke_plekken = new List<(int, int)>();
        (int, int) mogelijke_plek_als_begin_met_lege_vak = (0,0); //Sla het vak op dat leeg is maar mogelijk valide is
   
        Dictionary<string, List<(int,int)>> dict_alle = new Dictionary<string, List<(int,int)>>();

        //Hulp variable om te bepalen of een steen valide is 
        bool flag_mogelijk_valide_locatie =  false;
        bool flag_steen_ertussen = false; 
        bool flag_komt_van_lege_plek = false;
        int vorige_steen = 0;

        void reset_flags(){
           flag_mogelijk_valide_locatie = false;
           flag_steen_ertussen = false;
           flag_komt_van_lege_plek = false;
        }

        //Functie die een steen toevoegd aan de dictionary
        void voeg_stenen_toe_die_veranderen_bij_validezet(string key){
            if (!dict_alle.ContainsKey(key)){
                //Nieuwe valide locatie, voeg toe aan dictionary
                dict_alle[key] = new List<(int, int)>(tijdelijke_plekken);
            } else {
                //Bestaande valide locatie, voeg de stenen toe aan de bestaande lijst
                dict_alle[key].AddRange(tijdelijke_plekken);
            }
        }
    
        //Hoofd functie om valide zetten te vinden en stenen die moeten worden omgedraaid bij een valide zet
        //Deze functie past de flags aan en voegt de stenen toe aan de dictionary als er een valide zet is gevonden
        //Deze functie werkt als een state machine en moet door de for-loops in de juiste volgorde worden aangeroepen
        void vindt_valide_zetten(int j, int i){
            int status = status_map[j,i];

            // Als de stenen van de huidige speler direct naast elkaar liggen, dan is het geen valide zet
            if (vorige_steen == speler){
                flag_steen_ertussen = false;
                tijdelijke_plekken.Clear();
            }
                
            // Als de huidige steen van de speler is, er een lege plek is geweest en er stenen van de tegenstander ertussen liggen, dan is het een valide zet    
            if (status == speler && flag_mogelijk_valide_locatie && flag_steen_ertussen && flag_komt_van_lege_plek){
                valide_plekken.Add(mogelijke_plek_als_begin_met_lege_vak);
                flag_mogelijk_valide_locatie = true;flag_steen_ertussen = false;flag_komt_van_lege_plek = false;
                // De steen die volgt kan een steen van de tegenstander zijn dus flag_mogelijke_valide_locatie = true;
                voeg_stenen_toe_die_veranderen_bij_validezet($"{mogelijke_plek_als_begin_met_lege_vak.Item1}-{mogelijke_plek_als_begin_met_lege_vak.Item2}");
                tijdelijke_plekken.Clear();
                vorige_steen = speler;

            // Als de huidige steen leeg is, er een lege plek is geweest en er stenen van de tegenstander ertussen liggen, dan is het een valide zet
            } else if (status == 0 && flag_mogelijk_valide_locatie && flag_steen_ertussen && !flag_komt_van_lege_plek){
                valide_plekken.Add((j,i));
                reset_flags();
                voeg_stenen_toe_die_veranderen_bij_validezet($"{j}-{i}");
                tijdelijke_plekken.Clear();
                vorige_steen = speler;
            } 

            //Als de huidige steen leeg is, sla deze plek op als mogelijke beginplek voor een valide zet    
            if (status == 0){   
                mogelijke_plek_als_begin_met_lege_vak = (j,i);
                flag_komt_van_lege_plek = true;
                flag_mogelijk_valide_locatie = false;
                tijdelijke_plekken.Clear();
            //Als de huidige steen van de speler is, dan is het een mogelijke valide locatie en kan er een steen van de tegenstander volgen    
            } else if (status == speler){   
                flag_komt_van_lege_plek = false;
                flag_mogelijk_valide_locatie = true;
                tijdelijke_plekken.Clear();
            } else if (status == wissel() && (flag_mogelijk_valide_locatie||flag_komt_van_lege_plek)){
                flag_steen_ertussen = true;
                if (flag_komt_van_lege_plek)
                    //Als van een lege locatie komen dan is het alleen maar een mogelijke valide locatie als er een steen van de tegenstander tussen zit
                    //Het kan zo zijn dat er meerdere lege plekken achter elkaar liggen
                    flag_mogelijk_valide_locatie = true;
                tijdelijke_plekken.Add((j,i)); //Voeg de plekken bij die worden beinvloed als er een valide zet is.   
            } else{
                reset_flags();
                tijdelijke_plekken.Clear();    
            }
            vorige_steen = status;
        }
    
        //////
        // Voer de controle uit op alle mogelijke richtingen: horizontaal, verticaal en diagonaal
        //// 

        // Horizontaale controle
        for (int i = 0; i < grootte; i++){
            for (int j = 0; j < grootte; j++){
                vindt_valide_zetten(j,i);
            }
            reset_flags();
        }

       // Verticale controle
        for (int k = 0; k < grootte; k++){
            for (int v = 0; v < grootte; v++){
                vindt_valide_zetten(k,v);
            }
            reset_flags();
        }
        
        //////
        // Diagonale controle
        ////

        // Diagonale controle: linksboven naar rechtsonder 
        // Deel 1 | Bovenste helft
        for (int i = 0; i < grootte; i++){   
            int tijdelijk = 0    ;
            for (int j = 0; j < grootte-i; j++){ 
                vindt_valide_zetten(j,i+tijdelijk);    
                tijdelijk++;
            }
            reset_flags();
        }

        // Diagonale controle: linksboven naar rechtsonder 
        // Deel 2 | Onderste helft
        for (int i = 0; i < grootte; i++){   
            int tijdelijk = 0    ;
            for (int j = 0; j < i+1; j++){ 
                vindt_valide_zetten(grootte-1-j,i-tijdelijk);    
                tijdelijk++;
            }
            reset_flags();
        }

        // Diagonale controle: rechtsonder naar linksboven
        // Deel 1 | Bovenste helft
        for (int i = grootte-1; i !=-1; i--){   
            int tijdelijk = 0;        
            for (int j = 0; j < i+1; j++){ 
                vindt_valide_zetten(i-tijdelijk,j);    
                tijdelijk++;
            }
            reset_flags();
        }

        // Diagonale controle: rechtsonder naar linksboven
        // Deel 2 | Onderste helft
        for (int i = 1; i < grootte; i++){  
            int tijdelijk = 0 ;   
            for (int j = grootte-1 ; j != i-1 ; j--){ 
                vindt_valide_zetten(i+tijdelijk,j);    
                tijdelijk++;
            }
            reset_flags();
        }

       return (valide_plekken, dict_alle);
    }
    
    //Hulp functie om te controleren of er zetten zijn
    public bool check_of_speler_kan_zetten(List<(int,int)> valide_plekken){
         return valide_plekken.Count == 0;
    }

    //Stappen die worden genomen als een speler een steen heeft aangeklikt, deze functie word aangeroepen door de de reversie class
    public void speler_aanzet((int, int) a, Steen s)
    {
        reversi.zet_hint_stenen_uit();

        //Update de aangeklikte steen naar de juiste speler kleur
        s.geef_state(speler);

        reversi.flip_stenen(informatie.Item2, a);
        reversi.tel_stenen();

        //Zorgt ervoor dat er de stenen niet aanklikbaar zijn totdat de volgende valide stenen zijn bepaald
        reversi.zet_alle_stenen_op_niet_aanklikbaar();

        int wie = wissel_speler();

        //Update de speler die aan zet is in de reversi class
        reversi.wie_is_er_aan_zet(wie);
        informatie = valide_zetten();

        //Controleert of er nog zetten mogelijk zijn voor de huidige speler
        if (check_of_speler_kan_zetten(informatie.Item1)){

            //Controleert of de speler die na de huidige speler aanzet is ook nog valide zetten heeft
            //Er wordt ook direct van speler gewisseld
            wie = wissel_speler();
            reversi.wie_is_er_aan_zet(wie);
            informatie = valide_zetten();
            
            //Controleert of de speler die na de huidige speler aanzet is ook nog valide zetten heeft
            if(check_of_speler_kan_zetten(informatie.Item1)){
                //Geen valide zetten meer voor beide spelers, bepaal de winnaar
                reversi.bepaal_winnaar();
            }
        }
        
        //Laat de reversie class het bolletje bij de huidige speler aanzet tekenen
        reversi.update_wie_aanzet();

        //Zet de stenen die valide zijn op aanklikbaar
        reversi.zet_aangegeven_stenen_op_aanklikbaar(informatie.Item1, speler);
    }
}

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
