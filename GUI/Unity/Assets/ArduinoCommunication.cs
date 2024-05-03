using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Numerics;
using System;

public class ArduinoCommunication : MonoBehaviour
{
    // Start is called before the first frame update
    public static string arduinoMoveString = "";
    public static string setUpMoveString = "";
    SerialPort serialPort;
    
    void Start()
    {
        try
        {
            serialPort = new SerialPort("COM5",19200);
            serialPort.Open();
            print("Started");
        }
        catch (Exception ex)
        {
            Debug.LogError("Error opening serial port: " + ex.Message);
        }
    }

    // Update is called once per frame
    void Update()
    {
        /
        if (arduinoMoveString.Length > 0 && serialPort.IsOpen && serialPort != null)
        {
            
            if (arduinoMoveString[0] == 'z')
            {
                arduinoMoveString = arduinoMoveString.Substring(1);
                serialPort.WriteLine(arduinoMoveString);
                arduinoMoveString = "";
            }
            else
            {

                if(arduinoMoveString == "Q" || arduinoMoveString == "q" || arduinoMoveString =="w" || arduinoMoveString == "W" || arduinoMoveString == "e" || arduinoMoveString == "E" || arduinoMoveString == "t" || arduinoMoveString == "T")
                {
                    serialPort.WriteLine(arduinoMoveString);
                    arduinoMoveString = "";
                }
                else if(arduinoMoveString == "y")
                {
                    serialPort.WriteLine("f");
                    arduinoMoveString = "";
                }
                else if (arduinoMoveString == "o")
                {
                    serialPort.WriteLine("r");
                    arduinoMoveString = "";
                }
                else if (arduinoMoveString == "p")
                {
                    serialPort.WriteLine("R");
                    arduinoMoveString = "";
                }
                else
                {
                    print(RephraseToBottomRotation(arduinoMoveString));
                    serialPort.WriteLine(RephraseToBottomRotation(arduinoMoveString));
                    arduinoMoveString = "";

                }
            }
         
        }
        if (serialPort.IsOpen && serialPort.BytesToRead > 0)
        {
            string data = serialPort.ReadLine();
            Debug.Log("Data from Arduino: " + data);
        }

    }

    string RephraseToBottomRotation(string moveString)
    {
        List<string> moves = new List<string>(moveString.Split(new string[] { " " }, System.StringSplitOptions.RemoveEmptyEntries));

        for (int i = 0; i < moves.Count; i++)
        {
            if (moves[i][0] == 'D') continue;
            else if (moves[i][0] == 'R')
            {
                moves[i] = "fD" + moves[i].Substring(1);
                for (int j = i + 1; j < moves.Count; j++)
                {
                    switch (moves[j][0])
                    {
                        case 'R':
                            moves[j] = 'D' + moves[j].Substring(1);
                            break;
                        case 'L':
                            moves[j] = 'U' + moves[j].Substring(1);
                            break;
                        case 'U':
                            moves[j] = 'R' + moves[j].Substring(1);
                            break;
                        case 'D':
                            moves[j] = 'L' + moves[j].Substring(1);
                            break;
                        case 'F':
                        case 'B':
                            // Do nothing
                            break;
                    }
                }
            }
            else if (moves[i][0] == 'L')
            {
                moves[i] = "RRfD" + moves[i].Substring(1);
                for (int j = i + 1; j < moves.Count; j++)
                {
                    switch (moves[j][0])
                    {
                        case 'R':
                            moves[j] = 'U' + moves[j].Substring(1);
                            break;
                        case 'L':
                            moves[j] = 'D' + moves[j].Substring(1);
                            break;
                        case 'U':
                            moves[j] = 'R' + moves[j].Substring(1);
                            break;
                        case 'D':
                            moves[j] = 'L' + moves[j].Substring(1);
                            break;
                        case 'F':
                            moves[j] = 'B' + moves[j].Substring(1);
                            break;
                        case 'B':
                            moves[j] = 'F' + moves[j].Substring(1);
                            // Do nothing
                            break;
                    }
                }
            }
            else if (moves[i][0] == 'U')
            {
                moves[i] = "ffD" + moves[i].Substring(1);
                for (int j = i + 1; j < moves.Count; j++)
                {
                    switch (moves[j][0])
                    {
                        case 'R':
                            moves[j] = 'L' + moves[j].Substring(1);
                            break;
                        case 'L':
                            moves[j] = 'R' + moves[j].Substring(1);
                            break;
                        case 'U':
                            moves[j] = 'D' + moves[j].Substring(1);
                            break;
                        case 'D':
                            moves[j] = 'U' + moves[j].Substring(1);
                            break;
                        case 'F':
                        case 'B':
                            // Do nothing
                            break;
                    }
                }
            }
            else if (moves[i][0] == 'F')
            {
                moves[i] = "RfD" + moves[i].Substring(1);
                for (int j = i + 1; j < moves.Count; j++)
                {
                    switch (moves[j][0])
                    {
                        case 'R':
                            moves[j] = 'B' + moves[j].Substring(1);
                            break;
                        case 'L':
                            moves[j] = 'F' + moves[j].Substring(1);
                            break;
                        case 'U':
                            moves[j] = 'R' + moves[j].Substring(1);
                            break;
                        case 'D':
                            moves[j] = 'L' + moves[j].Substring(1);
                            break;
                        case 'F':
                            moves[j] = 'D' + moves[j].Substring(1);
                            break;
                        case 'B':
                            moves[j] = 'U' + moves[j].Substring(1);
                            break;
                    }
                }
            }
            else if (moves[i][0] == 'B')
            {
                moves[i] = "rfD" + moves[i].Substring(1);
                for (int j = i + 1; j < moves.Count; j++)
                {
                    switch (moves[j][0])
                    {
                        case 'R':
                            moves[j] = 'F' + moves[j].Substring(1);
                            break;
                        case 'L':
                            moves[j] = 'B' + moves[j].Substring(1);
                            break;
                        case 'U':
                            moves[j] = 'R' + moves[j].Substring(1);
                            break;
                        case 'D':
                            moves[j] = 'L' + moves[j].Substring(1);
                            break;
                        case 'F':
                            moves[j] = 'U' + moves[j].Substring(1);
                            break;
                        case 'B':
                            moves[j] = 'D' + moves[j].Substring(1);
                            break;
                    }
                }
            }
        }
        string arduinoMove = string.Join("   ", moves);
        arduinoMove = arduinoMove.Replace("D'", "d");
        arduinoMove = arduinoMove.Replace("D2", "s");
        print("Arduino Move:"+arduinoMove);
        return arduinoMove;
        
    }

    private void OnApplicationQuit()
    {
        if(serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }
}
