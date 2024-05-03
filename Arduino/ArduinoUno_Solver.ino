#define stepFlip 2
#define dirFlip 3

#define stepRotate 4
#define dirRotate 5

#define stepCover 6
#define dirCover 7

#define antiClockWise false
#define clockWise true

#define finalDelay 100

void setup()
{
  // put your setup code here, to run once:
  pinMode(stepFlip, OUTPUT);
  pinMode(dirFlip, OUTPUT);
  pinMode(stepRotate, OUTPUT);
  pinMode(dirRotate, OUTPUT);
  pinMode(stepCover, OUTPUT);
  pinMode(dirCover, OUTPUT);

  digitalWrite(stepCover, LOW);
  digitalWrite(dirCover, LOW);
  digitalWrite(stepFlip, LOW);
  digitalWrite(dirFlip, LOW);
  digitalWrite(stepRotate, LOW);
  digitalWrite(dirRotate, LOW);
  Serial.begin(19200);
}

void loop()
{
  if (Serial.available() > 0)
  {
    unsigned long startTime;
    String moves = Serial.readStringUntil('\n');
    Serial.println(moves);
    for (int i = 0; i < moves.length(); i++)
    {
      char move = moves.charAt(i);
      switch (move)
      {
      case 'D':
        moveMotor(antiClockWise, 1.8, 85, dirCover, stepCover, 20000, finalDelay);
        moveMotor(clockWise, 0.234375, 118, dirRotate, stepRotate, 1000, finalDelay);
        moveMotor(antiClockWise, 0.234375, 28, dirRotate, stepRotate, 1000, finalDelay);
        moveMotor(clockWise, 1.8, 85, dirCover, stepCover, 20000, finalDelay);
        // Serial.print("elasped time: ");
        //   Serial.println(millis()-startTime);
        break;
      case 'd':

        moveMotor(antiClockWise, 1.8, 85, dirCover, stepCover, 20000, finalDelay);
        moveMotor(antiClockWise, 0.234375, 110, dirRotate, stepRotate, 1000, finalDelay);
        moveMotor(clockWise, 0.234375, 20, dirRotate, stepRotate, 1000, finalDelay);
        moveMotor(clockWise, 1.8, 85, dirCover, stepCover, 20000, finalDelay);
        break;
      case 'f':
        moveMotor(antiClockWise, 1.8, 360, dirFlip, stepFlip, 4500, finalDelay + 200);
        break;
      case 'r':

        moveMotor(antiClockWise, 0.234375, 90, dirRotate, stepRotate, 1000, finalDelay + 200);

        break;
      case 'R':
        moveMotor(clockWise, 0.234375, 90, dirRotate, stepRotate, 1000, finalDelay + 200);
        break;
      case 's':

        moveMotor(antiClockWise, 1.8, 85, dirCover, stepCover, 20000, finalDelay);
        moveMotor(antiClockWise, 0.234375, 200, dirRotate, stepRotate, 1000, finalDelay);
        moveMotor(clockWise, 0.234375, 20, dirRotate, stepRotate, 1000, finalDelay);
        moveMotor(clockWise, 1.8, 85, dirCover, stepCover, 20000, finalDelay);

        break;
        // remaining for debugging
      case 'q':
        moveMotor(antiClockWise, 1.8, 55, dirCover, stepCover, 20000, finalDelay);
        break;
      case 'Q':
        moveMotor(clockWise, 1.8, 55, dirCover, stepCover, 20000, finalDelay);
        break;

      case 't':
        moveMotor(antiClockWise, 1.8, 1.8, dirCover, stepCover, 20000, finalDelay);
        break;
      case 'T':
        moveMotor(clockWise, 1.8, 1.8, dirCover, stepCover, 20000, finalDelay);
        break;
      case 'w':
        moveMotor(antiClockWise, 0.234375, 5, dirRotate, stepRotate, 1000, finalDelay);
        break;

      case 'W':
        moveMotor(clockWise, 0.234375, 5, dirRotate, stepRotate, 1000, finalDelay);
        break;

      case 'e':
        moveMotor(antiClockWise, 0.234375, 1, dirRotate, stepRotate, 1000, finalDelay);
        break;
      case 'E':
        moveMotor(clockWise, 0.234375, 1, dirRotate, stepRotate, 1000, finalDelay);
        break;
      default:
        break;
      case '1':
        moveMotor(clockWise, 3.75, 103, dirRotate, stepRotate, 1000, finalDelay);
        moveMotor(antiClockWise, 3.75, 13, dirRotate, stepRotate, 1000, finalDelay);
        break;
      case '2':
        moveMotor(antiClockWise, 3.75, 115, dirRotate, stepRotate, 1000, finalDelay);
        moveMotor(clockWise, 3.75, 25, dirRotate, stepRotate, 1000, finalDelay);
        break;
      case '3':
        moveMotor(clockWise, 3.75, 195, dirRotate, stepRotate, 1000, finalDelay);
        moveMotor(antiClockWise, 3.75, 15, dirRotate, stepRotate, 1000, finalDelay);
      case '4':
        moveMotor(antiClockWise, 1.8, 90, dirCover, stepCover, 20000, finalDelay);
        break;
      case '5':
        moveMotor(antiClockWise, 1.8, 360, dirCover, stepCover, 20000, finalDelay);
      case '6':
        moveMotor(antiClockWise, 1.8, 5, dirFlip, stepFlip, 5000, finalDelay + 200);
      }
    }
  }
}

void moveMotor(bool dir, float stepSize, float angle, int motorDir, int motorStep, int delayLoop, int delayFinish)
{
  float stepFloat = (1 / stepSize) * (float)angle;
  int step = (int)stepFloat;
  if (dir)
  {
    digitalWrite(motorDir, HIGH);
  }
  else
  {
    digitalWrite(motorDir, LOW);
  }
  for (int x = 0; x < step; x++)
  {
    digitalWrite(motorStep, HIGH);
    delayMicroseconds(delayLoop);
    digitalWrite(motorStep, LOW);
    delayMicroseconds(delayLoop);
  }
  delay(delayFinish);
}
