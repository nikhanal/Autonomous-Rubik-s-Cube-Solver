using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

public class PythonListener : MonoBehaviour
{
    Thread thread;
    Process process = new Process();
    public int connectionPort = 25001;
    TcpListener server;
    TcpClient client;
    bool running;
    public string data = "";
    public string scrambleStep = "";
    public string solveStep = "";
    private string pythonResponse = "";
    public static string pythonMessage = "";

    void Start()
    {
        // Start the server
        thread = new Thread(new ThreadStart(StartServer));
        thread.Start();

        Thread pythonThread = new Thread(new ThreadStart(ExecutePythonScript));
        pythonThread.Start();  
        
    }

    private void ExecutePythonScript()
    {
        string scriptPath = Application.dataPath + "\\Python\\YoloCubeDetection.py";
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = @"C:\Users\nimesh\anaconda3\python.exe",
            Arguments = scriptPath,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        Process process = new Process();
        process.StartInfo = startInfo;
        process.Start();
        process.WaitForExit();
    }
    void OnDestroy()
    {
        // Clean up resources when the MonoBehaviour is being destroyed
        running = false;

        // Close the client, server, and process
        SendQuitMessage();
        client?.Close();
        server?.Stop();
        process.Close();

        // Wait for the thread to finish
        thread?.Join();
    }

        
    void StartServer()
    {
        try
        {
            // Create the server
            server = new TcpListener(IPAddress.Any, connectionPort);
            server.Start();
            UnityEngine.Debug.Log("Server is listening on port " + connectionPort);

            // Accept client connection

            while (true)
            {
                connectClient();
            }
         
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError($"Error in StartServer: {ex.Message}");
        }
    }

    void connectClient()
    {
        client = server.AcceptTcpClient();
        UnityEngine.Debug.Log("Client connected!");

        // Start sending "Launch Camera" message
        //SendLaunchCameraMessage();

        // Start listening for data
        running = true;
        while (running)
        {
            running = Connection();
        }
    }
    

    void SendQuitMessage()
    {
        try
        {
            // Get the network stream and send the message
            NetworkStream nwStream = client.GetStream();
            string launchMessage = "Quit";
            byte[] launchBytes = Encoding.UTF8.GetBytes(launchMessage);
            nwStream.Write(launchBytes, 0, launchBytes.Length);
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError($"Error sending launch message: {ex.Message}");
        }
    }

    bool Connection()
    {
        try
        {
            //Read data from the network stream
            NetworkStream nwStream = client.GetStream();
            byte[] buffer = new byte[client.ReceiveBufferSize];
            int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);

            //Decode the bytes into a string
            string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            if (!string.IsNullOrEmpty(dataReceived))
            {
                data = dataReceived;
                UnityEngine.Debug.Log("Data received: " + data);
                if (data[0] == 'z')
                {
                    data = data.Substring(1);
                    scrambleStep = data;
                    data = "";
                    string response = "Scramble step received";
                    byte[] responseByte = Encoding.UTF8.GetBytes(response);
                    nwStream.Write(responseByte, 0, responseByte.Length);
                    SendScrambleSteps();
                    return true;
                }
                else if (data[0] == 'q')
                {
                    data = data.Substring(1);
                    ArduinoCommunication.arduinoMoveString = data;
                    data = "";
                    return true;
                }
                //Handle data according to your requirements
            }
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError($"Error in Connection: {ex.Message}");
            return false;
        }
        return true;
    }



    void Update()
    {
        if(pythonMessage.Length > 0)
        {
            try
            {
                NetworkStream nwStream = client.GetStream();
                byte[] messageBytes = Encoding.UTF8.GetBytes(pythonMessage);
                nwStream.Write(messageBytes, 0, messageBytes.Length);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"Error sending python message: {ex.Message}");
            }
            pythonMessage = "";
        }

    }


    public List<string> StringToList(string solution)
    {
        List<string> solutionList = new List<string>(solution.Split(new string[] { " " }, System.StringSplitOptions.RemoveEmptyEntries));
        return solutionList;
    }

    public void SendScrambleSteps()
    {
        print('1');
       
        if (scrambleStep != null && scrambleStep != "" && !CubeState.keyMove && !CubeState.autoRotateDrag && !CubeState.drag)
        {
            print(scrambleStep);
            keyboardControl.scrambleSteps = StringToList(scrambleStep);
        }
    }

    public void SendSolutionSteps()
    {
        print('2');

      
        if (solveStep != null && solveStep != "" && !CubeState.keyMove && !CubeState.autoRotateDrag && !CubeState.drag)
        {
            print(solveStep);
            keyboardControl.solveSteps = StringToList(solveStep);
            solveStep = "";
        }
    }

    public void launchPython(bool autoCamera = false)
    {
        try
        {
            // Get the network stream and send the message
            NetworkStream nwStream = client.GetStream();
            if (!autoCamera)
            {
                string message = "Launch Camera";
                byte[] launchBytes = Encoding.UTF8.GetBytes(message);
                nwStream.Write(launchBytes, 0, launchBytes.Length);
            }
            else
            {
                string message = "Launch Auto";
                byte[] launchBytes = Encoding.UTF8.GetBytes(message);
                nwStream.Write(launchBytes,0,launchBytes.Length);
                //perform moves to read cube face
            }
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError($"Error sending launch message: {ex.Message}");
        }
    }

 
   
    public void launchCameraAuto()
    {
        NetworkStream nwStream = client.GetStream();
        string message = "center";
        int bytesRead;
        byte[] buffer = new byte[4096];
        byte[] launchBytes = Encoding.UTF8.GetBytes(message);
        nwStream.Write(launchBytes, 0, launchBytes.Length);

        
        int flipCount = 0;
        string cubeCondition = "";
        
        while (true)
        {

            pythonResponse = "";
            bytesRead = nwStream.Read(buffer, 0, buffer.Length);
            pythonResponse = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            print("Received response: " + pythonResponse);
            Array.Clear(buffer, 0, buffer.Length);


            if (pythonResponse.Length > 0)
            {

                if (pythonResponse == "done")
                {
                    break;
                }
                else if (pythonResponse == "G")
                {
                    //move to next phase
                    ArduinoCommunication.arduinoMoveString = "zf";
                ;
                    message = "center";
                    launchBytes = Encoding.UTF8.GetBytes(message);
                    nwStream.Write(launchBytes, 0, launchBytes.Length);

                    bytesRead = nwStream.Read(buffer, 0, buffer.Length);
                    pythonResponse = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    print("Received response: " + pythonResponse);
                    Array.Clear(buffer, 0, buffer.Length);
                    cubeCondition += pythonResponse;

                    ArduinoCommunication.arduinoMoveString = "zf";
                 
                    ArduinoCommunication.arduinoMoveString = "zf";
                 
                    ArduinoCommunication.arduinoMoveString = "zf";
               
                    ArduinoCommunication.arduinoMoveString = "zr";
                  
                    ArduinoCommunication.arduinoMoveString = "zf";
              

                    message = "center";
                    launchBytes = Encoding.UTF8.GetBytes(message);
                    nwStream.Write(launchBytes, 0, launchBytes.Length);

                    bytesRead = nwStream.Read(buffer, 0, buffer.Length);
                    pythonResponse = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    print("Received response: " + pythonResponse);
                    Array.Clear(buffer, 0, buffer.Length);
                    cubeCondition += pythonResponse;

                    ArduinoCommunication.arduinoMoveString = "zf";
              
                    ArduinoCommunication.arduinoMoveString = "zf";
                
                    ArduinoCommunication.arduinoMoveString = "zf";
              
                    ArduinoCommunication.arduinoMoveString = "zR";
            

                    if (cubeCondition == "OW")
                    {
                        ArduinoCommunication.arduinoMoveString = "zrfs";
                    }
                    else if (cubeCondition == "WR")
                    {
                        ArduinoCommunication.arduinoMoveString = "zfs";
                    }
                    else if (cubeCondition == "RY")
                    {
                        ArduinoCommunication.arduinoMoveString = "zrfs";
                    }
                    else if (cubeCondition == "YO")
                    {
                        ArduinoCommunication.arduinoMoveString = "zfff";
                    }
              
                    break;

                }
                else
                {
                    //send data to arduino to flip
                    ArduinoCommunication.arduinoMoveString = "y";
                    
                    flipCount++;
                
                    if (flipCount == 4)
                    {
                        flipCount = 0;
                        ArduinoCommunication.arduinoMoveString = "zr";
                    }
                    else
                    {
                        message = "center";
                        launchBytes = Encoding.UTF8.GetBytes(message);
                        nwStream.Write(launchBytes, 0, launchBytes.Length);
                        
                    }
                    
                
                }
            }

        }
    }
}



