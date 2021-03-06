﻿namespace StellaServerAPI
{
    public enum MessageType
    {
        None,
        /// <summary> Get the names of the preloaded storyboard </summary>
        GetAvailableStoryboards,
        /// <summary> Start a preloaded storyboard (by providing the name) </summary>
        StartPreloadedStoryboard,
        /// <summary> Start a storyboard (by providing the yaml)</summary>
        StartStoryboard,
        /// <summary> Store a bitmap on StellaServer </summary>
        StoreBitmap,
        /// <summary> Get the timeUnitsPerFrame of a certain animation. </summary>
        GetTimeUnitsPerFrame,
        /// <summary> Set the timeUnitsPerFrame of a certain animation. </summary>
        SetTimeUnitsPerFrame,
        /// <summary> Get the rgb fade of a certain animation </summary>
        GetRgbFade,
        /// <summary> Set the rgb fade of a certain animation </summary>
        SetRgbFade,
        /// <summary> Get the brightness correction of a certain animation. Between -1 and 1 </summary>
        GetBrightnessCorrection,
        /// <summary> Set the brightness correction of a certain animation. Between -1 and 1 </summary>
        SetBrightnessCorrection
    }
}
