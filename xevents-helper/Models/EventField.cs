﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace xevents_helper.Models
{
    public class EventField
    {
        public string Name { get; set; }
        public string TypeName { get; set; }
        public bool IsOptional { get; set; }
        public string Description { get; set; }
    }
}