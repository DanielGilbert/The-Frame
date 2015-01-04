using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using TheFrame.Core;
using TheFrame.Mvvm;
using TheFrame.Plugin.Spectogram;

namespace TheFrame.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        private byte[] _data;
        private FrameManager _frameManager;
        private BackgroundWorker _backgroundWorker;
        private SpectogramDrawer _spectogramDrawer;
        public ICommand StartCommand { get; set; }

        public HomeViewModel()
        {
            _frameManager = new FrameManager();
            StartCommand = new RelayCommand(Start);

            _frameManager.Init();
            _backgroundWorker = new BackgroundWorker();
            _backgroundWorker.DoWork += _backgroundWorker_DoWork;

            _data = new byte[3 * 64];

            _spectogramDrawer = new SpectogramDrawer();
            _spectogramDrawer.Init();
        }

        void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            /*int n = 0;
            Random random = new Random(12);*/
            do
            {
                /*if (n > 7) n = 0;

                _data = new byte[3 * 8 * 8];

                for (int i = 0; i < 3 * 8 * n; i += 3)
                {
                    _data[i ] = 5;
                    _data[i + 1] =0;
                    _data[i + 2 ] = 0;
                }
            */
            lock(_spectogramDrawer.Data)
                _frameManager.PushData(0, _spectogramDrawer.Data);
            /*
                n++;
            */
                Thread.Sleep(30);
            } while (true);
            
        }



        private void Start(object obj)
        {

            _frameManager.StartAnimation();
            _spectogramDrawer.Show();
            _backgroundWorker.RunWorkerAsync();
        }
    }
}
