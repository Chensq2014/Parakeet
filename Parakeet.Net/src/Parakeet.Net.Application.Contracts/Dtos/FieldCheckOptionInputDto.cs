using System;
using System.Collections.Generic;
using System.Text;

namespace Parakeet.Net.Dtos
{
    public class FieldCheckOptionInputDto<TPrimaryKey> //where TPrimaryKey : struct
    {
        public FieldCheckOptionDto<TPrimaryKey> Field{ get; set; }
    }
}
