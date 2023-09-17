using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using ZXing;
using ZXing.Common;
using ZXing.QrCode.Internal;

namespace ReadBarcodesQR
{
    [Category("marcos.cuesta.read.barcodesQR")]
    [DisplayName("Read Barcode")]
    [Description("The text of the tooltip")]
    public class ExtractCodebar : CodeActivity{
        [Category("Input")]
        [RequiredArgument]
        [DisplayName("Full path to image")]
        [Description("The text of the tooltip")]
        public InArgument<string> path { get; set; }


        [Category("Output")]
        [DisplayName("Output")]
        [Description("The text of the tooltip")]
        public OutArgument<string[]> result { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            string path = this.path.Get(context);
            Console.WriteLine("Path: " + path);
            if (path == null) throw new ArgumentException("Path is null");

            var bitmap = new Bitmap(path);

            // Decodificación del código de barras
            var reader = new BarcodeReader();
            var result = reader.Decode(bitmap);
            if (result != null)
            {
                Console.WriteLine("Barcode content: " + result.Text);
                this.result.Set(context, new string[] {
                    result.Text,
                    result.BarcodeFormat.ToString(),
                    result.ResultMetadata.ToString(),
                    result.NumBits.ToString(),
                    result.Timestamp.ToString(),
                });
            }
            else
            {
                Console.WriteLine("Barcode can't be decoded, check the quality of the image");
                throw new ArgumentException("QR can't be decoded, check the quality of the image");
            }
        }
    }
}
