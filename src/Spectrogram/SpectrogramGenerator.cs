﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace Spectrogram
{
    public class SpectrogramGenerator
    {
        /// <summary>
        /// Number of pixel columns (FFT samples) in the spectrogram image
        /// </summary>
        public int Width { get => FFTs.Count; }

        /// <summary>
        /// Number of pixel rows (frequency bins) in the spectrogram image
        /// </summary>
        public int Height { get => Settings.Height; }

        /// <summary>
        /// Number of samples to use for each FFT (must be a power of 2)
        /// </summary>
        public int FftSize { get => Settings.FftSize; }

        /// <summary>
        /// Vertical resolution (frequency bin size depends on FftSize and SampleRate)
        /// </summary>
        public double HzPerPx { get => Settings.HzPerPixel; }

        /// <summary>
        /// Horizontal resolution (seconds per pixel depends on StepSize)
        /// </summary>
        public double SecPerPx { get => Settings.StepLengthSec; }

        /// <summary>
        /// Number of FFTs that remain to be processed for data which has been added but not yet analuyzed
        /// </summary>
        public int FftsToProcess { get => (UnprocessedData.Count - Settings.FftSize) / Settings.StepSize; }

        /// <summary>
        /// Total number of FFT steps processed
        /// </summary>
        public int FftsProcessed { get; private set; }

        /// <summary>
        /// Index of the pixel column which will be populated next. Location of vertical line for wrap-around displays.
        /// </summary>
        public int NextColumnIndex { get => (FftsProcessed + rollOffset) % Width; }

        /// <summary>
        /// This value is added to displayed frequency axis tick labels
        /// </summary>
        public int OffsetHz { get => Settings.OffsetHz; set { Settings.OffsetHz = value; } }

        /// <summary>
        /// Number of samples per second
        /// </summary>
        public int SampleRate { get => Settings.SampleRate; }

        /// <summary>
        /// Number of samples to step forward after each FFT is processed.
        /// This value controls the horizontal resolution of the spectrogram.
        /// </summary>
        public int StepSize { get => Settings.StepSize; }

        /// <summary>
        /// The spectrogram is trimmed to cut-off frequencies below this value.
        /// </summary>
        public double FreqMax { get => Settings.FreqMax; }

        /// <summary>
        /// The spectrogram is trimmed to cut-off frequencies above this value.
        /// </summary>
        public double FreqMin { get => Settings.FreqMin; }

        /// <summary>
        /// This module contains detailed FFT/Spectrogram settings
        /// </summary>
        private readonly Settings Settings;

        /// <summary>
        /// This is the list of FFTs which is translated to the spectrogram image when it is requested.
        /// The length of this list is the spectrogram width.
        /// The length of the arrays in this list is the spectrogram height.
        /// </summary>
        private readonly List<double[]> FFTs = new List<double[]>();

        /// <summary>
        /// This list contains data values which have not yet been processed.
        /// Process() processes all unprocessed data.
        /// This list may not be empty after processing if there aren't enough values to fill a full FFT (FftSize).
        /// </summary>
        private readonly List<double> UnprocessedData;

        /// <summary>
        /// Colormap to use when generating future FFTs.
        /// </summary>
        public Colormap Colormap = Colormap.Viridis;

        /// <summary>
        /// Instantiate a spectrogram generator.
        /// This module calculates the FFT over a moving window as data comes in.
        /// Using the Add() method to load new data and process it as it arrives.
        /// </summary>
        /// <param name="sampleRate">Number of samples per second (Hz)</param>
        /// <param name="fftSize">Number of samples to use for each FFT operation. This value must be a power of 2.</param>
        /// <param name="stepSize">Number of samples to step forward</param>
        /// <param name="minFreq">Frequency data lower than this value (Hz) will not be stored</param>
        /// <param name="maxFreq">Frequency data higher than this value (Hz) will not be stored</param>
        /// <param name="fixedWidth">Spectrogram output will always be sized to this width (column count)</param>
        /// <param name="offsetHz">This value will be added to displayed frequency axis tick labels</param>
        /// <param name="initialAudioList">Analyze this data immediately (alternative to calling Add() later)</param>
        public SpectrogramGenerator(
            int sampleRate,
            int fftSize,
            int stepSize,
            double minFreq = 0,
            double maxFreq = double.PositiveInfinity,
            int? fixedWidth = null,
            int offsetHz = 0,
            List<double> initialAudioList = null)
        {
            Settings = new Settings(sampleRate, fftSize, stepSize, minFreq, maxFreq, offsetHz);

            UnprocessedData = initialAudioList ?? new List<double>();

            if (fixedWidth.HasValue)
                SetFixedWidth(fixedWidth.Value);
        }

        public override string ToString()
        {
            double processedSamples = FFTs.Count * Settings.StepSize + Settings.FftSize;
            double processedSec = processedSamples / Settings.SampleRate;
            string processedTime = (processedSec < 60) ? $"{processedSec:N2} sec" : $"{processedSec / 60.0:N2} min";

            return $"Spectrogram ({Width}, {Height})" +
                   $"\n  Vertical ({Height} px): " +
                   $"{Settings.FreqMin:N0} - {Settings.FreqMax:N0} Hz, " +
                   $"FFT size: {Settings.FftSize:N0} samples, " +
                   $"{Settings.HzPerPixel:N2} Hz/px" +
                   $"\n  Horizontal ({Width} px): " +
                   $"{processedTime}, " +
                   $"window: {Settings.FftLengthSec:N2} sec, " +
                   $"step: {Settings.StepLengthSec:N2} sec, " +
                   $"overlap: {Settings.StepOverlapFrac * 100:N0}%";
        }

        [Obsolete("Assign to the Colormap field")]
        /// <summary>
        /// Set the colormap to use for future renders
        /// </summary>
        public void SetColormap(Colormap cmap)
        {
            Colormap = cmap ?? this.Colormap;
        }

        /// <summary>
        /// Load a custom window kernel to multiply against each FFT sample prior to processing.
        /// Windows must be at least the length of FftSize and typically have a sum of 1.0.
        /// </summary>
        public void SetWindow(double[] newWindow)
        {
            if (newWindow.Length > Settings.FftSize)
                throw new ArgumentException("window length cannot exceed FFT size");

            for (int i = 0; i < Settings.FftSize; i++)
                Settings.Window[i] = 0;

            int offset = (Settings.FftSize - newWindow.Length) / 2;
            Array.Copy(newWindow, 0, Settings.Window, offset, newWindow.Length);
        }

        [Obsolete("use the Add() method", true)]
        public void AddExtend(float[] values) { }

        [Obsolete("use the Add() method", true)]
        public void AddCircular(float[] values) { }

        [Obsolete("use the Add() method", true)]
        public void AddScroll(float[] values) { }

        /// <summary>
        /// Load new data into the spectrogram generator
        /// </summary>
        public void Add(IEnumerable<double> audio, bool process = true)
        {
            UnprocessedData.AddRange(audio);
            if (process)
                Process();
        }

        /// <summary>
        /// The roll offset is used to calculate NextColumnIndex and can be set to a positive number 
        /// to begin adding new columns to the center of the spectrogram.
        /// This can also be used to artificially move the next column index to zero even though some
        /// data has already been accumulated.
        /// </summary>
        private int rollOffset = 0;

        /// <summary>
        /// Reset the next column index such that the next processed FFT will appear at the far left of the spectrogram.
        /// </summary>
        /// <param name="offset"></param>
        public void RollReset(int offset = 0)
        {
            rollOffset = -FftsProcessed + offset;
        }

        /// <summary>
        /// Perform FFT analysis on all unprocessed data
        /// </summary>
        public double[][] Process()
        {
            if (FftsToProcess < 1)
                return null;

            int newFftCount = FftsToProcess;
            double[][] newFfts = new double[newFftCount][];

            Parallel.For(0, newFftCount, newFftIndex =>
            {
                FftSharp.Complex[] buffer = new FftSharp.Complex[Settings.FftSize];
                int sourceIndex = newFftIndex * Settings.StepSize;
                for (int i = 0; i < Settings.FftSize; i++)
                    buffer[i].Real = UnprocessedData[sourceIndex + i] * Settings.Window[i];

                FftSharp.Transform.FFT(buffer);

                newFfts[newFftIndex] = new double[Settings.Height];
                for (int i = 0; i < Settings.Height; i++)
                    newFfts[newFftIndex][i] = buffer[Settings.FftIndex1 + i].Magnitude / Settings.FftSize;
            });

            foreach (var newFft in newFfts)
                FFTs.Add(newFft);
            FftsProcessed += newFfts.Length;

            UnprocessedData.RemoveRange(0, newFftCount * Settings.StepSize);
            PadOrTrimForFixedWidth();

            return newFfts;
        }

        /// <summary>
        /// Return a list of the mel-scaled FFTs contained in this spectrogram
        /// </summary>
        /// <param name="melBinCount">Total number of output bins to use. Choose a value significantly smaller than Height.</param>
        public List<double[]> GetMelFFTs(int melBinCount)
        {
            if (Settings.FreqMin != 0)
                throw new InvalidOperationException("cannot get Mel spectrogram unless minimum frequency is 0Hz");

            var fftsMel = new List<double[]>();
            foreach (var fft in FFTs)
                fftsMel.Add(FftSharp.Transform.MelScale(fft, SampleRate, melBinCount));

            return fftsMel;
        }

        /// <summary>
        /// Create and return a spectrogram bitmap from the FFTs stored in memory.
        /// </summary>
        /// <param name="intensity">Multiply the output by a fixed value to change its brightness.</param>
        /// <param name="dB">If true, output will be log-transformed.</param>
        /// <param name="dBScale">If dB scaling is in use, this multiplier will be applied before log transformation.</param>
        /// <param name="roll">Behavior of the spectrogram when it is full of data. 
        /// Roll (true) adds new columns on the left overwriting the oldest ones.
        /// Scroll (false) slides the whole image to the left and adds new columns to the right.</param>
        public Bitmap GetBitmap(double intensity = 1, bool dB = false, double dBScale = 1, bool roll = false) =>
            Image.GetBitmap(FFTs, Colormap, intensity, dB, dBScale, roll, NextColumnIndex);

        /// <summary>
        /// Create a Mel-scaled spectrogram.
        /// </summary>
        /// <param name="melBinCount">Total number of output bins to use. Choose a value significantly smaller than Height.</param>
        /// <param name="intensity">Multiply the output by a fixed value to change its brightness.</param>
        /// <param name="dB">If true, output will be log-transformed.</param>
        /// <param name="dBScale">If dB scaling is in use, this multiplier will be applied before log transformation.</param>
        /// <param name="roll">Behavior of the spectrogram when it is full of data. 
        /// Roll (true) adds new columns on the left overwriting the oldest ones.
        /// Scroll (false) slides the whole image to the left and adds new columns to the right.</param>
        public Bitmap GetBitmapMel(int melBinCount = 25, double intensity = 1, bool dB = false, double dBScale = 1, bool roll = false) =>
            Image.GetBitmap(GetMelFFTs(melBinCount), Colormap, intensity, dB, dBScale, roll, NextColumnIndex);

        [Obsolete("use SaveImage()", true)]
        public void SaveBitmap(Bitmap bmp, string fileName) { }

        /// <summary>
        /// Generate the spectrogram and save it as an image file.
        /// </summary>
        /// <param name="fileName">Path of the file to save.</param>
        /// <param name="intensity">Multiply the output by a fixed value to change its brightness.</param>
        /// <param name="dB">If true, output will be log-transformed.</param>
        /// <param name="dBScale">If dB scaling is in use, this multiplier will be applied before log transformation.</param>
        /// <param name="roll">Behavior of the spectrogram when it is full of data. 
        /// Roll (true) adds new columns on the left overwriting the oldest ones.
        /// Scroll (false) slides the whole image to the left and adds new columns to the right.</param>
        public void SaveImage(string fileName, double intensity = 1, bool dB = false, double dBScale = 1, bool roll = false)
        {
            if (FFTs.Count == 0)
                throw new InvalidOperationException("Spectrogram contains no data. Use Add() to add signal data.");

            string extension = Path.GetExtension(fileName).ToLower();

            ImageFormat fmt;
            if (extension == ".bmp")
                fmt = ImageFormat.Bmp;
            else if (extension == ".png")
                fmt = ImageFormat.Png;
            else if (extension == ".jpg" || extension == ".jpeg")
                fmt = ImageFormat.Jpeg;
            else if (extension == ".gif")
                fmt = ImageFormat.Gif;
            else
                throw new ArgumentException("unknown file extension");

            Image.GetBitmap(FFTs, Colormap, intensity, dB, dBScale, roll, NextColumnIndex).Save(fileName, fmt);
        }

        /// <summary>
        /// Create and return a spectrogram bitmap from the FFTs stored in memory.
        /// The output will be scaled-down vertically by binning according to a reduction factor and keeping the brightest pixel value in each bin.
        /// </summary>
        /// <param name="intensity">Multiply the output by a fixed value to change its brightness.</param>
        /// <param name="dB">If true, output will be log-transformed.</param>
        /// <param name="dBScale">If dB scaling is in use, this multiplier will be applied before log transformation.</param>
        /// <param name="roll">Behavior of the spectrogram when it is full of data. 
        /// <param name="reduction"></param>
        public Bitmap GetBitmapMax(double intensity = 1, bool dB = false, double dBScale = 1, bool roll = false, int reduction = 4)
        {
            List<double[]> ffts2 = new List<double[]>();
            for (int i = 0; i < FFTs.Count; i++)
            {
                double[] d1 = FFTs[i];
                double[] d2 = new double[d1.Length / reduction];
                for (int j = 0; j < d2.Length; j++)
                    for (int k = 0; k < reduction; k++)
                        d2[j] = Math.Max(d2[j], d1[j * reduction + k]);
                ffts2.Add(d2);
            }
            return Image.GetBitmap(ffts2, Colormap, intensity, dB, dBScale, roll, NextColumnIndex);
        }

        /// <summary>
        /// Export spectrogram data using the Spectrogram File Format (SFF)
        /// </summary>
        public void SaveData(string filePath, int melBinCount = 0)
        {
            if (!filePath.EndsWith(".sff", StringComparison.OrdinalIgnoreCase))
                filePath += ".sff";
            new SFF(this, melBinCount).Save(filePath);
        }

        /// <summary>
        /// Defines the total number of FFTs (spectrogram columns) to store in memory. Determines Width.
        /// </summary>
        private int fixedWidth = 0;

        /// <summary>
        /// Configure the Spectrogram to maintain a fixed number of pixel columns.
        /// Zeros will be added to padd existing data to achieve this width, and extra columns will be deleted.
        /// </summary>
        public void SetFixedWidth(int width)
        {
            fixedWidth = width;
            PadOrTrimForFixedWidth();
        }

        private void PadOrTrimForFixedWidth()
        {
            if (fixedWidth > 0)
            {
                int overhang = Width - fixedWidth;
                if (overhang > 0)
                    FFTs.RemoveRange(0, overhang);

                while (FFTs.Count < fixedWidth)
                    FFTs.Insert(0, new double[Height]);
            }
        }

        /// <summary>
        /// Get a vertical image containing ticks and tick labels for the frequency axis.
        /// </summary>
        /// <param name="width">size (pixels)</param>
        /// <param name="offsetHz">number to add to each tick label</param>
        /// <param name="tickSize">length of each tick mark (pixels)</param>
        /// <param name="reduction">bin size for vertical data reduction</param>
        public Bitmap GetVerticalScale(int width, int offsetHz = 0, int tickSize = 3, int reduction = 1)
        {
            return Scale.Vertical(width, Settings, offsetHz, tickSize, reduction);
        }

        /// <summary>
        /// Return the vertical position (pixel units) for the given frequency
        /// </summary>
        public int PixelY(double frequency, int reduction = 1)
        {
            int pixelsFromZeroHz = (int)(Settings.PxPerHz * frequency / reduction);
            int pixelsFromMinFreq = pixelsFromZeroHz - Settings.FftIndex1 / reduction + 1;
            int pixelRow = Settings.Height / reduction - 1 - pixelsFromMinFreq;
            return pixelRow - 1;
        }

        /// <summary>
        /// Return the list of FFTs in memory underlying the spectrogram.
        /// This list may continue to evolve after it is returned.
        /// </summary>
        public List<double[]> GetFFTs()
        {
            return FFTs;
        }

        /// <summary>
        /// Return frequency and magnitude of the dominant frequency.
        /// </summary>
        /// <param name="latestFft">If true, only the latest FFT will be assessed.</param>
        public (double freqHz, double magRms) GetPeak(bool latestFft = true)
        {
            if (FFTs.Count == 0)
                return (double.NaN, double.NaN);

            if (latestFft == false)
                throw new NotImplementedException("peak of mean of all FFTs not yet supported");

            double[] freqs = FFTs[FFTs.Count - 1];

            int peakIndex = 0;
            double peakMagnitude = 0;
            for (int i = 0; i < freqs.Length; i++)
            {
                if (freqs[i] > peakMagnitude)
                {
                    peakMagnitude = freqs[i];
                    peakIndex = i;
                }
            }

            double maxFreq = SampleRate / 2;
            double peakFreqFrac = peakIndex / (double)freqs.Length;
            double peakFreqHz = maxFreq * peakFreqFrac;

            return (peakFreqHz, peakMagnitude);
        }
    }
}
