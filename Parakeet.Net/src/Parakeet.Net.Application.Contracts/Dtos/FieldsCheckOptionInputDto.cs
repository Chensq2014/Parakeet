using System;
using System.Collections.Generic;
using System.Text;

namespace Parakeet.Net.Dtos
{
    public class FieldsCheckOptionInputDto<TPrimaryKey> //where TPrimaryKey : struct
    {
        public List<FieldCheckOptionDto<TPrimaryKey>> Fields { get; set; }
    }
}
