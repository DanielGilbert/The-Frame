using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheFrame.Common.Delegates;

namespace TheFrame.Common
{
    public interface IPlugin
    {
        string Name { get; }
        string Description { get; }

        event NewDrawableDataDelegate OnNewDrawableData;

        void Init();

        void Show();

        void Hide();
    }
}
