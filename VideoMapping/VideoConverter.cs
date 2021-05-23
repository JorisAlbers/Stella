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
        private readonly int _rows;
        private readonly int _pixelsPerRow;

        public VideosConverter(DirectoryInfo inputFolder, DirectoryInfo outputFolder, int rows, int pixelsPerRow)
        {
            _inputFolder = inputFolder;
            _outputFolder = outputFolder;
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
            }
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
            /*for (int i = 0; i < matPerRow.Length; i++)
            {
                CvInvoke.Resize(matPerRow[i], matPerRow[i], new Size(_pixelsPerRow, matPerRow[i].Height));
                // save
                matPerRow[i].Save(Path.Combine(outputDirectoryInfo.FullName, $"vm{video.ID}_row{i}.png"));
                matPerRow[i].Dispose();
            }*/
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
