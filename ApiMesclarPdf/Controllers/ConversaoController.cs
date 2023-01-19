using ApiMesclarPdf.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ApiMesclarPdf.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConversaoController : ControllerBase
    {
        [HttpPost]
        public ActionResult EnviaArquivo([FromForm] ICollection<IFormFile> arquivo)
        {        
            try
            {                
                ConverterPDF arquivoPdf = new ConverterPDF();    

                var retFormato = arquivoPdf.ConverterEmByte(arquivo);

                return  File(retFormato, arquivo.FirstOrDefault().ContentType, "Arquivos");
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao realizar a mesclagem. Mais detalhes: " + ex);
            }            
        }
    }
}
