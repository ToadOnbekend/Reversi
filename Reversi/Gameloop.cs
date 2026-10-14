using System;
using System.Collections.Generic;
using System.Windows.Forms;

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
