using System;
using System.Collections.Generic;
using RestSharp;
using System.IO;
using Renci.SshNet;
using Microsoft.AspNetCore.Http;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace ApiMesclarPdf.Model
{
    public class ConverterPDF
    {
        public byte[] ConverterEmByte(ICollection<IFormFile> filePaths)
        {
            try
            {
                List<byte[]> listByte = new List<byte[]>();
                byte[] result = null;

                foreach (var formFile in filePaths)
                {
                    if (formFile.Length > 0)
                    {
                        using (var stream = new MemoryStream())
                        {
                            formFile.CopyToAsync(stream);
                            listByte.Add(stream.ToArray());
                        }
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
                    lstDocuments.Add(PdfReader.Open(new MemoryStream(pdf), PdfDocumentOpenMode.Import));
                }

                using (PdfDocument outPdf = new PdfDocument())
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
            catch(Exception ex)
            {
                throw new Exception("Erro realizar o Merge dos Pdf's. Mais detalhes " + ex);
            }            
        }
    }
}
