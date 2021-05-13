using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlogProj.Enums
{
    public enum ModerationType
    {
        [Display(Name = "None")]
        None,
        [Display(Name = "Politcal Propoganda")]
        PoliticalPropaganda,
        [Display(Name = "Offensive Language")]
        OffensiveLanguage,
        [Display(Name = "Sexual Content")]
       SexualContent,
        [Display(Name = "Bullying")]
        Bullying
    }
}
