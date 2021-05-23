using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenCvSharp;
using Size = System.Windows.Size;

namespace VideoMapping
{
    public class VideosConverter
    {
        private readonly DirectoryInfo _inputFolder;
        private readonly DirectoryInfo _outputFolder;
        private readonly DirectoryInfo _storyboardFolder;
        private readonly int _rows;
        private readonly int _pixelsPerRow;

        public VideosConverter(DirectoryInfo inputFolder, DirectoryInfo outputFolder, DirectoryInfo storyboardFolder, int rows, int pixelsPerRow)
        {
            _inputFolder = inputFolder;
            _outputFolder = outputFolder;
            _storyboardFolder = storyboardFolder;
            _rows = rows;
            _pixelsPerRow = pixelsPerRow;
        }

        public void Start()
        {
            _outputFolder.Create();
            FileInfo[] videos = _inputFolder.EnumerateFiles("*.mp4").ToArray();
            foreach (FileInfo fileInfo in videos)
            {
                DirectoryInfo videoOutputFolder =
                    new DirectoryInfo(Path.Combine(_outputFolder.FullName, fileInfo.Name));

                videoOutputFolder.Create();

                VideoConverter converter = new VideoConverter(fileInfo, videoOutputFolder, _rows, _pixelsPerRow);
                converter.Start();

                CreateStoryBoard(fileInfo.Name, _storyboardFolder.FullName);
            }
        }

        private static void CreateStoryBoard(string videoName, string storyboardFolder)
        {
            string s = $@"!Storyboard
Name: videomapping v{videoName}
Animations:
 - !Bitmap
   StartIndex:  0
   StripLength: 480
   ImageName: {videoName}\{videoName}_row0
 - !Bitmap
   StartIndex:  480
   StripLength: 480
   ImageName: {videoName}\{videoName}_row1
 - !Bitmap
   StartIndex:  960
   StripLength: 480
   ImageName: {videoName}\{videoName}_row2
 - !Bitmap
   StartIndex:  1440
   StripLength: 480
   ImageName: {videoName}\{videoName}_row3
 - !Bitmap
   StartIndex:  1920
   StripLength: 480
   ImageName: {videoName}\{videoName}_row4
 - !Bitmap
   StartIndex:  2400
   StripLength: 480
   ImageName: {videoName}\{videoName}_row5";

            File.WriteAllText(Path.Combine(storyboardFolder, $"videomapping {videoName}.yaml"), s);
        }
    }

    public class VideoConverter
    {
        private readonly FileInfo _video;
        private readonly DirectoryInfo _outputDirectoryInfo;
        private readonly int _rows;
        private readonly int _pixelsPerRow;

        public VideoConverter(FileInfo video, DirectoryInfo outputDirectoryInfo, int rows, int pixelsPerRow)
        {
            _video = video;
            _outputDirectoryInfo = outputDirectoryInfo;
            _rows = rows;
            _pixelsPerRow = pixelsPerRow;
        }

        public void Start()
        {
            ConvertVideo(_video, _outputDirectoryInfo);
        }

        private void ConvertVideo(FileInfo video, DirectoryInfo outputDirectoryInfo)
        {
            Directory.CreateDirectory(outputDirectoryInfo.FullName);

            using VideoCapture videoCapture = new VideoCapture(video.FullName);
            Mat frame = new Mat();
            videoCapture.Read(frame);

            int numberOfFrames = (int)videoCapture.Get(VideoCaptureProperties.FrameCount);
            int[] rowIndexes = CalculateRowIndexes(_rows, frame.Height);
            Mat[] matPerRow = InitializeMatPerRows(rowIndexes, frame, numberOfFrames);

            DistributeVideoOverRows(videoCapture,frame, matPerRow, rowIndexes);

            // Resize to mapping
            for (int i = 0; i < matPerRow.Length; i++)
            {
                Cv2.Resize(matPerRow[i], matPerRow[i], new OpenCvSharp.Size(_pixelsPerRow, matPerRow[i].Height));
                // save
                Cv2.ImWrite(Path.Combine(outputDirectoryInfo.FullName, $"vm_{video.Name}_row{i}.png"), matPerRow[i]);
                matPerRow[i].Dispose();
            }
        }

        private static int[] CalculateRowIndexes(int numberOfRows, int height)
        {
            int[] rowIndexes = new int[numberOfRows];
            int increment = height / numberOfRows - 1;

            for (int i = 0; i < numberOfRows - 1; i++)
            {
                rowIndexes[i] = i * increment;
            }

            rowIndexes[rowIndexes.Length - 1] = height - 1;

            return rowIndexes;
        }

        private static Mat[] InitializeMatPerRows(int[] rowIndexes, Mat firstFrame, int numberOfFrames)
        {
            Mat[] results = new Mat[rowIndexes.Length];
            for (int i = 0; i < rowIndexes.Length; i++)
            {
                results[i] = new Mat(numberOfFrames, firstFrame.Cols, firstFrame.Type());
                firstFrame.Row(rowIndexes[i]).CopyTo(results[i].Row(0));
            }

            return results;
        }

        private static void DistributeVideoOverRows(VideoCapture videoCapture, Mat frame, Mat[] matPerRow, int[] rowIndexes)
        {
            int frameIndex = 1;
            while (videoCapture.Read(frame) && !frame.Empty())
            {
                for (int i = 0; i < matPerRow.Length; i++)
                {
                    frame.Row(rowIndexes[i]).CopyTo(matPerRow[i].Row(frameIndex));
                }

                frameIndex++;
            }
        }
    }
}
