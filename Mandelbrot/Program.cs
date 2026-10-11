using System;
using System.Drawing;
using System.Windows.Forms;

public class Program
{
    static void Main(string[] str)
{
Application.Run(new Reversi());

}
}
public class Reversi : Form
{
    Button Nieuwspel;
    Button Help;
    Label Spelbord;
    Label Wie_aanzet;
    ComboBox Bordgrootte;


    Label player1;

    Label player2;
    
    MenuStrip menuStrip;

    ToolStripDropDownItem menu;

    int diameter_steen = 45;

    int afstand_lijnen = 50;
    int Grootte = 8; 

    Gameloop gameloop;
    Steen[,] map_stenen;

    int wissel_speler;

    bool spel_geinitialiseerd = false;
    (List<(int,int)>, Dictionary<string, List<(int,int)>>) informatie;

    public Reversi()
    {   
        ClientSize = new Size(800,600);
     

        menuStrip = new MenuStrip();
        menu = new ToolStripDropDownButton("Guide");
        menu.DropDownItems.Add("Close", null);
        menuStrip.Items.Add(menu);

        menu = new ToolStripDropDownButton("New Game");
        menu.DropDownItems.Add("New 4x4", null);
        menu.DropDownItems.Add("New 6x6", null);
        menu.DropDownItems.Add("New 8x8", null);
        menuStrip.Items.Add(menu);

        menu = new ToolStripDropDownButton("Help!");
        menuStrip.Items.Add(menu);


        Spelbord = new Label();
        Spelbord.Size = new Size(500, 500);
        Spelbord.Location = new Point (60, 60);

        player1 = new Label();
        player2 = new Label();

        player1.Text = $"Player 1 Stenen";
        player2.Text = $"Player 2 Stenen:";
        player1.ForeColor = Color.Red;
        player2.ForeColor = Color.Blue;

        menuStrip.Items.Add(menu);


        player1.Location =  new Point(100, 25);
        player2.Location =  new Point(100, 50);

        Controls.Add(player1);
        Controls.Add(player2);
        Controls.Add(menuStrip);

        gameloop = new Gameloop(this);
        Spelbord.Paint += Initialiseer_bord;

        this.Controls.Add(Spelbord);    


        map_stenen = new Steen[Grootte, Grootte];     

        wissel_speler = 2;
             
    }

    public void Initialiseer_bord(object o, PaintEventArgs pea)
    {
        // Zorgt ervoor dat de stenen niet worden overschreven als het bord opnieuw wordt getekend.
        if (!spel_geinitialiseerd)
        {
            
            Maak_en_plaats_begin_stenen();
            spel_geinitialiseerd = true;
            informatie = gameloop.valide_zetten();
            zet_aangegeven_stenen_op_aanklikbaar(informatie.Item1, wissel_speler);
        }
       
        TekenspelBord(o, pea);
    }
    public void Maak_en_plaats_begin_stenen(){
        int[] speler_plek_begin = [2,1,1,2];
        int w = 0;
        map_stenen = new Steen[Grootte, Grootte];
        for(int t = 0; t < Grootte; t++){
           for (int i = 0; i < Grootte; i++){
                map_stenen[i, t] = new Steen(
                                              new Point((i+1)*afstand_lijnen+1, (t+1)*afstand_lijnen+1),
                                              new Size(afstand_lijnen-1, afstand_lijnen-1),
                                              diameter_steen,
                                              (i, t), 
                                              this);
                if ((i+1 == Grootte/2 || i+1 == Grootte/2+1)  && (t+1 == Grootte/2 || t+1 == Grootte / 2 + 1))
                {  
                    //map_stenen[i,t].set_aanklikbaar(false);
                    map_stenen[i,t].geef_state(speler_plek_begin[w]);
                    map_stenen[i,t].Invalidate();
                    w++;
                }

                //map_stenen[i,t].set_aanklikbaar(false);
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

    public int[,] alle_stenen_states()
    {
        int[,] temp = new int[Grootte, Grootte];
        for(int t = 0; t < Grootte; t++){
            for (int i = 0; i < Grootte; i++)
            {
                temp[i,t] = map_stenen[i,t].geef_state_terug();
            }
        }

        return temp;
    }

    public void zet_aangegeven_stenen_op_aanklikbaar(List<(int,int)> Z, int speler)
    {
        foreach((int,int) z in Z)
        {
            map_stenen[z.Item1, z.Item2].stel_in_op_valide_hint(speler);
            map_stenen[z.Item1, z.Item2].set_aanklikbaar(true);
            Console.WriteLine($"zz is: {map_stenen[z.Item1, z.Item2].vertel_aanklikbaar()}");
            
        } 
    }

    public void zet_hint_stenen_uit()
    {
        foreach (Steen steen in map_stenen)
        {
            if (steen.geef_state_terug() == 3)
            {
                steen.geef_state(0);
                steen.Invalidate();
            }
        }
    }

    public void flip_stenen(Dictionary<string, List<(int,int)>> alle_combinaties, (int,int) locatie_steen)
    {   
        
        string key = $"{locatie_steen.Item1}-{locatie_steen.Item2}";

        if (alle_combinaties.TryGetValue(key, out List<(int, int)> stenen_die_veranderen))
        {
            Console.WriteLine($"Key: {key}, aantal stenen: {stenen_die_veranderen.Count}");

            foreach (var steen in stenen_die_veranderen)
            {
                map_stenen[steen.Item1, steen.Item2].geef_state(wissel_speler);
                map_stenen[steen.Item1, steen.Item2].Invalidate();
            }
        }
        else
        {
            Console.WriteLine($"Key bestaat niet: {key}");
        }
    }

    public void ontvangen((int, int) a, int st, bool aan, Steen s, MouseEventArgs mea){
        
        zet_hint_stenen_uit();
        Console.WriteLine($"\e[31ma: {a}\e[0m st {st} aan {aan}");

        Console.WriteLine($"Speler nu aanzet: {wissel_speler}");

        if (mea.Button == MouseButtons.Left)
            s.geef_state(wissel_speler);
        else if (mea.Button == MouseButtons.Right)
            s.geef_state(wissel_speler);
        flip_stenen(informatie.Item2, a);
        Console.WriteLine($"Item 2: {informatie.Item2.Count}");    
        

        zet_alle_stenen_op_niet_aanklikbaar();
        

        wissel_speler = gameloop.wissel_speler();
        //Bereken valide zetten

        informatie = gameloop.valide_zetten();
        
  
        zet_aangegeven_stenen_op_aanklikbaar(informatie.Item1, wissel_speler);
    }

    public int geef_grootte_bord()
    {
        return Grootte;
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

    public int wissel()
    {
        if (speler == 1){
            return 2;
        } else { 
            return 1;
        }
    }




    public (List<(int,int)>, Dictionary<string, List<(int,int)>>) valide_zetten()
    {   
        int[,] status_map = reversi.alle_stenen_states();
        int grootte = reversi.geef_grootte_bord();
       List<(int, int)> valide_plekken = new List<(int, int)>();
       List<(int, int)> tijdelijke_plekken = new List<(int, int)>();
       (int, int) mogelijke_plek_als_begin_met_lege_vak = (0,0);

        Dictionary<string, List<(int,int)>> dict_alle = new Dictionary<string, List<(int,int)>>();

        bool flag_mogelijk_valide_locatie =  false;
        int reden_valide = 0; //1 als op steen van speler staat (die aan de beurt is)
                             //2 als op lege plek staan
        bool flag_steen_ertussen = false; 
        bool wissel_voor_lege_plek = false;
        bool flag_komt_van_lege_plek = false;
        bool opgeslagen = false;
        bool anti_skip = false;
 
        int vorige_steen = 0;

        void reset_flags(){
           flag_mogelijk_valide_locatie = false;
           flag_steen_ertussen = false;
           flag_komt_van_lege_plek = false;
           anti_skip = false;
        }

        void voeg_stenen_toe_die_veranderen_bij_validezet(string key)
        {
            // if (!dict_alle.ContainsKey(key)){
            //     dict_alle.Add(key, tijdelijke_plekken);
            //     Console.WriteLine($"Tijdelijke plekken {tijdelijke_plekken[0]} XXX");
            //     dict_alle.TryGetValue(key, out List<(int,int)> tussen_plekken);
            //     Console.WriteLine($"\e[42m{key}Tijdelijke plekken {tussen_plekken.Count} XXX]\e[0m");
            // } else {
            //     dict_alle.TryGetValue(key, out List<(int,int)> tussen_plekken);
            //     tussen_plekken.AddRange(tijdelijke_plekken);
            //     Console.WriteLine($"\e42mTijdelijke plekken {tussen_plekken.Count} XXX]\e[0m");
            //     dict_alle.Remove(key, out List<(int,int)> oude_plekken);
            //     dict_alle.Add(key, tussen_plekken);
            //     Console.WriteLine($"Nieuwe plekken {dict_alle.Count} XXX");
            // }
            // Console.WriteLine($"Dict_alle---------------: {dict_alle.Count} XXX");
            if (!dict_alle.ContainsKey(key))
{
    dict_alle[key] = new List<(int, int)>(tijdelijke_plekken);
}
else
{
    dict_alle[key].AddRange(tijdelijke_plekken);
}
        }
    
        void algorithme(int j, int i){
            int status = status_map[j,i];

            if (vorige_steen == speler){
                flag_steen_ertussen = false;
                tijdelijke_plekken.Clear();
            }
                
            if (status == speler && flag_mogelijk_valide_locatie && flag_steen_ertussen && flag_komt_van_lege_plek){
                valide_plekken.Add(mogelijke_plek_als_begin_met_lege_vak);
                // sla lijst op tussen begin eind valide plek
                flag_mogelijk_valide_locatie = true;
                anti_skip = true;
                flag_steen_ertussen = false;
                flag_komt_van_lege_plek = false;
                //

                voeg_stenen_toe_die_veranderen_bij_validezet($"{mogelijke_plek_als_begin_met_lege_vak.Item1}-{mogelijke_plek_als_begin_met_lege_vak.Item2}");

                tijdelijke_plekken = [];
                    vorige_steen = speler;
            } else if (status == 0 && flag_mogelijk_valide_locatie && flag_steen_ertussen && !flag_komt_van_lege_plek){
                    // Als er een lege plek is gevonden, maar er is nog geen steen van de speler gevonden, dan is het niet valide.
                valide_plekken.Add((j,i));
                flag_mogelijk_valide_locatie = false;
                flag_steen_ertussen = false;
                flag_komt_van_lege_plek = false;

                voeg_stenen_toe_die_veranderen_bij_validezet($"{j}-{i}");

                tijdelijke_plekken = [];
                vorige_steen = speler;
            } 
                
            if (status == 0){   
                mogelijke_plek_als_begin_met_lege_vak = (j,i);
                flag_komt_van_lege_plek = true;
                flag_mogelijk_valide_locatie = false;
                tijdelijke_plekken.Clear();
            } else if (status == speler){   
                flag_komt_van_lege_plek = false;
                flag_mogelijk_valide_locatie = true;
                tijdelijke_plekken.Clear();
            } else if (status == wissel()&& (flag_mogelijk_valide_locatie||flag_komt_van_lege_plek)){
                flag_steen_ertussen = true;
                if (flag_komt_van_lege_plek)
                    flag_mogelijk_valide_locatie = true;
                tijdelijke_plekken.Add((j,i));    
            } else{
                    reset_flags();
                tijdelijke_plekken.Clear();    
            }
            vorige_steen = status;
        }
            
       
        Console.WriteLine($"\e[34m ------{speler} \e[0m");

        // Horizontaale controle
        for (int i = 0; i < grootte; i++){
            for (int j = 0; j < grootte; j++){
                algorithme(j,i);
            }
            reset_flags();
        }

       // Verticale controle
        for (int k = 0; k < grootte; k++){
            for (int v = 0; v < grootte; v++){
                algorithme(k,v);
            }
            reset_flags();
        }
        
        //////
        // Diagonale controle
        ////

        // Diagonale controle: linksboven naar rechtsonder 
        // Deel 1 | Bovenste helft van de diagonaal
        for (int i = 0; i < grootte; i++){   
            int tijdelijk = 0    ;
            for (int j = 0; j < grootte-i; j++){ 
                //valide_plekken.Add((j,i+tijdelijk));
                //Console.WriteLine($"| j: {j} i: {i+tijdelijk}");
                algorithme(j,i+tijdelijk);    
                tijdelijk++;
            }
            reset_flags();
        }

        // Diagonale controle: linksboven naar rechtsonder 
        // Deel 2 | Bovenste helft van de diagonaal
        for (int i = 0; i < grootte; i++){   
            int tijdelijk = 0    ;
            for (int j = 0; j < i+1; j++){ 
                //Console.WriteLine($"| j: {grootte-1-j} i: {i-tijdelijk}");
                //valide_plekken.Add((grootte-1-j,i-tijdelijk));
                algorithme(grootte-1-j,i-tijdelijk);    
                tijdelijk++;
            }
            reset_flags();
        }


        // Diagonale controle: rechtsonder naar linksboven
        // Deel 1 | Bovenste helft van de diagonaal
        for (int i = grootte-1; i !=-1; i--){   
            int tijdelijk = 0;        
            for (int j = 0; j < i+1; j++){ 
                //    Console.WriteLine($"| j: {i-tijdelijk} i: {j}");
                //valide_plekken.Add((i-tijdelijk,j));
            
                algorithme(i-tijdelijk,j);    
                tijdelijk++;
            }
            reset_flags();
        }

        // Diagonale controle: rechtsonder naar linksboven
        // Deel 2 | Bovenste helft van de diagonaal
        for (int i = 1; i < grootte; i++){  
            int tijdelijk = 0 ;   
            for (int j = grootte-1 ; j != i-1 ; j--){ 
                //Console.WriteLine($"| j: {i+tijdelijk} i: {j}");
                //valide_plekken.Add((i+tijdelijk,j));
                algorithme(i+tijdelijk,j);    
                tijdelijk++;
            }
            reset_flags();
        }

       Console.WriteLine($"Valide plekken\e[41m: {dict_alle.Count} XXX\e[0m"); 
       return (valide_plekken, dict_alle);
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
        this.MouseClick += Teken;
        this.Paint += Tekenb;
        steen_aanklikbaar = false;  
    } 



    public void Tekenb(object o, PaintEventArgs pea)
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
    // public int getStenen(int color)
    // {
    //     color = state;
    //     int stenen = 2; // default

    //     if (color == 1)
    //     {
    //          for(int i = 0; i > stenen; i++)
    //         {
    //             stenen = stenen + i;
    //         }
    //     }
    //     else if (color == 2)
    //     {
    //          for(int i = 0; i > stenen; i++)
    //         {
    //                 stenen = stenen + i;
    //         }
    //     } else
    //     {
    //         return 0;
    //     }
    //     return stenen;
    // }
    public void Teken(object o, MouseEventArgs mea)
    {
        if (steen_aanklikbaar){
            roep_game_aan(mea);
            Console.WriteLine($"Steen aangeklikt: {plek} met state: {state}");
            this.Invalidate();
            
        }
    }
    
    public void roep_game_aan(MouseEventArgs mea)
    {
        
        bord_waar_steen_staat.ontvangen(plek, state, steen_aanklikbaar, this, mea);
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
    
    public bool vertel_aanklikbaar()
    {
        return steen_aanklikbaar;
    }

    public void stel_in_op_valide_hint(int speler)
    {
        state = 3;
        steen_aanklikbaar = true;
        Invalidate();
       
  
        
    }
}

