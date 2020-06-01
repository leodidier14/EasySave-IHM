using IHM.ModelNameSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IHM.ObserverNameSpace
{
    interface IObserver
    {
        void update(IObservable observable, List<Backup> allBackup);

        void update(IObservable observable, Backup oneBackup);
    }
}
