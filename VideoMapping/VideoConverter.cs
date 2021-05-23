using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoMapping
{
    public class VideosConverter
    {
        private readonly DirectoryInfo _inputFolder;
        private readonly DirectoryInfo _outputFolder;

        public VideosConverter(DirectoryInfo inputFolder, DirectoryInfo outputFolder)
        {
            _inputFolder = inputFolder;
            _outputFolder = outputFolder;
        }
    }

    public class VideoConverter
    {
        private readonly VideoFile _video;
        private readonly DirectoryInfo _outputDirectoryInfo;

        public VideoConverter(VideoFile video, DirectoryInfo outputDirectoryInfo)
        {
            _video = video;
            _outputDirectoryInfo = outputDirectoryInfo;
        }
    }

    public class VideoFile
    {
        public FileInfo FileInfo { get; set; }

        public int ID { get; set; }
    }
}
