using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

namespace Common
{
    [ServiceContract]
    public interface IHealthMonitoring
    {
        [OperationContract]
        void IAmAlive();
    }

}
