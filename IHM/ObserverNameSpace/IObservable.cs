using IHM.ModelNameSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IHM.ObserverNameSpace
{
    interface IObservable
    {
        void attach(IObserver observer);
        void detach(IObserver observer);
        void notify(List<Backup> allBackup);
        void notify(Backup oneBackup);
    }
}

