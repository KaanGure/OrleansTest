using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrleansTest.Grains.GrainInterfaces
{
    public interface IAccountGrain : IGrainWithGuidKey
    {
        Task Initialize(string accountName);
        Task<string> GetName();
        Task<DateTime> GetCreationTime();
        Task<List<Guid>> GetCameraIds();
        Task AddCamera(Guid cameraId);
    }
}
