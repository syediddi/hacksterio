                       

#define RELAY1  7     
#define RELAY2  5
String state = "OFF";
void setup()

{    


Serial.begin(9600);
  pinMode(RELAY1, OUTPUT);       

}

void loop()
{   
   //Check if serial data is availble
    if(Serial.available() > 0){
       state = Serial.readString();
       Serial.println(state);
    }
    
    //If text is ON1. Turn on the relay1
    if(state == "ON1\r\n")
      digitalWrite(RELAY1,1); 
    //If text is ON2. Turn on the relay2
    else if(state == "ON2\r\n")
       digitalWrite(RELAY2,1); 
    //If text is OFF1. Turn off the relay1
    else if(state == "OFF1\r\n")
      digitalWrite(RELAY1,0);
    //If text is OFF2. Turn off the relay2
    else if(state == "OFF2\r\n")
      digitalWrite(RELAY2,0);
    //Do Nothing  
    else{
      digitalWrite(RELAY1,0);
      digitalWrite(RELAY2,0);
    }

}
