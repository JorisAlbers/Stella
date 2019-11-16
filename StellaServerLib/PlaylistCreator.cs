using System.Collections.Generic;
using System.Linq;
using StellaServerLib.Animation;
using Path = System.IO.Path;

namespace StellaServerLib
{
    /// <summary>
    /// Creates playlists from storyboards
    /// </summary>
    public static class PlaylistCreator
    {
        public static PlayList Create(string name, List<Storyboard> storyboards, int duration)
        {
            PlayListItem[] items = storyboards.Select(x => new PlayListItem(x, duration)).ToArray();
            return new PlayList($"[PL] {name} - {duration} seconds", items);
        }

        /// <summary>
        /// Creates playlist with storyboards with the same path prefix
        /// </summary>
        /// <param name="storyboards"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static PlayList[] CreateFromCategory(List<Storyboard> storyboards, int duration)
        {
            Dictionary<string, List<Storyboard>> groupedStoryboards = new Dictionary<string, List<Storyboard>>();
            foreach (Storyboard storyboard in storyboards)
            {
                int pathSeparatorIndex = storyboard.Name.LastIndexOf(Path.DirectorySeparatorChar);
                if (pathSeparatorIndex == -1)
                {
                    continue;
                }
                
                string prefix = storyboard.Name.Substring(0,pathSeparatorIndex);
                if (groupedStoryboards.TryGetValue(prefix, out List<Storyboard> group))
                {
                    group.Add(storyboard);
                }
                else
                {
                    groupedStoryboards.Add(prefix, new List<Storyboard>{storyboard});
                }
            }

            return groupedStoryboards
                .Select(x => new PlayList($"[PL] {x.Key} - {duration} seconds", x.Value.Select(y => new PlayListItem(y, duration)).ToArray()))
                .ToArray();
        }
    }
}
