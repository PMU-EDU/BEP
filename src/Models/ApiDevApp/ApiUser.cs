﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BES.Models.ApiDevApp
{
    public class ApiUser
    {
        [Key]
      
        public string Email { get; set; }
     
        public string Password { get; set; }
       
    }
}
