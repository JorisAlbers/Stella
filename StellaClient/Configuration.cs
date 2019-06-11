﻿using System;
using System.Collections.Generic;
using System.Text;

namespace StellaClient
{
    /// <summary>
    /// The configuration of the StellaClient
    /// </summary>
    public class Configuration
    {
        /// <summary> The ID of this StellaClient. </summary>
        public int Id { get; }

        /// <summary> The ip address of stellaServer. </summary>
        public string Ip { get; }

        /// <summary> The port of StellaServer.</summary>
        public int Port { get; }

        /// <summary> The number of leds available.</summary>
        public int LedCount { get; }

        /// <summary> The pin the pwm is outputted from. </summary>
        public int PwmPin { get; }

        /// <summary> The dma channel used to generate the pwm signal.</summary>
        public int DmaChannel { get; }

        public Configuration(int id, string ip, int port, int ledCount, int pwmPin, int dmaChannel)
        {
            Id = id;
            Ip = ip;
            Port = port;
            LedCount = ledCount;
            PwmPin = pwmPin;
            DmaChannel = dmaChannel;
        }
    }
}