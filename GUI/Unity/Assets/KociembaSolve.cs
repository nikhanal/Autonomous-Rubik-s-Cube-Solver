using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kociemba;
using UnityEngine.UI;

public class KociembaSolve : MonoBehaviour
{
    public ReadCube readCube;
    public CubeState cubeState;
    public PythonListener pythonListener;
    private bool doOnce = true;
    string solutionString = "";
    public static List<string> solutionList = new List<string>() { };
    public static bool showSteps = false;
    public List<string> allMoves = new List<string>() {
        "U","D","F","B","L","R",
        "U'","D'","F'","B'","L'","R'",
        "U2","D2","F2","B2","L2","R2"
    };
    public Scrollbar scrollbar;


    // Start is called before the first frame update
    void Start()
    {
        readCube = FindObjectOfType<ReadCube>();
        cubeState = FindObjectOfType<CubeState>();
        pythonListener = FindObjectOfType<PythonListener>();
    }

    // Update is called once per frame
    void Update()
    {
        if (CubeState.started && doOnce)
        {
            doOnce = false;
            SolveFromKociemba();
        }

    }

    public void SolveFromKociemba(bool SolveRealCube = false)
    {
        readCube.ReadState();
        //transform.rotation = Quaternion.Euler(-18, -56, 25);
        if(!CubeState.keyMove && !CubeState.autoRotateDrag && !CubeState.drag)
        {
           
            
            keyboardControl.cubeSolvingSteps = "";
            keyboardControl.count = 1;
            scrollbar.value = 1f;
            string moveString = cubeState.GetStateString();
            string info = "";
            solutionString = Search.solution(moveString, out  info);
            if(SolveRealCube)
            {
                ArduinoCommunication.arduinoMoveString = solutionString;
            }
            //string solution = SearchRunTime.solution(moveString, out info, buildTables: true);
            List<string> solutionList = pythonListener.StringToList(solutionString);
            keyboardControl.firstSolve = true;
          
            keyboardControl.kociembaSolveList = solutionList;
        }
    }


    public void Scramble(bool ScrambleRealCube = false)
    {
        if (!CubeState.keyMove)
        {
            
            keyboardControl.cubeSolvingSteps = "";
            keyboardControl.count = 1;
            scrollbar.value = 1f;
           
            List<string> moves = new List<string>();
            int shuffleLength = Random.Range(10, 20);
            for (int i = 0; i < shuffleLength; i++)
            {
                int randomMove = Random.Range(0, allMoves.Count);
                moves.Add(allMoves[randomMove]);
            }
           
            if (ScrambleRealCube)
            {
               
                readCube.ReadState();
                string moveString = cubeState.GetStateString();
                print(moveString);
                string solvedState = "UUUUUUUUURRRRRRRRRFFFFFFFFFDDDDDDDDDLLLLLLLLLBBBBBBBBB";
                if(moveString == solvedState)
                {
                    ArduinoCommunication.arduinoMoveString = string.Join(" ", moves);
                    
                }
                else
                {
                    PythonListener.pythonMessage = "G" + moveString;
                    return;
                }
            }
            keyboardControl.scrambleMoveList = moves;
          
            keyboardControl.TotalMovesDone = "";
        }
    }
}
