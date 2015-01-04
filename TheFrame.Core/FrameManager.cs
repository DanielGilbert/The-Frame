using BlinkStickDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TheFrame.Core
{
    

    public class FrameManager
    {
        #region Fields
        private byte[] _redColorData;

        private BackgroundWorker _imageRefreshWorker;
        private List<BlinkStick> _blinkSticks;
        private BlinkStick _currentBlinkStick;
        #endregion

        #region Properties
        public List<BlinkStick> BlinkSticks
        {
            get
            {
                return _blinkSticks;
            }
            set
            {
                _blinkSticks = value;
            }
        }

        public BlinkStick CurrentBlinkStick
        {
            get
            {
                return _currentBlinkStick;
            }
            set
            {
                _currentBlinkStick = value;
                if (_currentBlinkStick == null) return;

                if (_currentBlinkStick.OpenDevice ())
                    if (_currentBlinkStick.GetMode() != 2)
                        _currentBlinkStick.SetMode(2);
            }
        }
        #endregion

        public FrameManager()
        {
            _imageRefreshWorker = new BackgroundWorker();
            _imageRefreshWorker.DoWork += _imageRefreshWorker_DoWork;
            _imageRefreshWorker.WorkerSupportsCancellation = true;

            _redColorData = new byte[3 * 64];
        }

        void _imageRefreshWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            do
            {
                lock (_redColorData)
                {
                    _currentBlinkStick.SetColors(0, _redColorData);
                }

                Thread.Sleep(15);
            } while (!e.Cancel);
        }

        public void Init()
        {
            BlinkSticks = BlinkStick.FindAll().ToList();

            if (BlinkSticks.Count > 0)
            {
                CurrentBlinkStick = BlinkSticks.FirstOrDefault();
            }
        }


        public void SetColor(byte channel, byte index, string color)
        {
            if (_currentBlinkStick == null) return;
            if (!_currentBlinkStick.Connected) return;

            _currentBlinkStick.SetColor(channel, index, color);
        }


        public void SetColors(byte channel, byte[] colorData)
        {
            if (_currentBlinkStick == null) return;
            if (!_currentBlinkStick.Connected) return;

            _currentBlinkStick.SetColors(channel, colorData);
        }

        public void StartAnimation()
        {
            _imageRefreshWorker.RunWorkerAsync();
        }

        public void StopAnimation()
        {
            _imageRefreshWorker.CancelAsync();
        }

        public void PushData(byte channel, byte[] colorData)
        {
            _redColorData = colorData;
        }
    }
}
