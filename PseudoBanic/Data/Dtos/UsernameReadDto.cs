using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PseudoBanic.Data.Dtos
{
    [Table("Users")]
    class UsernameReadDto
    {
        [Key, JsonIgnore]
        public long ID { get; set; }
        public string Username { get; set; }
    }
}
