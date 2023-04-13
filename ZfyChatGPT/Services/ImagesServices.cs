using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using ZFY.ChatGpt.Dto.InputDto;
using ZFY.ChatGpt.Dto.OutDto;
using ZFY.ChatGpt.Services;

namespace ZfyChatGPT.Services
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
            return await imagesServices.CreateImages(inCreateImage);
        }

        public async Task Stop()
        {
            imagesServices.IsManualCancellation = true;
        }

        /*async Task SaveFile(CancellationToken cancellationToken)
        {
            using var stream = new MemoryStream(Encoding.Default.GetBytes("Hello from the Community Toolkit!"));
            var fileSaverResult = await FileSaver.Default.SaveAsync("test.txt", stream, cancellationToken);
            if (fileSaverResult.IsSuccessful)
            {
                await Toast.Make($"The file was saved successfully to location: {fileSaverResult.FilePath}").Show(cancellationToken);
            }
            else
            {
                await Toast.Make($"The file was not saved successfully with error: {fileSaverResult.Exception.Message}").Show(cancellationToken);
            }
        }*/
    }
}