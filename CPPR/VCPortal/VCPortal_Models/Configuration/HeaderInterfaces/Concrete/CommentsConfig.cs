using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCPortal_Models.Configuration.HeaderInterfaces.Abstract;

namespace VCPortal_Models.Configuration.HeaderInterfaces.Concrete;
public class CommentsConfig : ICommentsConfig
{
    public string Header { get; set; }
    public string Comment { get; set; }
}
