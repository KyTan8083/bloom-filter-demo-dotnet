using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestClient.Models
{
    public sealed class AddItemRequest
    {
        public string Item { get; set; } = string.Empty;
    }
}
