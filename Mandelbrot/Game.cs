using System;
using System.Diagnostics.Metrics;


public class Game
{
    Reversi board = new Reversi();
    int circles;

    private bool playerTurn;


    public Game()
    {
       
    }

    public void loop()
    {
        
    }

    private bool isPlayerTurn()
    {
        bool red = true;
        bool blue = false;

        if (playerTurn == red)
        {
            
        } 
        else if (playerTurn == blue)
        {
            
        } 
    

        return playerTurn;
    }


    private void determineWinner()
    {
        
        
    }

}