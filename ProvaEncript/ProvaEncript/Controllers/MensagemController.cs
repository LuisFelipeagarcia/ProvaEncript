using ProvaEncript.Context;
using ProvaEncript.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ProvaEncript.Controllers
{
    public class MensagemController : Controller
    {
        private Contexto db = new Contexto();
        private static string AesIV256BD = @"%j?TmFP6$BbMnY$@";
        private static string AesKey256BD = @"rxmBUJy]&,;3jKwDTzf(cui$<nc2EQr)";
        // GET: Mensagem
        public ActionResult Index()
        {
            List<MensagemModel> mensagens = db.Mensagens.ToList();

            //AesCryptoServiceProvider
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            aes.BlockSize = 128;
            aes.KeySize = 256;
            aes.IV = Encoding.UTF8.GetBytes(AesIV256BD);
            aes.Key = Encoding.UTF8.GetBytes(AesKey256BD);
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            for (int i = 0; i < mensagens.Count; i++)
            {
                byte[] src = Convert.FromBase64String(mensagens[i].Mensagem);

                using (ICryptoTransform decrypt = aes.CreateDecryptor())
                {
                    byte[] dest = decrypt.TransformFinalBlock(src, 0, src.Length);
                    mensagens[i].Mensagem = Encoding.Unicode.GetString(dest);
                }
            }
            return View(mensagens.ToList());
        }

        #region Create - GET

        [HttpGet]
        public ActionResult Create(string? msg,  string? msgAntiga)
        {
            if (msg != null) 
            {
                TempData["msg"] = msg;
                TempData["msgAntiga"] = msgAntiga;
            }
            
            return View();
        }
        #endregion

        #region Create - POST

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MensagemModel mensagenModel)
        {
            if (ModelState.IsValid)
            {
                string msgAntiga = mensagenModel.Mensagem;


                //AesCryptoServiceProvider
                AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
                aes.BlockSize = 128;
                aes.KeySize = 256;
                aes.IV = Encoding.UTF8.GetBytes(AesIV256BD);
                aes.Key = Encoding.UTF8.GetBytes(AesKey256BD);
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                // Convertendo String para byte Arrey
                byte[] src = Encoding.Unicode.GetBytes(mensagenModel.Mensagem);
                
                //Encriptação
                using (ICryptoTransform encrypt = aes.CreateEncryptor())
                {
                    byte[] dest = encrypt.TransformFinalBlock(src, 0, src.Length);

                    mensagenModel.Mensagem = Convert.ToBase64String(dest);
                    
                }
                string msg = mensagenModel.Mensagem;
                db.Mensagens.Add(mensagenModel);
                db.SaveChanges();

                return RedirectToAction(nameof(Create), new { @msg = msg, @msgAntiga = msgAntiga }) ;
            }
            return RedirectToAction(nameof(Create));
        }
        #endregion

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MensagemModel mensagemModel = db.Mensagens.Find(id);
            if (mensagemModel == null)
            {
                return HttpNotFound();
            }

            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            aes.BlockSize = 128;
            aes.KeySize = 256;
            aes.IV = Encoding.UTF8.GetBytes(AesIV256BD);
            aes.Key = Encoding.UTF8.GetBytes(AesKey256BD);
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            string msgE = mensagemModel.Mensagem;
            byte[] src = Convert.FromBase64String(mensagemModel.Mensagem);
            using (ICryptoTransform decrypt = aes.CreateDecryptor())
            {
                byte[] dest = decrypt.TransformFinalBlock(src, 0, src.Length);
                mensagemModel.Mensagem = Encoding.Unicode.GetString(dest);
            }
            string msgD = mensagemModel.Mensagem;
            TempData["msgE"] = msgE;
            TempData["msgD"] = msgD;
            //return RedirectToAction(nameof(Details));
            return View(); 
        }
    }
}