using System;
using System.Collections.Generic;
using System.Text;

namespace StellaServerLib.Animation
{
    /// <summary>
    /// Stores data to loop a set of storyboards. 
    /// </summary>
    public class PlayList
    {
        public string Name { get; }

        public PlayListItem[] Items { get; }
        
        public PlayList(string name, PlayListItem[] items)
        {
            Name = name;
            Items = items;
        }
    }
}
