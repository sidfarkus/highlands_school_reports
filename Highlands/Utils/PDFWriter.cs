using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using iTextSharp.text.pdf;

namespace Highlands.Utils
{
    class PDFOutputFieldAttribute : Attribute
    {
        public string FieldName { get; set; }

        public PDFOutputFieldAttribute(string fieldName)
        {
            FieldName = fieldName;
        }
    }

    static class PDFWriter
    {
        const string DefaultTemplatePath = @"Report Card Template.pdf";

        public static void WritePDF(string outFilename, Dictionary<string, string> keys)
        {
            using (var existingFileStream = new FileStream(DefaultTemplatePath, FileMode.Open))
            using (var newFileStream = new FileStream(outFilename, FileMode.Create))
            {
                var pdfReader = new PdfReader(existingFileStream);
                var stamper = new PdfStamper(pdfReader, newFileStream);

                try
                {
                    var form = stamper.AcroFields;
                    var fieldKeys = form.Fields.Keys;
                    foreach (string fieldKey in fieldKeys)
                    {
                        if (keys.ContainsKey(fieldKey))
                        {
                            form.SetField(fieldKey, keys[fieldKey]);
                        }
                    }
                    stamper.FormFlattening = true;
                }
                finally
                {
                    stamper.Close();
                    pdfReader.Close();
                }
            }
        }
    }
}
