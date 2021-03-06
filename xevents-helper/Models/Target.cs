﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace xevents_helper.Models
{
    public class Target
    {
        public string Name { get; set; }
        public string PackageName { get; set; }
        public IEnumerable<TargetParameter> Parameters { get; set; }
        public IEnumerable<TargetSetting> Settings { get; set; }
    }
}