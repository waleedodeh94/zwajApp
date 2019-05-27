using System.ComponentModel.DataAnnotations;

namespace ZwajApp.API.Dtos
{
    public class UserForRegisterDto
    {
        [Required]
        public string username { get; set; }    
        [StringLength(8,MinimumLength=4,ErrorMessage="error massege is not perfect")] 
        public string password { get; set; }
    }
}