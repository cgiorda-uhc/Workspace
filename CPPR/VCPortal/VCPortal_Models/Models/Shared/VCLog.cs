using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPortal_Models.Models.Shared;
public class VCLog
{
    public string log_level { get; set; }
    public string event_name { get; set; }
    public string source { get; set; }
    public string exception_message { get; set; }
    public string stack_trace { get; set; }
    public string state { get; set; }

}
