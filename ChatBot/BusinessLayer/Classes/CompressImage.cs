using ChatBot.BusinessLayer.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace ChatBot.BusinessLayer.Classes
{
    public class CompressImage : ICompressImage
    {
        public string Compress(IFormFile imageFile)
        {
            string imageBase64 = string.Empty;

            using (var image = Image.Load(imageFile.OpenReadStream()))
            {
                using (var outputStream = new MemoryStream())
                {
                    var options = new JpegEncoder
                    {
                        Quality = GetCompressionQuality(imageFile.Length)
                    };

                    image.Save(outputStream, options);

                    imageBase64 = Convert.ToBase64String(outputStream.ToArray());

                }
            }

            return imageBase64;
        }

        private int GetCompressionQuality(long imageLength)
        {
            if (imageLength >= 2 * 1024 * 1024)
            {
                return 1;
            }
            else if (imageLength >= 1 * 1024 * 1024)
            {
                return 5;
            }
            else if (imageLength >= 0.1 * 1024 * 1024)
            {
                return 60;
            }

            return 100;
        }
    }
}
