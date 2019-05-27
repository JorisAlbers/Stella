﻿using System.IO;

namespace StellaServer.Serialization
{
    /// <summary>
    /// Loads an object from disc
    /// </summary>
    public interface ILoader<T>
    {
        /// <summary>
        /// Load
        /// </summary>
        /// <returns></returns>
        T Load(StreamReader streamReader);
    }
}