using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class keyboardControl : MonoBehaviour
{

    private CubeState cubeState; //reading cube state
    private ReadCube readCube; // reading cube state
    private ArrowGenerator arrowGenerator; // generation of arrows
    private List<string> moveBuffer = new List<string>() { }; // store movelist from keyboard even when previous move is being animated
    public static List<string> scrambleSteps = new List<string>() { }; // store list of moves to scramble cube as physical cube obtained from python
    public static List<string> solveSteps = new List<string>() { };// store list of moves to solve from python script
    public static List<string> scrambleMoveList = new List<string>() { };// store list of moves to scramble cube when pressing scramble button
    public static List<string> kociembaSolveList = new List<string>() { };// store list of moves to solve cube obtained from c#
    private List<string> undoMoveList = new List<string>() { };// store the list of solving moves performed for undo operation
    private List<string> redoMoveList = new List<string>() { };//store the list of solving moves for redo operation
    public  Text solutionSteps;// display moves in screen
    public Text stepModeText;// display STEP MODE : ON or STEP MODE : OFF
    public Text displayText;// display DISPLAY: ON or DISPLAY : OFF

    public static int count = 1;
    public static bool stepMode = false;
    public static string cubeSolvingSteps = "";// hold string format of move list will be updated to solutionSteps.text later
    public static bool showSteps = true;
    public GameObject scrollArea;
    public Scrollbar scrollBar;
    public static bool firstSolve = false;

    private RotateBigCube rotateBigCube;// to rotate the cube from mouse
    public static string TotalMovesDone = "";
    private string solvedCubeString = "UUUUUUUUURRRRRRRRRFFFFFFFFFDDDDDDDDDLLLLLLLLLBBBBBBBBB"; 
    // Start is called before the first frame update
    void Start()
    {
        cubeState = FindObjectOfType<CubeState>();
        readCube = FindObjectOfType<ReadCube>();
        arrowGenerator = FindObjectOfType<ArrowGenerator>();
        rotateBigCube = FindObjectOfType<RotateBigCube>();
        scrollArea.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        //check if keys to rotate the face of cube is pressed and store in movelist buffer
        
        if (Input.GetKey(KeyCode.LeftShift))
            {
                if (Input.GetKeyDown(KeyCode.U))
                {
                    moveBuffer.Add("U'");

                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    moveBuffer.Add("D'");
                }
                else if (Input.GetKeyDown(KeyCode.F))
                {
                    moveBuffer.Add("F'");
                }
                else if (Input.GetKeyDown(KeyCode.B))
                {
                    moveBuffer.Add("B'");
                }
                else if (Input.GetKeyDown(KeyCode.L))
                {
                    moveBuffer.Add("L'");
                }
                else if (Input.GetKeyDown(KeyCode.R))
                {
                    moveBuffer.Add("R'");
                }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                ArduinoCommunication.arduinoMoveString = "Q";
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                ArduinoCommunication.arduinoMoveString = "W";
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                ArduinoCommunication.arduinoMoveString = "E";
            }
            else if (Input.GetKeyDown(KeyCode.T))
            {
                ArduinoCommunication.arduinoMoveString = "T";
            }

        }
            else
            {
                if (Input.GetKeyDown(KeyCode.U))
                {
                    moveBuffer.Add("U");
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    moveBuffer.Add("D");
                }
                else if (Input.GetKeyDown(KeyCode.F))
                {
                    moveBuffer.Add("F");
                }
                else if (Input.GetKeyDown(KeyCode.B))
                {
                    moveBuffer.Add("B");
                }
                else if (Input.GetKeyDown(KeyCode.L))
                {
                    moveBuffer.Add("L");
                }
                else if (Input.GetKeyDown(KeyCode.R))
                {
                    moveBuffer.Add("R");
                }
                else if (Input.GetKeyDown(KeyCode.Q))
                {
                    ArduinoCommunication.arduinoMoveString = "q";
                }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                ArduinoCommunication.arduinoMoveString = "w";
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                ArduinoCommunication.arduinoMoveString = "e";
            }
            else if (Input.GetKeyDown(KeyCode.T))
            {
                ArduinoCommunication.arduinoMoveString = "t";
            }
            else if (Input.GetKeyDown(KeyCode.Y))
            {
                ArduinoCommunication.arduinoMoveString = "y";
            }
            else if (Input.GetKeyDown(KeyCode.O))
            {
                ArduinoCommunication.arduinoMoveString = "o";
            }
            else if(Input.GetKeyDown(KeyCode.P))
            {
                ArduinoCommunication.arduinoMoveString = "p";
            }

        }

    
        //check if moveBuffer has pending move and if so perform that move
        if(CubeState.started && !CubeState.keyMove && !CubeState.autoRotateDrag && !CubeState.drag && moveBuffer.Count>0)
        {
            if(cubeState.GetStateString() != solvedCubeString)
            {
                TotalMovesDone += moveBuffer[0];
            }
            DoMove(moveBuffer[0]);
            moveBuffer.Remove(moveBuffer[0]);
        }

        //check if scrambleSteps from python has any remaining moves and if so perform move
        if (CubeState.started && !CubeState.keyMove && !CubeState.autoRotateDrag && !CubeState.drag && scrambleSteps.Count > 0)
        {
            DoMove(scrambleSteps[0]);
            scrambleSteps.Remove(scrambleSteps[0]);
        }

        //check if solve moves from pytho has pending move and if so perform move
        if (CubeState.started && !CubeState.keyMove && !CubeState.autoRotateDrag && !CubeState.drag && solveSteps.Count > 0)
        {
            if (cubeState.GetStateString() != solvedCubeString)
            {
                TotalMovesDone += solveSteps[0];
            }
            DoMove(solveSteps[0]);
            solveSteps.Remove(solveSteps[0]);
        }

        //check if scramble move from scramble button has any pending move and if so perform
        if (CubeState.started && !CubeState.keyMove && !CubeState.autoRotateDrag && !CubeState.drag && scrambleMoveList.Count > 0)
        {
            DoMove(scrambleMoveList[0]);           
            scrambleMoveList.Remove(scrambleMoveList[0]);
        }
  
        /*check if solve move list from c# has been generated but the first move has not been performed
          display only the arrow and solution step on screen without actually performing the move for the first time
         */
        if (CubeState.started && !CubeState.keyMove && !CubeState.autoRotateDrag && !CubeState.drag && kociembaSolveList.Count > 0 && firstSolve)
        {
            CubeState.autoCubeRotate = true;
            rotateBigCube.RotateCube("F");
            arrowGenerator.DeactivateArrow();
            count = 1;
            if (stepMode)
            {
                cubeSolvingSteps += "Use Arrow Keys to navigate. \n";
            }
            undoMoveList = new List<string> ();
            redoMoveList = new List<string>();
            firstSolve = false;
        }

        //check if solve list from c# has move pending and it is not in step mode then perform pending moves
        if (CubeState.started && !CubeState.autoCubeRotate && !CubeState.keyMove && !CubeState.autoRotateDrag && !CubeState.drag && kociembaSolveList.Count > 0 && redoMoveList.Count == 0 && !stepMode)
        {
            
         
            // first deactivate all previous arrows then activate the current move arrow
            arrowGenerator.DeactivateArrow();
            arrowGenerator.DisplayArrow(kociembaSolveList[0]);
            if (RephraseString(kociembaSolveList[0]) != "Dont")
            {
                cubeSolvingSteps += $"{count}. {RephraseString(kociembaSolveList[0])} \n";
                count++;
            }
            undoMoveList.Insert(0, ReverseStep(kociembaSolveList[0]));
            if (cubeState.GetStateString() != solvedCubeString)
            {
                TotalMovesDone += kociembaSolveList[0];
            }
            DoMove(kociembaSolveList[0]);          
         
            kociembaSolveList.Remove(kociembaSolveList[0]);         
        }

        //check if solve list from C# has moves remaining and in step mode then perform moves only if user press the right arrow key
        if (CubeState.started && !CubeState.keyMove && !CubeState.autoRotateDrag && !CubeState.drag && kociembaSolveList.Count > 0 && redoMoveList.Count==0 && Input.GetKeyDown(KeyCode.RightArrow) && stepMode)
        {

            arrowGenerator.DeactivateArrow();
            arrowGenerator.DisplayArrow(kociembaSolveList[0]);

            if (RephraseString(kociembaSolveList[0]) != "Dont")
            {
                cubeSolvingSteps += $"{count}. {RephraseString(kociembaSolveList[0])} \n";
                count++;
            }      
            // store performed move in previousMoveList to give user chance to revert cube state
            undoMoveList.Insert(0, ReverseStep(kociembaSolveList[0]));
            if (cubeState.GetStateString() != solvedCubeString)
            {
                TotalMovesDone += kociembaSolveList[0];
            }
            DoMove(kociembaSolveList[0]);
        
            kociembaSolveList.Remove(kociembaSolveList[0]);
            
        }

        /*check if in step mode and user press left arrow key and if the previous move list has moves remaining if so perform move
          if so perform move. this is done to allow user to revert to previous cube state on pressing left arrow key
         */
        if (CubeState.started && !CubeState.keyMove && !CubeState.autoRotateDrag && !CubeState.drag && undoMoveList.Count > 0 && Input.GetKeyDown(KeyCode.LeftArrow))
        {            
            count--;
            string removeString = "";      
            removeString = $"{count}. {RephraseString(ReverseStep(undoMoveList[0]))} \n";
            cubeSolvingSteps = cubeSolvingSteps.Replace(removeString, "");
            arrowGenerator.DeactivateArrow();
           
            if (cubeState.GetStateString() != solvedCubeString)
            {
                TotalMovesDone += undoMoveList[0];
            }
            DoMove(undoMoveList[0]);
            redoMoveList.Insert(0, ReverseStep(undoMoveList[0]));
       
            undoMoveList.Remove(undoMoveList[0]);
        }
        if (CubeState.started && !CubeState.keyMove && !CubeState.autoRotateDrag && !CubeState.drag && redoMoveList.Count > 0 && !stepMode)
        {

            arrowGenerator.DeactivateArrow();
            arrowGenerator.DisplayArrow(redoMoveList[0]);
            if (RephraseString(redoMoveList[0]) != "Dont")
            {
                cubeSolvingSteps += $"{count}. {RephraseString(redoMoveList[0])} \n";
                count++;
            }
            if (cubeState.GetStateString() != solvedCubeString)
            {
                TotalMovesDone += redoMoveList[0];
            }
            DoMove(redoMoveList[0]);
            undoMoveList.Insert(0, ReverseStep(redoMoveList[0]));
            redoMoveList.Remove(redoMoveList[0]);

        }
        if (CubeState.started && !CubeState.keyMove && !CubeState.autoRotateDrag && !CubeState.drag && redoMoveList.Count > 0 && Input.GetKeyDown(KeyCode.RightArrow) && stepMode)
        {

            arrowGenerator.DeactivateArrow();
            arrowGenerator.DisplayArrow(redoMoveList[0]);
            if (RephraseString(redoMoveList[0]) != "Dont")
            {
                cubeSolvingSteps += $"{count}. {RephraseString(redoMoveList[0])} \n";
                count++;
            }
            if (cubeState.GetStateString() != solvedCubeString)
            {
                TotalMovesDone += redoMoveList[0];
            }
            DoMove(redoMoveList[0]);
            undoMoveList.Insert(0, ReverseStep(redoMoveList[0]));
            redoMoveList.Remove(redoMoveList[0]);
            
        }

        


        // Display the text on screen only if DISPLAY: ON
        if (showSteps)
        {
            solutionSteps.text = cubeSolvingSteps;
            if (TotalMovesDone.Length > 0 && cubeState.GetStateString() == solvedCubeString)
            {
                cubeSolvingSteps += $"\nTotal Moves Done: {TotalMovesDone}\n";
                solutionSteps.text = cubeSolvingSteps;
                print(TotalMovesDone);
                TotalMovesDone = "";
                
            }
        }
        else
        {
            solutionSteps.text = "";
        }



        //Remove the arrow from screen after all the solve moves from c# has been performed
        if(CubeState.started && !CubeState.keyMove && !CubeState.autoRotateDrag && !CubeState.drag && kociembaSolveList.Count == 0  && redoMoveList.Count == 0)
        {
            arrowGenerator.DeactivateArrow();
        }
    }

    //Function to change the text of step mode button on screen    
    public void StepMode()
    {
        if (stepMode)
        {
            stepModeText.text = "Step Mode: OFF";
        }
        else
        {
            stepModeText.text = "Step Mode: ON";
        }
        stepMode = !stepMode;
    }

    public void DisplaySteps()
    {
        if(showSteps)
        {
            displayText.text = "Display: OFF";
           
            scrollArea.SetActive(false);
            arrowGenerator.DeactivateMainArrow();
            
        }
        else
        {
            displayText.text = "Display: ON";
           
            scrollArea.SetActive(true);
            arrowGenerator.ActivateMainArrow();
        }
        showSteps = !showSteps;
    }


    void DoMove(string move)
    {
        readCube.ReadState();
        CubeState.keyMove = true;
        if(move == "U")
        {
            
            RotateSide(cubeState.up, -90);
            
        }
        else if(move =="U'")
        {
            
            RotateSide(cubeState.up, 90);
           
        }
        else if (move == "D" )
        {
            
            RotateSide(cubeState.down, -90);
           
        }
        else if (move == "D'")
        {
           
            RotateSide(cubeState.down, +90);
            
        }
        else if (move == "F")
        {
           
            RotateSide(cubeState.front, -90);
            
        }
        else if (move == "F'")
        {
            
            RotateSide(cubeState.front, 90);
            
        }
        else if (move == "B")
        {

            RotateSide(cubeState.back, -90);
            
        }
        else if (move == "B'")
        {

            RotateSide(cubeState.back, 90);


        }
        else if (move == "L")
        {
           
            RotateSide(cubeState.left, -90);
           
        }
        else if (move == "L'")
        {
          
            RotateSide(cubeState.left, 90);
           
        }
        else if (move == "R")
        {
            
            RotateSide(cubeState.right, -90);

        }
        else if (move == "R'")
        {
           
            RotateSide(cubeState.right, 90);
           
        }
        else if(move == "U2")
        {
            RotateSide(cubeState.up, 180);
            
        }
        else if (move == "D2")
        {
            RotateSide(cubeState.down, 180);
        }
        else if (move == "F2")
        {
            RotateSide(cubeState.front, 180);
        }
        else if (move == "B2")
        {

            RotateSide(cubeState.back, 180);

        }
        else if (move == "L2")
        {
            RotateSide(cubeState.left, 180);
        }
        else if (move == "R2")
        {
            RotateSide(cubeState.right, 180);
        }
        else if (move == "TurnBack")
        {
           
            rotateBigCube.RotateCube("B");
            
        }
        else if(move == "TurnFront")
        {
            
            rotateBigCube.RotateCube("F");
        }
    }


    
    void RotateSide(List<GameObject> side, float angle)
    {
        PivotRotation pr = side[4].transform.parent.GetComponent<PivotRotation>();
        pr.keyRotate(side, angle);
    }

    //function to reverse the step i.e F --> F', L' --> L and so on
    string ReverseStep(string step)
    {
        if (step == "F")
        {
            return "F'";
        }
        else if (step == "B")
        {
            return "B'";
        }
        else if (step == "U")
        {
            return "U'";
        }
        else if (step == "D")
        {
            return "D'";
        }
        else if (step == "L")
        {
            return "L'";
        }
        else if (step == "R")
        {
            return "R'";
        }
        else if (step == "F'")
        {
            return "F";
        }
        else if (step == "B'")
        {
            return "B";
        }
        else if (step == "U'")
        {
            return "U";
        }
        else if (step == "D'")
        {
            return "D";
        }
        else if (step == "L'")
        {
            return "L";
        }
        else if (step == "R'")
        {
            return "R";
        }
        else if(step == "F2" || step =="B2" || step == "U2" || step == "D2" || step == "L2" || step == "R2")
        {
            return step;
        }
        else
        {
            return step;
        }
    }

    //function to give full move i.e F-->Rotate front face 90 degrees in clockwise direction and so on
    string RephraseString(string solution)
    {
        if (solution == "F")
        {
            return "Rotate Front Face 90 degrees in clockwise direction";
        }
        else if (solution == "B")
        {
            return "Rotate Back Face 90 degrees in clockwise direction";
        }
        else if (solution == "U")
        {
            return "Rotate Upper Face 90 degrees in clockwise direction";
        }
        else if (solution == "D")
        {
            return "Rotate Down Face 90 degrees in clockwise direction";
        }
        else if (solution == "L")
        {
            return "Rotate Left Face 90 degrees in clockwise direction";
        }
        else if (solution == "R")
        {
            return "Rotate Right Face 90 degrees in clockwise direction";
        }
        else if (solution == "F'")
        {
            return "Rotate Front Face 90 degrees in anti-clockwise direction";
        }
        else if (solution == "B'")
        {
            return "Rotate Back Face 90 degrees in anti-clockwise direction";
        }
        else if (solution == "U'")
        {
            return "Rotate Upper Face 90 degrees in anti-clockwise direction";
        }
        else if (solution == "D'")
        {
            return "Rotate Down Face 90 degrees in anti-clockwise direction";
        }
        else if (solution == "L'")
        {
            return "Rotate Left Face 90 degrees in anti-clockwise direction";
        }
        else if (solution == "R'")
        {
            return "Rotate Right Face 90 degrees in anti-clockwise direction";
        }
        else if (solution == "F2")
        {
            return "Rotate Front Face 180 degrees in clockwise direction";
        }
        else if (solution == "B2")
        {
            return "Rotate Back Face 180 degrees in clockwise direction";
        }
        else if (solution == "U2")
        {
            return "Rotate Upper Face 180 degrees in clockwise direction";
        }
        else if (solution == "D2")
        {
            return "Rotate Down Face 180 degrees in clockwise direction";
        }
        else if (solution == "L2")
        {
            return "Rotate Left Face 180 degrees in clockwise direction";
        }
        else if (solution == "R2")
        {
            return "Rotate Right Face 180 degrees in clockwise direction";
        }
        else
        {
            return "Dont";
        }
    }


}
