﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace xevents_helper.Models
{
    public enum EventRetentionMode
    {
        NotSpecified,
        AllowSingleEventLoss,
        AllowMultipleEventLoss,
        NoEventLoss
    }
}