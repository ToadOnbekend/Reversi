using System;
using System.Collections.Generic;
using System.Windows.Forms;

public class Gameloop
{
    // Using standard fields instead of class-level tuples
    public List<ValueTuple<int, int>> valide_plekken_lijst;
    public Dictionary<string, List<ValueTuple<int, int>>> dict_alle;
    
    int speler = 2;     
    Reversi reversi;
    int[,] status_map;

    // Helper flags moved to class scope
    bool flag_mogelijk_valide_locatie = false;
    bool flag_steen_ertussen = false; 
    bool flag_komt_van_lege_plek = false;
    int vorige_steen = 0;
    ValueTuple<int, int> mogelijke_plek_als_begin_met_lege_vak = (0, 0);
    List<ValueTuple<int, int>> tijdelijke_plekken = new List<ValueTuple<int, int>>();

    public Gameloop(Reversi r)
    {
        reversi = r;
        valide_plekken_lijst = new List<ValueTuple<int, int>>();
        dict_alle = new Dictionary<string, List<ValueTuple<int, int>>>();
        valide_zetten();
    }

    public int wissel_speler()
    {   
        Console.WriteLine("Gewisseld");
        switch(speler)
        {
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

    public int wissel()
    {
        if (speler == 1)
        {
            return 2;
        } 
        else 
        { 
            return 1;
        }
    }

    private void ResetFlags()
    {
       flag_mogelijk_valide_locatie = false;
       flag_steen_ertussen = false;
       flag_komt_van_lege_plek = false;
    }

    private void VoegStenenToe(string key)
    {
        if (!dict_alle.ContainsKey(key))
        {
            dict_alle[key] = new List<ValueTuple<int, int>>(tijdelijke_plekken);
        } 
        else 
        {
            dict_alle[key].AddRange(tijdelijke_plekken);
        }
    }

    private void VindtValideZetten(int j, int i)
    {
        int status = status_map[j, i];

        if (vorige_steen == speler)
        {
            flag_steen_ertussen = false;
            tijdelijke_plekken.Clear();
        }
            
        if (status == speler && flag_mogelijk_valide_locatie && flag_steen_ertussen && flag_komt_van_lege_plek)
        {
            valide_plekken_lijst.Add(mogelijke_plek_als_begin_met_lege_vak);
            flag_mogelijk_valide_locatie = true; 
            flag_steen_ertussen = false; 
            flag_komt_van_lege_plek = false;
            
            VoegStenenToe($"{mogelijke_plek_als_begin_met_lege_vak.Item1}-{mogelijke_plek_als_begin_met_lege_vak.Item2}");
            tijdelijke_plekken.Clear();
            vorige_steen = speler;
        } 
        else if (status == 0 && flag_mogelijk_valide_locatie && flag_steen_ertussen && !flag_komt_van_lege_plek)
        {
            valide_plekken_lijst.Add(new ValueTuple<int, int>(j, i));
            ResetFlags();
            VoegStenenToe($"{j}-{i}");
            tijdelijke_plekken.Clear();
            vorige_steen = speler;
        } 

        if (status == 0)
        {   
            mogelijke_plek_als_begin_met_lege_vak = new ValueTuple<int, int>(j, i);
            flag_komt_van_lege_plek = true;
            flag_mogelijk_valide_locatie = false;
            tijdelijke_plekken.Clear();
        } 
        else if (status == speler)
        {   
            flag_komt_van_lege_plek = false;
            flag_mogelijk_valide_locatie = true;
            tijdelijke_plekken.Clear();
        } 
        else if (status == wissel() && (flag_mogelijk_valide_locatie || flag_komt_van_lege_plek))
        {
            flag_steen_ertussen = true;
            if (flag_komt_van_lege_plek)
            {
                flag_mogelijk_valide_locatie = true;
            }
            tijdelijke_plekken.Add(new ValueTuple<int, int>(j, i));   
        } 
        else
        {
            ResetFlags();
            tijdelijke_plekken.Clear();    
        }
        vorige_steen = status;
    }

    public void valide_zetten()
    {   
        status_map = reversi.alle_stenen_states();
        int grootte = reversi.geef_grootte_bord();
 
        valide_plekken_lijst.Clear();
        tijdelijke_plekken.Clear();
        dict_alle.Clear();

        // Horizontale controle
        for (int i = 0; i < grootte; i++)
        {
            for (int j = 0; j < grootte; j++)
            {
                VindtValideZetten(j, i);
            }
            ResetFlags();
        }

        // Verticale controle
        for (int k = 0; k < grootte; k++)
        {
            for (int v = 0; v < grootte; v++)
            {
                VindtValideZetten(k, v);
            }
            ResetFlags();
        }
        
        // Diagonale controle: linksboven naar rechtsonder (Bovenste helft)
        for (int i = 0; i < grootte; i++)
        {   
            int tijdelijk = 0;
            for (int j = 0; j < grootte - i; j++)
            { 
                VindtValideZetten(j, i + tijdelijk);    
                tijdelijk++;
            }
            ResetFlags();
        }

        // Diagonale controle: linksboven naar rechtsonder (Onderste helft)
        for (int i = 0; i < grootte; i++)
        {   
            int tijdelijk = 0;
            for (int j = 0; j < i + 1; j++)
            { 
                VindtValideZetten(grootte - 1 - j, i - tijdelijk);    
                tijdelijk++;
            }
            ResetFlags();
        }

        // Diagonale controle: rechtsonder naar linksboven (Bovenste helft)
        for (int i = grootte - 1; i != -1; i--)
        {   
            int tijdelijk = 0;        
            for (int j = 0; j < i + 1; j++)
            { 
                VindtValideZetten(i - tijdelijk, j);    
                tijdelijk++;
            }
            ResetFlags();
        }

        // Diagonale controle: rechtsonder naar linksboven (Onderste helft)
        for (int i = 1; i < grootte; i++)
        {  
            int tijdelijk = 0;   
            for (int j = grootte - 1; j != i - 1; j--)
            { 
                VindtValideZetten(i + tijdelijk, j);    
                tijdelijk++;
            }
            ResetFlags();
        }
    }
    
    public bool check_of_speler_kan_zetten(List<ValueTuple<int, int>> valide_plekken)
    {
         return valide_plekken.Count == 0;
    }

    public void speler_aanzet(ValueTuple<int, int> a, Steen s)
    {
        reversi.zet_hint_stenen_uit();

        s.geef_state(speler);

        reversi.flip_stenen(dict_alle, a);
        reversi.tel_stenen();

        reversi.zet_alle_stenen_op_niet_aanklikbaar();

        int wie = wissel_speler();

        reversi.wie_is_er_aan_zet(wie);
        valide_zetten();

        if (check_of_speler_kan_zetten(valide_plekken_lijst))
        {
            wie = wissel_speler();
            reversi.wie_is_er_aan_zet(wie);
            valide_zetten();
            
            if (check_of_speler_kan_zetten(valide_plekken_lijst))
            {
                reversi.bepaal_winnaar();
            }
        }
        
        reversi.update_wie_aanzet();
        reversi.zet_aangegeven_stenen_op_aanklikbaar(valide_plekken_lijst, speler);
    }
}