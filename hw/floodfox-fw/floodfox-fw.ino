#include <SoftwareSerial.h>
#include <Bounce2.h>
#include "minishield.h"
#include "Ultrasonic.h"

SoftwareSerial sigfox(2, 3); // RX, TX

Bounce btn_a = Bounce(); 
Bounce btn_b = Bounce(); 

Ultrasonic dist(A5);

long distance;

float voltage_idle;
float voltage_tx;
float temperature;

bool temper = false;

byte payload[12];

void setup()
{
	minishield_setup();
	color(GREEN);

	btn_a.attach(BTN_A);
	btn_a.interval(10);

	btn_b.attach(BTN_B);
	btn_b.interval(10); 

	Serial.begin(9600);

	sigfox.begin(9600);
	sigfox.setTimeout(3000);

	Serial.println("Flood Fox v0.1");

	delay(1000);
	color(NONE);
	delay(1000);


	// echo OFF
	sigfox.print("ATE0\r\n");
}

void loop() {

	btn_a.update();
	btn_b.update();

//   if (sigfox.available()) {
//     Serial.write(sigfox.read());
//   }
//   if (Serial.available()) {
//     sigfox.write(Serial.read());
//   }

// return;
	if (btn_a.fell())
	{
		measure();
		update_sigfox();
		send();
	}

	if (btn_b.fell())
	{
		measure();
		update_sigfox();
		send();
	}

}

void alarm()
{
	color(RED);
	Serial.println("Alarm!");


	delay(200);

	color(NONE);
}

long measure()
{
	color(ORANGE);
	dist.distanceMeasure();
	distance = dist.microsecondsToCentimeters();
	color(NONE);

	Serial.print("Distance: ");
	Serial.println(distance);

	if (btnB() == LOW)
	{
		Serial.println("Temper OK");
		temper = false;
	}
	else
	{
		Serial.println("Temper ERR");
		temper = true;
	}
}


void send()
{
	/*
		Payload bytes:

		| 0 | 1 | 2 | 3  | 4  | 5 | 6 | 7 | 8 | 9 | 10 | 11 | 12 |
		| S | D     | Vi | Vt | T | 0 | 0 | 0 | 0 | 0  | 0  | 0  |

		S: 	status byte [8 bit]
	
			S[0] : always 1
			S[1] : temper switch (1 = open)  

		D:	distance [16 bit, little-endian]
		Vi:	voltage idle [8 bit]*
		Vt: voltage tx [8 bit]*
		T:	temperature of module [8bit]

		* Voltage is calculated as (byte)(X - 1)*10 where X is voltage in [V]
		
		Sigfox parser:
		temp::bool:1 d:1:uint:16:little-endian ui::uint:8 ut::uint:8 t::uint:8

	*/

	// status
	payload[0] = 0x01;
	payload[0] |= (temper && 0x01) << 1;

	// distance
	payload[1] = (byte)distance;
	payload[2] = (byte)(distance >> 8);

	// v
	payload[3] = voltage_2_byte(voltage_idle);
	payload[4] = voltage_2_byte(voltage_tx);

	// temperature
	payload[5] = (byte)temperature;


	send_payload();
}

byte voltage_2_byte(float f)
{
	if (f < 1.0) return 0;

	return (byte)((f-1)*100);
}

void send_payload()
{
	if (temper)
	{
		color(RED);
	}
	else
	{
		color(GREEN);
	}

	sigfox.print("AT$SS=");
	Serial.print("Payload: ");

	for(byte i = 0; i < 12; i++)
	{
		if (payload[i] < 0x10){
			Serial.print("0");
			sigfox.print("0");		
		}

		Serial.print(payload[i], HEX);
		sigfox.print(payload[i], HEX);

		Serial.print(" ");
		sigfox.print(" ");
	}

	Serial.println();
	sigfox.print("\r");

	Serial.println(rx());

	color(NONE);

	delay(100);
}



void update_sigfox()
{
	temperature = read_float("ATI26");
	voltage_idle = read_float("ATI27");
	voltage_tx = read_float("ATI28");
}

float read_float(const char* at)
{
	empty();

	Serial.print("Read: ");
	Serial.println(at);

	sigfox.print(at);
	sigfox.print("\r");

	float u = rx().toFloat();
	delay(100);

	Serial.println(u);
	return u;
}


void empty()
{
	while(sigfox.available()){
		sigfox.read();
	}
}




String rx()
{
 	String buffer = "";
 	bool begin = true;

 	while (1)
 	{
 		while (sigfox.available())
 		{
 			char c = sigfox.read();

 			if (c == '\r') continue; 

 			if (begin)
 			{
 				if (c == '\n') continue;
 				begin = false;
 			}

 			if (c == '\n') {
				Serial.print(">>> ");
				Serial.println(buffer);

 				return buffer;
 			}

 			buffer += c;
 		}
 	}
}

