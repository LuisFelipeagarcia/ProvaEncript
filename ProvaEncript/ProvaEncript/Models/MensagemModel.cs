using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProvaEncript.Models
{
    public class MensagemModel
    {
        [Key]
        [Display(Name = "ID:")]
        public int Id { get; set; }
        [Display(Name = "Mensagem:")]
        [Required(ErrorMessage = "Digite a mensagem!")]
        public string Mensagem { get; set; }
    }
}