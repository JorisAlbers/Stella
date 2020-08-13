using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using StellaServerLib;
using StellaServerLib.Animation;

namespace StellaServer.Animation.Details
{
    public class PlaylistDetailsContolViewModel : ReactiveObject
    {
        private readonly PlayList _playList;
        private readonly BitmapRepository _bitmapRepository;

        public List<PlaylistItemDetailsViewModel> Items { get; set; }


        public PlaylistDetailsContolViewModel(PlayList playList, BitmapRepository bitmapRepository)
        {
            _playList = playList;
            _bitmapRepository = bitmapRepository;

            Items = playList.Items.Select(x => new PlaylistItemDetailsViewModel(x, bitmapRepository)).ToList();
        }
    }

    public class PlaylistItemDetailsViewModel: ReactiveObject
    {
        private readonly PlayListItem _playListItem;
        private readonly BitmapRepository _bitmapRepository;

        [Reactive] public int Duration { get; set; }

        public PlaylistItemDetailsViewModel(PlayListItem playListItemItem, BitmapRepository bitmapRepository)
        {
            _playListItem = playListItemItem;
            _bitmapRepository = bitmapRepository;
        }
    }
}
