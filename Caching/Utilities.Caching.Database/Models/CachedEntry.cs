﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Utilities.Caching.Database.Models
{
    //[Table("CachedEntries", Schema = "dbo")]
    public class CachedEntry
    {

        public int Id { get; set; }
        public string Name { get; set; }
        [MaxLength()]
        public string Object { get; set; }
        public DateTime Created { get; set; }
        public DateTime Changed { get; set; }
        public DateTime? TimeOut { get; set; }
    }
}
