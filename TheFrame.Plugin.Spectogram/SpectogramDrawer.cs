using CSCore.CoreAudioAPI;
using CSCore;
using CSCore.Codecs;
using CSCore.DSP;
using CSCore.SoundOut;
using CSCore.Streams;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheFrame.Common;
using TheFrame.Common.Delegates;
using CSCore.SoundIn;

namespace TheFrame.Plugin.Spectogram
{
    public class SpectogramDrawer : IPlugin
    {
        private BackgroundWorker _spectogramDataGrabber;
        private WasapiLoopbackCapture _loopbackCapture;
        private SoundInSource _soundInSource;
        private BasicSpectrumProvider _spectrumProvider;
        private MatrixSpectrumProvider _matrixSpectrumProvider;
        private IWaveSource _finalSource;
        public byte[] Data;
        const FftSize fftSize = FftSize.Fft4096;
        public string Name
        {
            get { return "Spectogram"; }
        }

        public string Description
        {
            get { return "Awesome Spectogram Thingy"; }
        }

        public event NewDrawableDataDelegate OnNewDrawableData = delegate { };
        private IWaveSource source;
        private WasapiOut _soundOut;

        public void Init()
        {
            FftProvider fftProvider = new FftProvider(2, FftSize.Fft1024);

            Data = new byte[3 * 64];
            _spectogramDataGrabber = new BackgroundWorker();
            _spectogramDataGrabber.DoWork += _spectogramDataGrabber_DoWork;
            _spectogramDataGrabber.WorkerSupportsCancellation = true;

            //_loopbackCapture = new WasapiLoopbackCapture();
            

            //_loopbackCapture.DataAvailable += _loopbackCapture_DataAvailable;
            //_loopbackCapture.Initialize();
            //_loopbackCapture.Start();

            //_soundInSource = new SoundInSource(_loopbackCapture);
            


            //SingleBlockNotificationStream _singleBlockNotificationStream = new SingleBlockNotificationStream(_soundInSource);
            //_finalSource = _singleBlockNotificationStream.ToWaveSource();

            //_singleBlockNotificationStream.SingleBlockRead += _singleBlockNotificationStream_SingleBlockRead;
            //byte[] buffer = new byte[_loopbackCapture.WaveFormat.BytesPerSecond / 2];
            //_soundInSource.DataAvailable += (s, e) =>
            //{
            //    int read;
            //    while ((read = _loopbackCapture.Read(buffer, 0, buffer.Length)) > 0)
            //        _writer.Write(buffer, 0, read);
            //};
            

            //////_soundOut = new WasapiOut();
            //////_soundOut.Initialize(new LoopStream(source));
            //////_soundOut.Play();
            //////_soundOut.Volume = 1;

            _loopbackCapture = new WasapiLoopbackCapture();
            _loopbackCapture.Device = MMDeviceEnumerator.DefaultAudioEndpoint(DataFlow.Render, Role.Console);
            //_soundIn.Device = SelectedDevice;
            _loopbackCapture.Initialize();

            var soundInSource = new SoundInSource(_loopbackCapture);

            _spectrumProvider = new BasicSpectrumProvider(_loopbackCapture.WaveFormat.Channels, _loopbackCapture.WaveFormat.SampleRate, FftSize.Fft1024);


            _matrixSpectrumProvider = new MatrixSpectrumProvider(FftSize.Fft1024, 8, 8)
            {
                SpectrumProvider = _spectrumProvider,
                ScalingStrategy = ScalingStrategy.Sqrt,
                MaximumFrequency = 16000,
                MinimumFrequency = 100,
                
                UseAverage = true,
                IsXLogScale = true,
                SpectrumResolution = 8
            };

            var singleBlockNotificationStream = new SingleBlockNotificationStream(soundInSource);
            _finalSource = singleBlockNotificationStream.ToWaveSource();
            //_writer = new WaveWriter(fileName, _finalSource.WaveFormat);

            byte[] buffer = new byte[_finalSource.WaveFormat.BytesPerSecond / 2];
            soundInSource.DataAvailable += (s, e) =>
            {
                int read;
                while ((read = _finalSource.Read(buffer, 0, buffer.Length)) > 0)
                {

                }
            };

            singleBlockNotificationStream.SingleBlockRead += SingleBlockNotificationStreamOnSingleBlockRead;

            _loopbackCapture.Start();
        }

        private void SingleBlockNotificationStreamOnSingleBlockRead(object sender, SingleBlockReadEventArgs e)
        {
            _spectrumProvider.Add(e.Left, e.Right);
        }


        void _singleBlockNotificationStream_SingleBlockRead(object sender, SingleBlockReadEventArgs e)
        {

        }

        void _loopbackCapture_DataAvailable(object sender, DataAvailableEventArgs e)
        {
            lock (Data)
            {
                int count = e.ByteCount;
            }
        }

        void _spectogramDataGrabber_DoWork(object sender, DoWorkEventArgs e)
        {
            do
            {
                lock (Data)
                {
                    Data = _matrixSpectrumProvider.GetDisplayData();
                }
                Thread.Sleep(50);
            } while (!e.Cancel);
        }

        public void Show()
        {
            
            _spectogramDataGrabber.RunWorkerAsync();
        }

        public void Hide()
        {
            _spectogramDataGrabber.CancelAsync();
        }
    }
}
