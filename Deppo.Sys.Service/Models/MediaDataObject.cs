using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Sys.Service.Models;

public class MediaDataObject
{
    public Guid Oid { get; set; }
    public byte[] MediaData { get; set; } = new byte[0];
}