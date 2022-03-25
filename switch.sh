#!/bin/bash
./rs232 send "*usbserial" baud=57600,data=8,parity=None,newline=true "EZS OUT$1 VS IN$2"