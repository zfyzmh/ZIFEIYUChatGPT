using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using ZFY.ChatGpt.Dto.InputDto;
using ZFY.ChatGpt.Dto.OutDto;

namespace ZIFEIYU.Services
{
    public class ImagesServices
    {
        private readonly ZFY.ChatGpt.Services.ImagesServices imagesServices;
        internal bool IsManualCancellation;

        public ImagesServices(ZFY.ChatGpt.Services.ImagesServices imagesServices)
        {
            this.imagesServices = imagesServices;
        }

        public async Task<OutImage> CreateImages(InCreateImages inCreateImage)
        {
            inCreateImage.N = 3;
            return await imagesServices.CreateImages(inCreateImage);
        }

        public Task Stop()
        {
            throw new NotImplementedException();
        }
    }
}