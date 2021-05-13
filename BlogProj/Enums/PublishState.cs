using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace BlogProj.Enums
{  public enum PublishState
        {
        [Description("Production Ready")]
            ProductionReady,
        [Description("Preview Ready")]
        PreviewReady,
        [Description("Not Ready")]
        NotReady
        }
}
