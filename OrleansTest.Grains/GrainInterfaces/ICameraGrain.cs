using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrleansTest.Grains.GrainInterfaces
{
    public interface ICameraGrain : IGrainWithGuidKey
    {
        Task Initialize(string cameraName);

        Task<string> GetName();
    }
}
