using System.IO;
using Aijkl.VRChat.Posters.Twitter.Shared.Models;
using Xabe.FFmpeg;

namespace Aijkl.VRChat.Posters.Twitter
{
    public class ImageToVideoConverter
    {
        private readonly FFMpegParameters _ffmpegParameters;
        public ImageToVideoConverter(FFMpegParameters ffmpegParameters)
        {
            _ffmpegParameters = ffmpegParameters;
            if (!string.IsNullOrEmpty(_ffmpegParameters.FFMpegPath))
            {
                FFmpeg.SetExecutablesPath(_ffmpegParameters.FFMpegPath);
            }
        }

        public string GenerateOutputPath(string outputDirectory, string fileNameWithoutExtension)
        {
            return Path.Combine(outputDirectory, $"{fileNameWithoutExtension}.{_ffmpegParameters.OutputFormat}");
        }
        public void CreateVideoFromImage(string inputPath,string filePath)
        {
            FFmpeg.Conversions.New()
                .AddParameter($"-i {inputPath}")
                .AddParameter($"-vcodec {_ffmpegParameters.Codec}")
                .AddParameter($"-pix_fmt {_ffmpegParameters.PixelFormat}")
                .SetOutput(filePath)
                .SetOverwriteOutput(true)
                .Start().Wait();
        }
    }
}
