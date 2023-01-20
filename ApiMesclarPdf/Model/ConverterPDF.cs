using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;
using PdfSharp.Pdf.IO;
using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Layout;
using PdfProperties = iText.Layout.Properties;
using PdfImage = iText.Layout.Element.Image;
using iTextSharp.text;

namespace ApiMesclarPdf.Model
{
    public class ConverterPDF
    {
        public byte[] ConverterEmByte(ICollection<IFormFile> filePaths)
        {
            try
            {

                List<byte[]> listByte = new List<byte[]>();
                byte[] ConverterByte = null;
                byte[] result = null;

                foreach (var formFile in filePaths)
                {
                    bool formatPdf = formFile.FileName.Contains("pdf");

                    if (formatPdf)
                    {
                        using (var stream = new MemoryStream())
                        {
                            formFile.CopyToAsync(stream);
                            listByte.Add(stream.ToArray());
                        }
                    }
                    else
                    {
                        using (var stream = new MemoryStream())
                        {
                            formFile.CopyToAsync(stream);
                            ConverterByte = stream.ToArray();
                        }

                        var retorno = ConverterImageParaPDF(ConverterByte);
                        listByte.Add(retorno);
                    }
                }

                result = MergePdf(listByte);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao converter os arquivos de upload em byte. Mais detalhes " + ex);
            }

        }

        public static byte[] MergePdf(List<byte[]> pdfs)
        {
            try
            {
                byte[] result = null;

                List<PdfSharp.Pdf.PdfDocument> lstDocuments = new List<PdfSharp.Pdf.PdfDocument>();
                foreach (var pdf in pdfs)
                {
                    lstDocuments.Add(PdfSharp.Pdf.IO.PdfReader.Open(new MemoryStream(pdf), PdfDocumentOpenMode.Import));
                }

                using (PdfSharp.Pdf.PdfDocument outPdf = new PdfSharp.Pdf.PdfDocument())
                {
                    for (int i = 1; i <= lstDocuments.Count; i++)
                    {
                        foreach (PdfSharp.Pdf.PdfPage page in lstDocuments[i - 1].Pages)
                        {
                            outPdf.AddPage(page);
                        }
                    }

                    using (MemoryStream mem = new MemoryStream())
                    {
                        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                        outPdf.Save(mem, false);
                        result = mem.ToArray();
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro realizar o Merge dos Pdf's. Mais detalhes " + ex);
            }
        }

        public static byte[] ConverterImageParaPDF(byte[] imageBytes)
        {
            using (var mem = new MemoryStream())
            {
                using (var pdfWriter = new PdfWriter(mem))
                {
                    var pdf = new PdfDocument(pdfWriter);
                    var documento = new iText.Layout.Document(pdf);

                    var img = new PdfImage(ImageDataFactory.Create(imageBytes));
                        //.SetAutoScale(true)
                        //.SetHorizontalAlignment(PdfProperties.HorizontalAlignment.CENTER);

                    documento.Add(img);
                    documento.Close();
                    pdf.Close();
                    return mem.ToArray();
                }
            }
        }
    }
}
