using System.IO;
using OpenCvSharp;


namespace StellaServerLib.VideoMapping;

public class VideoConverter
{
    private readonly BitmapRepository _outputBitmapRepository;
    private readonly int _rows;
    private readonly int _pixelsPerRow;

    public VideoConverter(BitmapRepository outputBitmapRepository, int rows, int pixelsPerRow)
    {
        _outputBitmapRepository = outputBitmapRepository;
        _rows = rows;
        _pixelsPerRow = pixelsPerRow;
    }


    public void Convert(FileInfo video, string videoFilePrefix)
    {
        using VideoCapture videoCapture = new VideoCapture(video.FullName);
        Mat frame = new Mat();
        videoCapture.Read(frame);

        int numberOfFrames = (int)videoCapture.Get(VideoCaptureProperties.FrameCount);
        int[] rowIndexes = CalculateRowIndexes(_rows, frame.Height);
        Mat[] matPerRow = InitializeMatPerRows(rowIndexes, frame, numberOfFrames);

        DistributeVideoOverRows(videoCapture, frame, matPerRow, rowIndexes);

        // Resize to mapping
        for (int i = 0; i < matPerRow.Length; i++)
        {
            Cv2.Resize(matPerRow[i], matPerRow[i], new OpenCvSharp.Size(_pixelsPerRow, matPerRow[i].Height));
            // save
            _outputBitmapRepository.Save(matPerRow[i], @$"{videoFilePrefix}_row{i}");

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