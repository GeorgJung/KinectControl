void setup()   
{                
  pinMode(13, OUTPUT); 
  Serial.begin(9600);  
}

void loop()                     
{
  
if(Serial.available())
{
  int c = Serial.read();
  if (c == '1')
  {    
    digitalWrite(13,HIGH);
  }
  else if (c == '0')
  {
    digitalWrite(13,LOW);
  }
} 
}
