
#ifndef Ultrasonic_H
#define Ultrasonic_H

class Ultrasonic
{
	public:
		Ultrasonic(int pin);
        void distanceMeasure(void);
		long microsecondsToCentimeters(void);
		long microsecondsToInches(void);
	private:
		int _pin;//pin number of Arduino that is connected with SIG pin of Ultrasonic Ranger.
        long duration;// the Pulse time received;
};

#endif